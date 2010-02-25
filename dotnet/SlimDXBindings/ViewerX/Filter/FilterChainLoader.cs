using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SlimDX.Direct3D10;
using System.IO;
using System.Reflection;
using SlimDXBindings.Viewer10.Helpers;
using Microsoft.Practices.Unity;
using InstibulbWpfUI;
using System.Windows;

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

        public IFilterChain Load(Stream stream, FakeEventMapper mapper)
        {
            FilterChain newChain = new FilterChain();
            newChain.EventMapper = mapper;
            newChain.MyTextureBuddy = new TextureBuddy(device);
            newChain.MyEffectBuddy = new EffectBuddy(device);

            XDocument doc = XDocument.Load(System.Xml.XmlReader.Create(stream));

            // load up resources
            ReadResources(newChain, doc.Element("FilterChain"));

            // load up filters
            ReadFilters(newChain, doc.Element("FilterChain"));

            return newChain;
        }

        public IFilterChain ReadFile(string fileName, FakeEventMapper mapper)
        {
            IFilterChain newChain = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                newChain = Load(fs, mapper);
            }
            return newChain;
        }

        public IFilterChain ReadResource(string resName, FakeEventMapper mapper)
        {
            IFilterChain newChain = null;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resName))
            {
                newChain = Load(s, mapper);
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
                    case "MouseTest":
                        newFilter = BuildMouseTest(c.Element, newChain);
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
            var list = from item in newChain where item is ISendsMessages select item;
            if (list != null && list.Count() > 0)
            {
                foreach (ISendsMessages s in list)
                {
                    s.MessagePoster = newChain.PostMessage;
                }
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
            WPFVisualTexture tex = chain.MyTextureBuddy.CreateVisualTexture(control, tsInfo.Width, tsInfo.Height);

            var controlList = tstripElement.Element("Elements").Descendants("Element");
            foreach (var cont in controlList)
            {
                string name = cont.Attribute("Name").Value;
                FrameworkElement elem = container.Resolve<FrameworkElement>(name);
                tex.SwappableControls.Add(elem);
            }

            FakeEventThrower thrower = new FakeEventThrower()
            {
                DrawArea = new double[4] { 0, 0, 1.0, 240.0/256.0},
                AllowingEvents = true,
            };

            thrower.ThrownTypes.Add(FakedEventTypes.MOUSECLICK);
            thrower.ThrownTypes.Add(FakedEventTypes.MOUSEDOUBLECLICK);
            thrower.ThrownTypes.Add(FakedEventTypes.MOUSEDOWN);
            thrower.ThrownTypes.Add(FakedEventTypes.MOUSEUP);
            thrower.ThrownTypes.Add(FakedEventTypes.MOUSEENTER);
            thrower.ThrownTypes.Add(FakedEventTypes.MOUSELEAVE);
            thrower.ThrownTypes.Add(FakedEventTypes.KEYPRESS);
            
            //thrower.ThrownTypes.Add(FakedEventTypes.MOUSEMOVE);
            chain.EventMapper.AddEventThrower(thrower);

            var w = new WpfEmbeddedControl(device, tsInfo.Name, tsInfo.Width, tsInfo.Height, tsInfo.EffectName, tsInfo.Technique, chain.MyEffectBuddy, thrower, tex, "texture2d");
            return w;
        }


        IFilterChainLink BuildMouseTest(XElement tstripElement, FilterChain chain)
        {

            var tsInfo = new
            {
                Name = tstripElement.Attribute("Name").Value,
                Height = int.Parse(tstripElement.Attribute("Height").Value),
                Width = int.Parse(tstripElement.Attribute("Width").Value),
                EffectName = GetOptionalAttrValue(tstripElement.Attribute("Effect"), "None"),
                Technique = GetOptionalAttrValue(tstripElement.Attribute("Technique"), "None"),
            };

            var w = new MouseTestingFilter(device, tsInfo.Name, tsInfo.EffectName, tsInfo.Technique, chain.MyEffectBuddy);
            w.BindScalar("mousePosition");
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
            List<string> commands = new List<string>();
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
                commands.Add(k.Attribute("Command").Value);
            }
            FakeEventThrower thrower = new FakeEventThrower();

            thrower.AllowingEvents = true;
            thrower.ThrownTypes.Add(FakedEventTypes.MOUSECLICK);
            thrower.DrawArea[0] = 0.0;
            thrower.DrawArea[1] = 240.0 / 256.0; // top
            thrower.DrawArea[2] = 1; // width
            thrower.DrawArea[3] = 16 / 256.0; // height

            // chain.EventMapper.AddEventThrower(thrower);
            
            ToolStrip ts = new ToolStrip(device, tsInfo.Name, tsInfo.Width, tsInfo.Height, chain.MyTextureBuddy, tsInfo.Items.Count(), thrower);
            //texture[0] = chain.MyTextureBuddy.FromFile(@"D:\Projects\FishBulb2010\dotnet\SlimDXBindings\Viewer10\Filter\Resources\NT0.png");
            //texture[1] = chain[0].results;// .MyTextureBuddy.FromFile(@"D:\Projects\FishBulb2010\dotnet\SlimDXBindings\Viewer10\Filter\Resources\NT1.png");
            ts.AddTextures(texture.ToArray());
            ts.Commands = commands;
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
