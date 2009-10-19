using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo.Interaction;
using System.ComponentModel;
using System.Windows;
using System.IO;
using System.Reflection;

namespace InstiBulb.GameDisplay
{
    public enum DisplayContexts
    {
        None,
        OpenGL,
        OpenGLSmooth,
        Wpf_WriteableBitmap,
        SlimDX,
        XNA
    }

    public class DisplayContextSwitcher : INotifyPropertyChanged
    {
        

        private DisplayContexts currentDisplayContext;

        NESDisplay display;



        private void EnumerateDisplays(string folderName)
        {
            contextNames = new List<string>();
            loadedDisplayContexts = new Dictionary<string, Type>();
            SetupPlugin(Assembly.GetExecutingAssembly());
            foreach (string fileName in Directory.GetFiles(folderName, "*.dll"))
            {
                SetupPlugin(fileName);
            }
            NotifyPropertyChanged("ContextNames");
        }

        private void SetupPlugin(string fileName)
        {
            try
            {

                SetupPlugin(Assembly.LoadFrom(fileName));
            }
            catch (BadImageFormatException)
            {
            }
        }

        private void SetupPlugin(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsPublic && t.IsDefined(typeof(NES.CPU.nitenedo.Interaction.NESDisplayPluginAttribute), true))
                {
                    try
                    {

                        IDisplayContext renderer = (IDisplayContext)Activator.CreateInstance(t);
                        if (renderer != null)
                        {

                            contextNames.Add(renderer.DisplayName);
                            loadedDisplayContexts.Add(renderer.DisplayName, t);

                        }
                    }
                    catch (Exception)
                    {
                    }
                }

            }

        }

        public NESDisplay Display
        {
            get { return display; }
            set { display = value; }
        }

        private List<string> contextNames; //  = new string[] { "OpenGL", "OpenGL RGB", "WPF WriteableBitmap", "SlimDX", "XNA" }; //

        Dictionary<string, Type> loadedDisplayContexts;

        public string[] ContextNames
        {
            get { return contextNames.ToArray<string>(); }
            set { contextNames = value.ToList<string>(); }
        }

        private string currentContext = "";

        public string CurrentContextName
        {
            get { return currentContext; }
            set { currentContext = value;

            UpdateDisplayContext();
            }
        }

        public void UpdateDisplayContext()
        {
            Type c;
            if (loadedDisplayContexts.TryGetValue(currentContext, out c))
            {
                bool pauseState = display.Target.Paused;
                display.Target.Paused = true;
                display.DestroyContext();

                IDisplayContext renderer = (IDisplayContext)Activator.CreateInstance(c);


                display.SetupRenderer(renderer);

                display.Target.PPU.PixelWidth = display.Context.PixelWidth;

                display.Target.PPU.LoadPalABGR();
                if (display.Context.PixelFormat == NES.CPU.nitenedo.Interaction.NESPixelFormats.RGB)
                {
                    display.Target.PPU.LoadPalRGBA();

                }

                if (display.Context.PixelFormat == NESPixelFormats.Indexed)
                {
                    display.Target.PPU.FillRGB = false;
                }
                else
                {
                    display.Target.PPU.FillRGB = true;
                }

                PropertyChanged(this, new PropertyChangedEventArgs("CurrentContextName"));
                PropertyChanged(this, new PropertyChangedEventArgs("ContextPropertiesPanel"));

                display.Target.Paused = pauseState;
            }
        }

        public DisplayContextSwitcher(NESDisplay display) 
        {
            EnumerateDisplays(Environment.CurrentDirectory);
            this.display = display;
        }

        UIElement contextPropertiesPanel = new System.Windows.Controls.Border();

        public UIElement ContextPropertiesPanel
        {
            get { return contextPropertiesPanel; }
            set { contextPropertiesPanel = value; }
        }

        public DisplayContexts CurrentDisplayContext
        {
            get { return currentDisplayContext; }
            set {
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
