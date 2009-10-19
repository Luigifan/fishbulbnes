using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace InstiBulb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                String.Format("Exception occurred; {0}", e.Exception.Message), "RERROR IN CODSE!");
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DisassemblyExtensions.OpCodeInfo = null;
            base.OnExit(e);
        }
    }
}
