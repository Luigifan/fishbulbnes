using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D10;
using System.Drawing;
using SlimDXBindings.Viewer10.Helpers;
using InstibulbWpfUI;

namespace SlimDXBindings.Viewer10.Filter
{
    public class WpfEmbeddedControl : BasicPostProcessingFilter, IGetsMessages, IAmResizable
    {
        readonly FakeEventThrower thrower;
        readonly WPFVisualTexture visTexture;

        public WpfEmbeddedControl(Device device, string name, int Width, int Height, string shader, string technique, EffectBuddy effectBuddy, FakeEventThrower thrower, WPFVisualTexture visTexture, string resourceName)
            :base(device, name, Width, Height, shader, technique, effectBuddy)
        {
            this.thrower = thrower;
            this.visTexture = visTexture;
            this.thrower.FakeThisEvent +=  new EventHandler<FakeEventArgs>(runFakeEvent)  ;
            this.BindScalar("controlVisibility");
            this.SetStaticResource(resourceName, visTexture);
        }

        void runFakeEvent(object o, FakeEventArgs args)
        {
            if (this.visTexture.EmbeddedControl != null)
                this.visTexture.EmbeddedControl.HandleEvent(o, args);
        }

        #region IGetsMessages Members

        float currentVisibility = 0;

        public void RecieveMessage(MessageForRenderer message)
        {
            string[] msgParts = message.Message.Split(new char[] {':'} );
            switch (msgParts[0])
            {
                case "Show":
                    if (msgParts.Length > 1)
                    {
                        int control = int.Parse(msgParts[1]);
                        visTexture.SwapControl(control);
                    }
                    this.thrower.AllowingEvents = isShown = true;
                    break;
                case "Hide":
                    this.thrower.AllowingEvents = isShown = false;
                    break;

            }
            isUpdated = true;
        }
        
        public override void Update()
        {

            if (visTexture.IsDirty && currentVisibility > 0)
            {
                visTexture.UpdateVisual();
                isUpdated = true;
            }

            if (isShown && currentVisibility < 1)
            {
                currentVisibility += 0.05f;
                isUpdated = true;
                this.SetScalar("controlVisibility", currentVisibility);
            }
            if (!isShown && currentVisibility > 0)
            {
                currentVisibility -= 0.05f;
                isUpdated = true;
                this.SetScalar("controlVisibility", currentVisibility);
            }
            //Console.WriteLine(string.Format("WpfVisualChainLink is updated: {0}", isUpdated));
        }

        public override void AfterDraw()
        {
            if ((isShown && currentVisibility >= 1) || (!isShown && currentVisibility < 0))
            {
                isUpdated = false;
            }
        }

        #endregion

        #region IAmResizable Members

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public void Resize(int width, int height)
        {
            visTexture.Width = width;
            visTexture.Height = height;
            visTexture.MakeDirty();


            base.UpdateSize(width, height);
            // make a new quad, with new texture coords
            base.BuildQuad(new Vector4(-1,1,1,-1), new Vector4(0,0,(float)width/2048.0f,(float)height/2048f));
            //base.BuildQuad(edgePositions, Vector4 texCoords);
            
        }

        #endregion
    }
}
