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
                if (this[i].FeedsNextStage)
                    input = this[i].results;
            }
            result = this[this.Count - 1].results;
        }

        #region IDisposable Members

        public void Dispose()
        {
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
    }
}
