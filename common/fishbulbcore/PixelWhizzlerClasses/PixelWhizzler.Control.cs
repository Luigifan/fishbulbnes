using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NES.CPU.PPUClasses
{
	public partial class PixelWhizzler : NES.CPU.PixelWhizzlerClasses.IPPU
	{

        private bool _isDebugging;

        public bool IsDebugging
        {
            get { return _isDebugging; }
            set { _isDebugging = value; }
        }


        public void Initialize()
        {
            _PPUAddress = 0;
            _PPUStatus = 0;
            _PPUControlByte0 = 0;
            _PPUControlByte1 = 0;
            _hScroll = 0;
            scanlineNum = 0;
            scanlinePos = 0;
            _spriteAddress = 0;
        }

        public void WriteState(Queue<int> writer)
        {

            writer.Enqueue(_PPUStatus);
            writer.Enqueue(_PPUControlByte0);
            writer.Enqueue(_hScroll);
            writer.Enqueue(_vScroll);
            writer.Enqueue(scanlineNum);
            writer.Enqueue(scanlinePos);
            writer.Enqueue(currentYPosition);
            writer.Enqueue(currentXPosition);
            writer.Enqueue(nameTableIndex);
            writer.Enqueue(_backgroundPatternTableIndex);


            writer.Enqueue( patternEntry );
            writer.Enqueue( patternEntryByte2);
            writer.Enqueue(currentAttributeByte);
            writer.Enqueue( xNTXor );
            writer.Enqueue( yNTXor );
            writer.Enqueue( fetchTile ? 0 : 1);
            writer.Enqueue (xPosition);
            writer.Enqueue(yPosition);


            writer.Enqueue(lastcpuClock);
            writer.Enqueue(vbufLocation);


            for (int i = 0; i < 0x4000; i += 4)
            {
                
                writer.Enqueue(chrRomHandler.GetPPUByte(0, i) << 24 |
                                    (chrRomHandler.GetPPUByte(0, i + 1) << 16) |
                                    (chrRomHandler.GetPPUByte(0, i + 2) << 8) |
                                    (chrRomHandler.GetPPUByte(0, i + 3) << 0) 
                                    
                        );
            }
            writer.Enqueue(_spriteAddress);

            for (int i = 0; i < 0x100; i += 4)
            {

                writer.Enqueue((spriteRAM[i] << 24) |
                                    (spriteRAM[i + 1] << 16) |
                                    (spriteRAM[i + 2] << 8) |
                                    (spriteRAM[i + 3])
                        );
            }

            for (int i = 0; i < pal.Length; i += 4)
            {

                writer.Enqueue((pal[i] << 24) |
                                    (pal[i + 1] << 16) |
                                    (pal[i + 2] << 8) |
                                    (pal[i + 3])
                        );
            }

            for (int i = 0; i < _palette.Length; i += 4)
            {

                writer.Enqueue((_palette[i] << 24) |
                                    (_palette[i + 1] << 16) |
                                    (_palette[i + 2] << 8) |
                                    (_palette[i + 3])
                        );
            }

        }

        public void ReadState(Queue<int> state)
        {
            _PPUStatus =  state.Dequeue();
            _PPUControlByte0 = state.Dequeue();
            _hScroll= state.Dequeue();
            _vScroll= state.Dequeue();
            scanlineNum= state.Dequeue();
            scanlinePos= state.Dequeue();
            currentYPosition= state.Dequeue();
            currentXPosition= state.Dequeue();
            nameTableIndex= state.Dequeue();
            _backgroundPatternTableIndex= state.Dequeue();
            //_mirroring= state.Dequeue();
            //oneScreenMirrorOffset= state.Dequeue();
            //currentMirrorMask= state.Dequeue();

            patternEntry = state.Dequeue();
            patternEntryByte2 = state.Dequeue();
            currentAttributeByte = state.Dequeue();
            xNTXor = state.Dequeue();
            yNTXor = state.Dequeue();
            fetchTile = (state.Dequeue() == 1);
            xPosition = state.Dequeue();
            yPosition = state.Dequeue();

            lastcpuClock = state.Dequeue();
            vbufLocation = state.Dequeue();

            int packedByte = 0;
            for (int i = 0; i < 0x4000; i += 4)
            {
                packedByte = state.Dequeue();
                chrRomHandler.SetByte(0, i, (byte)(packedByte >> 24));
                chrRomHandler.SetByte(0, i + 1, (byte)(packedByte >> 16));
                chrRomHandler.SetByte(0, i + 2, (byte)(packedByte >> 8));
                chrRomHandler.SetByte(0, i + 3, (byte)(packedByte ));
            }

            _spriteAddress= state.Dequeue();

            for (int i = 0; i < 0x100; i += 4)
            {
                packedByte = state.Dequeue();
                spriteRAM[i] = (byte)(packedByte >> 24);
                spriteRAM[i + 1] = (byte)(packedByte >> 16);
                spriteRAM[i + 2] = (byte)(packedByte >> 8);
                spriteRAM[i + 3] = (byte)(packedByte);
            }

            for (int i = 0; i < pal.Length; i += 4)
            {
                packedByte = state.Dequeue();
                pal[i] = (byte)(packedByte >> 24);
                pal[i + 1] = (byte)(packedByte >> 16);
                pal[i + 2] = (byte)(packedByte >> 8);
                pal[i + 3] = (byte)(packedByte);
            }

            for (int i = 0; i < _palette.Length; i += 4)
            {
                packedByte = state.Dequeue();
                _palette[i] = (byte)(packedByte >> 24);
                _palette[i + 1] = (byte)(packedByte >> 16);
                _palette[i + 2] = (byte)(packedByte >> 8);
                _palette[i + 3] = (byte)(packedByte);
            }

            UnpackSprites();
            PreloadSprites(scanlineNum);

        }
    }
}
