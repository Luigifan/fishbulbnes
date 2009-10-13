using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.Machine.ControlPanel;
using NES.CPU.Machine.ROMLoader;
using System.IO;
using NES.CPU.Fastendo;
using NES.CPU.Machine.Carts;

namespace NES.CPU.nitenedo
{

    public partial class NESMachine
    {

        public void Initialize()
        {
            _cpu.Clock = 0;
            frameCount = 0;
            SetupTimer();
        }

        public void Reset()
        {
            if (_cpu != null)
            {
                ForceStop();
                _ppu.Initialize();
                _cart.InitializeCart();
                _cpu.ResetCPU();
                ClearGenieCodes();
            }
        }

        public void PowerOn()
        {
            if (_cpu != null)
            {

                _ppu.Initialize();
                _cart.InitializeCart();
                _cpu.PowerOn();
            }
        }

        string _currCartName = string.Empty;
        public string CurrentCartName
        {
            get
            {
                return _currCartName;
            }
        }

        public RunningStatuses RunState
        {
            get 
            { 
                if (_cart == null)
                {
                    return RunningStatuses.Unloaded;
                }
                // if ispaused return runningstatuses.paused
                if (keepRunning)
                {
                    return RunningStatuses.Running;
                }

                return RunningStatuses.Off;
            }
        }

        public void EjectCart()
        {
            SRAMWriter(_cart.CheckSum, _cart.SRAM);

            ForceStop();
            _cart = null;
            _cart = new NESCart();
            _currCartName = null;
            //_ppu.CurrentScanLine = 0;
        }


        public void GoTendo(string fileName)
        {
            _currCartName = Path.GetFileName(fileName);

            _cart = iNESFileHandler.GetCart(fileName, _ppu);
            if (_cart != null)
            {
                if (SRAMReader != null)
                    _cart.SRAM = SRAMReader(_cart.CheckSum);

                _cpu.Cart = (IClockedMemoryMappedIOElement)_cart;
            }
            else
            {
                throw new CartLoadException("Couldn't load file");
                
            }

            PowerOn();

        }

        private int[][] lastSaveState = new int[10][];

        private int currentSaveSlot;

        public int CurrentSaveSlot
        {
            get { return currentSaveSlot; }
            set {
                if (value >= 0 && value <= 10)
                {
                    currentSaveSlot = value;
                }
            }
        }

        public bool HasState(int index)
        {
            return (lastSaveState != null && lastSaveState[index] != null);
        }

        public void GetState(int index)
        {
            Queue<int> state = new Queue<int>();
            state = new Queue<int>();
            _cpu.GetState(state);
            _ppu.WriteState(state);
            _cart.WriteState(state);
            lastSaveState[index] = new int[state.Count];
            state.CopyTo(lastSaveState[index], 0);
        }

        public void SetState(int index)
        {
            if (lastSaveState != null)
            {
                Queue<int> cloneState = new Queue<int>(lastSaveState[index]);
                _cpu.SetState(cloneState);
                _ppu.ReadState(cloneState);
                _cart.ReadState(cloneState);
            }
        }

        public void ReadState(int index, BinaryReader input)
        {
            Queue<int> state = new Queue<int>();
            int nextItem = 0;

            while (input.BaseStream.Position < input.BaseStream.Length)
            {
                nextItem = input.ReadInt32();
                state.Enqueue(nextItem);
            }
            lastSaveState[index] = new int[state.Count];
            state.CopyTo(lastSaveState[index], 0);
        }

        public void WriteState(int index, BinaryWriter output)
        {
            if (lastSaveState != null)
            {
                Queue<int> cloneState = new Queue<int>(lastSaveState[index]);
                while (cloneState.Count > 0)
                {
                    output.Write(cloneState.Dequeue());
                }
            }
        }

        public void ClearGenieCodes()
        {
            _cpu.GenieCodes.Clear();
            _cpu.Cheating = false;
        }

        public bool AddGameGenieCode(string code)
        {
            byte[] hexCode = new byte[code.Length];
            int i = 0;
            foreach (char c in code.ToUpper())
            {
                byte digit = 0;
                switch (c)
                {
                    case 'A':
                        digit = 0x0;
                        break;
                    case 'P':
                        digit = 0x1;
                        break;
                    case 'Z':
                        digit = 0x2;
                        break;
                    case 'L':
                        digit = 0x3;
                        break;
                    case 'G':
                        digit = 0x4;
                        break;
                    case 'I':
                        digit = 0x5;
                        break;
                    case 'T':
                        digit = 0x6;
                        break;
                    case 'Y':
                        digit = 0x7;
                        break;
                    case 'E':
                        digit = 0x8;
                        break;
                    case 'O':
                        digit = 0x9;
                        break;
                    case 'X':
                        digit = 0xA;
                        break;
                    case 'U':
                        digit = 0xB;
                        break;
                    case 'K':
                        digit = 0xC;
                        break;
                    case 'S':
                        digit = 0xD;
                        break;
                    case 'V':
                        digit = 0xE;
                        break;
                    case 'N':
                        digit = 0xF;
                        break;
                }
                hexCode[i++] = digit;
            }

            // magic spell that makes the genie appear!
            // http://tuxnes.sourceforge.net/gamegenie.html
            int address = 0x8000 +
                  ((hexCode[3] & 7) << 12)
                | ((hexCode[5] & 7) << 8) | ((hexCode[4] & 8) << 8)
                | ((hexCode[2] & 7) << 4) | ((hexCode[1] & 8) << 4)
                | (hexCode[4] & 7) | (hexCode[3] & 8);


            int data = 0;
            int compare = 0;
            if (hexCode.Length == 6)
            {
                data =
                     ((hexCode[1] & 7) << 4) | ((hexCode[0] & 8) << 4)
                    | (hexCode[0] & 7) | (hexCode[5] & 8);
            }
            else if (hexCode.Length == 8)
            {
                data =
                     ((hexCode[1] & 7) << 4) | ((hexCode[0] & 8) << 4)
                    | (hexCode[0] & 7) | (hexCode[7] & 8);
                compare =
                     ((hexCode[7] & 7) << 4) | ((hexCode[6] & 8) << 4)
                    | (hexCode[6] & 7) | (hexCode[5] & 8);
                data |= compare << 8;
            }
            else
            {
                // not a genie code!  
                return false;
            }
            try
            {
                _cpu.GenieCodes.Add(address, data);
                _cpu.Cheating = true;
            }
            catch {
                _cpu.Cheating = false;
            }
            return _cpu.Cheating;
        }

    }
}
