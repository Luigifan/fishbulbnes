using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace NES.CPU.Fastendo
{
    public partial class CPU2A03 
	{
        public void ResetCPU()
        {
            _statusRegister = 0x34;
            _operationCounter = 0;
            _stackPointer = 0xFD;
            setupticks();
            Ticks = 0;
            ProgramCounter = GetByte(0xFFFC) + GetByte(0xFFFD) * 0x100;
        }

        public void PowerOn()
        {
            // powers up with this state
            _statusRegister = 0x34;
            _stackPointer = 0xFD;
            _operationCounter = 0;
            setupticks();
            Ticks = 0;

            // wram initialized to 0xFF, with some exceptions
            // probably doesn't affect games, but why not?
            for (int i = 0; i < 0x800; ++i)
            {
                Rams[i] = 0xFF;
            }
            Rams[0x0008] = 0xF7;
            Rams[0x0009] = 0xEF;
            Rams[0x000A] = 0xDF;
            Rams[0x000F] = 0xBF;

             ProgramCounter = GetByte(0xFFFC) + GetByte(0xFFFD) * 0x100;
        }

        public void GetState(Queue<int> outStream)
        {
            outStream.Enqueue(_programCounter);
            outStream.Enqueue(_accumulator);
            outStream.Enqueue(_indexRegisterX);
            outStream.Enqueue(_indexRegisterY);
            outStream.Enqueue(_statusRegister);
            outStream.Enqueue(_stackPointer);
            for (int i = 0; i < 0x800; i+=4)
            {
                outStream.Enqueue( (Rams[i] << 24) | 
                                    (Rams[i+1] << 16) | 
                                    (Rams[i+2] << 8)  | 
                                    (Rams[i + 3])
                        );
            }
        }

        public void SetState(Queue<int> inStream)
        {
            _programCounter = inStream.Dequeue();
            _accumulator = inStream.Dequeue();
            _indexRegisterX = inStream.Dequeue();
            _indexRegisterY = inStream.Dequeue();
            _statusRegister = inStream.Dequeue();
            _stackPointer = inStream.Dequeue();
            int packedByte = 0;
            for (int i = 0; i < 0x800; i+=4)
            {
                packedByte = inStream.Dequeue();
                Rams[i] =(byte)(packedByte >> 24); 
                Rams[i + 1] =(byte)(packedByte >> 16); 
                Rams[i + 2] =(byte)(packedByte >> 8); 
                Rams[i + 3] =(byte)(packedByte); 

            }
        }

    }
}
