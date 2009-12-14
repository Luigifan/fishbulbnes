using System;
using SlimDX.Direct3D10;
namespace SlimDXBindings.Viewer10.Filter
{
    public interface IFilterChain
    {
        void PostMessage(MessageForRenderer message);
        void NotifyScreenSize(int width, int height);
        void Draw(SlimDX.Direct3D10.Texture2D[] input);
        SlimDX.Direct3D10.Texture2D Result { get; }

    }
}
