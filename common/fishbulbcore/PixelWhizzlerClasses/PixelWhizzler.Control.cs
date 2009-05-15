using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NES.CPU.PPUClasses
{
	public partial class PixelWhizzler
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
            writer.Enqueue(_mirroring);
            writer.Enqueue(oneScreenMirrorOffset);
            writer.Enqueue((int)currentMirrorMask);

            for (int i = 0; i < 0x4000; i += 4)
            {

                writer.Enqueue((_vidRAM[i] << 24) |
                                    (_vidRAM[i + 1] << 16) |
                                    (_vidRAM[i + 2] << 8) |
                                    (_vidRAM[i + 3])
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
            _mirroring= state.Dequeue();
            oneScreenMirrorOffset= state.Dequeue();
            currentMirrorMask= state.Dequeue();

            int packedByte = 0;
            for (int i = 0; i < 0x4000; i += 4)
            {
                packedByte = state.Dequeue();
                _vidRAM[i] = (byte)(packedByte >> 24);
                _vidRAM[i + 1] = (byte)(packedByte >> 16);
                _vidRAM[i + 2] = (byte)(packedByte >> 8);
                _vidRAM[i + 3] = (byte)(packedByte);
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

            for (int i = 0; i < _palette.Length; i += 4)
            {
                packedByte = state.Dequeue();
                _palette[i] = (byte)(packedByte >> 24);
                _palette[i + 1] = (byte)(packedByte >> 16);
                _palette[i + 2] = (byte)(packedByte >> 8);
                _palette[i + 3] = (byte)(packedByte);
            }


        }
    }
}
