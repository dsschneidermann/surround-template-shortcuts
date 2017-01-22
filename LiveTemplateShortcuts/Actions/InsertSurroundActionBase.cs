using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Context;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.SurroundTemplates;
using JetBrains.ReSharper.LiveTemplates.ContextActions;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;

namespace LiveTemplateShortcuts.Actions
{
    public class InsertSurroundActionBase : IExecutableAction
    {
        private readonly string _mnemonic;

        public InsertSurroundActionBase(string mnemonic)
        {
            _mnemonic = mnemonic;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
            return textControl?.Selection.HasSelection() ?? false;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            var surroundManager = Shell.Instance.GetComponent<SurroundManager>();

            if (solution == null ||
                textControl == null)
            {
                return;
            }

            var templateItems =
                textControl.Selection.Ranges.Value.Select(selectionRange => selectionRange.ToDocRangeNormalized())
                    .Select(textRange => new TemplateAcceptanceContext(solution, textControl.Document, textRange.StartOffset, textRange))
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