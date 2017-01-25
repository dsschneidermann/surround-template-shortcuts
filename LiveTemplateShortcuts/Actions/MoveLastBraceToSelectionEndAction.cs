using System.Collections.Generic;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.Coords;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;
using LiveTemplateShortcuts.Navigation;

namespace LiveTemplateShortcuts.Actions
{
    [Action("LiveTemplateShortcuts Move Last Brace To Selection End", Id = 523601, VsShortcuts = new[] { "Control+Alt+0" }, IdeaShortcuts = new[] { "Control+Alt+0" },
        ShortcutScope = ShortcutScope.TextEditor, DefaultShortcutText = "LiveTemplateShortcutsMoveLastBraceToSelectionEnd")]
    public class LiveTemplateShortcuts_MoveLastBraceToSelectionEndAction : IExecutableAction
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            if (textControl == null ||
                solution == null)
            {
                return;
            }
            var sourceFile = textControl.Document.GetPsiSourceFile(solution);
            if (sourceFile == null)
            {
                return;
            }
            var cachedPsiFile = solution.GetPsiServices().Files.GetCachedPsiFile(sourceFile, sourceFile.PrimaryPsiLanguage);
            var file = cachedPsiFile?.PsiFile;
            if (file == null)
            {
                return;
            }
            var selections = textControl.Selection.Ranges.GetValue();
            if (selections.Count > 1)
            {
                textControl.EmulateTyping('}');
            }
            else
            {
                var selectionRange = textControl.SingleSelectionRange();
                if (!selectionRange.IsEmpty)
                {
                    var finder = new NodesFromSelectionFinder(selectionRange.StartOffset.Offset, selectionRange.EndOffset.Offset);
                    file.ProcessThisAndDescendants(finder);

                    var match = FindBraceMatch(finder.SelectionNodes);

                    if (match.IsMatched())
                    {
                        textControl.Document.DeleteText(match.Node.GetNavigationRange().TextRange);
                    }
                    var caretPos = selectionRange.EndOffset.Offset;
                    textControl.Selection.SetRanges(new[] { TextControlPosRange.FromDocRange(textControl, caretPos, caretPos) });
                }
                textControl.EmulateTyping('}');
            }
        }

        private BraceMatch FindBraceMatch(IEnumerable<ITreeNode> nodes)
        {
            var match = BraceMatch.Init();
            foreach (var node in nodes.Where(x => x.NodeType == CSharpTokenType.LBRACE || x.NodeType == CSharpTokenType.RBRACE))
            {
                if (node.NodeType == CSharpTokenType.LBRACE)
                {
                    match = BraceMatch.NoMatch(match.OpenBraces + 1);
                }
                else if (match.OpenBraces > 0)
                {
                    match = BraceMatch.NoMatch(match.OpenBraces - 1);
                }
                else
                {
                    // Return first closing brace that is not in balance
                    return BraceMatch.Match(node);
                }
            }
            return match;
        }

        private class BraceMatch
        {
            private BraceMatch(ITreeNode node, int openBraces)
            {
                OpenBraces = openBraces;
                Node = node;
            }

            public ITreeNode Node { get; }
            public int OpenBraces { get; }

            public bool IsMatched()
            {
                return Node != null;
            }

            public static BraceMatch Init()
            {
                return new BraceMatch(null, 0);
            }

            public static BraceMatch NoMatch(int openBraceCounter)
            {
                return new BraceMatch(null, openBraceCounter);
            }

            public static BraceMatch Match(ITreeNode braceNode)
            {
                return new BraceMatch(braceNode, 0);
            }
        }
    }
}