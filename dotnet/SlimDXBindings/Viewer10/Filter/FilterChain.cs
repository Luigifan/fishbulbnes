using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;

namespace SlimDXBindings.Viewer10.Filter
{
    public class FilterChain : List<IFilterChainLink>, IFilterChain, IDisposable
    {
        private Texture2D result;

        public Texture2D Result
        {
            get { return result; }
        }

        public void Draw(Texture2D input)
        {
            if (dumpFiles)
                Texture2D.ToFile(input, ImageFileFormat.Dds, "c:\\00filterChainInput.dds");

            for (int i = 0; i < this.Count; ++i)
            {
                foreach (KeyValuePair<string, string> pair in this[i].NeededResources)
                {
                    string s = pair.Key;
                    string resName = pair.Value;
                    if (s == "input")
                    {
                        this[i].SetShaderResource(resName, input);
                    }
                    else
                    {
                        var p = (from BasicPostProcessingFilter filter in this 
                                 where filter.FilterName == s select filter).FirstOrDefault<BasicPostProcessingFilter>();
                        if (p != null)
                        {
                            this[i].SetShaderResource(resName, p.results);
                        }

                    }
                }
                this[i].ProcessEffect();
                if (dumpFiles)
                    Texture2D.ToFile(this[i].results, ImageFileFormat.Dds, "c:\\" + i.ToString() + this[i].FilterName + ".dds");

                if (this[i].FeedsNextStage)
                    input = this[i].results;
            }
            result = this[this.Count - 1].results;
            if (dumpFiles)
                Texture2D.ToFile(result, ImageFileFormat.Dds, "c:\\99filterChainResult.dds");

            dumpFiles = false;
        }

        bool dumpFiles = false;

        #region IDisposable Members

        public void Dispose()
        {
            if (result != null)
                result.Dispose(); 
            
            foreach (var p in this)
            {
                p.Dispose();
            }

        }

        #endregion

        public void SetVariable(string name, float constant)
        {
            foreach (BasicPostProcessingFilter b in this)
            {
                b.SetScalar(name, constant);
            }
        }

        public void SetResource(string name, Resource res)
        {
            foreach (BasicPostProcessingFilter b in this)
            {
                b.SetStaticResource(name, res);
            }
        }
    }
}
