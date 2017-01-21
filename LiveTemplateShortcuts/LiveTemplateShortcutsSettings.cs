using JetBrains.Application.Settings;
using JetBrains.UI;

namespace LiveTemplateShortcuts
{
    [SettingsKey(typeof(UserInterfaceSettings), "LiveTemplate Shortcuts settings")]
    public class LiveTemplateShortcutsSettings
    {
        [SettingsEntry(true, "Enabled")]
        public bool Enabled { get; set; }
    }
}