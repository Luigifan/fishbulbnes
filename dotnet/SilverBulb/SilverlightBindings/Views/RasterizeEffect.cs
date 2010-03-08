using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace SilverlightBindings.Views
{
    public class RasterizeEffect : ShaderEffect
    {

        //public static readonly DependencyProperty NesOutProperty =
        //       DependencyProperty.Register("NesOut", typeof(Brush),
        //       typeof(RasterizeEffect),
        //       new PropertyMetadata(null, ShaderEffect.PixelShaderSamplerCallback(0,SamplingMode.NearestNeighbor)));

        //public static readonly DependencyProperty PaletteProperty =
        //       DependencyProperty.Register("Palette", typeof(Brush),
        //       typeof(RasterizeEffect),
        //       new PropertyMetadata(null, ShaderEffect.PixelShaderSamplerCallback(1, SamplingMode.NearestNeighbor)));


        //public Brush NesOut
        //{
        //    get { return (Brush) GetValue(RasterizeEffect.NesOutProperty); }
        //    set { SetValue(RasterizeEffect.NesOutProperty, value); }
        //}

        //public Brush Palette
        //{
        //    get { return (Brush)GetValue(RasterizeEffect.PaletteProperty); }
        //    set { SetValue(RasterizeEffect.PaletteProperty, value); }
        //}

        private static PixelShader pixelShader;

        static RasterizeEffect()
        {
            pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"/SilverlightBindings;component/rasterize.ps", UriKind.RelativeOrAbsolute);
        }
        public RasterizeEffect()
        {
            this.PixelShader = pixelShader;
        }
    }
}
