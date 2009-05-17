using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.nitenedo.Interaction
{
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

        void DrawDefaultDisplay();

        void SetPausedState(bool state);

        object UIControl { get; }

        string PropertiesPanel { get; }

        string DisplayName { get; }

    }
}
