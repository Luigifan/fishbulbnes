using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SlimDX.Direct3D10;

namespace SlimDXBindings.Viewer10.Filter
{
    public class FilterChainLoaderException : Exception
    {
        public FilterChainLoaderException(string Message, Exception innerException)
            : base(Message, innerException)
        {
        }
    }

    public class FilterChainLoader
    {

        readonly Device device;

        public FilterChainLoader(Device device)
        {
            this.device = device;
        }

        public IFilterChain ReadFile(string fileName)
        {
            FilterChain newChain = new FilterChain();
            
            newChain.MyTextureBuddy = new TextureBuddy(device);

            XDocument doc = XDocument.Load(System.Xml.XmlReader.Create(fileName));

            // load up resources
            ReadResources(newChain, doc.Element("FilterChain"));

            // load up filters
            ReadFilters(newChain, doc.Element("FilterChain"));

            return newChain;
        }

        private void ReadFilters(FilterChain newChain, XElement doc)
        {
            var filters = from c in doc.Element("Filters").Descendants("Filter")
                          select new
                          {
                              Name = c.Attribute("Name").Value,
                              Height = int.Parse(c.Attribute("Height").Value),
                              Width = int.Parse(c.Attribute("Width").Value),
                              EffectName = c.Attribute("Effect").Value,
                              Technique = c.Attribute("Technique").Value,
                              Inputs = c.Element("Inputs").Descendants("Input")
                          };

            foreach (var c in filters)
            {
                BasicPostProcessingFilter newFilter = new BasicPostProcessingFilter(device, c.Name, c.Width, c.Height, c.EffectName, c.Technique);

                // set up this filters inputs
                
                // local texture resources
                var staticInputs = from g in c.Inputs
                             where g.Attribute("Type").Value == "Resource"
                             select  new
                             {
                                 ResourceName = g.Attribute("ResourceName").Value,
                                 VariableName = g.Attribute("EffectVariable").Value,
                             };

                foreach (var res in staticInputs)
                {
                    newFilter.SetShaderResource(res.VariableName, newChain.MyTextureBuddy.GetTexture<Texture2D>(res.ResourceName));
                }

                // texture resources bound to previous links in chain
                var chainInputs = from g in c.Inputs
                                  where g.Attribute("Type").Value == "FilterOutput"
                                   select new
                                   {
                                       Filter = g.Attribute("Filter").Value,
                                       VariableName = g.Attribute("EffectVariable").Value,
                                   };

                foreach (var res in chainInputs)
                {
                    newFilter.AddNeededResource(res.Filter, res.VariableName);
                }

                // scalars 
                var scalarInputs = from g in c.Inputs
                                  where g.Attribute("Type").Value == "Scalar"
                                  select new
                                  {
                                      Source = g.Attribute("Source").Value,
                                      VariableName = g.Attribute("EffectVariable").Value,
                                  };

                foreach (var res in scalarInputs)
                {
                    newFilter.BindScalar(res.VariableName);
                    // TODO
                    // newChain.SetupBoundVariable(newFilter.FilterName, res.Source);
                }


                newChain.Add(newFilter);
            }
        }

        private static void ReadResources(FilterChain newChain, XElement doc)
        {
            var resourceDefs = from c in doc.Element("Resources").Descendants("Resource")
                               select new { Name = c.Attribute("Name").Value, Source = c.Attribute("Source").Value };

            foreach (var k in resourceDefs)
            {
                string[] sourceSplits = k.Source.Split(new char[] { ':' });
                if (sourceSplits.Length < 2)
                {
                    throw new FilterChainLoaderException("Invalid Resource.Source Attribute", null);
                }

                switch (sourceSplits[0])
                {
                    case "nes":
                        newChain.RegisterInput(k.Name, sourceSplits[1]);
                        break;
                    case "file":
                        newChain.MyTextureBuddy.FromFile(sourceSplits[1]);
                        break;
                    case "sys":
                        if (sourceSplits[1] == "noise")
                        {
                            newChain.MyTextureBuddy.CreateNoiseMap2D(k.Name, int.Parse(sourceSplits[2]));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
