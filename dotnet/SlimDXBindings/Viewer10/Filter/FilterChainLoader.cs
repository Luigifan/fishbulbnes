using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SlimDX.Direct3D10;
using System.IO;
using System.Reflection;
using SlimDXBindings.Viewer10.Helpers;
using SlimDXBindings.Viewer10.ControlPanel;
using Microsoft.Practices.Unity;

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
        IUnityContainer container;

        public FilterChainLoader(Device device, IUnityContainer container)
        {
            this.device = device;
            this.container = container;
        }

        public IFilterChain Load(Stream stream)
        {
            FilterChain newChain = new FilterChain();

            newChain.MyTextureBuddy = new TextureBuddy(device);
            newChain.MyEffectBuddy = new EffectBuddy(device);

            XDocument doc = XDocument.Load(System.Xml.XmlReader.Create(stream));

            // load up resources
            ReadResources(newChain, doc.Element("FilterChain"));

            // load up filters
            ReadFilters(newChain, doc.Element("FilterChain"));

            return newChain;
        }

        public IFilterChain ReadFile(string fileName)
        {
            IFilterChain newChain = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                newChain = Load(fs);
            }
            return newChain;
        }

        public IFilterChain ReadResource(string resName)
        {
            IFilterChain newChain = null;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resName))
            {
                newChain = Load(s);
            }
            return newChain;
        }

        static string GetOptionalAttrValue(XAttribute attr, string defValue)
        {
            if (attr == null)
            {
                return defValue;
            }
            return attr.Value;
        }

        private void ReadFilters(FilterChain newChain, XElement doc)
        {
            var filters = from c in doc.Element("Filters").Descendants("Filter")
                          select new
                          {
                              Name = c.Attribute("Name").Value,
                              FilterType = (c.Attribute("FilterType") == null) ? "Basic" : c.Attribute("FilterType").Value,
                              Height = int.Parse(c.Attribute("Height").Value),
                              Width = int.Parse(c.Attribute("Width").Value),
                              EffectName = GetOptionalAttrValue(c.Attribute("Effect"), "None"),
                              Technique = GetOptionalAttrValue(c.Attribute("Technique"), "None"),
                              Inputs = c.Element("Inputs") == null ? null : c.Element("Inputs").Descendants("Input"),
                              Element = c

                          };

            foreach (var c in filters)
            {
                IFilterChainLink newFilter = null;
                switch (c.FilterType)
                {
                    case "ToolStrip":
                        newFilter = BuildToolStrip(c.Element, newChain);
                        break;
                    case "WPFVisual":
                        newFilter = BuildVisual(c.Element, newChain);
                        break;
                    default:
                        newFilter = new BasicPostProcessingFilter(device, c.Name, c.Width, c.Height, c.EffectName, c.Technique, newChain.MyEffectBuddy);
                        break;
                }
                

                // set up this filters inputs
                
                // local texture resources
                if (c.Inputs != null)
                {
                    var staticInputs = from g in c.Inputs
                                       where g.Attribute("Type").Value == "Resource"
                                       select new
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
                }

                newChain.Add(newFilter);
            }
        }

        IFilterChainLink BuildVisual(XElement tstripElement, FilterChain chain)
        {

            var tsInfo = new
            {
                Name = tstripElement.Attribute("Name").Value,
                Height = int.Parse(tstripElement.Attribute("Height").Value),
                Width = int.Parse(tstripElement.Attribute("Width").Value),
                EffectName = GetOptionalAttrValue(tstripElement.Attribute("Effect"), "None"),
                Technique = GetOptionalAttrValue(tstripElement.Attribute("Technique"), "None"),
                Visual = tstripElement.Attribute("VisualName").Value
            };

            EmbeddableUserControl control = container.Resolve<EmbeddableUserControl>(tsInfo.Visual);
            var tex = chain.MyTextureBuddy.CreateVisualTexture(control, tsInfo.Width, tsInfo.Height);

            var w = new BasicPostProcessingFilter(device, tsInfo.Name, tsInfo.Width, tsInfo.Height, tsInfo.EffectName, tsInfo.Technique, chain.MyEffectBuddy);
            w.SetStaticResource("texture2d", tex);
            return w;
        }

        IFilterChainLink BuildToolStrip(XElement tstripElement, FilterChain chain)
        {

            var tsInfo = new
                          {
                              Name = tstripElement.Attribute("Name").Value,
                              Height = int.Parse(tstripElement.Attribute("Height").Value),
                              Width = int.Parse(tstripElement.Attribute("Width").Value),
                              Items = tstripElement.Element("Items").Descendants("Item")
                          };

            List<Texture2D> texture = new List<Texture2D>();

            foreach (var k in tsInfo.Items)
            {
                switch (k.Attribute("Type").Value)
                {
                    case "ImageFile":
                        texture.Add(chain.MyTextureBuddy.FromFile(k.Attribute("FileName").Value));
                        break;
                    case "FilterOutput":
                        string resName = k.Attribute("Filter").Value;
                        var res = from link in chain where link.FilterName == resName select link.results;
                        texture.Add(res.FirstOrDefault());
                        break;
                }
            }

            ToolStrip ts = new ToolStrip(device, tsInfo.Name, tsInfo.Width, tsInfo.Height, chain.MyTextureBuddy, tsInfo.Items.Count());

            //texture[0] = chain.MyTextureBuddy.FromFile(@"D:\Projects\FishBulb2010\dotnet\SlimDXBindings\Viewer10\Filter\Resources\NT0.png");
            //texture[1] = chain[0].results;// .MyTextureBuddy.FromFile(@"D:\Projects\FishBulb2010\dotnet\SlimDXBindings\Viewer10\Filter\Resources\NT1.png");
            ts.AddTextures(texture.ToArray());
            return ts;
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
