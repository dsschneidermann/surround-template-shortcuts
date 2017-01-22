using System;
using System.Linq;
using System.Reflection;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised;
using JetBrains.VsIntegration.Shell.ActionManagement;
using LiveTemplateShortcuts.Framework.Dump;

namespace LiveTemplateShortcuts
{
    public static class PluginShortcutsApplier
    {
        public static bool VisualStudioIsPresent()
        {
            return Shell.Instance.HasComponent<DTE>();
        }

        /// <summary>
        ///     Must run on main UI thread
        /// </summary>
        private static void AssignKeyboardShortcut(string shortcutName, string keyboardShortcut, int shortcutId)
        {
            shortcutName.Dump("shortcutName", WriteToOutput);
            var dte = Shell.Instance.GetComponent<DTE>();
            
            var command = dte.Commands.Item(shortcutName, shortcutId);

            if (command == null)
            {
                return;
            }

            var currentBindings = (object[]) command.Bindings;
            if (currentBindings.Length == 1 &&
                string.Equals(currentBindings[0].ToString(), keyboardShortcut, StringComparison.InvariantCultureIgnoreCase))
            {
                GetOutputWindowPane(dte, "Output", true).OutputString($"Keyboard shortcut '{keyboardShortcut}' is '{shortcutName}'\n");
                return;
            }
            
            command.Bindings = keyboardShortcut;
            GetOutputWindowPane(dte, "Output", true).OutputString($"Keyboard shortcut '{keyboardShortcut}' set to '{shortcutName}'\n");
        }

        private static void WriteToOutput(string message)
        {
            var dte = Shell.Instance.GetComponent<DTE>();
            GetOutputWindowPane(dte, "Output", true).OutputString(message);
        }

        private static OutputWindowPane GetOutputWindowPane(DTE dte, string name, bool show)
        {
            /* If compilation generates:: 'EnvDTE.Constants' can be used only as one of its applicable interfaces
             * then set DTE assembly reference property Embed Interop Types = false  */

            var win = dte.Windows.Item(Constants.vsWindowKindOutput);
            if (show)
            {
                win.Visible = true;
            }

            var ow = (OutputWindow) win.Object;
            OutputWindowPane owpane;
            try
            {
                owpane = ow.OutputWindowPanes.Item(name);
            }
            catch (Exception)
            {
                owpane = ow.OutputWindowPanes.Add(name);
            }

            owpane.Activate();
            return owpane;
        }

        private static void ExecuteActionOnUiThread(string description, Action fOnExecute)
        {
            var threading = Shell.Instance.GetComponent<IThreading>();
            threading.ReentrancyGuard.ExecuteOrQueueEx(description, fOnExecute);
        }

        public static void ApplyKeyboardBindings()
        {
            ExecuteActionOnUiThread(
                "LiveTemplateShortcuts ApplyKeyboardBindings", () =>
                    {
                        if (!VisualStudioIsPresent())
                        {
                            return;
                        }
                        var shortcut = GetShortcutActionForType<LiveTemplateShortcutsInsertSurround1Action>();
                        AssignKeyboardShortcut(shortcut.Item1, shortcut.Item2, shortcut.Item3);
                    });
        }

        private static Tuple<string, string, int> GetShortcutActionForType<T>()
        {
            var name = typeof(T).Name;
            var shortcutName = $"ReSharper_{name.Substring(0, name.Length - 6)}";
            var attribute = typeof(T).GetCustomAttribute<ActionAttribute>(false);
            var vsShortcut = attribute.VsShortcuts.First().Replace("Control", "Ctrl");
            var vsScope = attribute.ShortcutScope == ShortcutScope.TextEditor ? "Text Editor" : "Global";
            var shortcutString = $"{vsScope}::{vsShortcut}";
            var shortcutId = attribute.Id;
            return new Tuple<string, string, int>(shortcutName, shortcutString, shortcutId);
        }
    }
}