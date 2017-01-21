using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Threading;

namespace LiveTemplateShortcuts
{
    [ShellComponent(Requirement = InstantiationRequirement.Instant)]
    public class LiveTemplateShortcuts
    {
        public LiveTemplateShortcuts(Lifetime lifetime, ISettingsStore settingsStore, IThreading threading) { }

        public static LiveTemplateShortcuts Instance => Shell.Instance.GetComponent<LiveTemplateShortcuts>();
    }
}