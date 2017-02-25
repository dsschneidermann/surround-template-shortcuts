using JetBrains.Application.Settings;
using JetBrains.UI;

namespace SurroundTemplateShortcuts
{
    [SettingsKey(typeof(UserInterfaceSettings), "SurroundTemplateShortcuts settings")]
    public class SurroundTemplateShortcutsSettings
    {
        [SettingsEntry(true, "Enabled")]
        public bool Enabled { get; set; }
    }
}