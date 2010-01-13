using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using System.Collections.ObjectModel;
using NES.CPU.nitenedo;

namespace InstiBulb.WinViewModels
{
    public class WinCheatPanelVM : CheatPanelVM
    {

        public WinCheatPanelVM(NESMachine nes) : base(nes) { }

        ObservableCollection<CheatVM> activeCheats = new ObservableCollection<CheatVM>();

        public ObservableCollection<CheatVM> ActiveCheats
        {
            get { return activeCheats; }
        }

        public override void OnPropertyChanged(string propName)
        {
            if (propName == "GameGenieCodes")
            {
                activeCheats.Clear();
                foreach (var cheat in Cheats)
                    activeCheats.Add(cheat);

            }
        }
    }
}
