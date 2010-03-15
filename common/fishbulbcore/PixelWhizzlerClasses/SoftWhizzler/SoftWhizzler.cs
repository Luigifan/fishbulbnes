using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.PPUClasses;

namespace NES.CPU.PixelWhizzlerClasses.SoftWhizzler
{
    public class SoftWhizzler : PixelWhizzler
    {
        protected override void DrawPixel()
        {

            byte tilePixel = _tilesAreVisible ? GetNameTablePixel() : (byte)0;
            bool foregroundPixel =false;
            byte spritePixel = _spritesAreVisible ? GetSpritePixel(out foregroundPixel) : (byte)0;

            if (!hitSprite && spriteZeroHit && tilePixel != 0)
            {
                hitSprite = true;
                _PPUStatus = _PPUStatus | 0x40;
            }

            int pixel = (foregroundPixel || (tilePixel == 0 && spritePixel != 0)) ? spritePixel : tilePixel;
            rgb32OutBuffer[vbufLocation] = pal[_palette[pixel]];
        }

        public override void FillBuffer()
        {

        }

        /// <summary>
        /// Called by SetByte() for writes to palette ram
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        protected override void WriteToNESPalette(int address, byte data)
        {
            int palAddress = (address) & 0x1F;
            _palette[palAddress] = data;
            // rgb32OutBuffer[255 * 256 + palAddress] = data;
            if ((_PPUAddress & 0xFFEF) == 0x3F00)
            {
                _palette[(palAddress ^ 0x10) & 0x1F] = data;
                // rgb32OutBuffer[255 * 256 + palAddress ^ 0x10] = data;
            }
        }

        protected override void ClearNESPalette()
        {
            
        }


    }
}
