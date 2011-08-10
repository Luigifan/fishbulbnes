var cpuOpArray = new Array();

function buildCpuOpArray() {
    cpuOpArray[0] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuProgramCounter = cpuProgramCounter + 1;
        cpuPushStack(cpuProgramCounter >> 8 & 255);
        cpuPushStack(cpuProgramCounter & 255);
        var newStatus = cpuStatusRegister | 16 | 32;
        cpuPushStack(newStatus);
        cpuStatusRegister = cpuStatusRegister | 20;
        cpuAddressBus = 65534;
        var lowByte = getCurrentByte();
        cpuAddressBus = 65535;
        var highByte = getCurrentByte();
        cpuProgramCounter = lowByte + highByte * 256;

        cpuClock = cpuClock + 7 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[1] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[2] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[3] = function () {
        cpuCurinst_AddressingMode = 0;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[4] = function () {
        cpuCurinst_AddressingMode = 0;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[5] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[6] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        cpuSetFlag(1, ((data & 128) === 128));
        data = (data << 1) & 254;
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }
        _setZNFlags(data);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[7] = function () {
        cpuCurinst_AddressingMode = 0;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[8] = function () {
        cpuCurinst_AddressingMode = 1;

        var newStatus = cpuStatusRegister | 16 | 32;
        cpuPushStack(newStatus);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[9] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[10] = function () {
        cpuCurinst_AddressingMode = 2;

        var data = decodeOperand();
        cpuSetFlag(1, ((data & 128) === 128));
        data = (data << 1) & 254;
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }
        _setZNFlags(data);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[11] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[12] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[13] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[14] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        cpuSetFlag(1, ((data & 128) === 128));
        data = (data << 1) & 254;
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }
        _setZNFlags(data);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[15] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[16] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 128) !== 128) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[17] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[18] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[19] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[20] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[21] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[22] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        cpuSetFlag(1, ((data & 128) === 128));
        data = (data << 1) & 254;
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }
        _setZNFlags(data);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[23] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[24] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuSetFlag(1, false);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[25] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[26] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[27] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[28] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[29] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator | decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[30] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        cpuSetFlag(1, ((data & 128) === 128));
        data = (data << 1) & 254;
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }
        _setZNFlags(data);

        cpuClock = cpuClock + 7 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[31] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[32] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuPushStack((cpuProgramCounter >> 8) & 255);
        cpuPushStack((cpuProgramCounter - 1) & 255);
        cpuProgramCounter = decodeAddress();

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[33] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[34] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[35] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[36] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var operand = decodeOperand();
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, (operand & 64) === 64);
        if ((operand & 128) === 128) {
            cpuStatusRegister = cpuStatusRegister | 128;
        }
        else {
            cpuStatusRegister = cpuStatusRegister & 127;
        }
        if ((operand & cpuAccumulator) === 0) {
            cpuStatusRegister = cpuStatusRegister | 2;
        }
        else {
            cpuStatusRegister = cpuStatusRegister & 253;
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[37] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[38] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 1;
        }
        cpuSetFlag(1, (data & 128) === 128);
        data = data << 1;
        data = data & 255;
        data = data | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[39] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[40] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuStatusRegister = cpuPopStack();

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[41] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[42] = function () {
        cpuCurinst_AddressingMode = 2;

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 1;
        }
        cpuSetFlag(1, (data & 128) === 128);
        data = data << 1;
        data = data & 255;
        data = data | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[43] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[44] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var operand = decodeOperand();
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, (operand & 64) === 64);
        if ((operand & 128) === 128) {
            cpuStatusRegister = cpuStatusRegister | 128;
        }
        else {
            cpuStatusRegister = cpuStatusRegister & 127;
        }
        if ((operand & cpuAccumulator) === 0) {
            cpuStatusRegister = cpuStatusRegister | 2;
        }
        else {
            cpuStatusRegister = cpuStatusRegister & 253;
        }

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[45] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[46] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 1;
        }
        cpuSetFlag(1, (data & 128) === 128);
        data = data << 1;
        data = data & 255;
        data = data | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[47] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[48] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 128) === 128) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[49] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[50] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[51] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[52] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[53] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[54] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 1;
        }
        cpuSetFlag(1, (data & 128) === 128);
        data = data << 1;
        data = data & 255;
        data = data | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[55] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[56] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuSetFlag(1, true);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[57] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[58] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[59] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[60] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[61] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator & decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[62] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 1;
        }
        cpuSetFlag(1, (data & 128) === 128);
        data = data << 1;
        data = data & 255;
        data = data | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 7 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[63] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[64] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuStatusRegister = cpuPopStack();
        var low = cpuPopStack();
        var high = cpuPopStack();
        cpuProgramCounter = ((256 * high) + low);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[65] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[66] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[67] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[68] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[69] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[70] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var rst = decodeOperand();
        cpuSetFlag(1, (rst & 1) === 1);
        rst = rst >> 1 & 255;
        _setZNFlags(rst);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = rst;
        }
        else {
            cpuSetByte(decodeAddress(), rst);
        }

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[71] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[72] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuPushStack(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[73] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[74] = function () {
        cpuCurinst_AddressingMode = 2;

        var rst = decodeOperand();
        cpuSetFlag(1, (rst & 1) === 1);
        rst = rst >> 1 & 255;
        _setZNFlags(rst);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = rst;
        }
        else {
            cpuSetByte(decodeAddress(), rst);
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[75] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[76] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 11 && cpuCurinst_Parameters0 === 255) {
            cpuProgramCounter = 255 | cpuCurinst_Parameters1 << 8;
        }
        else {
            cpuProgramCounter = decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[77] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[78] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var rst = decodeOperand();
        cpuSetFlag(1, (rst & 1) === 1);
        rst = rst >> 1 & 255;
        _setZNFlags(rst);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = rst;
        }
        else {
            cpuSetByte(decodeAddress(), rst);
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[79] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[80] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 64) !== 64) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[81] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[82] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[83] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[84] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[85] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[86] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var rst = decodeOperand();
        cpuSetFlag(1, (rst & 1) === 1);
        rst = rst >> 1 & 255;
        _setZNFlags(rst);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = rst;
        }
        else {
            cpuSetByte(decodeAddress(), rst);
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[87] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[88] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuSetFlag(4, false);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[89] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[90] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[91] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[92] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[93] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = (cpuAccumulator ^ decodeOperand());
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[94] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var rst = decodeOperand();
        cpuSetFlag(1, (rst & 1) === 1);
        rst = rst >> 1 & 255;
        _setZNFlags(rst);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = rst;
        }
        else {
            cpuSetByte(decodeAddress(), rst);
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[95] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[96] = function () {
        cpuCurinst_AddressingMode = 1;

        var high, low;
        low = (cpuPopStack() + 1) & 255;
        high = cpuPopStack();
        cpuProgramCounter = ((high << 8) | low);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[97] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[98] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[99] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[100] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[101] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[102] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 128;
        }
        cpuSetFlag(1, (data & 1) === 1);
        data = (data >> 1) | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[103] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[104] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuAccumulator = cpuPopStack();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[105] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[106] = function () {
        cpuCurinst_AddressingMode = 2;

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 128;
        }
        cpuSetFlag(1, (data & 1) === 1);
        data = (data >> 1) | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[107] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[108] = function () {
        cpuCurinst_AddressingMode = 11;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 11 && cpuCurinst_Parameters0 === 255) {
            cpuProgramCounter = 255 | cpuCurinst_Parameters1 << 8;
        }
        else {
            cpuProgramCounter = decodeAddress();
        }

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[109] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[110] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 128;
        }
        cpuSetFlag(1, (data & 1) === 1);
        data = (data >> 1) | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[111] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[112] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 64) === 64) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[113] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[114] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[115] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[116] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[117] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[118] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 128;
        }
        cpuSetFlag(1, (data & 1) === 1);
        data = (data >> 1) | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[119] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[120] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuSetFlag(4, true);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[121] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[122] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[123] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[124] = function () {
        cpuCurinst_AddressingMode = 15;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[125] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = (cpuStatusRegister & 1);
        var result = (cpuAccumulator + data + carryFlag);
        cpuSetFlag(1, result > 255);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ data) & 128) !== 128 && ((cpuAccumulator ^ result) & 128) === 128);
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[126] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var oldbit = 0;
        if (cpuGetFlag(1)) {
            oldbit = 128;
        }
        cpuSetFlag(1, (data & 1) === 1);
        data = (data >> 1) | oldbit;
        _setZNFlags(data);
        if (cpuCurinst_AddressingMode === 2) {
            cpuAccumulator = data;
        }
        else {
            cpuSetByte(decodeAddress(), data);
        }

        cpuClock = cpuClock + 7 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[127] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[128] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[129] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[130] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[131] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[132] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuIndexRegisterY);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[133] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[134] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuIndexRegisterX);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[135] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[136] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuIndexRegisterY = cpuIndexRegisterY - 1;
        cpuIndexRegisterY = cpuIndexRegisterY & 255;
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[137] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[138] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuAccumulator = cpuIndexRegisterX;
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[139] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[140] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuIndexRegisterY);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[141] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[142] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuIndexRegisterX);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[143] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[144] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 1) !== 1) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[145] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[146] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[147] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[148] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuIndexRegisterY);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[149] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[150] = function () {
        cpuCurinst_AddressingMode = 6;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuIndexRegisterX);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[151] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[152] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuAccumulator = cpuIndexRegisterY;
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[153] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[154] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuStackPointer = cpuIndexRegisterX;

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[155] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[156] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[157] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuSetByte(decodeAddress(), cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[158] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[159] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[160] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterY = decodeOperand();
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[161] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[162] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterX = decodeOperand();
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[163] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[164] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterY = decodeOperand();
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[165] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[166] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterX = decodeOperand();
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[167] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[168] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuIndexRegisterY = cpuAccumulator;
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[169] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[170] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuIndexRegisterX = cpuAccumulator;
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[171] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[172] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterY = decodeOperand();
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[173] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[174] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterX = decodeOperand();
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[175] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[176] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 1) === 1) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[177] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[178] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[179] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[180] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterY = decodeOperand();
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[181] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[182] = function () {
        cpuCurinst_AddressingMode = 6;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterX = decodeOperand();
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[183] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[184] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, false);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[185] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[186] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuIndexRegisterX = cpuStackPointer;
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[187] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[188] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterY = decodeOperand();
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[189] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuAccumulator = decodeOperand();
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[190] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        cpuIndexRegisterX = decodeOperand();
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[191] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[192] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuIndexRegisterY + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[193] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[194] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[195] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[196] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuIndexRegisterY + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[197] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[198] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val--;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[199] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[200] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuIndexRegisterY = cpuIndexRegisterY + 1;
        cpuIndexRegisterY = cpuIndexRegisterY & 255;
        _setZNFlags(cpuIndexRegisterY);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[201] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[202] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuIndexRegisterX = cpuIndexRegisterX - 1;
        cpuIndexRegisterX = cpuIndexRegisterX & 255;
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[203] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[204] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuIndexRegisterY + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[205] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[206] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val--;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[207] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[208] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 2) !== 2) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[209] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[210] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[211] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[212] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[213] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[214] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val--;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[215] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[216] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuSetFlag(8, false);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[217] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[218] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[219] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[220] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[221] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuAccumulator + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[222] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val--;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 7 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[223] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[224] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuIndexRegisterX + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[225] = function () {
        cpuCurinst_AddressingMode = 12;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[226] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[227] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[228] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuIndexRegisterX + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[229] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[230] = function () {
        cpuCurinst_AddressingMode = 4;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val++;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[231] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[232] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuIndexRegisterX = cpuIndexRegisterX + 1;
        cpuIndexRegisterX = cpuIndexRegisterX & 255;
        _setZNFlags(cpuIndexRegisterX);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[233] = function () {
        cpuCurinst_AddressingMode = 3;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[234] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[235] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[236] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = (cpuIndexRegisterX + 256 - decodeOperand());
        _compare(data);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[237] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[238] = function () {
        cpuCurinst_AddressingMode = 8;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val++;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[239] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[240] = function () {
        cpuCurinst_AddressingMode = 7;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        if ((cpuStatusRegister & 2) === 2) {
            cpuCurinst_ExtraTiming = 1;
            var addr = cpuCurinst_Parameters0 & 255;
            if ((addr & 128) === 128) {
                addr = addr - 256;
                cpuProgramCounter += addr;
            }
            else {
                cpuProgramCounter += addr;
            }
            if ((cpuProgramCounter & 255) < addr) {
                cpuCurinst_ExtraTiming = 2;
            }
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[241] = function () {
        cpuCurinst_AddressingMode = 13;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 5 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[242] = function () {
        cpuCurinst_AddressingMode = 14;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[243] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[244] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 3 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[245] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[246] = function () {
        cpuCurinst_AddressingMode = 5;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val++;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[247] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[248] = function () {
        cpuCurinst_AddressingMode = 1;

        cpuSetFlag(8, true);

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[249] = function () {
        cpuCurinst_AddressingMode = 10;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[250] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 2 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[251] = function () {
        cpuCurinst_AddressingMode = 1;
        cpuClock = cpuClock + 0 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[252] = function () {
        cpuCurinst_AddressingMode = 1;

        if (cpuCurinst_AddressingMode === 9) {
            decodeAddress();
        }

        cpuClock = cpuClock + 6 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[253] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var data = decodeOperand();
        var carryFlag = ((cpuStatusRegister ^ 1) & 1);
        var result = (cpuAccumulator - data - carryFlag);
        cpuSetFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((cpuAccumulator ^ result) & 128) === 128 && ((cpuAccumulator ^ data) & 128) === 128);
        cpuSetFlag(1, (result < 256));
        cpuAccumulator = (result & 255);
        _setZNFlags(cpuAccumulator);

        cpuClock = cpuClock + 4 + cpuCurinst_ExtraTiming;
    };

    cpuOpArray[254] = function () {
        cpuCurinst_AddressingMode = 9;
        cpuCurinst_Parameters0 = cpuGetByte(cpuProgramCounter++);
        cpuCurinst_Parameters1 = cpuGetByte(cpuProgramCounter++);

        var val = decodeOperand();
        val++;
        cpuSetByte(decodeAddress(), val);
        _setZNFlags(val);

        cpuClock = cpuClock + 7 + cpuCurinst_ExtraTiming;
    }; 

}
