using System;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Context;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.SurroundTemplates;
using JetBrains.ReSharper.LiveTemplates.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.Coords;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;
using SurroundTemplateShortcuts.Framework.Dump;
using SurroundTemplateShortcuts.Framework.Stack;
using SurroundTemplateShortcuts.Navigation;

namespace SurroundTemplateShortcuts.Actions
{
    public class InsertSurroundMnemonicActionBase : IExecutableAction
    {
        private readonly string _mnemonic;

        protected InsertSurroundMnemonicActionBase(string mnemonic)
        {
            _mnemonic = mnemonic;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);

            // TODO: Add option here to user-control the auto-expand to word feature
            //return textControl?.Selection.HasSelection() ?? false;
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            var surroundManager = Shell.Instance.GetComponent<SurroundManager>();
            if (textControl == null ||
                solution == null)
            {
                return;
            }

            var singleSelection = textControl.SingleSelectionRange();
            if (singleSelection.IsValid() &&
                singleSelection.IsEmpty)
            {
                var sourceFile = textControl.Document.GetPsiSourceFile(solution);
                if (sourceFile == null)
                {
                    return;
                }
                var cachedPsiFile = solution.GetPsiServices().Files
                    .GetCachedPsiFile(sourceFile, sourceFile.PrimaryPsiLanguage);
                var file = cachedPsiFile?.PsiFile;
                if (file == null)
                {
                    return;
                }

                var finder = new NodeFromCaretFinder(singleSelection.StartOffset.Offset);
                file.ProcessThisAndDescendants(finder);

                var selected = NodeFromCaretFinder.GetParentLanguageElementFromNode(finder.NodeAtCaret);

                textControl.Selection.SetRanges(
                    new[]
                    {
                        TextControlPosRange.FromDocRange(
                            textControl,
                            selected.GetNavigationRange().TextRange)
                    });

                try
                {
                    finder.NodeAtCaret.Dump(
                        $"Node at caret {singleSelection.StartOffset.Offset}",
                        WriteOutputHelper.Write);

                    selected.Dump("Selected node", false, WriteOutputHelper.Write);

                    var ranges = finder.NodeAtCaret.GetRecursive(x => x.Parent);
                    foreach (var range in ranges)
                    {
                        var navRange = range.GetNavigationRange();
                        range.Dump(
                            $"Range of node: {navRange.StartOffset.Offset} to {navRange.EndOffset.Offset}",
                            false,
                            WriteOutputHelper.Write);
                    }
                }
                catch (Exception ex)
                {
                    WriteOutputHelper.Write(ex.ToString());
                }
            }

            var selections = textControl.Selection.Ranges.GetValue().Select(x => x.ToDocRangeNormalized());
            var templateItems =
                selections.Select(textRange =>
                        new TemplateAcceptanceContext(
                            solution, textControl.Document,
                            textRange.StartOffset, textRange))
                    .Select(tac => surroundManager.GetSurroundTemplates(tac))
                    .Select(templates => templates.FirstOrDefault(x => x.Template.Mnemonic == _mnemonic))
                    .Where(x => x != null);

            foreach (var surroundItem in templateItems)
            {
                var shortcutAction = new SurroundWithAction(surroundItem);
                shortcutAction.Execute(solution, textControl);
            }
        }
    }
}