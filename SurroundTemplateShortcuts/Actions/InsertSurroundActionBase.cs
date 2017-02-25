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
                var cachedPsiFile = solution.GetPsiServices().Files.GetCachedPsiFile(sourceFile, sourceFile.PrimaryPsiLanguage);
                var file = cachedPsiFile?.PsiFile;
                if (file == null)
                {
                    return;
                }

                var finder = new NodeFromCaretFinder(singleSelection.StartOffset.Offset);
                file.ProcessThisAndDescendants(finder);

                textControl.Selection.SetRanges(new[] { TextControlPosRange.FromDocRange(textControl, finder.NodeAtCaret.GetNavigationRange().TextRange) });
            }

            var selections = textControl.Selection.Ranges.GetValue().Select(x => x.ToDocRangeNormalized());
            var templateItems =
                selections.Select(textRange => new TemplateAcceptanceContext(solution, textControl.Document, textRange.StartOffset, textRange))
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