using System;
using System.Windows;
using System.Threading;
using ESTool;

namespace Starter
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            Mutex mutex = new Mutex(true, "SingleInstance", out createNew);

            if (API.FindWindow(null, "EasyStarterByAbelGuaizi") == IntPtr.Zero && createNew)
            {
                StartButton startbutton = new StartButton();
                startbutton.Show();
                API.DealMessage(e.Args);
            }
            else
            {
                API.DealMessage(e.Args);
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}
