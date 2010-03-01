﻿using System;
namespace InstiBulb.ThreeDee
{
    public interface IMenuIconItem
    {
        System.Windows.UIElement Billboard { get; set; }
        double DesiredRadius { get; set; }
        bool IsActivatable { get; set; }
        System.IO.Stream MakePNG(int width, int height, System.Windows.UIElement canvas, int dpi);
        
        void Rebuild();
    }
}