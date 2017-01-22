using System;
using EnvDTE;
using JetBrains.ReSharper.Resources.Shell;

namespace LiveTemplateShortcuts
{
    public class WriteOutputHelper
    {
        public static void Write(string message)
        {
            var dte = Shell.Instance.GetComponent<DTE>();
            GetOutputWindowPane(dte, "Output", true).OutputString(message);
        }

        public static OutputWindowPane GetOutputWindowPane(DTE dte, string name, bool show)
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
    }
}