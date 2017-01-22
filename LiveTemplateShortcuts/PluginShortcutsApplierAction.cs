using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.UI.ActionsRevised;

namespace LiveTemplateShortcuts
{
    [Action("LiveTemplateShortcuts Create Keyboard Binding", Id = 523600)]
    public class PluginShortcutsApplierAction : IExecutableAction, IInsertLast<ToolsMenu>
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return PluginShortcutsApplier.VisualStudioIsPresent();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            PluginShortcutsApplier.ApplyKeyboardBindings();
        }
    }
}