using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl.Coords;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;
using LiveTemplateShortcuts.Navigation;

namespace LiveTemplateShortcuts.Actions
{
    [Action("LiveTemplateShortcuts Move Last Brace To Selection End", Id = 523601, VsShortcuts = new[] { "Control+Alt+0" }, IdeaShortcuts = new[] { "Control+Alt+0" },
        ShortcutScope = ShortcutScope.TextEditor)]
    public class MoveLastBraceAction : IExecutableAction
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
                    var braceMoveFinder = new BraceMoveFinder(selectionRange.StartOffset.Offset, selectionRange.EndOffset.Offset);
                    file.ProcessThisAndDescendants(braceMoveFinder);
                    if (braceMoveFinder.LastMatchingBraceElement != null)
                    {
                        var braceRange = braceMoveFinder.LastMatchingBraceElement.GetNavigationRange();
                        textControl.Document.DeleteText(braceRange.TextRange);
                    }
                    var caretPos = selectionRange.EndOffset.Offset;
                    textControl.Selection.SetRanges(new[] { TextControlPosRange.FromDocRange(textControl, caretPos, caretPos) });
                }
                textControl.EmulateTyping('}');
            }
        }
    }
}