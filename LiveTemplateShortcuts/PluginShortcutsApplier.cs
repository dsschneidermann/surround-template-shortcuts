using System;
using System.Linq;
using System.Reflection;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.UI.ActionsRevised;

namespace LiveTemplateShortcuts
{
    public static class PluginShortcutsApplier
    {
        public static bool VisualStudioIsPresent()
        {
            return Shell.Instance.HasComponent<DTE>();
        }

        public static void AssignKeyboardShortcut(string shortcutName, string keyboardShortcut, int shortcutId)
        {
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
                WriteOutputHelper.Write($"Keyboard shortcut '{keyboardShortcut}' is '{shortcutName}'\n");
                return;
            }
            command.Bindings = keyboardShortcut;
            WriteOutputHelper.Write($"Keyboard shortcut '{keyboardShortcut}' set to '{shortcutName}'\n");
        }

        public static Tuple<string, string, int> GetShortcutActionForType<T>()
        {
            var attribute = typeof(T).GetCustomAttribute<ActionAttribute>(false);
            var name = typeof(T).Name;
            var shortcutName = $"ReSharper_{name.Substring(0, name.Length - 6)}";
            var vsShortcut = attribute.VsShortcuts.First().Replace("Control", "Ctrl");
            var vsScope = attribute.ShortcutScope == ShortcutScope.TextEditor ? "Text Editor" : "Global";
            var shortcutString = $"{vsScope}::{vsShortcut}";
            var shortcutId = attribute.Id;
            return new Tuple<string, string, int>(shortcutName, shortcutString, shortcutId);
        }
    }
}