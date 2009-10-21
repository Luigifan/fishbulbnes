using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InstiBulb.Effects
{
    internal static class Global
{
    /// <summary>
    /// Helper method for generating a "pack://" URI for a given relative file based on the
    /// assembly that this class is in.
    /// </summary>
    public static Uri MakePackUri(string relativeFile)
    {
        string uriString = "pack://application:,,,/" + AssemblyShortName + ";component/" + relativeFile;
        return new Uri(uriString);
    }

    private static string AssemblyShortName
    {
        get
        {
            if (_assemblyShortName == null)
            {
                Assembly a = typeof(Global).Assembly;

                // Pull out the short name.
                _assemblyShortName = a.ToString().Split(',')[0];
            }

            return _assemblyShortName;
        }
    }

    private static string _assemblyShortName;
}

    public class RasterizeEffect : ShaderEffect
    {
        static RasterizeEffect()
        {
            _pixelShader.UriSource = Global.MakePackUri("Effects/Simple.ps");
            
        }

        public RasterizeEffect()
        {
            this.PixelShader = _pixelShader;



            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(PaletteProperty);

            var paletteBmp = new WriteableBitmap(64, 1, 96, 96, PixelFormats.Pbgra32, null);

            Palette = new ImageBrush(paletteBmp);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public Brush Palette
        {
            get { return (Brush)GetValue(PaletteProperty); }
            set { SetValue(PaletteProperty, value); }
        }


        // Brush-valued properties turn into sampler-property in the shader.
        // This helper sets "ImplicitInput" as the default, meaning the default
        // sampler is whatever the rendering of the element it's being applied to is.
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(RasterizeEffect), 0);

        public static readonly DependencyProperty PaletteProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Palette", typeof(RasterizeEffect), 1);


        private static PixelShader _pixelShader = new PixelShader();

    }
    
}
