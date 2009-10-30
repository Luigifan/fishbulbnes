//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SlimDX.Direct3D9;
//using SlimDX;

//namespace SlimDXBindings.Viewer.Filters
//{
//    class PostProcessEffect
//    {
//        private Quadrangle _Quadrangle = null;
//        internal Device _GraphicsDevice = null;

//        protected GraphicsDevice GraphicsDevice
//        {
//            get
//            {
//                return _GraphicsDevice;
//            }
//        }

//        protected PostProcessEffect(GraphicsDevice graphicsDevice)
//        {
//            _GraphicsDevice = graphicsDevice;
//            _Quadrangle = Quadrangle.Find(graphicsDevice);
//        }

//        public abstract void PostProcess(Texture2D sourceTexture,
//            RenderTarget2D result);

//        protected void GetTargetDimensions(Surface target,
//            out Vector2 dimentions)
//        {
//            dimentions.X = target.Width;
//            dimentions.Y = target.Height;
//        }

//        protected void DrawQuad(Effect effect)
//        {
//            _Quadrangle.Draw(effect);
//        }

//    }
//}
