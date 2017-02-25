using System;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Threading;

namespace SurroundTemplateShortcuts
{
    [ShellComponent(Requirement = InstantiationRequirement.Instant)]
    public class SurroundTemplateShortcuts
    {
        public SurroundTemplateShortcuts(Lifetime lifetime, ISettingsStore settingsStore, IThreading threading) { }

        public static SurroundTemplateShortcuts Instance => Shell.Instance.GetComponent<SurroundTemplateShortcuts>();
    }
}