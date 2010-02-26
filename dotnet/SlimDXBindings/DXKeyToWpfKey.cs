using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SlimDXBindings
{
    public class DXKeyToWpfKey
    {
        public SlimDX.DirectInput.Key? ConvertWPFKey(System.Windows.Input.Key key)
        {
            SlimDX.DirectInput.Key? result = null; 

            switch (key)
            {
                case System.Windows.Input.Key.Back:
                    result = SlimDX.DirectInput.Key.Backspace;
                    break;
                case System.Windows.Input.Key.Tab:
                    result = SlimDX.DirectInput.Key.Tab;
                    break;
                case System.Windows.Input.Key.Return:
                    result = SlimDX.DirectInput.Key.Return;
                    break;
                case System.Windows.Input.Key.Pause:
                    result = SlimDX.DirectInput.Key.Pause;
                    break;

                case System.Windows.Input.Key.Space:
                    result = SlimDX.DirectInput.Key.Space;
                    break;
                case System.Windows.Input.Key.PageUp:
                    result = SlimDX.DirectInput.Key.PageUp;
                    break;
                case System.Windows.Input.Key.End:
                    result = SlimDX.DirectInput.Key.End;
                    break;
                case System.Windows.Input.Key.Home:
                    result = SlimDX.DirectInput.Key.Home;
                    break;
                case System.Windows.Input.Key.Left:
                    result = SlimDX.DirectInput.Key.LeftArrow;
                    break;
                case System.Windows.Input.Key.Up:
                    result = SlimDX.DirectInput.Key.UpArrow;
                    break;
                case System.Windows.Input.Key.Right:
                    result = SlimDX.DirectInput.Key.RightArrow;
                    break;
                case System.Windows.Input.Key.Down:
                    result = SlimDX.DirectInput.Key.DownArrow;
                    break;

                case System.Windows.Input.Key.Insert:
                    result = SlimDX.DirectInput.Key.Insert;
                    break;
                case System.Windows.Input.Key.Delete:
                    result = SlimDX.DirectInput.Key.Delete;
                    break;

                case System.Windows.Input.Key.D0:
                    result = SlimDX.DirectInput.Key.D0;
                    break;
                case System.Windows.Input.Key.D1:
                    result = SlimDX.DirectInput.Key.D1;
                    break;
                case System.Windows.Input.Key.D2:
                    result = SlimDX.DirectInput.Key.D2;
                    break;
                case System.Windows.Input.Key.D3:
                    result = SlimDX.DirectInput.Key.D3;
                    break;
                case System.Windows.Input.Key.D4:
                    result = SlimDX.DirectInput.Key.D4;
                    break;
                case System.Windows.Input.Key.D5:
                    result = SlimDX.DirectInput.Key.D5;
                    break;
                case System.Windows.Input.Key.D6:
                    result = SlimDX.DirectInput.Key.D6;
                    break;
                case System.Windows.Input.Key.D7:
                    result = SlimDX.DirectInput.Key.D7;
                    break;
                case System.Windows.Input.Key.D8:
                    result = SlimDX.DirectInput.Key.D8;
                    break;
                case System.Windows.Input.Key.D9:
                    result = SlimDX.DirectInput.Key.D9;
                    break;
                case System.Windows.Input.Key.A:
                    result = SlimDX.DirectInput.Key.A;
                    break;
                case System.Windows.Input.Key.B:
                    result = SlimDX.DirectInput.Key.B;
                    break;
                case System.Windows.Input.Key.C:
                    result = SlimDX.DirectInput.Key.C;
                    break;
                case System.Windows.Input.Key.D:
                    result = SlimDX.DirectInput.Key.D;
                    break;
                case System.Windows.Input.Key.E:
                    result = SlimDX.DirectInput.Key.E;
                    break;
                case System.Windows.Input.Key.F:
                    result = SlimDX.DirectInput.Key.F;
                    break;
                case System.Windows.Input.Key.G:
                    result = SlimDX.DirectInput.Key.G;
                    break;
                case System.Windows.Input.Key.H:
                    result = SlimDX.DirectInput.Key.H;
                    break;
                case System.Windows.Input.Key.I:
                    result = SlimDX.DirectInput.Key.I;
                    break;
                case System.Windows.Input.Key.J:
                    result = SlimDX.DirectInput.Key.J;
                    break;
                case System.Windows.Input.Key.K:
                    result = SlimDX.DirectInput.Key.K;
                    break;
                case System.Windows.Input.Key.L:
                    result = SlimDX.DirectInput.Key.L;
                    break;
                case System.Windows.Input.Key.M:
                    result = SlimDX.DirectInput.Key.M;
                    break;
                case System.Windows.Input.Key.N:
                    result = SlimDX.DirectInput.Key.N;
                    break;
                case System.Windows.Input.Key.O:
                    result = SlimDX.DirectInput.Key.O;
                    break;
                case System.Windows.Input.Key.P:
                    result = SlimDX.DirectInput.Key.P;
                    break;
                case System.Windows.Input.Key.Q:
                    result = SlimDX.DirectInput.Key.Q;
                    break;
                case System.Windows.Input.Key.R:
                    result = SlimDX.DirectInput.Key.R;
                    break;
                case System.Windows.Input.Key.S:
                    result = SlimDX.DirectInput.Key.S;
                    break;
                case System.Windows.Input.Key.T:
                    result = SlimDX.DirectInput.Key.T;
                    break;
                case System.Windows.Input.Key.U:
                    result = SlimDX.DirectInput.Key.U;
                    break;
                case System.Windows.Input.Key.V:
                    result = SlimDX.DirectInput.Key.V;
                    break;
                case System.Windows.Input.Key.W:
                    result = SlimDX.DirectInput.Key.W;
                    break;
                case System.Windows.Input.Key.X:
                    result = SlimDX.DirectInput.Key.X;
                    break;
                case System.Windows.Input.Key.Y:
                    result = SlimDX.DirectInput.Key.Y;
                    break;
                case System.Windows.Input.Key.Z:
                    result = SlimDX.DirectInput.Key.Z;
                    break;

                case System.Windows.Input.Key.Sleep:
                    result = SlimDX.DirectInput.Key.Sleep;
                    break;


                case System.Windows.Input.Key.F1:
                    result = SlimDX.DirectInput.Key.F1;
                    break;
                case System.Windows.Input.Key.F2:
                    result = SlimDX.DirectInput.Key.F2;
                    break;
                case System.Windows.Input.Key.F3:
                    result = SlimDX.DirectInput.Key.F3;
                    break;
                case System.Windows.Input.Key.F4:
                    result = SlimDX.DirectInput.Key.F4;
                    break;
                case System.Windows.Input.Key.F5:
                    result = SlimDX.DirectInput.Key.F5;
                    break;
                case System.Windows.Input.Key.F6:
                    result = SlimDX.DirectInput.Key.F6;
                    break;
                case System.Windows.Input.Key.F7:
                    result = SlimDX.DirectInput.Key.F7;
                    break;
                case System.Windows.Input.Key.F8:
                    result = SlimDX.DirectInput.Key.F8;
                    break;
                case System.Windows.Input.Key.F9:
                    result = SlimDX.DirectInput.Key.F9;
                    break;
                case System.Windows.Input.Key.F10:
                    result = SlimDX.DirectInput.Key.F10;
                    break;
                case System.Windows.Input.Key.F11:
                    result = SlimDX.DirectInput.Key.F11;
                    break;
                case System.Windows.Input.Key.F12:
                    result = SlimDX.DirectInput.Key.F12;
                    break;
                case System.Windows.Input.Key.F13:
                    result = SlimDX.DirectInput.Key.F13;
                    break;
                case System.Windows.Input.Key.F14:
                    result = SlimDX.DirectInput.Key.F14;
                    break;
                case System.Windows.Input.Key.F15:
                    result = SlimDX.DirectInput.Key.F15;
                    break;
                case System.Windows.Input.Key.NumLock:
                    result = SlimDX.DirectInput.Key.NumberLock;
                    break;
                case System.Windows.Input.Key.Scroll:
                    result = SlimDX.DirectInput.Key.ScrollLock;
                    break;
                case System.Windows.Input.Key.LeftShift:
                    result = SlimDX.DirectInput.Key.LeftShift;
                    break;
                case System.Windows.Input.Key.RightShift:
                    result = SlimDX.DirectInput.Key.RightShift;
                    break;
                case System.Windows.Input.Key.LeftCtrl:
                    result = SlimDX.DirectInput.Key.LeftControl;
                    break;
                case System.Windows.Input.Key.RightCtrl:
                    result = SlimDX.DirectInput.Key.RightControl;
                    break;
                case System.Windows.Input.Key.LeftAlt:
                    result = SlimDX.DirectInput.Key.LeftAlt;
                    break;
                case System.Windows.Input.Key.RightAlt:
                    result = SlimDX.DirectInput.Key.RightAlt;
                    break;
                case System.Windows.Input.Key.VolumeDown:
                    result = SlimDX.DirectInput.Key.VolumeDown;
                    break;
                case System.Windows.Input.Key.VolumeUp:
                    result = SlimDX.DirectInput.Key.VolumeUp;
                    break;

                case System.Windows.Input.Key.MediaStop:
                    result = SlimDX.DirectInput.Key.MediaStop;
                    break;
                case System.Windows.Input.Key.AbntC1:
                    result = SlimDX.DirectInput.Key.AbntC1;
                    break;
                case System.Windows.Input.Key.AbntC2:
                    result = SlimDX.DirectInput.Key.AbntC2;
                    break;

            }
            return result;

        }
    }
}
