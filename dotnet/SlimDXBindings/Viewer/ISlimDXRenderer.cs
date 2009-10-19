using System;
namespace SlimDXBindings.Viewer
{
    public interface ISlimDXRenderer : IDisposable
    {
        void InitializeScene();
        void Render();
    }
}
