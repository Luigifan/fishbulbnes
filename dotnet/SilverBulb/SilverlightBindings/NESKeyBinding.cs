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

namespace SilverlightBindings
{
    public enum PadValues : int
    {
        A = 1,
        B = 2,
        Select = 4,
        Start = 8,
        Up = 16,
        Down = 32,
        Left = 64,
        Right = 128,
        FullScreen = 256,
    }

    public class NesKeyBinding
    {
        public PadValues BoundValue { get; set; }
        public Key Key { get; set; }
    }
}
