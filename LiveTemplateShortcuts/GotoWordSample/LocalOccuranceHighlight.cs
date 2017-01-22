using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.Navigation.CustomHighlighting;
using JetBrains.TextControl;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.TextControl.Graphics;
using JetBrains.Util;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace LiveTemplateShortcuts.GotoWordSample
{
    internal sealed class LocalOccurrencesHighlighter
    {
        private static readonly string Prefix = typeof(LocalOccurrencesHighlighter).Name + ".";

        private static readonly Key GotoWordOccurrence = new Key("GotoWordOccurrence");

        private static readonly Key GotoWordFocusedOccurrence = new Key("GotoWordFocusedOccurrence");

        private static readonly ErrorStripeAttributes ErrorStripeUsagesAttributes = new ErrorStripeAttributes(ErrorStripeKind.ERROR, "ReSharper Write Usage Marker on Error Stripe");

        private readonly Lifetime _Lifetime;
        private readonly IDocumentMarkupManager _MarkupManager;
        private readonly SequentialLifetimes _SequentialFocused;
        private readonly SequentialLifetimes _SequentialOccurrences;
        private readonly IShellLocks _ShellLocks;
        private readonly object _SyncRoot;
        private readonly ITextControl _TextControl;
        private readonly Rect _TextControlViewportRect;
        private IList<LocalOccurrence> _Occurrences;
        private LocalOccurrence _SelectedOccurrence;
        private volatile bool _ShouldDropHighlightings;
        private volatile bool _UpdateSelectedScheduled;

        public LocalOccurrencesHighlighter(Lifetime lifetime, IShellLocks shellLocks, ITextControl textControl, IDocumentMarkupManager markupManager)
        {
            _Lifetime = lifetime;
            _ShellLocks = shellLocks;
            _MarkupManager = markupManager;
            _TextControl = textControl;
            _SequentialOccurrences = new SequentialLifetimes(lifetime);
            _SequentialFocused = new SequentialLifetimes(lifetime);
            _TextControlViewportRect = textControl.Scrolling.ViewportRect.Value;

            _SyncRoot = new object();

            _Lifetime.AddAction(DropHighlightings);
        }

        public void UpdateOccurrences(IList<LocalOccurrence> occurrences, IEnumerable<LocalOccurrence> tailOccurrences = null)
        {
            lock (_SyncRoot)
            {
                _Occurrences = occurrences;
                _SelectedOccurrence = occurrences.Count == 0 ? null : occurrences[0];

                if (!_ShouldDropHighlightings &&
                    occurrences.Count == 0)
                {
                    return;
                }

                _SequentialOccurrences.Next(lifetime =>
                    {
                        _ShellLocks.ExecuteOrQueueReadLock(lifetime, Prefix + "UpdateOccurrence", () => UpdateOccurrencesHighlighting(lifetime, occurrences));

                        _UpdateSelectedScheduled = true;
                    });

                _SequentialFocused.Next(lifetime =>
                    {
                        _ShellLocks.AssertReadAccessAllowed();

                        _ShellLocks.ExecuteOrQueueReadLock(lifetime, Prefix + "UpdateOccurrence", UpdateFocusedOccurrence);
                    });
            }
        }

        public void UpdateSelectedOccurrence(LocalOccurrence occurrence)
        {
            lock (_SyncRoot)
            {
                var occurrences = _Occurrences.NotNull("occurrences != null");

                // late selection change event may happend - ignore
                if (!occurrences.Contains(occurrence))
                {
                    return;
                }

                _SelectedOccurrence = occurrence;

                if (!_UpdateSelectedScheduled)
                {
                    _SequentialFocused.Next(lifetime =>
                        {
                            _ShellLocks.ExecuteOrQueueReadLock(lifetime, Prefix + "UpdateSelectedOccurrence", UpdateFocusedOccurrence);
                        });
                }
            }
        }

        private void DropHighlightings()
        {
            lock (_SyncRoot)
            {
                _SequentialOccurrences.TerminateCurrent();
                _SequentialFocused.TerminateCurrent();

                _Occurrences = null;
                _SelectedOccurrence = null;

                if (_ShouldDropHighlightings)
                {
                    _ShellLocks.ExecuteOrQueueReadLock(Prefix + "DropHighlightings", () =>
                        {
                            UpdateOccurrencesHighlighting(EternalLifetime.Instance, EmptyList<LocalOccurrence>.InstanceList);
                            UpdateFocusedOccurrence();
                        });
                }
            }
        }

        private void UpdateFocusedOccurrence()
        {
            LocalOccurrence selectedOccurrence;
            lock (_SyncRoot)
            {
                selectedOccurrence = _SelectedOccurrence;
                _UpdateSelectedScheduled = false;
            }

            var documentMarkup = _MarkupManager.GetMarkupModel(_TextControl.Document);

            if (_ShouldDropHighlightings)
            {
                var toRemove = new LocalList<IHighlighter>();
                foreach (var highlighter in documentMarkup.GetHighlightersEnumerable(GotoWordFocusedOccurrence))
                {
                    toRemove.Add(highlighter);
                }

                foreach (var highlighter in toRemove)
                {
                    documentMarkup.RemoveHighlighter(highlighter);
                }
            }

            if (selectedOccurrence != null)
            {
                var range = selectedOccurrence.Range.TextRange;
                documentMarkup.AddHighlighter(GotoWordFocusedOccurrence, range, AreaType.EXACT_RANGE, 0, CustomHighlightingManagerIds.NavigationHighlighterID,
                                              ErrorStripeAttributes.Empty, null);

                _ShouldDropHighlightings = true;

                // todo: better positioning
                var position = Math.Max(selectedOccurrence.LineNumber - 2, 0);
                var target = _TextControl.Coords.FromDocLineColumn(new DocumentCoords((Int32<DocLine>) position, (Int32<DocColumn>) 0));
                _TextControl.Scrolling.ScrollTo(target, TextControlScrollType.TopOfView);
            }
            else
            {
                _TextControl.Scrolling.ScrollTo(_TextControlViewportRect.Location);
            }
        }

        private void UpdateOccurrencesHighlighting(Lifetime updateLifetime, IList<LocalOccurrence> occurrences)
        {
            if (updateLifetime.IsTerminated)
            {
                return;
            }

            var documentMarkup = _MarkupManager.GetMarkupModel(_TextControl.Document);

            // collect and remove obsolete hightlightings
            if (_ShouldDropHighlightings)
            {
                var toRemove = new LocalList<IHighlighter>();

                foreach (var highlighter in documentMarkup.GetHighlightersEnumerable(GotoWordOccurrence))
                {
                    if (updateLifetime.IsTerminated)
                    {
                        return;
                    }

                    toRemove.Add(highlighter);
                }

                foreach (var highlighter in toRemove)
                {
                    if (updateLifetime.IsTerminated)
                    {
                        return;
                    }


                    documentMarkup.RemoveHighlighter(highlighter);
                }
            }

            // add new highlighters
            foreach (var occurrence in occurrences)
            {
                if (updateLifetime.IsTerminated)
                {
                    return;
                }

                documentMarkup.AddHighlighter(GotoWordOccurrence, occurrence.Range.TextRange, AreaType.EXACT_RANGE, 0, HotspotSessionUi.CURRENT_HOTSPOT_HIGHLIGHTER,
                                              ErrorStripeUsagesAttributes, null);

                _ShouldDropHighlightings = true;
            }
        }
    }
}