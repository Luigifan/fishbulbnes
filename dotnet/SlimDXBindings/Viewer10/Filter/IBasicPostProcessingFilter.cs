using System;
using SlimDX.Direct3D10;
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
        
        //fluent interface
        IFilterChainLink AddNeededResource(string name, string bindsTo);
        IFilterChainLink BindScalar(string name);
        IFilterChainLink SetStaticResource(string name, Resource res);
        IFilterChainLink ClearNeededResources();
        IFilterChainLink DontFeedNextStage();
    }
}
