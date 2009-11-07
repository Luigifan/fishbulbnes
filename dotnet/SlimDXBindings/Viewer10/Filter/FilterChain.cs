using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;

namespace SlimDXBindings.Viewer10.Filter
{
    public class FilterChain : List<IFilterChainLink>, IFilterChain, IDisposable
    {
        private TextureBuddy myTextureBuddy;

        public TextureBuddy MyTextureBuddy
        {
            get { return myTextureBuddy; }
            set { myTextureBuddy = value; }
        }

        List<string> inputs = new List<string>();

        public void RegisterInput(string name, string item)
        {
            inputs.Add(name);
        }

        private Texture2D result;

        public Texture2D Result
        {
            get { return result; }
        }

        public void Draw(Texture2D[] input)
        {
            if (dumpFiles)
            {
                for (int i = 0; i < input.Length; ++i)
                {
                    Texture2D tex = input[i];
                    Texture2D.ToFile(tex, ImageFileFormat.Dds, string.Format("c:\\00-{0}-filterChainInput.dds", inputs[i]) );
                }
            }

            for (int i = 0; i < this.Count; ++i)
            {
                foreach (KeyValuePair<string, string> pair in this[i].NeededResources)
                {
                    string s = pair.Key;
                    string resName = pair.Value;
                    if (inputs.Contains( s) )
                    {
                        this[i].SetShaderResource(resName, input[inputs.IndexOf(s)]);
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

            }
            result = this[this.Count - 1].results;
            if (dumpFiles)
                Texture2D.ToFile(result, ImageFileFormat.Dds, "c:\\99filterChainResult.dds");

            dumpFiles = false;
        }

        bool dumpFiles = false;

        public bool DumpFiles
        {
            get { return dumpFiles; }
            set { dumpFiles = value; }
        }

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
