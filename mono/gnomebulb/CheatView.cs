
using System;
using Fishbulb.Common.UI;
using UIComposition;

namespace GtkNes
{
	
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CheatView : Gtk.Bin, IBindableElement
	{
		
		public CheatView()
		{
			this.Build();
            this.btnAddCheat.Pressed += new EventHandler(btnAddCheat_Pressed);
            this.btnClearCheats.Pressed += new EventHandler(btnClearCheats_Pressed);
		}

        void btnClearCheats_Pressed(object sender, EventArgs e)
        {
            dataContext.ExecuteCommand("ClearCheats", null);
        }

        void btnAddCheat_Pressed(object sender, EventArgs e)
        {
            if(txtCheatCode.Text.Length == 6 || txtCheatCode.Text.Length == 8)
                dataContext.ExecuteCommand("AddCheat", txtCheatCode.Text);
        }

        #region IBindableElement Members

        IViewModel dataContext;

        public Fishbulb.Common.UI.IViewModel DataContext
        {
            get
            {
                return dataContext;
            }
            set
            {
                this.dataContext = value;
                this.cheatList.CreateBinding("List", dataContext, "GameGenieCodes");
            }
        }

        #endregion
    }
}
