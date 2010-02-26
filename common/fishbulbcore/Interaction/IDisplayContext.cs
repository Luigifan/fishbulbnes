using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.nitenedo.Interaction
{
    public class InvalidDisplayContextException : Exception
    {
        public InvalidDisplayContextException(string s) : base(s) { }
        public InvalidDisplayContextException(string s, Exception innerException) : base(s, innerException) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class NESDisplayPluginAttribute : Attribute
    {
    }

    public enum NESPixelFormats
    {
        RGB,
        BGR,
        Indexed
    }


    public enum CallbackType
    {
        None,
        NoArgs,
        Array,
        IntPtr
    }

    /// <summary>
    /// Defines what the main windows interaction with the current renderer
    /// </summary>
    public interface IDisplayContext
    {

        NESMachine AttachedMachine { get; set; }

        CallbackType DesiredCallback { get;  }

        int PixelWidth { get; }

        NESPixelFormats PixelFormat { get; set; }

        void CreateDisplay();

        void TearDownDisplay();

        void UpdateNESScreen();

        void UpdateNESScreen(int[] pixels);

        void UpdateNESScreen(IntPtr pixelData);

        void DrawDefaultDisplay();

        void ToggleFullScreen();

        void SetPausedState(bool state);

        object UIControl { get; }

        object PropertiesPanel { get; }

        string DisplayName { get; }

    }
}
