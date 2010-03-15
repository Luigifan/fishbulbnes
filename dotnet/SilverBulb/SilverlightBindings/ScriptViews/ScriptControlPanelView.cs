using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Fishbulb.Common.UI;
using System.Windows.Browser;
using FishBulb;

namespace SilverlightCommonUI.ScriptViews
{
    public class ScriptControlPanelVM : SilverlightControlPanelVM
    {
        public ScriptControlPanelVM(IPlatformDelegates delegates) : base(delegates)
        {
            HtmlPage.RegisterScriptableObject("ControlPanel", this);

            delegates.RomLoadedEvent += new EventHandler<RomLoadedEventArgs>(delegates_RomLoadedEvent);
        }

        void delegates_RomLoadedEvent(object sender, RomLoadedEventArgs e)
        {
            if (e.ResultStream != null)
            {
                TargetMachine.EjectCart();
                using (e.ResultStream)
                    TargetMachine.LoadCart(e.ResultStream);

                UpdateCartInfo();
                TargetMachine.PowerOn();
                TargetMachine.ThreadRuntendo();
            }
            isLoadingRom = false;
        }

        [ScriptableMember]
        public void PowerOn()
        {
            var command = Commands["PowerToggle"];
            if (command.CanExecute(null))
                command.Execute(null);

            
        }

        [ScriptableMember]
        public string GetPowerStatusText()
        {
            return PowerStatusText;
        }

        volatile bool isLoadingRom = false;

        [ScriptableMember]
        public void LoadRom(string name)
        {
            if (isLoadingRom) return;
            isLoadingRom = true;
            PlatformDelegates.LoadFile(name);
        }


    }
}
