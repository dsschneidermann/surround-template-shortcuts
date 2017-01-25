using System;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised;

namespace LiveTemplateShortcuts.Actions
{
    [Action("LiveTemplate Shortcuts: Set Keyboard Bindings", Id = 523600)]
    public class PluginShortcutsApplierAction : IExecutableAction, IInsertLast<ToolsMenu>
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return PluginShortcutsApplier.VisualStudioIsPresent();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            ExecuteActionOnUiThread("LiveTemplateShortcuts ApplyKeyboardBindings", () =>
                {
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic1Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic2Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic3Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic4Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic5Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic6Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic7Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic8Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic9Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_InsertSurroundMnemonic0Action>();
                    AssignKeyboardAction<LiveTemplateShortcuts_MoveClosingBraceToSelectionEndAction>();
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