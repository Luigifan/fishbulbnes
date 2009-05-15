using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Fastendo
{
    public partial class CPU2A03
    {
        bool zeroResult;
        bool negativeResult;


        void SetZNFlags(int data)
        {

            //zeroResult = (data & 0xFF) == 0;
            //negativeResult = (data & 0x80) == 0x80;

            if ((data & 0xFF) == 0)
                _statusRegister |= (int)CPUStatusMasks.ZeroResultMask;
            else
                _statusRegister &= ~((int)CPUStatusMasks.ZeroResultMask);

            if ((data & 0x80) == 0x80)
                _statusRegister |= (int)CPUStatusMasks.NegativeResultMask;
            else
                _statusRegister &= ~((int)CPUStatusMasks.NegativeResultMask);
            //SetFlag(CPUStatusBits.ZeroResult, (data & 0xFF) == 0);
            //SetFlag(CPUStatusBits.NegativeResult, (data & 0x80) == 0x80);
        }

        #region load/store operations
        public void LDA()
        {

            _accumulator = DecodeOperand();

            SetZNFlags(_accumulator);

        }

        public void LDX()
        {

            _indexRegisterX = DecodeOperand(); 
            SetZNFlags( _indexRegisterX);
        }

        public void LDY()
        {
            _indexRegisterY = DecodeOperand();
            SetZNFlags( _indexRegisterY);
        }

        public void STA()
        {
            SetByte(DecodeAddress(), _accumulator);
        }

        public void STX()
        {
            SetByte(DecodeAddress(), _indexRegisterX);
        }

        public void STY()
        {
            SetByte(DecodeAddress(), _indexRegisterY);
        }
        #endregion

        #region status bit operations
        public void SED()
        {
            SetFlag(CPUStatusMasks.DecimalModeMask, true);
            // StatusRegister = StatusRegister | 0x8;
        }

        public void CLD()
        {
            SetFlag(CPUStatusMasks.DecimalModeMask, false);
//            StatusRegister = StatusRegister & 0xF7;
        }
        #endregion

        public void JMP()
        {
            // 6052 indirect jmp bug
            if (_currentInstruction.AddressingMode == AddressingModes.Indirect 
                && _currentInstruction.Parameters0 == 0xFF)
            {
                _programCounter = 0xFF | _currentInstruction.Parameters1 << 8;
            }
            else
            {
                _programCounter = DecodeAddress();
            }
        }

        public void DEC()
        {
            byte val = (byte)DecodeOperand();
            val--;
            SetByte(DecodeAddress(), val);
            SetZNFlags( val);
        }

        public void INC()
        {
            byte val = (byte)DecodeOperand();
            val++;
            SetByte(DecodeAddress(), val);
            SetZNFlags(val);
        }

        public void ADC()
        {
            // start the read process
            uint data = (uint)DecodeOperand();
            int carryFlag = (_statusRegister & 0x01);
            uint result = (uint)(_accumulator + data + carryFlag);

            // carry flag

            SetFlag(CPUStatusMasks.CarryMask, result > 0xFF);

            // overflow flag
            // SetFlag(CPUStatusBits.Overflow, (result > 0x7f || ~result > 0x7f));
            SetFlag(CPUStatusMasks.OverflowMask,
                    ((_accumulator ^ data) & 0x80) != 0x80 &&
                    ((_accumulator ^ result) & 0x80) == 0x80);

            // occurs when bit 7 is set
            _accumulator = (int)(result & 0xFF);
            SetZNFlags(_accumulator);

        }

        public void LSR()
        {
            int rst = DecodeOperand();
            //LSR shifts all bits right one position. 0 is shifted into bit 7 and the original bit 0 is shifted into the Carry. 

            SetFlag(CPUStatusMasks.CarryMask, (rst & 1) == 1);
            //target.SetFlag(CPUStatusBits.Carry, (rst & 1) == 1);
            rst = rst >> 1 & 0xFF;

            SetZNFlags( rst);

            if (CurrentInstruction.AddressingMode == AddressingModes.Accumulator)
            {
                _accumulator = rst;
            }
            else
            {
                SetByte(DecodeAddress(), rst);
            }
        }

        public void SBC()
        {
            // start the read process

            uint data = (uint)DecodeOperand() ;

            int carryFlag = ((_statusRegister ^ 0x01) & 0x1);

            uint result = (uint)(_accumulator - data - carryFlag);

            // set overflow flag if sign bit of accumulator changed
            SetFlag(CPUStatusMasks.OverflowMask,
                    ((_accumulator ^ result) & 0x80) == 0x80 &&
                    ((_accumulator ^ data) & 0x80) == 0x80);

            SetFlag(CPUStatusMasks.CarryMask, (result  < 0x100));

            _accumulator = (int)(result & 0xFF);
            SetZNFlags(_accumulator);


        }

        public void AND()
        {
            _accumulator = (_accumulator & DecodeOperand());
            SetZNFlags(_accumulator);
        }

        public void ORA()
        {

            _accumulator = (_accumulator | DecodeOperand());
            SetZNFlags(_accumulator);
        }

        public void EOR()
        {
            _accumulator = (_accumulator ^ DecodeOperand());
            SetZNFlags( Accumulator);
        }

        public void ASL()
        {
            int data = DecodeOperand();
            // set carry flag
            
            SetFlag(CPUStatusMasks.CarryMask, ((data & 128) == 128));

            data = (data << 1) & 0xFE;

            if (CurrentInstruction.AddressingMode == AddressingModes.Accumulator)
            {
                _accumulator = data;
            }
            else
            {
                SetByte(DecodeAddress(), data);
            }


            SetZNFlags( data);
        }

        public void BIT()
        {

            int operand = DecodeOperand();
            // overflow is bit 6
            SetFlag(CPUStatusMasks.OverflowMask, (operand & 64) == 64);
            //if ((operand & 64) == 64)
            //{
            //    _statusRegister = _statusRegister | 0x40;
            //}
            //else
            //{
            //    _statusRegister = _statusRegister & 0xBF;
            //}

            // negative is bit 7
            if ((operand & 128) == 128)
            {
                _statusRegister = _statusRegister | 128;
            }
            else
            {
                _statusRegister = _statusRegister & 127;
            }

            if ((operand & Accumulator) == 0)
            {
                _statusRegister = _statusRegister | 0x2;
            }
            else
            {
                _statusRegister = _statusRegister & 0xFD;
            }

        }

        public void SEC()
        {
            // carry flag bit 0
            SetFlag(CPUStatusMasks.CarryMask, true);
        }

        public void CLC()
        {
            SetFlag(CPUStatusMasks.CarryMask, false);
        }

        public void SEI()
        {
            //StatusRegister = StatusRegister | 0x4;
            SetFlag(CPUStatusMasks.InterruptDisableMask, true);
        }

        public void CLI()
        {
//            StatusRegister = StatusRegister & 0xFB;
            SetFlag(CPUStatusMasks.InterruptDisableMask, false);
        }


        public void CLV()
        {
            SetFlag(CPUStatusMasks.OverflowMask, false);

        }


        void Compare(int data)
        {
            SetFlag(CPUStatusMasks.CarryMask, data > 0xFF);
            SetZNFlags( data & 0xFF);
        }

        public void CMP()
        {
            int data = (Accumulator + 0x100 - DecodeOperand());
            Compare(data);
        }

        public void CPX()
        {
            int data = (_indexRegisterX + 0x100 - DecodeOperand());
            Compare(data);
        }

        public void CPY()
        {
            int data = (_indexRegisterY + 0x100 - DecodeOperand());
            Compare(data);
        }

        public void NOP()
        {
            if (_currentInstruction.AddressingMode == AddressingModes.AbsoluteX)
            {
                DecodeAddress();
            }
        }

        #region Branch instructions
        private void Branch()
        {
            System.Diagnostics.Debug.Assert(cpuTiming[_currentInstruction.OpCode] == 2);

            _currentInstruction.ExtraTiming = 1;
            int addr = _currentInstruction.Parameters0 & 0xFF;
            if ((addr & 128) == 128)
            {
                addr = addr - 0x100;
                _programCounter += addr;
            }
            else
            {
                _programCounter += addr;
            }

            if ((_programCounter & 0xFF) < addr)
            {
                _currentInstruction.ExtraTiming = 2;
            }

        }

        public void BCC()
        {
            
            if ((_statusRegister & 0x1) != 0x1)
                Branch();
        }

        public void BCS()
        {
            if ((_statusRegister & 0x1) == 0x1)
                Branch();
        }

        public void BPL()
        {
            if ((_statusRegister & 0x80) != 0x80)
                Branch();
        }

        public void BMI()
        {
            if ((_statusRegister & 0x80) == 0x80)
                Branch();
        }

        public void BVC()
        {
            if ((_statusRegister & 0x40) != 0x40)
                Branch();
        }

        public void BVS()
        {
            if ((_statusRegister & 0x40) == 0x40)
                Branch();
        }

        public void BNE()
        {
            if ((_statusRegister & 0x2) != 0x2)
                Branch();
        }

        public void BEQ()
        {
            if ((_statusRegister & 0x2) == 0x2)
                Branch();
        }
        #endregion

        #region Register instructions

        public void DEX()
        {
            _indexRegisterX = _indexRegisterX - 1;
            _indexRegisterX = _indexRegisterX & 0xFF;
            SetZNFlags( _indexRegisterX);
        }

        public void DEY()
        {
            _indexRegisterY = _indexRegisterY - 1;
            _indexRegisterY = _indexRegisterY & 0xFF;
            SetZNFlags( _indexRegisterY);
        }

        public void INX()
        {
            _indexRegisterX = _indexRegisterX + 1;
            _indexRegisterX = _indexRegisterX & 0xFF;
            SetZNFlags( _indexRegisterX);
        }

        public void INY()
        {
            _indexRegisterY = _indexRegisterY + 1;
            _indexRegisterY = _indexRegisterY & 0xFF;
            SetZNFlags( _indexRegisterY);
        }


        public void TAX()
        {
            _indexRegisterX = _accumulator;
            SetZNFlags( _indexRegisterX);

        }

        public void TXA()
        {
            _accumulator = _indexRegisterX;
            SetZNFlags(_accumulator);
        }

        public void TAY()
        {
            _indexRegisterY = _accumulator;
            SetZNFlags( _indexRegisterY);
        }

        public void TYA()
        {
            _accumulator = _indexRegisterY;
            SetZNFlags(_accumulator);
        }

        public void TXS()
        {
            _stackPointer = _indexRegisterX;
        }
        public void TSX()
        {
            _indexRegisterX = _stackPointer;
            SetZNFlags( _indexRegisterX);
        }
        #endregion

        public void PHA()
        {
            PushStack(_accumulator);
        }

        public void PLA()
        {
            _accumulator = PopStack();
            SetZNFlags(_accumulator);
        }

        public void PHP()
        {
             //PHP and BRK push the current status with bits 4 and 5 set on the stack; 
            // BRK then sets the I flag.
            int newStatus = _statusRegister | 0x10 | 0x20;
            PushStack(newStatus);
        }

        public void PLP()
        {
            _statusRegister = PopStack(); // | 0x20;
        }

        public void JSR()
        {
            PushStack( (_programCounter >> 8) & 0xFF);
            PushStack((_programCounter - 1) & 0xFF);

            _programCounter = DecodeAddress();
        }

        public void ROR()
        {
            int data = DecodeOperand();

            // old carry bit shifted into bit 7
            int oldbit =0;
            if (GetFlag(CPUStatusMasks.CarryMask))
            {
                oldbit =  0x80;
            }

            // original bit 0 shifted to carry
            //            target.SetFlag(CPUStatusBits.Carry, (); 

            SetFlag(CPUStatusMasks.CarryMask, (data & 0x01) == 0x01);

            data = (data >> 1 ) | oldbit;

            SetZNFlags( data);
            
            if (CurrentInstruction.AddressingMode == AddressingModes.Accumulator)
            {
                _accumulator = data;
            }
            else
            {
                SetByte(DecodeAddress(), data);
            }
        }

        public void ROL()
        {
            int data= DecodeOperand();

            int oldbit = 0;
            if (GetFlag(CPUStatusMasks.CarryMask))
            {
                oldbit = 1;
            }
            SetFlag(CPUStatusMasks.CarryMask, (data & 128) == 128);

            
            data = data << 1;
            data = data & 0xFF;
            data = data | oldbit;
            SetZNFlags( data);

            if (CurrentInstruction.AddressingMode == AddressingModes.Accumulator)
            {
                _accumulator = data;
            }
            else
            {
                SetByte(DecodeAddress(), data);
            }
        }

        public void RTS()
        {
            int high, low;
            low = (PopStack() + 1) & 0xFF;
            high = PopStack();
            _programCounter = ((high << 8) | low) ;
        }

        public void RTI()
        {
            _statusRegister = PopStack();// | 0x20;
            int low = PopStack();
            int high = PopStack();
            _programCounter = ((256 * high) + low);
        }

        public void BRK()
        {
            //BRK causes a non-maskable interrupt and increments the program counter by one. 
            //Therefore an RTI will go to the address of the BRK +2 so that BRK may be used to replace a two-byte instruction 
            // for debugging and the subsequent RTI will be correct. 
            // push pc onto stack (high byte first)
            _programCounter = _programCounter + 1;
            PushStack(_programCounter >> 8 & 0xFF);
            PushStack(_programCounter & 0xFF);
            // push sr onto stack

            //PHP and BRK push the current status with bits 4 and 5 set on the stack; 

            int newStatus = _statusRegister | 0x10 | 0x20;

            PushStack(newStatus);

            // set interrupt disable, and break flags
            // BRK then sets the I flag.
            _statusRegister = _statusRegister | 0x14;

            // point pc to interrupt service routine
            AddressBus = 0xFFFE;
            int lowByte = GetByte();
            AddressBus = 0xFFFF;
            int highByte = GetByte();

            _programCounter = lowByte + highByte * 0x100;
        }
    }

}
