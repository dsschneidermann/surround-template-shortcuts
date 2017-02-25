using System;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised;

namespace SurroundTemplateShortcuts.Actions
{
    [Action("SurroundTemplateShortcuts: Set Keyboard Bindings", Id = 523600)]
    public class SurroundTemplateShortcuts_PluginShortcutsApplierAction : IExecutableAction, IInsertLast<ToolsMenu>
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return PluginShortcutsApplier.VisualStudioIsPresent();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            ExecuteActionOnUiThread("SurroundTemplateShortcuts ApplyKeyboardBindings", () =>
                {
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic1Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic2Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic3Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic4Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic5Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic6Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic7Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic8Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic9Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_InsertSurroundMnemonic0Action>();
                    AssignKeyboardAction<SurroundTemplateShortcuts_MoveClosingBraceToSelectionEndAction>();
                });
        }

        private static void AssignKeyboardAction<T>()
        {
            var shortcut = PluginShortcutsApplier.GetShortcutActionForType<T>();
            PluginShortcutsApplier.AssignKeyboardShortcut(shortcut.Item1, shortcut.Item2, shortcut.Item3);
        }

        private static void ExecuteActionOnUiThread(string description, Action fOnExecute)
        {
            var threading = Shell.Instance.GetComponent<IThreading>();
            threading.ReentrancyGuard.ExecuteOrQueueEx(description, fOnExecute);
        }
    }
}