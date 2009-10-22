using System;
using NES.CPU.nitenedo.Interaction;
namespace SlimDXBindings.Viewer
{
    public interface ISlimDXRenderer : IDisposable
    {
        void InitializeScene();
        void Render();
        NESPixelFormats PixelFormat { get; }
    }
}
