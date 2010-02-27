using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using NES.CPU.nitenedo;
using SlimDXBindings.Viewer10;

namespace SlimDXBindings.ViewerX
{
    public class D3D10DisplayViewModel : IViewModel
    {
        public D3D10DisplayViewModel()
        {
            commands.Add("DumpFilesCommand", new InstigatorCommand(o => Viewer.DumpFiles(), o => Viewer.CanDumpFiles()));
            commands.Add("FullScreenCommand", new InstigatorCommand(o => Viewer.ToggleFullScreen(), o => true));
        }

        private D3D10NesViewer _viewer;

        public D3D10NesViewer Viewer
        {
            get { return _viewer; }
            set { _viewer = value; }
        }

        public string CurrentView
        {
            get { throw new NotImplementedException(); }
        }

        Dictionary<string, ICommandWrapper> commands = new Dictionary<string, ICommandWrapper>();

        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return commands; }
        }

        public IEnumerable<IViewModel> ChildViewModels
        {
            get { throw new NotImplementedException(); }
        }

        public string CurrentRegion
        {
            get { throw new NotImplementedException(); }
        }

        public string Header
        {
            get { throw new NotImplementedException(); }
        }

        public NESMachine TargetMachine
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
