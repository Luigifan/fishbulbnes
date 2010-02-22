using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using InstiBulb.ThreeDee;
using System.Windows;
using System.Windows.Input;
using Fishbulb.Common.UI;
using InstiBulb.Commands;

namespace InstiBulb.ThreeDee
{

    public class ToolSpinner3D : Viewport3D
    {
        public ToolSpinner3D() : base()
        {
            prevCmd = new DelegateCommand(new CommandExecuteHandler(MovePrevious), new CommandCanExecuteHandler(CanMovePrevious));
            nextCmd = new DelegateCommand(new CommandExecuteHandler(MoveNext), new CommandCanExecuteHandler(CanMoveNext));
            Icons = new List<Visual3D>();
            
        }

        protected override void OnInitialized(EventArgs e)
        {
            RebuildIconCollection();
            base.OnInitialized(e);
        }

        public List<Visual3D> Icons
        {
            get;
            set;
        }

        private ICommand prevCmd ;
        private ICommand nextCmd;

        public ICommand PreviousCommand
        {
            get
            {
                return prevCmd;
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return nextCmd;
            }
        }

        void MoveNext(object o)
        {
            menuSpinner.Next();
        }

        bool CanMoveNext(object o)
        {
            return true;
        }

        void MovePrevious(object o)
        {
            menuSpinner.Previous();
        }

        bool CanMovePrevious(object o)
        {
            return true;
        }


        InteractiveCanvasSpinnerFactory menuSpinner;
        void RebuildIconCollection()
        {
            ContainerUIElement3D spinnerContainer = new ContainerUIElement3D();
            

            var iconList = from UIElement3D child in this.Icons where child is Icon3D select child;

            if (iconList != null && iconList.Count() > 0)
            {
                foreach (Icon3D icon in iconList)
                    icon.Rebuild();

                menuSpinner = new InteractiveCanvasSpinnerFactory(spinnerContainer, iconList.ToList(), 5, 0);
                menuSpinner.JumpTo(0);
                this.Children.Add(spinnerContainer);
            }
        }
    }
}
