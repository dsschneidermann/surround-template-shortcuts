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
        public SurroundTemplateShortcuts(Lifetime lifetime, ISettingsStore settingsStore, IThreading threading)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public static SurroundTemplateShortcuts Instance => Shell.Instance.GetComponent<SurroundTemplateShortcuts>();

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllName = args.Name.Contains(",")
                ? args.Name.Substring(0, args.Name.IndexOf(','))
                : args.Name
                    .Replace(".dll", string.Empty)
                    .Replace(".exe", string.Empty);

            dllName = dllName.Replace(".", "_");

            if (dllName.EndsWith("_resources")) return null;

            System.Resources.ResourceManager rm =
                new System.Resources.ResourceManager(GetType().Namespace + ".Properties.Resources",
                    System.Reflection.Assembly.GetExecutingAssembly());

            byte[] bytes = (byte[]) rm.GetObject(dllName);

            return System.Reflection.Assembly.Load(bytes);
        }
    }
}