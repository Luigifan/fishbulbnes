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

    /// <summary>
    /// Defines what the main windows interaction with the current renderer
    /// </summary>
    public interface IDisplayContext
    {
        int PixelWidth { get; }

        NESPixelFormats PixelFormat { get; set; }

        void CreateDisplay();

        void TearDownDisplay();

        void UpdateNESScreen(int[] pixels);

        void UpdateNESScreen(int[] pixels, int[] palette);

        void UpdateNESScreen(IntPtr pixelData);

        void DrawDefaultDisplay();

        void SetPausedState(bool state);

        object UIControl { get; }

        object PropertiesPanel { get; }

        string DisplayName { get; }

    }
}
