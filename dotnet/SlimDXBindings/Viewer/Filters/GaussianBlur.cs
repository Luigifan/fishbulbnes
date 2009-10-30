//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SlimDX;
//using SlimDX.Direct3D9;
//using System.Reflection;

//namespace SlimDXBindings.Viewer.Filters
//{
//    public class GaussianBlur
//    {
//        Device device;

//        private Effect Effect = null;

//        float[] Parameter_Offsets = null;
//        float[] Parameter_Weights = null;
//        int Parameter_ShaderIndex = 0;

//        private float _StandardDeviation = 0.8f;
//        private float _Amplitude = 0.4f;
//        private bool _Horizontal = true;

//        private Vector2 TargetDimensions = new Vector2();

//        private bool _DirtyWeights = true;
//        private float[] Weights = new float[9];
//        private float[] Offsets = new float[9];

//        public bool Horizontal
//        {
//            get
//            {
//                return _Horizontal;
//            }
//            set
//            {
//                _Horizontal = value;
//                Parameter_ShaderIndex = (_Horizontal ? 0 : 1);
                
//            }
//        }

//        public float StandardDeviation
//        {
//            get
//            {
//                return _StandardDeviation;
//            }
//            set
//            {
//                if (_StandardDeviation != value)
//                {
//                    _DirtyWeights = true;
//                }
//                _StandardDeviation = value;
//            }
//        }

//        public float Amplitude
//        {
//            get
//            {
//                return _Amplitude;
//            }
//            set
//            {
//                if (_Amplitude != value)
//                {
//                    _DirtyWeights = true;
//                }

//                _Amplitude = value;

//            }
//        }

//    public GaussianBlur(Device device)
//    {
//        Effect = Effect.FromStream(device,
//                Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer.Filters.GaussianBlur.fx"),
//                ShaderFlags.None);

//        Parameter_SourceTexture = Effect.Parameters["SourceTexture"];

//        Parameter_Offsets = Effect.Parameters["Offsets"];
//        Parameter_Weights = Effect.Parameters["Weights"];
//        Parameter_ShaderIndex = Effect.Parameters["ShaderIndex"];
//    }

//    public override void PostProcess(Texture sourceTexture,
//        RenderToSurface result)
//    {
//        Parameter_SourceTexture.SetValue(sourceTexture);

//        if (_DirtyWeights)
//        {
//            if (_Horizontal)
//            {
                
//                Gaussian.Fill(Offsets, 1.0f / TargetDimensions.X,
//                Weights, 0, _StandardDeviation, _Amplitude);
//            }
//            else
//            {
//                Gaussian.Fill(Offsets, 1.0f / TargetDimensions.Y,
//                Weights, 0, _StandardDeviation, _Amplitude);
//            }

//            Parameter_Weights.SetValue(Weights);
//            Parameter_Offsets.SetValue(Offsets);

//            _DirtyWeights = false;
//        }



//        device.SetRenderTarget(0, surf);
//        _GraphicsDevice.Clear(Color.Black);

//        DrawQuad(Effect);

//        _GraphicsDevice.SetRenderTarget(0, null);
//    }

//}
