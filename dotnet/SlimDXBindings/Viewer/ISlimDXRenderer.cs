using System;
using NES.CPU.nitenedo.Interaction;
namespace SlimDXBindings.Viewer
{
    public interface ISlimDXRenderer : IDisposable
    {
        void InitializeScene();
        void Render();
        void UpdateTime();
        NESPixelFormats PixelFormat { get; }
    }
}
