using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Context;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.SurroundTemplates;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.LiveTemplates.ContextActions;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;

namespace LiveTemplateShortcuts
{
    [Action("LiveTemplateShortcuts Insert Surround 1", Id = 523611)]
    public class LiveTemplateShortcutsInsertSurround1Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround1Action()
            : base("1") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 2", Id = 523612)]
    public class LiveTemplateShortcutsInsertSurround2Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround2Action()
            : base("2") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 3", Id = 523613)]
    public class LiveTemplateShortcutsInsertSurround3Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround3Action()
            : base("3") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 4", Id = 523614)]
    public class LiveTemplateShortcutsInsertSurround4Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround4Action()
            : base("4") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 5", Id = 523615)]
    public class LiveTemplateShortcutsInsertSurround5Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround5Action()
            : base("5") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 6", Id = 523616)]
    public class LiveTemplateShortcutsInsertSurround6Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround6Action()
            : base("6") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 7", Id = 523617)]
    public class LiveTemplateShortcutsInsertSurround7Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround7Action()
            : base("7") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 8", Id = 523618)]
    public class LiveTemplateShortcutsInsertSurround8Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround8Action()
            : base("8") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 9", Id = 523619)]
    public class LiveTemplateShortcutsInsertSurround9Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround9Action()
            : base("9") { }
    }

    [Action("LiveTemplateShortcuts Insert Surround 0", Id = 523610)]
    public class LiveTemplateShortcutsInsertSurround0Action : LiveTemplateShortcutsInsertSurroundActionBase
    {
        public LiveTemplateShortcutsInsertSurround0Action()
            : base("0") { }
    }

    public class LiveTemplateShortcutsInsertSurroundActionBase : IExecutableAction, IInsertLast<ToolsMenu>
    {
        private readonly string _mnemonic;

        public LiveTemplateShortcutsInsertSurroundActionBase(string mnemonic)
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
            var surroundManager = Shell.Instance.GetComponent<SurroundManager>();
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION);

            var templateItems =
                textControl?.Selection.Ranges.Value.Select(selectionRange => selectionRange.ToDocRangeNormalized())
                    .Select(textRange => new TemplateAcceptanceContext(solution, textControl.Document, textRange.StartOffset, textRange))
                    .Select(tac => surroundManager.GetSurroundTemplates(tac))
                    .Select(templates => templates.FirstOrDefault(x => x.Template.Mnemonic == _mnemonic))
                    .Where(x => x != null) ?? Enumerable.Empty<SurroundManager.SurroundTemplateItem>();

            foreach (var surroundItem in templateItems)
            {
                var shortcutAction = new SurroundWithAction(surroundItem);
                shortcutAction.Execute(solution, textControl);
            }
        }
    }
}