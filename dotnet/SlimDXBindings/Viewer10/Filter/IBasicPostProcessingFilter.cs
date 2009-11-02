using System;
namespace SlimDXBindings.Viewer10.Filter
{
    public interface IFilterChainLink : IDisposable
    {
        bool FeedsNextStage { get; set; }
        string FilterName { get; set; }
        System.Collections.Generic.Dictionary<string, string> NeededResources { get; set; }
        void ProcessEffect();
        SlimDX.Direct3D10.Texture2D results { get; }
        void SetShaderResource(string variableName, SlimDX.Direct3D10.Resource resource);
        void SetScalar(string variableName, float constant);
    }
}
