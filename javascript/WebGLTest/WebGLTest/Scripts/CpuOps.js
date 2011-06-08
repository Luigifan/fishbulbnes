    var frameOn = true;

    var cpuStatusRegister;
    var cpuAccumulator;
    var cpuIndexRegisterX;
    var cpuIndexRegisterY;
    var cpuStackPointer;
    var cpuProgramCounter;
    var cpuCurinst_Address;
    var cpuCurinst_OpCode;
    var cpuCurinst_AddressingMode;
    var cpuCurinst_ExtraTiming;
    var cpuCurinst_Parameters0;
    var cpuCurinst_Parameters1;
    var cpuClock = 0;
    var cpuLowByte = 0;
    var cpuHighByte = 0;
    var cpuNextEvent;
    var cpuHandleIRQ  = false;
    var cpuHandleNMI = false;

    var cpuPixelWhizzler;
    var cpuCart;

    var cpuRams = new Array(8192);

    function decodeOperand() {
        /// <returns type="Number" integer="true"></returns>
        switch (cpuCurinst_AddressingMode) {
            case 3:
                cpuDataBus = cpuCurinst_Parameters0;
                return cpuCurinst_Parameters0;
            case 2:
                return cpuAccumulator;
            default:
                cpuDataBus = cpuGetByte(decodeAddress());
                return cpuDataBus;
        }
    }


    function runFrame() {
        frameOn = true;
        findNextEvent();
        while (frameOn) {
            step();
        }
    }


    function findNextEvent() {
        cpuNextEvent = cpuClock + get_nextEventAt();
    }
    
    function handleNextEvent() {
        handleEvent(cpuClock);
        findNextEvent();
    }

    function cpuGetByte(address) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var result = 0;
        switch (address & 61440) {
            case 0:
            case 4096:
                if (address < 2048) {
                    result = cpuRams[address];
                }
                else {
                    result = address >> 8;
                }
                break;
            case 8192:
            case 12288:
                result = getPPUByte(cpuClock, address);
                break;
            case 16384:
                switch (address) {
                    case 16406:
                        break;
                    case 16407:
                        break;
                    case 16405:
                        break;
                    default:
                        result = address >> 8;
                        break;
                }
                break;
            case 20480:
                result = address >> 8;
                break;
            case 24576:
            case 28672:
            case 32768:
            case 36864:
            case 40960:
            case 45056:
            case 49152:
            case 53248:
            case 57344:
            case 61440:
                result = cpuCart.getByte(cpuClock, address);
                break;
            default:
                throw new Error('Bullshit!');
        }
        return result & 255;
    }
    
    function setCurrentByte() {
        cpuSetByte(cpuAddressBus, cpuDataBus & 255);
    }
    
    function cpuSetByte(address, data) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <param name="data" type="Number" integer="true">
        /// </param>
        if (address < 2048) {
            cpuRams[address & 2047] = data;
            return;
        }
        switch (address & 61440) {
            case 0:
            case 4096:
                cpuRams[address & 2047] = data;
                break;
            case 20480:
                cpuCart.setByte(cpuClock, address, data);
                break;
            case 24576:
            case 28672:
            case 32768:
            case 36864:
            case 40960:
            case 45056:
            case 49152:
            case 53248:
            case 57344:
            case 61440:
                cpuCart.setByte(cpuClock, address, data);
                break;
            case 8192:
            case 12288:
                setPPUByte(cpuClock, address, data);
                break;
            case 16384:
                switch (address) {
                    case 16384:
                    case 16385:
                    case 16386:
                    case 16387:
                    case 16388:
                    case 16389:
                    case 16390:
                    case 16391:
                    case 16392:
                    case 16393:
                    case 16394:
                    case 16395:
                    case 16396:
                    case 16397:
                    case 16398:
                    case 16399:
                    case 16405:
                    case 16407:
                        break;
                    case 16404:
                        copySprites(cpuRams, data * 256);
                        cpuCurinst_ExtraTiming = cpuCurinst_ExtraTiming + 512;
                        break;
                    case 16406:
                        break;
                }
                break;
        }
    }
    
    function cpuSetFlag(Flag, value) {
        /// <param name="Flag" type="NES.CPU.Fastendo.CPUStatusMasks">
        /// </param>
        /// <param name="value" type="Boolean">
        /// </param>
        cpuStatusRegister = ((value) ? (cpuStatusRegister | Flag) : (cpuStatusRegister & ~Flag));
        cpuStatusRegister |= 32; //  NES.CPU.Fastendo.CPUStatusMasks.expansionMask;
    }
    
    function cpuGetFlag(Flag) {
        /// <param name="Flag" type="NES.CPU.Fastendo.CPUStatusMasks">
        /// </param>
        /// <returns type="Boolean"></returns>
        return ((cpuStatusRegister & Flag) === Flag);
    }

    function cpuPushStack(data) {
        /// <param name="data" type="Number" integer="true">
        /// </param>
        cpuRams[cpuStackPointer + 256] = data;
        cpuStackPointer--;
        if (cpuStackPointer < 0) {
            cpuStackPointer = 255;
        }
    }
    
    function cpuPopStack() {
        /// <returns type="Number" integer="true"></returns>
        cpuStackPointer++;
        if (cpuStackPointer > 255) {
            cpuStackPointer = 0;
        }
        return cpuRams[cpuStackPointer + 256] & 255;
    }


    function step() {
        cpuCurinst_ExtraTiming = 0;
        if (cpuNextEvent <= cpuClock) {
            handleNextEvent();
        }
        if (cpuHandleNMI) {
            cpuHandleNMI = false;
            cpuClock += 7;
            nonMaskableInterrupt();
        }
        else if (cpuHandleIRQ) {
            cpuHandleIRQ = false;
            cpuClock += 7;
            interruptRequest();
        }
        cpuCurinst_Address = cpuProgramCounter;
        cpuCurinst_OpCode = cpuGetByte(cpuProgramCounter++);
        
        cpuOpArray[cpuCurinst_OpCode]();
    }

    function interruptRequest() {
        if (cpuGetFlag(4)) {
            return;
        }
        cpuSetFlag(4, true);
        var newStatusReg = cpuStatusRegister & ~16 | 32;
        cpuPushStack(cpuProgramCounter >> 8);
        cpuPushStack(cpuProgramCounter);
        cpuPushStack(cpuStatusRegister);
        cpuProgramCounter = puGetByte(65534) + (cpuGetByte(65535) << 8);
    }

    function nonMaskableInterrupt() {
        var newStatusReg = cpuStatusRegister & ~16 | 32;
        cpuSetFlag(4, true);
        cpuPushStack(cpuProgramCounter >> 8);
        cpuPushStack(cpuProgramCounter & 255);
        cpuPushStack(newStatusReg);
        var lowByte = cpuGetByte(65530);
        var highByte = cpuGetByte(65531);
        var jumpTo = lowByte | (highByte << 8);
        cpuProgramCounter = jumpTo;
    }


    function resetCPU() {
        cpuStatusRegister = 52;
        cpuStackPointer = 253;
        cpuProgramCounter = cpuGetByte(65532) + cpuGetByte(65533) * 256;
    }

    function powerOn() {
        cpuStatusRegister = 52;
        cpuStackPointer = 253;
        for (var i = 0; i < 2048; ++i) {
            cpuRams[i] = 255;
        }
        cpuRams[8] = 247;
        cpuRams[9] = 239;
        cpuRams[10] = 223;
        cpuRams[15] = 191;
        cpuProgramCounter = cpuGetByte(65532) + cpuGetByte(65533) * 256;
    }
    
    function decodeAddress() {
        /// <returns type="Number" integer="true"></returns>
        cpuCurinst_ExtraTiming = 0;
        var result = 0;
        switch (cpuCurinst_AddressingMode) {
            case 8:
                result = ((cpuCurinst_Parameters1 << 8) | cpuCurinst_Parameters0);
                break;
            case 9:
                result = (((cpuCurinst_Parameters1 << 8) | cpuCurinst_Parameters0) + cpuIndexRegisterX);
                if ((result & 255) < cpuIndexRegisterX) {
                    cpuCurinst_ExtraTiming = 1;
                }
                break;
            case 10:
                result = (((cpuCurinst_Parameters1 << 8) | cpuCurinst_Parameters0) + cpuIndexRegisterY);
                if ((result & 255) < cpuIndexRegisterY) {
                    cpuCurinst_ExtraTiming = 1;
                }
                break;
            case 4:
                result = cpuCurinst_Parameters0 & 255;
                break;
            case 5:
                result = (cpuCurinst_Parameters0 + cpuIndexRegisterX) & 255;
                break;
            case 6:
                result = (cpuCurinst_Parameters0 + cpuIndexRegisterY) & 255;
                break;
            case 11:
                cpuLowByte = cpuCurinst_Parameters0;
                cpuHighByte = cpuCurinst_Parameters1 << 8;
                var indAddr = (cpuHighByte | cpuLowByte) & 65535;
                var indirectAddr = cpuGetByte(indAddr);
                cpuLowByte = (cpuLowByte + 1) & 255;
                indAddr = (cpuHighByte | cpuLowByte) & 65535;
                indirectAddr |= (cpuGetByte(indAddr) << 8);
                result = indirectAddr;
                break;
            case 12:
                var addr = (cpuCurinst_Parameters0 + cpuIndexRegisterX) & 255;
                cpuLowByte = cpuGetByte(addr);
                addr = addr + 1;
                cpuHighByte = cpuGetByte(addr & 255);
                cpuHighByte = cpuHighByte << 8;
                result = cpuHighByte | cpuLowByte;
                break;
            case 13:
                cpuLowByte = cpuGetByte(cpuCurinst_Parameters0);
                cpuHighByte = cpuGetByte((cpuCurinst_Parameters0 + 1) & 255) << 8;
                addr = (cpuLowByte | cpuHighByte);
                result = addr + cpuIndexRegisterY;
                if ((result & 255) > cpuIndexRegisterY) {
                    cpuCurinst_ExtraTiming = 1;
                }
                break;
            case 7:
                result = (cpuProgramCounter + cpuCurinst_Parameters0);
                break;
            default:
                throw new Error('Executors.DecodeAddress() recieved an invalid addressmode');
        }
        return result;
    }


    function _setZNFlags( data) {
        /// <param name="data" type="Number" integer="true">
        /// </param>
        if ((data & 255) === 0) {
            cpuStatusRegister |= 2; // NES.CPU.Fastendo.CPUStatusMasks.zeroResultMask;
        }
        else {
            cpuStatusRegister &= ~2; ////~(NES.CPU.Fastendo.CPUStatusMasks.zeroResultMask);
        }
        if ((data & 128) === 128) {
            cpuStatusRegister |= 128;  // NES.CPU.Fastendo.CPUStatusMasks.negativeResultMask;
        }
        else {
            cpuStatusRegister &= ~128;  //~(NES.CPU.Fastendo.CPUStatusMasks.negativeResultMask);
        }
    }

    function _compare(data) {
        /// <param name="data" type="Number" integer="true">
        /// </param>
        cpuSetFlag(1, data > 255);
        _setZNFlags( data & 255);
    }
    
