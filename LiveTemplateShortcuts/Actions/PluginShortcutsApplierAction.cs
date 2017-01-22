using System;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised;

namespace LiveTemplateShortcuts.Actions
{
    [Action("LiveTemplateShortcuts Set Keyboard Bindings", Id = 523600)]
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
                    AssignKeyboardAction<InsertSurround1Action>();
                    AssignKeyboardAction<InsertSurround2Action>();
                    AssignKeyboardAction<InsertSurround3Action>();
                    AssignKeyboardAction<InsertSurround4Action>();
                    AssignKeyboardAction<InsertSurround5Action>();
                    AssignKeyboardAction<InsertSurround6Action>();
                    AssignKeyboardAction<InsertSurround7Action>();
                    AssignKeyboardAction<InsertSurround8Action>();
                    AssignKeyboardAction<InsertSurround9Action>();
                    AssignKeyboardAction<InsertSurround0Action>();
                    AssignKeyboardAction<MoveLastBraceAction>();
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