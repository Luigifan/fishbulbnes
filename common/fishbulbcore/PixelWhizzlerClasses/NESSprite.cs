using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public struct NESSprite
    {

        public int YPosition;

        public int XPosition;


        public int SpriteNumber;

        public bool Foreground;

        public bool IsVisible;

        //WhissaSpritePixel(_ppu.PatternTableIndex, currentSprites[i].TileIndex, xPos, yLine, currentSprites[i].AttributeByte, currentSprites[i].FlipX, currentSprites[i].FlipY);

        public int TileIndex;


        public int AttributeByte;

        public bool FlipX;

        public bool FlipY;

        public bool Changed;

    }
}
