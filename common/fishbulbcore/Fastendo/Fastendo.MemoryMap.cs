using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo;
using NES.CPU.Machine.Carts;
using NES.CPU.Machine.BeepsBoops;
using NES.CPU.PixelWhizzlerClasses;

namespace NES.CPU.Fastendo
{
    public partial class CPU2A03
    {
        // full ram map
        byte[] Rams = new byte[0x2000];

        // custom ram handlers
        private IPPU _pixelWhizzler;
        private IClockedMemoryMappedIOElement _cart;
        private InputHandler _padOne ;
        private InputHandler _padTwo;

        IClockedMemoryMappedIOElement soundBopper;

        public IClockedMemoryMappedIOElement SoundBopper
        {
            get { return soundBopper; }
            set { 
                soundBopper = value;
                soundBopper.NMIHandler = irqUpdater;
            }
        }

        MachineEvent nmiHandler;

        void NMIHandler()
        {
            _handleNMI = true;
        }

        MachineEvent irqUpdater;

        void IRQUpdater()
        {
            _handleIRQ = soundBopper.IRQAsserted | _cart.IRQAsserted;
        }

        public InputHandler PadOne
        {
            get { return _padOne; }
            set { _padOne = value; }
        }

        public InputHandler PadTwo
        {
            get { return _padTwo; }
            set { _padTwo = value; }
        }


        public IClockedMemoryMappedIOElement Cart
        {
            get { return _cart; }
            set { _cart = value;
            _cart.NMIHandler = irqUpdater;
            }
        }

        public IPPU PixelWhizzler
        {
            get { return _pixelWhizzler; }
            set { _pixelWhizzler = value;
            _pixelWhizzler.NMIHandler = nmiHandler;
            }
        }

        public void LoadBytes(int offset, int[] bytes)
        {
            Array.Copy(bytes, 0, Rams, offset, bytes.Length);
        }

        public void LoadBytes(int offset, int[] bytes, int length)
        {
            Array.Copy(bytes, 0, Rams, offset, length);
        }

        private int _stackPointer = 0xFF;

        public int StackPointer
        {
            get { return _stackPointer ; }
            //set { _stackPointer = 0x100 + (value & 0xFF); }
        }

        public void PushStack(int data)
        {
            Rams[_stackPointer + 0x100] = (byte)data ;
            _stackPointer--;
            if (_stackPointer < 0x00)
            {
                _stackPointer = 0xFF;
            }
        }

        public int PopStack()
        {
            _stackPointer++;
            if (_stackPointer > 0xFF)
            {
                _stackPointer = 0x00;
            }
            return Rams[_stackPointer + 0x100] & 0xFF;
        }

        public int GetByte()
        {
            DataBus = GetByte(AddressBus);
            return DataBus;
        }

        public int GetByte(int address)
        {
            int result=0;


            // check high byte, find appropriate handler
            switch (address & 0xF000)
            {
                // zeropage
                case 0x0000:
                case 0x1000:
                    if (address < 0x800)
                        result = Rams[address];
                    else
                        result = address >> 8;
                    break;
                case 0x2000:
                case 0x3000:
                    result = _pixelWhizzler.GetByte(clock, address);
                    break;
                // other io ports (sound, joystick, dma sprite xfer)
                case 0x4000:
                    switch (address)
                    {
                        case 0x4016:
                            result = _padOne.GetByte(clock, address);
                            break;
                        case 0x4017:
                            result = _padTwo.GetByte(clock, address);
                            break;
                        case 0x4015:
                            result = soundBopper.GetByte(clock, address);
                            break;
                        default:
                            // return open bus?
                            result = address >> 8;
                            break;
                    }
                    break;
                    
                case 0x5000:
                    // ??
                    result = address >> 8;
                    break;
                // cart saveram 
                case 0x6000:
                case 0x7000:
                // cart 
                case 0x8000:
                case 0x9000:
                case 0xA000:
                case 0xB000:
                case 0xC000:
                case 0xD000:
                case 0xE000:
                case 0xF000:
                    result = _cart.GetByte(clock, address);
                    break;
                //ppu owns this part of the map
                default:
                    throw new Exception("Bullshit!");
                // cart sram, lives here

            }
            if (_cheating && memoryPatches.ContainsKey(address))
            {
                
                return memoryPatches[address].Activated ? memoryPatches[address].GetData(result) & 0xFF : result & 0xFF;
            }

            return result & 0xFF;
        }


        public void SetByte()
        {

            SetByte(AddressBus, DataBus & 0xFF);
        }

        public void SetByte(int address, int data)
        {
            // check high byte, find appropriate handler
            if (address < 0x800)
            {
                Rams[address & 0x7FF] = (byte)data;
                return;
            }
            switch (address & 0xF000)
            {
                // ram
                case 0x0000:
                // nes sram
                case 0x01000:
                    Rams[address & 0x7FF] = (byte)data;
                    break;
                // cart saveram 
                case 0x06000:
                case 0x07000:
                // cart rom banks
                case 0x08000:
                case 0x09000:
                case 0x0A000:
                case 0x0B000:
                case 0x0C000:
                case 0x0D000:
                case 0x0E000:
                case 0x0F000:
                    Cart.SetByte(clock, address, data);
                    break;
                //ppu owns this part of the map
                case 0x02000:
                case 0x03000:
                    _pixelWhizzler.SetByte(clock, address, data);
                    break;
                // other io ports (sound, joystick, dma sprite xfer)
                case 0x04000:

                    switch (address)
                    {
                        case 0x4000:
                        case 0x4001:
                        case 0x4002:
                        case 0x4003:
                        case 0x4004:
                        case 0x4005:
                        case 0x4006:
                        case 0x4007:
                        case 0x4008:
                        case 0x4009:
                        case 0x400A:
                        case 0x400B:
                        case 0x400C:
                        case 0x400D:
                        case 0x400E:
                        case 0x400F:
                        case 0x4015:
                        case 0x4017:
                            soundBopper.SetByte(clock, address, data);
                            break;
                        case 0x4014:
                            _pixelWhizzler.CopySprites(ref Rams, data * 0x100);
                            CurrentInstruction.ExtraTiming = CurrentInstruction.ExtraTiming + 512;
                            break;
                        case 0x4016:
                            _padOne.SetByte(clock, address, data & 1);
                            _padTwo.SetByte(clock, address, data & 1);
                            break;
                    }
                    break;

            }
        }

        public void FindNextEvent()
        {
            nextEvent =  clock + _pixelWhizzler.NextEventAt;
        }

        private void HandleNextEvent()
        {
            _pixelWhizzler.HandleEvent(Clock);
            FindNextEvent();
        }
    }
}
