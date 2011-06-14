//! bulbascript.debug.js
//

(function() {
function executeScript() {

Type.registerNamespace('NES.CPU.Machine.Carts');

////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Machine.Carts.NameTableMirroring

NES.CPU.Machine.Carts.NameTableMirroring = function() { 
    /// <field name="oneScreen" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="vertical" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="horizontal" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="fourScreen" type="Number" integer="true" static="true">
    /// </field>
};
NES.CPU.Machine.Carts.NameTableMirroring.prototype = {
    oneScreen: 0, 
    vertical: 1, 
    horizontal: 2, 
    fourScreen: 3
}
NES.CPU.Machine.Carts.NameTableMirroring.registerEnum('NES.CPU.Machine.Carts.NameTableMirroring', false);


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Machine.Carts.INESCart

NES.CPU.Machine.Carts.INESCart = function() { 
};
NES.CPU.Machine.Carts.INESCart.prototype = {
    getByte : null,
    setByte : null,
    get_nmiHandler : null,
    set_nmiHandler : null,
    get_irqAsserted : null,
    set_irqAsserted : null,
    get_nextEventAt : null,
    handleEvent : null,
    resetClock : null,
    loadiNESCart : null,
    get_whizzler : null,
    set_whizzler : null,
    get_CPU : null,
    set_CPU : null,
    initializeCart : null,
    updateScanlineCounter : null,
    get_chrRom : null,
    set_chrRom : null,
    get_chrRamStart : null,
    get_ppuBankStarts : null,
    set_ppuBankStarts : null,
    get_checkSum : null,
    get_SRAM : null,
    set_SRAM : null,
    get_mirroring : null,
    get_cartName : null,
    get_numberOfPrgRoms : null,
    get_numberOfChrRoms : null,
    get_mapperID : null,
    getPPUByte : null,
    setPPUByte : null,
    actualChrRomOffset : null,
    get_bankSwitchesChanged : null,
    set_bankSwitchesChanged : null,
    get_currentBank : null,
    get_usesSRAM : null,
    set_usesSRAM : null
}
NES.CPU.Machine.Carts.INESCart.registerInterface('NES.CPU.Machine.Carts.INESCart');


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Machine.Carts.BaseCart

NES.CPU.Machine.Carts.BaseCart = function NES_CPU_Machine_Carts_BaseCart() {
    /// <field name="_iNesHeader" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_romControlBytes" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_nesCart" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_chrRom" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_current8" type="Number" integer="true">
    /// </field>
    /// <field name="_currentA" type="Number" integer="true">
    /// </field>
    /// <field name="_currentC" type="Number" integer="true">
    /// </field>
    /// <field name="_currentE" type="Number" integer="true">
    /// </field>
    /// <field name="_sramCanWrite" type="Boolean">
    /// </field>
    /// <field name="_sramEnabled" type="Boolean">
    /// </field>
    /// <field name="_sramCanSave" type="Boolean">
    /// </field>
    /// <field name="_prgRomCount" type="Number" integer="true">
    /// </field>
    /// <field name="_chrRomCount" type="Number" integer="true">
    /// </field>
    /// <field name="_mapperId" type="Number" integer="true">
    /// </field>
    /// <field name="_bank8start" type="Number" integer="true">
    /// </field>
    /// <field name="_bankAstart" type="Number" integer="true">
    /// </field>
    /// <field name="_bankCstart" type="Number" integer="true">
    /// </field>
    /// <field name="_bankEstart" type="Number" integer="true">
    /// </field>
    /// <field name="_prgRomBank6" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_ROMHashfunction" type="NES.CPU.Machine.Carts.ROMHashFunctionDelegate">
    /// </field>
    /// <field name="_chrRomOffset" type="Number" integer="true">
    /// </field>
    /// <field name="chrRamStart" type="Number" integer="true">
    /// </field>
    /// <field name="whizzler" type="NES.CPU.PixelWhizzlerClasses.IPPU">
    /// </field>
    /// <field name="irqRaised" type="Boolean">
    /// </field>
    /// <field name="_checkSum" type="String">
    /// </field>
    /// <field name="mirroring" type="Number" integer="true">
    /// </field>
    /// <field name="_cartName" type="String">
    /// </field>
    /// <field name="_updateIRQ" type="NES.CPU.Fastendo.MachineEvent">
    /// </field>
    /// <field name="ppuBankStarts" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_bankStartCache" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_currentBank" type="Number" integer="true">
    /// </field>
    /// <field name="bankSwitchesChanged" type="Boolean">
    /// </field>
    /// <field name="_oneScreenOffset" type="Number" integer="true">
    /// </field>
    /// <field name="_usesSRAM" type="Boolean">
    /// </field>
    this._iNesHeader = new Int32Array(16);
    this._romControlBytes = new Int32Array(2);
    this._current8 = -1;
    this._currentA = -1;
    this._currentC = -1;
    this._currentE = -1;
    this._prgRomBank6 = new Int32Array(8192);
    this.mirroring = -1;
    this.ppuBankStarts = new Int32Array(16);
    this._bankStartCache = new Int32Array(256 * 16);
    for (var i = 0; i < 16; ++i) {
        this.ppuBankStarts[i] = i * 1024;
    }
}
NES.CPU.Machine.Carts.BaseCart.prototype = {
    _nesCart: null,
    _chrRom: null,
    
    get_chrRom: function NES_CPU_Machine_Carts_BaseCart$get_chrRom() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return this._chrRom;
    },
    set_chrRom: function NES_CPU_Machine_Carts_BaseCart$set_chrRom(value) {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        this._chrRom = value;
        return value;
    },
    
    _sramCanWrite: false,
    _sramEnabled: false,
    _sramCanSave: false,
    _prgRomCount: 0,
    _chrRomCount: 0,
    
    get_chrRomCount: function NES_CPU_Machine_Carts_BaseCart$get_chrRomCount() {
        /// <value type="Number" integer="true"></value>
        return this._chrRomCount;
    },
    
    get_prgRomCount: function NES_CPU_Machine_Carts_BaseCart$get_prgRomCount() {
        /// <value type="Number" integer="true"></value>
        return this._prgRomCount;
    },
    
    _mapperId: 0,
    _bank8start: 0,
    _bankAstart: 0,
    _bankCstart: 0,
    _bankEstart: 0,
    _ROMHashfunction: null,
    
    get_romHashFunction: function NES_CPU_Machine_Carts_BaseCart$get_romHashFunction() {
        /// <value type="NES.CPU.Machine.Carts.ROMHashFunctionDelegate"></value>
        return this._ROMHashfunction;
    },
    set_romHashFunction: function NES_CPU_Machine_Carts_BaseCart$set_romHashFunction(value) {
        /// <value type="NES.CPU.Machine.Carts.ROMHashFunctionDelegate"></value>
        this._ROMHashfunction = value;
        return value;
    },
    
    _chrRomOffset: 0,
    chrRamStart: 0,
    
    loadiNESCart: function NES_CPU_Machine_Carts_BaseCart$loadiNESCart(header, prgRoms, chrRoms, prgRomData, chrRomData, chrRomOffset) {
        /// <param name="header" type="Array" elementType="Number" elementInteger="true">
        /// </param>
        /// <param name="prgRoms" type="Number" integer="true">
        /// </param>
        /// <param name="chrRoms" type="Number" integer="true">
        /// </param>
        /// <param name="prgRomData" type="Array" elementType="Number" elementInteger="true">
        /// </param>
        /// <param name="chrRomData" type="Array" elementType="Number" elementInteger="true">
        /// </param>
        /// <param name="chrRomOffset" type="Number" integer="true">
        /// </param>
        this._romControlBytes[0] = header[6];
        this._romControlBytes[1] = header[7];
        this._mapperId = (this._romControlBytes[0] & 240) >>> 4;
        this._mapperId += this._romControlBytes[1] & 240;
        this._chrRomOffset = chrRomOffset;
        for (var i = 0; i < header.length; ++i) {
            this._iNesHeader[i] = header[i];
        }
        this._prgRomCount = prgRoms;
        this._chrRomCount = chrRoms;
        this._nesCart = new Int32Array(prgRomData.length);
        for (var i = 0; i < prgRomData.length; ++i) {
            this._nesCart[i] = prgRomData[i];
        }
        if (this._chrRomCount === 0) {
            chrRomData = new Int32Array(32768);
        }
        this._chrRom = new Int32Array(chrRomData.length + 4096);
        this.chrRamStart = chrRomData.length;
        for (var i = 0; i < chrRomData.length; ++i) {
            this._chrRom[i] = chrRomData[i];
        }
        this._prgRomCount = this._iNesHeader[4];
        this._chrRomCount = this._iNesHeader[5];
        this._romControlBytes[0] = this._iNesHeader[6];
        this._romControlBytes[1] = this._iNesHeader[7];
        this._sramCanSave = (this._romControlBytes[0] & 2) === 2;
        this._sramEnabled = true;
        this.set_usesSRAM((this._romControlBytes[0] & 2) === 2);
        this._mirror(0, 0);
        if ((this._romControlBytes[0] & 1) === 1) {
            this._mirror(0, 1);
        }
        else {
            this._mirror(0, 2);
        }
        if ((this._romControlBytes[0] & 8) === 8) {
            this._mirror(0, 3);
        }
        this.initializeCart();
    },
    
    whizzler: null,
    
    get_whizzler: function NES_CPU_Machine_Carts_BaseCart$get_whizzler() {
        /// <value type="NES.CPU.PixelWhizzlerClasses.IPPU"></value>
        return this.whizzler;
    },
    set_whizzler: function NES_CPU_Machine_Carts_BaseCart$set_whizzler(value) {
        /// <value type="NES.CPU.PixelWhizzlerClasses.IPPU"></value>
        this.whizzler = value;
        return value;
    },
    
    irqRaised: false,
    
    get_irqRaised: function NES_CPU_Machine_Carts_BaseCart$get_irqRaised() {
        /// <value type="Boolean"></value>
        return this.irqRaised;
    },
    set_irqRaised: function NES_CPU_Machine_Carts_BaseCart$set_irqRaised(value) {
        /// <value type="Boolean"></value>
        this.irqRaised = value;
        return value;
    },
    
    updateScanlineCounter: function NES_CPU_Machine_Carts_BaseCart$updateScanlineCounter() {
    },
    
    getByte: function NES_CPU_Machine_Carts_BaseCart$getByte(clock, address) {
        /// <param name="clock" type="Number" integer="true">
        /// </param>
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var bank = 0;
        switch (address & 57344) {
            case 24576:
                return this._prgRomBank6[address & 8191];
            case 32768:
                bank = this._bank8start;
                break;
            case 40960:
                bank = this._bankAstart;
                break;
            case 49152:
                bank = this._bankCstart;
                break;
            case 57344:
                bank = this._bankEstart;
                break;
        }
        if (bank + (address & 8191) > this._nesCart.length) {
            throw new Error('THis is broken!');
        }
        return this._nesCart[bank + (address & 8191)];
    },
    
    _setupBankStarts: function NES_CPU_Machine_Carts_BaseCart$_setupBankStarts(reg8, regA, regC, regE) {
        /// <param name="reg8" type="Number" integer="true">
        /// </param>
        /// <param name="regA" type="Number" integer="true">
        /// </param>
        /// <param name="regC" type="Number" integer="true">
        /// </param>
        /// <param name="regE" type="Number" integer="true">
        /// </param>
        reg8 = this._maskBankAddress(reg8);
        regA = this._maskBankAddress(regA);
        regC = this._maskBankAddress(regC);
        regE = this._maskBankAddress(regE);
        this._current8 = reg8;
        this._currentA = regA;
        this._currentC = regC;
        this._currentE = regE;
        this._bank8start = reg8 * 8192;
        this._bankAstart = regA * 8192;
        this._bankCstart = regC * 8192;
        this._bankEstart = regE * 8192;
    },
    
    _maskBankAddress: function NES_CPU_Machine_Carts_BaseCart$_maskBankAddress(bank) {
        /// <param name="bank" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        if (bank >= this._prgRomCount * 2) {
            var i = 255;
            while ((bank & i) >= this._prgRomCount * 2) {
                i = i >> 1;
            }
            return (bank & i);
        }
        else {
            return bank;
        }
    },
    
    _checkSum: null,
    
    get_checkSum: function NES_CPU_Machine_Carts_BaseCart$get_checkSum() {
        /// <value type="String"></value>
        return this._checkSum;
    },
    
    get_CPU: function NES_CPU_Machine_Carts_BaseCart$get_CPU() {
        /// <value type="NES.CPU.Fastendo.CPU2A03"></value>
        throw new Error('CPU Not Implemented');
    },
    set_CPU: function NES_CPU_Machine_Carts_BaseCart$set_CPU(value) {
        /// <value type="NES.CPU.Fastendo.CPU2A03"></value>
        throw new Error('CPU Not Implemented');
        return value;
    },
    
    get_SRAM: function NES_CPU_Machine_Carts_BaseCart$get_SRAM() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return this._prgRomBank6;
    },
    set_SRAM: function NES_CPU_Machine_Carts_BaseCart$set_SRAM(value) {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        if (value != null && value.length === this._prgRomBank6.length) {
            this._prgRomBank6 = value;
        }
        return value;
    },
    
    _cartName: null,
    
    get_cartName: function NES_CPU_Machine_Carts_BaseCart$get_cartName() {
        /// <value type="String"></value>
        return this._cartName;
    },
    set_cartName: function NES_CPU_Machine_Carts_BaseCart$set_cartName(value) {
        /// <value type="String"></value>
        this._cartName = value;
        return value;
    },
    
    get_numberOfPrgRoms: function NES_CPU_Machine_Carts_BaseCart$get_numberOfPrgRoms() {
        /// <value type="Number" integer="true"></value>
        return this._prgRomCount;
    },
    
    get_numberOfChrRoms: function NES_CPU_Machine_Carts_BaseCart$get_numberOfChrRoms() {
        /// <value type="Number" integer="true"></value>
        return this._chrRomCount;
    },
    
    get_mapperID: function NES_CPU_Machine_Carts_BaseCart$get_mapperID() {
        /// <value type="Number" integer="true"></value>
        return this._mapperId;
    },
    
    get_mirroring: function NES_CPU_Machine_Carts_BaseCart$get_mirroring() {
        /// <value type="NES.CPU.Machine.Carts.NameTableMirroring"></value>
        return this.mirroring;
    },
    
    _updateIRQ: null,
    
    get_nmiHandler: function NES_CPU_Machine_Carts_BaseCart$get_nmiHandler() {
        /// <value type="NES.CPU.Fastendo.MachineEvent"></value>
        return this._updateIRQ;
    },
    set_nmiHandler: function NES_CPU_Machine_Carts_BaseCart$set_nmiHandler(value) {
        /// <value type="NES.CPU.Fastendo.MachineEvent"></value>
        this._updateIRQ = value;
        return value;
    },
    
    get_irqAsserted: function NES_CPU_Machine_Carts_BaseCart$get_irqAsserted() {
        /// <value type="Boolean"></value>
        return false;
    },
    set_irqAsserted: function NES_CPU_Machine_Carts_BaseCart$set_irqAsserted(value) {
        /// <value type="Boolean"></value>
        return value;
    },
    
    get_nextEventAt: function NES_CPU_Machine_Carts_BaseCart$get_nextEventAt() {
        /// <value type="Number" integer="true"></value>
        return -1;
    },
    
    handleEvent: function NES_CPU_Machine_Carts_BaseCart$handleEvent(Clock) {
        /// <param name="Clock" type="Number" integer="true">
        /// </param>
    },
    
    resetClock: function NES_CPU_Machine_Carts_BaseCart$resetClock(Clock) {
        /// <param name="Clock" type="Number" integer="true">
        /// </param>
    },
    
    get_ppuBankStarts: function NES_CPU_Machine_Carts_BaseCart$get_ppuBankStarts() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return this.ppuBankStarts;
    },
    set_ppuBankStarts: function NES_CPU_Machine_Carts_BaseCart$set_ppuBankStarts(value) {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        this.ppuBankStarts = value;
        return value;
    },
    
    get_bankStartCache: function NES_CPU_Machine_Carts_BaseCart$get_bankStartCache() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return this._bankStartCache;
    },
    
    _currentBank: 0,
    
    get_currentBank: function NES_CPU_Machine_Carts_BaseCart$get_currentBank() {
        /// <value type="Number" integer="true"></value>
        return this._currentBank;
    },
    
    bankSwitchesChanged: false,
    
    get_bankSwitchesChanged: function NES_CPU_Machine_Carts_BaseCart$get_bankSwitchesChanged() {
        /// <value type="Boolean"></value>
        return this.bankSwitchesChanged;
    },
    set_bankSwitchesChanged: function NES_CPU_Machine_Carts_BaseCart$set_bankSwitchesChanged(value) {
        /// <value type="Boolean"></value>
        this.bankSwitchesChanged = value;
        return value;
    },
    
    getPPUByte: function NES_CPU_Machine_Carts_BaseCart$getPPUByte(clock, address) {
        /// <param name="clock" type="Number" integer="true">
        /// </param>
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var bank = address >> 10;
        var newAddress = this.ppuBankStarts[bank] + (address & 1023);
        return this._chrRom[newAddress];
    },
    
    actualChrRomOffset: function NES_CPU_Machine_Carts_BaseCart$actualChrRomOffset(address) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var bank = address >> 10;
        var newAddress = this.ppuBankStarts[bank] + (address & 1023);
        return newAddress;
    },
    
    setPPUByte: function NES_CPU_Machine_Carts_BaseCart$setPPUByte(clock, address, data) {
        /// <param name="clock" type="Number" integer="true">
        /// </param>
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <param name="data" type="Number" integer="true">
        /// </param>
        var bank = address >> 10;
        var newAddress = this.ppuBankStarts[bank] + (address & 1023);
        this._chrRom[newAddress] = data;
    },
    
    _oneScreenOffset: 0,
    
    get__oneScreenOffset: function NES_CPU_Machine_Carts_BaseCart$get__oneScreenOffset() {
        /// <value type="Number" integer="true"></value>
        return this._oneScreenOffset;
    },
    set__oneScreenOffset: function NES_CPU_Machine_Carts_BaseCart$set__oneScreenOffset(value) {
        /// <value type="Number" integer="true"></value>
        this._oneScreenOffset = value;
        return value;
    },
    
    _mirror: function NES_CPU_Machine_Carts_BaseCart$_mirror(clockNum, mirroring) {
        /// <param name="clockNum" type="Number" integer="true">
        /// </param>
        /// <param name="mirroring" type="Number" integer="true">
        /// </param>
        this.mirroring = mirroring;
        if (clockNum > -1) {
            drawTo(clockNum);
        }
        switch (mirroring) {
            case 0:
                this.ppuBankStarts[8] = this.chrRamStart + 0 + this._oneScreenOffset;
                this.ppuBankStarts[9] = this.chrRamStart + 0 + this._oneScreenOffset;
                this.ppuBankStarts[10] = this.chrRamStart + 0 + this._oneScreenOffset;
                this.ppuBankStarts[11] = this.chrRamStart + 0 + this._oneScreenOffset;
                break;
            case 1:
                this.ppuBankStarts[8] = this.chrRamStart + 0;
                this.ppuBankStarts[9] = this.chrRamStart + 1024;
                this.ppuBankStarts[10] = this.chrRamStart + 0;
                this.ppuBankStarts[11] = this.chrRamStart + 1024;
                break;
            case 2:
                this.ppuBankStarts[8] = this.chrRamStart + 0;
                this.ppuBankStarts[9] = this.chrRamStart + 0;
                this.ppuBankStarts[10] = this.chrRamStart + 1024;
                this.ppuBankStarts[11] = this.chrRamStart + 1024;
                break;
            case 3:
                this.ppuBankStarts[8] = this.chrRamStart + 0;
                this.ppuBankStarts[9] = this.chrRamStart + 1024;
                this.ppuBankStarts[10] = this.chrRamStart + 2048;
                this.ppuBankStarts[11] = this.chrRamStart + 3072;
                break;
        }
        updatePixelInfo();
    },
    
    _usesSRAM: false,
    
    get_usesSRAM: function NES_CPU_Machine_Carts_BaseCart$get_usesSRAM() {
        /// <value type="Boolean"></value>
        return this._usesSRAM;
    },
    set_usesSRAM: function NES_CPU_Machine_Carts_BaseCart$set_usesSRAM(value) {
        /// <value type="Boolean"></value>
        this._usesSRAM = value;
        return value;
    },
    
    get_chrRamStart: function NES_CPU_Machine_Carts_BaseCart$get_chrRamStart() {
        /// <value type="Number" integer="true"></value>
        return this.chrRamStart;
    },
    
    get_ppuBankStarts: function NES_CPU_Machine_Carts_BaseCart$get_ppuBankStarts() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return this.ppuBankStarts;
    },
    set_ppuBankStarts: function NES_CPU_Machine_Carts_BaseCart$set_ppuBankStarts(value) {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        throw new Error('CPU Not Implemented');
        return value;
    }
}


Type.registerNamespace('NES.CPU.Machine.ROMLoader');

////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Machine.ROMLoader.ByteArrayHolder

NES.CPU.Machine.ROMLoader.ByteArrayHolder = function NES_CPU_Machine_ROMLoader_ByteArrayHolder() {
    /// <field name="_position" type="Number" integer="true">
    /// </field>
    /// <field name="_data" type="Array" elementType="Number" elementInteger="true">
    /// </field>
}
NES.CPU.Machine.ROMLoader.ByteArrayHolder.prototype = {
    _position: 0,
    
    get_position: function NES_CPU_Machine_ROMLoader_ByteArrayHolder$get_position() {
        /// <value type="Number" integer="true"></value>
        return this._position;
    },
    set_position: function NES_CPU_Machine_ROMLoader_ByteArrayHolder$set_position(value) {
        /// <value type="Number" integer="true"></value>
        this._position = value;
        return value;
    },
    
    _data: null,
    
    get_data: function NES_CPU_Machine_ROMLoader_ByteArrayHolder$get_data() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return this._data;
    },
    set_data: function NES_CPU_Machine_ROMLoader_ByteArrayHolder$set_data(value) {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        this._data = value;
        return value;
    },
    
    read: function NES_CPU_Machine_ROMLoader_ByteArrayHolder$read(target, start, end) {
        /// <param name="target" type="Array" elementType="Number" elementInteger="true">
        /// </param>
        /// <param name="start" type="Number" integer="true">
        /// </param>
        /// <param name="end" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var bytesRead = 0;
        for (var i = start; i < start + end; ++i) {
            if (this._position >= this._data.length) {
                break;
            }
            target[i] = this._data[this._position];
            this._position++;
            bytesRead++;
        }
        return bytesRead;
    }
}


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Machine.ROMLoader.iNESFileHandler

NES.CPU.Machine.ROMLoader.iNESFileHandler = function NES_CPU_Machine_ROMLoader_iNESFileHandler() {
}
NES.CPU.Machine.ROMLoader.iNESFileHandler.loadROM = function NES_CPU_Machine_ROMLoader_iNESFileHandler$loadROM(ppu, zipStream) {
    /// <param name="ppu" type="NES.CPU.PixelWhizzlerClasses.IPPU">
    /// </param>
    /// <param name="zipStream" type="NES.CPU.Machine.ROMLoader.ByteArrayHolder">
    /// </param>
    /// <returns type="NES.CPU.Machine.Carts.INESCart"></returns>
    var _cart = null;
    var iNesHeader = new Int32Array(16);
    var bytesRead = zipStream.read(iNesHeader, 0, 16);
    var mapperId = (iNesHeader[6] & 240);
    mapperId = mapperId / 16;
    mapperId += iNesHeader[7];
    var prgRomCount = iNesHeader[4];
    var chrRomCount = iNesHeader[5];
    var theRom = new Int32Array(prgRomCount * 16384);
    var chrRom = new Int32Array(chrRomCount * 16384);
    var chrOffset = 0;
    bytesRead = zipStream.read(theRom, 0, theRom.length);
    chrOffset = zipStream.get_position();
    bytesRead = zipStream.read(chrRom, 0, chrRom.length);
    switch (mapperId) {
        case 0:
        case 2:
        case 3:
        case 7:
            _cart = new NES.CPU.NESCart();
            break;
        case 1:
            break;
        case 4:
            break;
    }
    if (_cart != null) {
        _cart.set_whizzler(ppu);
        set_chrRomHandler(_cart);
        _cart.loadiNESCart(iNesHeader, prgRomCount, chrRomCount, theRom, chrRom, chrOffset);
    }
    return _cart;
}


Type.registerNamespace('NES.CPU');

////////////////////////////////////////////////////////////////////////////////
// NES.CPU.NESCart

NES.CPU.NESCart = function NES_CPU_NESCart() {
    /// <field name="_prgRomBank6$1" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_prevBSSrc$1" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    this._prgRomBank6$1 = new Int32Array(2048);
    this._prevBSSrc$1 = new Int32Array(8);
    NES.CPU.NESCart.initializeBase(this);
}
NES.CPU.NESCart.prototype = {
    
    initializeCart: function NES_CPU_NESCart$initializeCart() {
        for (var i = 0; i < 8; ++i) {
            this._prevBSSrc$1[i] = -1;
        }
        switch (this._mapperId) {
            case 0:
            case 1:
            case 2:
            case 3:
                if (this.get_chrRomCount() > 0) {
                    this._copyBanks$1(0, 0, 0, 1);
                }
                this._setupBankStarts(0, 1, this.get_prgRomCount() * 2 - 2, this.get_prgRomCount() * 2 - 1);
                break;
            case 7:
                this._setupBankStarts(0, 1, 2, 3);
                this._mirror(0, 0);
                break;
            default:
                throw new Error('Mapper ' + this._mapperId.toString() + ' not implemented.');
        }
    },
    
    _copyBanks$1: function NES_CPU_NESCart$_copyBanks$1(clock, dest, src, numberOf8kBanks) {
        /// <param name="clock" type="Number" integer="true">
        /// </param>
        /// <param name="dest" type="Number" integer="true">
        /// </param>
        /// <param name="src" type="Number" integer="true">
        /// </param>
        /// <param name="numberOf8kBanks" type="Number" integer="true">
        /// </param>
        if (dest >= this.get_chrRomCount()) {
            dest = this.get_chrRomCount() - 1;
        }
        var oneKsrc = src * 8;
        var oneKdest = dest * 8;
        for (var i = 0; i < (numberOf8kBanks * 8); ++i) {
            this.ppuBankStarts[oneKdest + i] = (oneKsrc + i) * 1024;
        }
    },
    
    setByte: function NES_CPU_NESCart$setByte(clock, address, val) {
        /// <param name="clock" type="Number" integer="true">
        /// </param>
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <param name="val" type="Number" integer="true">
        /// </param>
        if (address >= 24576 && address <= 32767) {
            if (this._sramEnabled) {
                this._prgRomBank6$1[address & 8191] = val;
            }
            return;
        }
        if (this._mapperId === 7) {
            var newbank8 = 0;
            newbank8 = 4 * (val & 15);
            this._setupBankStarts(newbank8, newbank8 + 1, newbank8 + 2, newbank8 + 3);
            if ((val & 16) === 16) {
                this.set__oneScreenOffset(1024);
            }
            else {
                this.set__oneScreenOffset(0);
            }
            this._mirror(clock, 0);
        }
        if (this._mapperId === 3 && address >= 32768) {
            this._copyBanks$1(clock, 0, val, 1);
        }
        if (this._mapperId === 2 && address >= 32768) {
            var newbank8 = 0;
            newbank8 = (val * 2);
            this._setupBankStarts(newbank8, newbank8 + 1, this._currentC, this._currentE);
        }
    }
}


Type.registerNamespace('NES.CPU.Fastendo');

////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.AddressingModes

NES.CPU.Fastendo.AddressingModes = function() { 
    /// <field name="bullshit" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="implicit" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="accumulator" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="immediate" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="zeroPage" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="zeroPageX" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="zeroPageY" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="relative" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="absolute" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="absoluteX" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="absoluteY" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="indirect" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="indexedIndirect" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="indirectIndexed" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="indirectZeroPage" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="indirectAbsoluteX" type="Number" integer="true" static="true">
    /// </field>
};
NES.CPU.Fastendo.AddressingModes.prototype = {
    bullshit: 0, 
    implicit: 1, 
    accumulator: 2, 
    immediate: 3, 
    zeroPage: 4, 
    zeroPageX: 5, 
    zeroPageY: 6, 
    relative: 7, 
    absolute: 8, 
    absoluteX: 9, 
    absoluteY: 10, 
    indirect: 11, 
    indexedIndirect: 12, 
    indirectIndexed: 13, 
    indirectZeroPage: 14, 
    indirectAbsoluteX: 15
}
NES.CPU.Fastendo.AddressingModes.registerEnum('NES.CPU.Fastendo.AddressingModes', false);


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.CPUStatusBits

NES.CPU.Fastendo.CPUStatusBits = function() { 
    /// <field name="carry" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="zeroResult" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="interruptDisable" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="decimalMode" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="breakCommand" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="expansion" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="overflow" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="negativeResult" type="Number" integer="true" static="true">
    /// </field>
};
NES.CPU.Fastendo.CPUStatusBits.prototype = {
    carry: 0, 
    zeroResult: 1, 
    interruptDisable: 2, 
    decimalMode: 3, 
    breakCommand: 4, 
    expansion: 5, 
    overflow: 6, 
    negativeResult: 7
}
NES.CPU.Fastendo.CPUStatusBits.registerEnum('NES.CPU.Fastendo.CPUStatusBits', false);


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.CPUStatusMasks

NES.CPU.Fastendo.CPUStatusMasks = function() { 
    /// <field name="carryMask" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="zeroResultMask" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="interruptDisableMask" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="decimalModeMask" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="breakCommandMask" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="expansionMask" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="overflowMask" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="negativeResultMask" type="Number" integer="true" static="true">
    /// </field>
};
NES.CPU.Fastendo.CPUStatusMasks.prototype = {
    carryMask: 1, 
    zeroResultMask: 2, 
    interruptDisableMask: 4, 
    decimalModeMask: 8, 
    breakCommandMask: 16, 
    expansionMask: 32, 
    overflowMask: 64, 
    negativeResultMask: 128
}
NES.CPU.Fastendo.CPUStatusMasks.registerEnum('NES.CPU.Fastendo.CPUStatusMasks', false);


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.IMemoryMappedIOElement

NES.CPU.Fastendo.IMemoryMappedIOElement = function() { 
};
NES.CPU.Fastendo.IMemoryMappedIOElement.prototype = {
    getByte : null,
    setByte : null
}
NES.CPU.Fastendo.IMemoryMappedIOElement.registerInterface('NES.CPU.Fastendo.IMemoryMappedIOElement');


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.IClockedMemoryMappedIOElement

NES.CPU.Fastendo.IClockedMemoryMappedIOElement = function() { 
};
NES.CPU.Fastendo.IClockedMemoryMappedIOElement.prototype = {
    getByte : null,
    setByte : null,
    get_nmiHandler : null,
    set_nmiHandler : null,
    get_irqAsserted : null,
    set_irqAsserted : null,
    get_nextEventAt : null,
    handleEvent : null,
    resetClock : null
}
NES.CPU.Fastendo.IClockedMemoryMappedIOElement.registerInterface('NES.CPU.Fastendo.IClockedMemoryMappedIOElement');


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.CPU2A03

NES.CPU.Fastendo.CPU2A03 = function NES_CPU_Fastendo_CPU2A03() {
    /// <field name="_ticks" type="Number" integer="true">
    /// </field>
    /// <field name="_operationCounter" type="Number" integer="true">
    /// </field>
    /// <field name="_accumulator" type="Number" integer="true">
    /// </field>
    /// <field name="_indexRegisterX" type="Number" integer="true">
    /// </field>
    /// <field name="_indexRegisterY" type="Number" integer="true">
    /// </field>
    /// <field name="_programCounter" type="Number" integer="true">
    /// </field>
    /// <field name="_statusRegister" type="Number" integer="true">
    /// </field>
    /// <field name="_addressBus" type="Number" integer="true">
    /// </field>
    /// <field name="_dataBus" type="Number" integer="true">
    /// </field>
    /// <field name="_memoryLock" type="Boolean">
    /// </field>
    /// <field name="_reset" type="Boolean">
    /// </field>
    /// <field name="curinst_AddressingMode" type="NES.CPU.Fastendo.AddressingModes">
    /// </field>
    /// <field name="curinst_Address" type="Number" integer="true">
    /// </field>
    /// <field name="curinst_OpCode" type="Number" integer="true">
    /// </field>
    /// <field name="curinst_Parameters0" type="Number" integer="true">
    /// </field>
    /// <field name="curinst_Parameters1" type="Number" integer="true">
    /// </field>
    /// <field name="curinst_ExtraTiming" type="Number" integer="true">
    /// </field>
    /// <field name="curinst_Length" type="Number" integer="true">
    /// </field>
    /// <field name="_clock" type="Number" integer="true">
    /// </field>
    /// <field name="_handleNMI" type="Boolean">
    /// </field>
    /// <field name="_handleIRQ" type="Boolean">
    /// </field>
    /// <field name="_nextEvent" type="Number" integer="true">
    /// </field>
    /// <field name="_runningHard" type="Boolean">
    /// </field>
    /// <field name="_clockcount" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_instruction" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="addressmode" type="Array" elementType="AddressingModes">
    /// </field>
    /// <field name="_lowByte" type="Number" integer="true">
    /// </field>
    /// <field name="_highByte" type="Number" integer="true">
    /// </field>
    /// <field name="_cpuTiming" type="Array" elementType="Number" elementInteger="true" static="true">
    /// </field>
    /// <field name="_rams" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_cart" type="NES.CPU.Machine.Carts.INESCart">
    /// </field>
    /// <field name="_pixelWhizzler" type="NES.CPU.PixelWhizzlerClasses.IPPU">
    /// </field>
    /// <field name="_soundBopper" type="NES.CPU.Fastendo.IClockedMemoryMappedIOElement">
    /// </field>
    /// <field name="_nmiHandler" type="NES.CPU.Fastendo.MachineEvent">
    /// </field>
    /// <field name="_irqUpdater" type="NES.CPU.Fastendo.MachineEvent">
    /// </field>
    /// <field name="_stackPointer" type="Number" integer="true">
    /// </field>
    /// <field name="_instructionUsage" type="Array" elementType="Number" elementInteger="true">
    /// </field>
    /// <field name="_debugging" type="Boolean">
    /// </field>
    /// <field name="_instructionHistoryPointer" type="Number" integer="true">
    /// </field>
    /// <field name="_instructionHistory" type="Array" elementType="Instruction">
    /// </field>
    this._nextEvent = ~0;
    this._clockcount = new Int32Array(256);
    this._instruction = new Int32Array(256);
    this.addressmode = new Int32Array(256);
    this._rams = new Int32Array(8192);
    this._instructionUsage = new Int32Array(256);
    this._instructionHistory = new Int32Array(256);
    this._nmiHandler = ss.Delegate.create(this, this._nmiHandler);
    this._irqUpdater = ss.Delegate.create(this, this._irqUpdater);
    this.setupticks();
}

NES.CPU.Fastendo.CPU2A03.prototype = {
    _ticks: 0,
    _operationCounter: 0,
    _accumulator: 0,
    _indexRegisterX: 0,
    _indexRegisterY: 0,
    
    get_accumulator: function NES_CPU_Fastendo_CPU2A03$get_accumulator() {
        /// <value type="Number" integer="true"></value>
        return this._accumulator;
    },
    set_accumulator: function NES_CPU_Fastendo_CPU2A03$set_accumulator(value) {
        /// <value type="Number" integer="true"></value>
        this._accumulator = value;
        return value;
    },
    
    get_indexRegisterY: function NES_CPU_Fastendo_CPU2A03$get_indexRegisterY() {
        /// <value type="Number" integer="true"></value>
        return this._indexRegisterY;
    },
    set_indexRegisterY: function NES_CPU_Fastendo_CPU2A03$set_indexRegisterY(value) {
        /// <value type="Number" integer="true"></value>
        this._indexRegisterY = value;
        return value;
    },
    
    get_indexRegisterX: function NES_CPU_Fastendo_CPU2A03$get_indexRegisterX() {
        /// <value type="Number" integer="true"></value>
        return this._indexRegisterX;
    },
    set_indexRegisterX: function NES_CPU_Fastendo_CPU2A03$set_indexRegisterX(value) {
        /// <value type="Number" integer="true"></value>
        this._indexRegisterX = value;
        return value;
    },
    
    _programCounter: 0,
    
    get_programCounter: function NES_CPU_Fastendo_CPU2A03$get_programCounter() {
        /// <value type="Number" integer="true"></value>
        return this._programCounter;
    },
    set_programCounter: function NES_CPU_Fastendo_CPU2A03$set_programCounter(value) {
        /// <value type="Number" integer="true"></value>
        this._programCounter = value;
        return value;
    },
    
    _statusRegister: 0,
    
    get_statusRegister: function NES_CPU_Fastendo_CPU2A03$get_statusRegister() {
        /// <value type="Number" integer="true"></value>
        return this._statusRegister;
    },
    set_statusRegister: function NES_CPU_Fastendo_CPU2A03$set_statusRegister(value) {
        /// <value type="Number" integer="true"></value>
        this._statusRegister = value;
        return value;
    },
    
    get_addressCodePage: function NES_CPU_Fastendo_CPU2A03$get_addressCodePage() {
        /// <value type="Number" integer="true"></value>
        var retval = this.get_addressBus() >> 8;
        return retval;
    },
    
    get_addressLowByte: function NES_CPU_Fastendo_CPU2A03$get_addressLowByte() {
        /// <value type="Number" integer="true"></value>
        return this.get_addressBus() & 255;
    },
    
    _addressBus: 0,
    
    get_addressBus: function NES_CPU_Fastendo_CPU2A03$get_addressBus() {
        /// <value type="Number" integer="true"></value>
        return this._addressBus;
    },
    set_addressBus: function NES_CPU_Fastendo_CPU2A03$set_addressBus(value) {
        /// <value type="Number" integer="true"></value>
        this._addressBus = value;
        return value;
    },
    
    _dataBus: 0,
    
    get_dataBus: function NES_CPU_Fastendo_CPU2A03$get_dataBus() {
        /// <value type="Number" integer="true"></value>
        return this._dataBus;
    },
    set_dataBus: function NES_CPU_Fastendo_CPU2A03$set_dataBus(value) {
        /// <value type="Number" integer="true"></value>
        this._dataBus = value;
        return value;
    },
    
    _getSRMask: function NES_CPU_Fastendo_CPU2A03$_getSRMask(flag) {
        /// <param name="flag" type="NES.CPU.Fastendo.CPUStatusBits">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        switch (flag) {
            case NES.CPU.Fastendo.CPUStatusBits.carry:
                return 1;
            case NES.CPU.Fastendo.CPUStatusBits.zeroResult:
                return 2;
            case NES.CPU.Fastendo.CPUStatusBits.interruptDisable:
                return 4;
            case NES.CPU.Fastendo.CPUStatusBits.decimalMode:
                return 8;
            case NES.CPU.Fastendo.CPUStatusBits.breakCommand:
                return 16;
            case NES.CPU.Fastendo.CPUStatusBits.expansion:
                return 32;
            case NES.CPU.Fastendo.CPUStatusBits.overflow:
                return 64;
            case NES.CPU.Fastendo.CPUStatusBits.negativeResult:
                return 128;
        }
        return 0;
    },
    
    setFlag: function NES_CPU_Fastendo_CPU2A03$setFlag(Flag, value) {
        /// <param name="Flag" type="NES.CPU.Fastendo.CPUStatusMasks">
        /// </param>
        /// <param name="value" type="Boolean">
        /// </param>
        this._statusRegister = ((value) ? (this._statusRegister | Flag) : (this._statusRegister & ~Flag));
        this._statusRegister |= NES.CPU.Fastendo.CPUStatusMasks.expansionMask;
    },
    
    getFlag: function NES_CPU_Fastendo_CPU2A03$getFlag(Flag) {
        /// <param name="Flag" type="NES.CPU.Fastendo.CPUStatusMasks">
        /// </param>
        /// <returns type="Boolean"></returns>
        var flag = Flag;
        return ((this._statusRegister & flag) === flag);
    },
    
    interruptRequest: function NES_CPU_Fastendo_CPU2A03$interruptRequest() {
        if (this.getFlag(NES.CPU.Fastendo.CPUStatusMasks.interruptDisableMask)) {
            return;
        }
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.interruptDisableMask, true);
        var newStatusReg = this._statusRegister & ~16 | 32;
        this.pushStack(this.get_programCounter() / 256);
        this.pushStack(this.get_programCounter());
        this.pushStack(this.get_statusRegister());
        this.set_programCounter(this.getByte(65534) + (this.getByte(65535) << 8));
    },
    
    _memoryLock: false,
    
    get_memoryLock: function NES_CPU_Fastendo_CPU2A03$get_memoryLock() {
        /// <value type="Boolean"></value>
        return this._memoryLock;
    },
    set_memoryLock: function NES_CPU_Fastendo_CPU2A03$set_memoryLock(value) {
        /// <value type="Boolean"></value>
        this._memoryLock = value;
        return value;
    },
    
    nonMaskableInterrupt: function NES_CPU_Fastendo_CPU2A03$nonMaskableInterrupt() {
        var newStatusReg = this._statusRegister & ~16 | 32;
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.interruptDisableMask, true);
        this.pushStack(this._programCounter >> 8);
        this.pushStack(this._programCounter & 255);
        this.pushStack(newStatusReg);
        var lowByte = this.getByte(65530);
        var highByte = this.getByte(65531);
        var jumpTo = lowByte | (highByte << 8);
        this.set_programCounter(jumpTo);
    },
    
    _reset: false,
    
    get_reset: function NES_CPU_Fastendo_CPU2A03$get_reset() {
        /// <value type="Boolean"></value>
        return this._reset;
    },
    set_reset: function NES_CPU_Fastendo_CPU2A03$set_reset(value) {
        /// <value type="Boolean"></value>
        this._reset = value;
        if (this._reset) {
            this.resetCPU();
        }
        return value;
    },
    
    curinst_AddressingMode: 0,
    curinst_Address: 0,
    curinst_OpCode: 0,
    curinst_Parameters0: 0,
    curinst_Parameters1: 0,
    curinst_ExtraTiming: 0,
    curinst_Length: 0,
    
    get_operationCounter: function NES_CPU_Fastendo_CPU2A03$get_operationCounter() {
        /// <value type="Number" integer="true"></value>
        return this._operationCounter;
    },
    
    _clock: 0,
    
    get_clock: function NES_CPU_Fastendo_CPU2A03$get_clock() {
        /// <value type="Number" integer="true"></value>
        return this._clock;
    },
    set_clock: function NES_CPU_Fastendo_CPU2A03$set_clock(value) {
        /// <value type="Number" integer="true"></value>
        this._clock = value;
        return value;
    },
    
    _handleNMI: false,
    _handleIRQ: false,
    _runningHard: false,
    
    get_runningHard: function NES_CPU_Fastendo_CPU2A03$get_runningHard() {
        /// <value type="Boolean"></value>
        return this._runningHard;
    },
    set_runningHard: function NES_CPU_Fastendo_CPU2A03$set_runningHard(value) {
        /// <value type="Boolean"></value>
        this._runningHard = value;
        return value;
    },
    
    checkEvent: function NES_CPU_Fastendo_CPU2A03$checkEvent() {
        if (this._nextEvent === -1) {
            this.findNextEvent();
        }
    },
    
    runFast: function NES_CPU_Fastendo_CPU2A03$runFast() {
        while (this._clock < 29780) {
            this.step();
        }
    },
    
    step: function NES_CPU_Fastendo_CPU2A03$step() {
        this.curinst_ExtraTiming = 0;
        if (this._nextEvent <= this._clock) {
            this._handleNextEvent();
        }
        if (this._handleNMI) {
            this._handleNMI = false;
            this._clock += 7;
            this.nonMaskableInterrupt();
        }
        else if (this._handleIRQ) {
            this._handleIRQ = false;
            this._clock += 7;
            this.interruptRequest();
        }
        this.curinst_Address = this._programCounter;
        this.curinst_OpCode = this.getByte(this._programCounter++);
        this.curinst_AddressingMode = this.addressmode[this.curinst_OpCode];
        switch (this.curinst_AddressingMode) {
            case NES.CPU.Fastendo.AddressingModes.absolute:
            case NES.CPU.Fastendo.AddressingModes.absoluteX:
            case NES.CPU.Fastendo.AddressingModes.absoluteY:
            case NES.CPU.Fastendo.AddressingModes.indirect:
                this.curinst_Parameters0 = this.getByte(this._programCounter++);
                this.curinst_Parameters1 = this.getByte(this._programCounter++);
                break;
            case NES.CPU.Fastendo.AddressingModes.zeroPage:
            case NES.CPU.Fastendo.AddressingModes.zeroPageX:
            case NES.CPU.Fastendo.AddressingModes.zeroPageY:
            case NES.CPU.Fastendo.AddressingModes.relative:
            case NES.CPU.Fastendo.AddressingModes.indexedIndirect:
            case NES.CPU.Fastendo.AddressingModes.indirectIndexed:
            case NES.CPU.Fastendo.AddressingModes.indirectZeroPage:
            case NES.CPU.Fastendo.AddressingModes.immediate:
                this.curinst_Parameters0 = this.getByte(this._programCounter++);
                break;
            case NES.CPU.Fastendo.AddressingModes.accumulator:
            case NES.CPU.Fastendo.AddressingModes.implicit:
                break;
            default:
                break;
        }
        this.execute();
        this._clock += NES.CPU.Fastendo.CPU2A03._cpuTiming[this.curinst_OpCode] + this.curinst_ExtraTiming;
    },
    
    runCycles: function NES_CPU_Fastendo_CPU2A03$runCycles(count) {
        /// <summary>
        /// runs up to x clock cycles, then returns
        /// </summary>
        /// <param name="count" type="Number" integer="true">
        /// </param>
        var startCycles = this._ticks;
        while (this._ticks - startCycles < count) {
            this.step();
        }
    },
    
    get_ticks: function NES_CPU_Fastendo_CPU2A03$get_ticks() {
        /// <summary>
        /// number of full clock ticks elapsed since emulation started
        /// </summary>
        /// <value type="Number" integer="true"></value>
        return this._ticks;
    },
    set_ticks: function NES_CPU_Fastendo_CPU2A03$set_ticks(value) {
        /// <summary>
        /// number of full clock ticks elapsed since emulation started
        /// </summary>
        /// <value type="Number" integer="true"></value>
        this._ticks = value;
        return value;
    },
    
    setupticks: function NES_CPU_Fastendo_CPU2A03$setupticks() {
        this._clockcount[0] = 7;
        this.addressmode[0] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[1] = 6;
        this.addressmode[1] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[2] = 2;
        this.addressmode[2] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[3] = 2;
        this.addressmode[3] = NES.CPU.Fastendo.AddressingModes.bullshit;
        this._clockcount[4] = 3;
        this.addressmode[4] = NES.CPU.Fastendo.AddressingModes.bullshit;
        this._clockcount[5] = 3;
        this.addressmode[5] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[6] = 5;
        this.addressmode[6] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[7] = 2;
        this.addressmode[7] = NES.CPU.Fastendo.AddressingModes.bullshit;
        this._clockcount[8] = 3;
        this.addressmode[8] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[9] = 3;
        this.addressmode[9] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[10] = 2;
        this.addressmode[10] = NES.CPU.Fastendo.AddressingModes.accumulator;
        this._clockcount[11] = 2;
        this.addressmode[11] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[12] = 4;
        this.addressmode[12] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[13] = 4;
        this.addressmode[13] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[14] = 6;
        this.addressmode[14] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[15] = 2;
        this.addressmode[15] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[16] = 2;
        this.addressmode[16] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[17] = 5;
        this.addressmode[17] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[18] = 3;
        this.addressmode[18] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[19] = 2;
        this.addressmode[19] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[20] = 3;
        this.addressmode[20] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[21] = 4;
        this.addressmode[21] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[22] = 6;
        this.addressmode[22] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[23] = 2;
        this.addressmode[23] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[24] = 2;
        this.addressmode[24] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[25] = 4;
        this.addressmode[25] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[26] = 2;
        this.addressmode[26] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[27] = 2;
        this.addressmode[27] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[28] = 4;
        this.addressmode[28] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[29] = 4;
        this.addressmode[29] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[30] = 7;
        this.addressmode[30] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[31] = 2;
        this.addressmode[31] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[32] = 6;
        this.addressmode[32] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[33] = 6;
        this.addressmode[33] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[34] = 2;
        this.addressmode[34] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[35] = 2;
        this.addressmode[35] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[36] = 3;
        this.addressmode[36] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[37] = 3;
        this.addressmode[37] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[38] = 5;
        this.addressmode[38] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[39] = 2;
        this.addressmode[39] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[40] = 4;
        this.addressmode[40] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[41] = 3;
        this.addressmode[41] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[42] = 2;
        this.addressmode[42] = NES.CPU.Fastendo.AddressingModes.accumulator;
        this._clockcount[43] = 2;
        this.addressmode[43] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[44] = 4;
        this.addressmode[44] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[45] = 4;
        this.addressmode[45] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[46] = 6;
        this.addressmode[46] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[47] = 2;
        this.addressmode[47] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[48] = 2;
        this.addressmode[48] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[49] = 5;
        this.addressmode[49] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[50] = 3;
        this.addressmode[50] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[51] = 2;
        this.addressmode[51] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[52] = 4;
        this.addressmode[52] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[53] = 4;
        this.addressmode[53] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[54] = 6;
        this.addressmode[54] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[55] = 2;
        this.addressmode[55] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[56] = 2;
        this.addressmode[56] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[57] = 4;
        this.addressmode[57] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[58] = 2;
        this.addressmode[58] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[59] = 2;
        this.addressmode[59] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[60] = 4;
        this.addressmode[60] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[61] = 4;
        this.addressmode[61] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[62] = 7;
        this.addressmode[62] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[63] = 2;
        this.addressmode[63] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[64] = 6;
        this.addressmode[64] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[65] = 6;
        this.addressmode[65] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[66] = 2;
        this.addressmode[66] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[67] = 2;
        this.addressmode[67] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[68] = 2;
        this.addressmode[68] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[69] = 3;
        this.addressmode[69] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[70] = 5;
        this.addressmode[70] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[71] = 2;
        this.addressmode[71] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[72] = 3;
        this.addressmode[72] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[73] = 3;
        this.addressmode[73] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[74] = 2;
        this.addressmode[74] = NES.CPU.Fastendo.AddressingModes.accumulator;
        this._clockcount[75] = 2;
        this.addressmode[75] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[76] = 3;
        this.addressmode[76] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[77] = 4;
        this.addressmode[77] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[78] = 6;
        this.addressmode[78] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[79] = 2;
        this.addressmode[79] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[80] = 2;
        this.addressmode[80] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[81] = 5;
        this.addressmode[81] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[82] = 3;
        this.addressmode[82] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[83] = 2;
        this.addressmode[83] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[84] = 2;
        this.addressmode[84] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[85] = 4;
        this.addressmode[85] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[86] = 6;
        this.addressmode[86] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[87] = 2;
        this.addressmode[87] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[88] = 2;
        this.addressmode[88] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[89] = 4;
        this.addressmode[89] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[90] = 3;
        this.addressmode[90] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[91] = 2;
        this.addressmode[91] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[92] = 2;
        this.addressmode[92] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[93] = 4;
        this.addressmode[93] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[94] = 7;
        this.addressmode[94] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[95] = 2;
        this.addressmode[95] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[96] = 6;
        this.addressmode[96] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[97] = 6;
        this.addressmode[97] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[98] = 2;
        this.addressmode[98] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[99] = 2;
        this.addressmode[99] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[100] = 3;
        this.addressmode[100] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[101] = 3;
        this.addressmode[101] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[102] = 5;
        this.addressmode[102] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[103] = 2;
        this.addressmode[103] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[104] = 4;
        this.addressmode[104] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[105] = 3;
        this.addressmode[105] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[106] = 2;
        this.addressmode[106] = NES.CPU.Fastendo.AddressingModes.accumulator;
        this._clockcount[107] = 2;
        this.addressmode[107] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[108] = 5;
        this.addressmode[108] = NES.CPU.Fastendo.AddressingModes.indirect;
        this._clockcount[109] = 4;
        this.addressmode[109] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[110] = 6;
        this.addressmode[110] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[111] = 2;
        this.addressmode[111] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[112] = 2;
        this.addressmode[112] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[113] = 5;
        this.addressmode[113] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[114] = 3;
        this.addressmode[114] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[115] = 2;
        this.addressmode[115] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[116] = 4;
        this.addressmode[116] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[117] = 4;
        this.addressmode[117] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[118] = 6;
        this.addressmode[118] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[119] = 2;
        this.addressmode[119] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[120] = 2;
        this.addressmode[120] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[121] = 4;
        this.addressmode[121] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[122] = 4;
        this.addressmode[122] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[123] = 2;
        this.addressmode[123] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[124] = 6;
        this.addressmode[124] = NES.CPU.Fastendo.AddressingModes.indirectAbsoluteX;
        this._clockcount[125] = 4;
        this.addressmode[125] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[126] = 7;
        this.addressmode[126] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[127] = 2;
        this.addressmode[127] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[128] = 2;
        this.addressmode[128] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[129] = 6;
        this.addressmode[129] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[130] = 2;
        this.addressmode[130] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[131] = 2;
        this.addressmode[131] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[132] = 2;
        this.addressmode[132] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[133] = 2;
        this.addressmode[133] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[134] = 2;
        this.addressmode[134] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[135] = 2;
        this.addressmode[135] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[136] = 2;
        this.addressmode[136] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[137] = 2;
        this.addressmode[137] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[138] = 2;
        this.addressmode[138] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[139] = 2;
        this.addressmode[139] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[140] = 4;
        this.addressmode[140] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[141] = 4;
        this.addressmode[141] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[142] = 4;
        this.addressmode[142] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[143] = 2;
        this.addressmode[143] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[144] = 2;
        this.addressmode[144] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[145] = 6;
        this.addressmode[145] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[146] = 3;
        this.addressmode[146] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[147] = 2;
        this.addressmode[147] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[148] = 4;
        this.addressmode[148] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[149] = 4;
        this.addressmode[149] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[150] = 4;
        this.addressmode[150] = NES.CPU.Fastendo.AddressingModes.zeroPageY;
        this._clockcount[151] = 2;
        this.addressmode[151] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[152] = 2;
        this.addressmode[152] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[153] = 5;
        this.addressmode[153] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[154] = 2;
        this.addressmode[154] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[155] = 2;
        this.addressmode[155] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[156] = 4;
        this.addressmode[156] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[157] = 5;
        this.addressmode[157] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[158] = 5;
        this.addressmode[158] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[159] = 2;
        this.addressmode[159] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[160] = 3;
        this.addressmode[160] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[161] = 6;
        this.addressmode[161] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[162] = 3;
        this.addressmode[162] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[163] = 2;
        this.addressmode[163] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[164] = 3;
        this.addressmode[164] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[165] = 3;
        this.addressmode[165] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[166] = 3;
        this.addressmode[166] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[167] = 2;
        this.addressmode[167] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[168] = 2;
        this.addressmode[168] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[169] = 3;
        this.addressmode[169] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[170] = 2;
        this.addressmode[170] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[171] = 2;
        this.addressmode[171] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[172] = 4;
        this.addressmode[172] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[173] = 4;
        this.addressmode[173] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[174] = 4;
        this.addressmode[174] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[175] = 2;
        this.addressmode[175] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[176] = 2;
        this.addressmode[176] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[177] = 5;
        this.addressmode[177] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[178] = 3;
        this.addressmode[178] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[179] = 2;
        this.addressmode[179] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[180] = 4;
        this.addressmode[180] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[181] = 4;
        this.addressmode[181] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[182] = 4;
        this.addressmode[182] = NES.CPU.Fastendo.AddressingModes.zeroPageY;
        this._clockcount[183] = 2;
        this.addressmode[183] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[184] = 2;
        this.addressmode[184] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[185] = 4;
        this.addressmode[185] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[186] = 2;
        this.addressmode[186] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[187] = 2;
        this.addressmode[187] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[188] = 4;
        this.addressmode[188] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[189] = 4;
        this.addressmode[189] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[190] = 4;
        this.addressmode[190] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[191] = 2;
        this.addressmode[191] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[192] = 3;
        this.addressmode[192] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[193] = 6;
        this.addressmode[193] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[194] = 2;
        this.addressmode[194] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[195] = 2;
        this.addressmode[195] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[196] = 3;
        this.addressmode[196] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[197] = 3;
        this.addressmode[197] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[198] = 5;
        this.addressmode[198] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[199] = 2;
        this.addressmode[199] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[200] = 2;
        this.addressmode[200] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[201] = 3;
        this.addressmode[201] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[202] = 2;
        this.addressmode[202] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[203] = 2;
        this.addressmode[203] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[204] = 4;
        this.addressmode[204] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[205] = 4;
        this.addressmode[205] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[206] = 6;
        this.addressmode[206] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[207] = 2;
        this.addressmode[207] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[208] = 2;
        this.addressmode[208] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[209] = 5;
        this.addressmode[209] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[210] = 3;
        this.addressmode[210] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[211] = 2;
        this.addressmode[211] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[212] = 2;
        this.addressmode[212] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[213] = 4;
        this.addressmode[213] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[214] = 6;
        this.addressmode[214] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[215] = 2;
        this.addressmode[215] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[216] = 2;
        this.addressmode[216] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[217] = 4;
        this.addressmode[217] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[218] = 3;
        this.addressmode[218] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[219] = 2;
        this.addressmode[219] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[220] = 2;
        this.addressmode[220] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[221] = 4;
        this.addressmode[221] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[222] = 7;
        this.addressmode[222] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[223] = 2;
        this.addressmode[223] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[224] = 3;
        this.addressmode[224] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[225] = 6;
        this.addressmode[225] = NES.CPU.Fastendo.AddressingModes.indexedIndirect;
        this._clockcount[226] = 2;
        this.addressmode[226] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[227] = 2;
        this.addressmode[227] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[228] = 3;
        this.addressmode[228] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[229] = 3;
        this.addressmode[229] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[230] = 5;
        this.addressmode[230] = NES.CPU.Fastendo.AddressingModes.zeroPage;
        this._clockcount[231] = 2;
        this.addressmode[231] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[232] = 2;
        this.addressmode[232] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[233] = 3;
        this.addressmode[233] = NES.CPU.Fastendo.AddressingModes.immediate;
        this._clockcount[234] = 2;
        this.addressmode[234] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[235] = 2;
        this.addressmode[235] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[236] = 4;
        this.addressmode[236] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[237] = 4;
        this.addressmode[237] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[238] = 6;
        this.addressmode[238] = NES.CPU.Fastendo.AddressingModes.absolute;
        this._clockcount[239] = 2;
        this.addressmode[239] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[240] = 2;
        this.addressmode[240] = NES.CPU.Fastendo.AddressingModes.relative;
        this._clockcount[241] = 5;
        this.addressmode[241] = NES.CPU.Fastendo.AddressingModes.indirectIndexed;
        this._clockcount[242] = 3;
        this.addressmode[242] = NES.CPU.Fastendo.AddressingModes.indirectZeroPage;
        this._clockcount[243] = 2;
        this.addressmode[243] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[244] = 2;
        this.addressmode[244] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[245] = 4;
        this.addressmode[245] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[246] = 6;
        this.addressmode[246] = NES.CPU.Fastendo.AddressingModes.zeroPageX;
        this._clockcount[247] = 2;
        this.addressmode[247] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[248] = 2;
        this.addressmode[248] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[249] = 4;
        this.addressmode[249] = NES.CPU.Fastendo.AddressingModes.absoluteY;
        this._clockcount[250] = 4;
        this.addressmode[250] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[251] = 2;
        this.addressmode[251] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[252] = 2;
        this.addressmode[252] = NES.CPU.Fastendo.AddressingModes.implicit;
        this._clockcount[253] = 4;
        this.addressmode[253] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[254] = 7;
        this.addressmode[254] = NES.CPU.Fastendo.AddressingModes.absoluteX;
        this._clockcount[255] = 2;
        this.addressmode[255] = NES.CPU.Fastendo.AddressingModes.implicit;
    },
    
    resetCPU: function NES_CPU_Fastendo_CPU2A03$resetCPU() {
        this._statusRegister = 52;
        this._operationCounter = 0;
        this._stackPointer = 253;
        this.setupticks();
        this.set_ticks(0);
        this.set_programCounter(this.getByte(65532) + this.getByte(65533) * 256);
    },
    
    powerOn: function NES_CPU_Fastendo_CPU2A03$powerOn() {
        this._statusRegister = 52;
        this._stackPointer = 253;
        this._operationCounter = 0;
        this.setupticks();
        this.set_ticks(0);
        for (var i = 0; i < 2048; ++i) {
            this._rams[i] = 255;
        }
        this._rams[8] = 247;
        this._rams[9] = 239;
        this._rams[10] = 223;
        this._rams[15] = 191;
        this.set_programCounter(this.getByte(65532) + this.getByte(65533) * 256);
    },
    
    _lowByte: 0,
    _highByte: 0,
    
    _decodeAddress: function NES_CPU_Fastendo_CPU2A03$_decodeAddress() {
        /// <returns type="Number" integer="true"></returns>
        this.curinst_ExtraTiming = 0;
        var result = 0;
        switch (this.curinst_AddressingMode) {
            case NES.CPU.Fastendo.AddressingModes.absolute:
                result = ((this.curinst_Parameters1 << 8) | this.curinst_Parameters0);
                break;
            case NES.CPU.Fastendo.AddressingModes.absoluteX:
                result = (((this.curinst_Parameters1 << 8) | this.curinst_Parameters0) + this._indexRegisterX);
                if ((result & 255) < this._indexRegisterX) {
                    this.curinst_ExtraTiming = 1;
                }
                break;
            case NES.CPU.Fastendo.AddressingModes.absoluteY:
                result = (((this.curinst_Parameters1 << 8) | this.curinst_Parameters0) + this._indexRegisterY);
                if ((result & 255) < this._indexRegisterY) {
                    this.curinst_ExtraTiming = 1;
                }
                break;
            case NES.CPU.Fastendo.AddressingModes.zeroPage:
                result = this.curinst_Parameters0 & 255;
                break;
            case NES.CPU.Fastendo.AddressingModes.zeroPageX:
                result = (this.curinst_Parameters0 + this._indexRegisterX) & 255;
                break;
            case NES.CPU.Fastendo.AddressingModes.zeroPageY:
                result = (this.curinst_Parameters0 + this._indexRegisterY) & 255;
                break;
            case NES.CPU.Fastendo.AddressingModes.indirect:
                this._lowByte = this.curinst_Parameters0;
                this._highByte = this.curinst_Parameters1 << 8;
                var indAddr = (this._highByte | this._lowByte) & 65535;
                var indirectAddr = this.getByte(indAddr);
                this._lowByte = (this._lowByte + 1) & 255;
                indAddr = (this._highByte | this._lowByte) & 65535;
                indirectAddr |= (this.getByte(indAddr) << 8);
                result = indirectAddr;
                break;
            case NES.CPU.Fastendo.AddressingModes.indexedIndirect:
                var addr = (this.curinst_Parameters0 + this._indexRegisterX) & 255;
                this._lowByte = this.getByte(addr);
                addr = addr + 1;
                this._highByte = this.getByte(addr & 255);
                this._highByte = this._highByte << 8;
                result = this._highByte | this._lowByte;
                break;
            case NES.CPU.Fastendo.AddressingModes.indirectIndexed:
                this._lowByte = this.getByte(this.curinst_Parameters0);
                this._highByte = this.getByte((this.curinst_Parameters0 + 1) & 255) << 8;
                addr = (this._lowByte | this._highByte);
                result = addr + this._indexRegisterY;
                if ((result & 255) > this._indexRegisterY) {
                    this.curinst_ExtraTiming = 1;
                }
                break;
            case NES.CPU.Fastendo.AddressingModes.relative:
                result = (this._programCounter + this.curinst_Parameters0);
                break;
            default:
                throw new Error('Executors.DecodeAddress() recieved an invalid addressmode');
        }
        return result;
    },
    
    _decodeOperand: function NES_CPU_Fastendo_CPU2A03$_decodeOperand() {
        /// <returns type="Number" integer="true"></returns>
        switch (this.curinst_AddressingMode) {
            case NES.CPU.Fastendo.AddressingModes.immediate:
                this.set_dataBus(this.curinst_Parameters0);
                return this.curinst_Parameters0;
            case NES.CPU.Fastendo.AddressingModes.accumulator:
                return this._accumulator;
            default:
                this.set_dataBus(this.getByte(this._decodeAddress()));
                return this.get_dataBus();
        }
    },
    
    _storeOperand: function NES_CPU_Fastendo_CPU2A03$_storeOperand(address) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
    },
    
    execute: function NES_CPU_Fastendo_CPU2A03$execute() {
        switch (this.curinst_OpCode) {
            case 105:
            case 101:
            case 117:
            case 109:
            case 125:
            case 121:
            case 97:
            case 113:
                this.ADC();
                break;
            case 41:
            case 37:
            case 53:
            case 45:
            case 61:
            case 57:
            case 33:
            case 49:
                this.AND();
                break;
            case 10:
            case 6:
            case 22:
            case 14:
            case 30:
                this.ASL();
                break;
            case 144:
                this.BCC();
                break;
            case 176:
                this.BCS();
                break;
            case 240:
                this.BEQ();
                break;
            case 36:
            case 44:
                this.BIT();
                break;
            case 48:
                this.BMI();
                break;
            case 208:
                this.BNE();
                break;
            case 16:
                this.BPL();
                break;
            case 0:
                this.BRK();
                break;
            case 80:
                this.BVC();
                break;
            case 112:
                this.BVS();
                break;
            case 24:
                this.CLC();
                break;
            case 216:
                this.CLD();
                break;
            case 88:
                this.CLI();
                break;
            case 184:
                this.CLV();
                break;
            case 201:
            case 197:
            case 213:
            case 205:
            case 221:
            case 217:
            case 193:
            case 209:
                this.CMP();
                break;
            case 224:
            case 228:
            case 236:
                this.CPX();
                break;
            case 192:
            case 196:
            case 204:
                this.CPY();
                break;
            case 198:
            case 214:
            case 206:
            case 222:
                this.DEC();
                break;
            case 202:
                this.DEX();
                break;
            case 136:
                this.DEY();
                break;
            case 73:
            case 69:
            case 85:
            case 77:
            case 93:
            case 89:
            case 65:
            case 81:
                this.EOR();
                break;
            case 230:
            case 246:
            case 238:
            case 254:
                this.INC();
                break;
            case 232:
                this.INX();
                break;
            case 200:
                this.INY();
                break;
            case 76:
            case 108:
                this.JMP();
                break;
            case 32:
                this.JSR();
                break;
            case 169:
            case 165:
            case 181:
            case 173:
            case 189:
            case 185:
            case 161:
            case 177:
                this.LDA();
                break;
            case 162:
            case 166:
            case 182:
            case 174:
            case 190:
                this.LDX();
                break;
            case 160:
            case 164:
            case 180:
            case 172:
            case 188:
                this.LDY();
                break;
            case 74:
            case 70:
            case 86:
            case 78:
            case 94:
                this.LSR();
                break;
            case 234:
            case 26:
            case 58:
            case 90:
            case 122:
            case 218:
            case 250:
            case 4:
            case 20:
            case 52:
            case 68:
            case 100:
            case 128:
            case 130:
            case 137:
            case 194:
            case 212:
            case 226:
            case 244:
            case 12:
            case 28:
            case 60:
            case 92:
            case 124:
            case 220:
            case 252:
                this.NOP();
                break;
            case 9:
            case 5:
            case 21:
            case 13:
            case 29:
            case 25:
            case 1:
            case 17:
                this.ORA();
                break;
            case 72:
                this.PHA();
                break;
            case 8:
                this.PHP();
                break;
            case 104:
                this.PLA();
                break;
            case 40:
                this.PLP();
                break;
            case 42:
            case 38:
            case 54:
            case 46:
            case 62:
                this.ROL();
                break;
            case 106:
            case 102:
            case 118:
            case 110:
            case 126:
                this.ROR();
                break;
            case 64:
                this.RTI();
                break;
            case 96:
                this.RTS();
                break;
            case 233:
            case 229:
            case 245:
            case 237:
            case 253:
            case 249:
            case 225:
            case 241:
                this.SBC();
                break;
            case 56:
                this.SEC();
                break;
            case 248:
                this.SED();
                break;
            case 120:
                this.SEI();
                break;
            case 133:
            case 149:
            case 141:
            case 157:
            case 153:
            case 129:
            case 145:
                this.STA();
                break;
            case 134:
            case 150:
            case 142:
                this.STX();
                break;
            case 132:
            case 148:
            case 140:
                this.STY();
                break;
            case 170:
                this.TAX();
                break;
            case 168:
                this.TAY();
                break;
            case 186:
                this.TSX();
                break;
            case 138:
                this.TXA();
                break;
            case 154:
                this.TXS();
                break;
            case 152:
                this.TYA();
                break;
        }
    },
    
    _setZNFlags: function NES_CPU_Fastendo_CPU2A03$_setZNFlags(data) {
        /// <param name="data" type="Number" integer="true">
        /// </param>
        if ((data & 255) === 0) {
            this._statusRegister |= NES.CPU.Fastendo.CPUStatusMasks.zeroResultMask;
        }
        else {
            this._statusRegister &= ~(NES.CPU.Fastendo.CPUStatusMasks.zeroResultMask);
        }
        if ((data & 128) === 128) {
            this._statusRegister |= NES.CPU.Fastendo.CPUStatusMasks.negativeResultMask;
        }
        else {
            this._statusRegister &= ~(NES.CPU.Fastendo.CPUStatusMasks.negativeResultMask);
        }
    },
    
    LDA: function NES_CPU_Fastendo_CPU2A03$LDA() {
        this._accumulator = this._decodeOperand();
        this._setZNFlags(this._accumulator);
    },
    
    LDX: function NES_CPU_Fastendo_CPU2A03$LDX() {
        this._indexRegisterX = this._decodeOperand();
        this._setZNFlags(this._indexRegisterX);
    },
    
    LDY: function NES_CPU_Fastendo_CPU2A03$LDY() {
        this._indexRegisterY = this._decodeOperand();
        this._setZNFlags(this._indexRegisterY);
    },
    
    STA: function NES_CPU_Fastendo_CPU2A03$STA() {
        this.setByte(this._decodeAddress(), this._accumulator);
    },
    
    STX: function NES_CPU_Fastendo_CPU2A03$STX() {
        this.setByte(this._decodeAddress(), this._indexRegisterX);
    },
    
    STY: function NES_CPU_Fastendo_CPU2A03$STY() {
        this.setByte(this._decodeAddress(), this._indexRegisterY);
    },
    
    SED: function NES_CPU_Fastendo_CPU2A03$SED() {
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.decimalModeMask, true);
    },
    
    CLD: function NES_CPU_Fastendo_CPU2A03$CLD() {
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.decimalModeMask, false);
    },
    
    JMP: function NES_CPU_Fastendo_CPU2A03$JMP() {
        if (this.curinst_AddressingMode === NES.CPU.Fastendo.AddressingModes.indirect && this.curinst_Parameters0 === 255) {
            this._programCounter = 255 | this.curinst_Parameters1 << 8;
        }
        else {
            this._programCounter = this._decodeAddress();
        }
    },
    
    DEC: function NES_CPU_Fastendo_CPU2A03$DEC() {
        var val = this._decodeOperand();
        val--;
        this.setByte(this._decodeAddress(), val);
        this._setZNFlags(val);
    },
    
    INC: function NES_CPU_Fastendo_CPU2A03$INC() {
        var val = this._decodeOperand();
        val++;
        this.setByte(this._decodeAddress(), val);
        this._setZNFlags(val);
    },
    
    ADC: function NES_CPU_Fastendo_CPU2A03$ADC() {
        var data = this._decodeOperand();
        var carryFlag = (this._statusRegister & 1);
        var result = (this._accumulator + data + carryFlag);
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, result > 255);
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((this._accumulator ^ data) & 128) !== 128 && ((this._accumulator ^ result) & 128) === 128);
        this._accumulator = (result & 255);
        this._setZNFlags(this._accumulator);
    },
    
    LSR: function NES_CPU_Fastendo_CPU2A03$LSR() {
        var rst = this._decodeOperand();
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, (rst & 1) === 1);
        rst = rst >> 1 & 255;
        this._setZNFlags(rst);
        if (this.curinst_AddressingMode === NES.CPU.Fastendo.AddressingModes.accumulator) {
            this._accumulator = rst;
        }
        else {
            this.setByte(this._decodeAddress(), rst);
        }
    },
    
    SBC: function NES_CPU_Fastendo_CPU2A03$SBC() {
        var data = this._decodeOperand();
        var carryFlag = ((this._statusRegister ^ 1) & 1);
        var result = (this._accumulator - data - carryFlag);
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, ((this._accumulator ^ result) & 128) === 128 && ((this._accumulator ^ data) & 128) === 128);
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, (result < 256));
        this._accumulator = (result & 255);
        this._setZNFlags(this._accumulator);
    },
    
    AND: function NES_CPU_Fastendo_CPU2A03$AND() {
        this._accumulator = (this._accumulator & this._decodeOperand());
        this._setZNFlags(this._accumulator);
    },
    
    ORA: function NES_CPU_Fastendo_CPU2A03$ORA() {
        this._accumulator = (this._accumulator | this._decodeOperand());
        this._setZNFlags(this._accumulator);
    },
    
    EOR: function NES_CPU_Fastendo_CPU2A03$EOR() {
        this._accumulator = (this._accumulator ^ this._decodeOperand());
        this._setZNFlags(this.get_accumulator());
    },
    
    ASL: function NES_CPU_Fastendo_CPU2A03$ASL() {
        var data = this._decodeOperand();
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, ((data & 128) === 128));
        data = (data << 1) & 254;
        if (this.curinst_AddressingMode === NES.CPU.Fastendo.AddressingModes.accumulator) {
            this._accumulator = data;
        }
        else {
            this.setByte(this._decodeAddress(), data);
        }
        this._setZNFlags(data);
    },
    
    BIT: function NES_CPU_Fastendo_CPU2A03$BIT() {
        var operand = this._decodeOperand();
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, (operand & 64) === 64);
        if ((operand & 128) === 128) {
            this._statusRegister = this._statusRegister | 128;
        }
        else {
            this._statusRegister = this._statusRegister & 127;
        }
        if ((operand & this.get_accumulator()) === 0) {
            this._statusRegister = this._statusRegister | 2;
        }
        else {
            this._statusRegister = this._statusRegister & 253;
        }
    },
    
    SEC: function NES_CPU_Fastendo_CPU2A03$SEC() {
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, true);
    },
    
    CLC: function NES_CPU_Fastendo_CPU2A03$CLC() {
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, false);
    },
    
    SEI: function NES_CPU_Fastendo_CPU2A03$SEI() {
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.interruptDisableMask, true);
    },
    
    CLI: function NES_CPU_Fastendo_CPU2A03$CLI() {
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.interruptDisableMask, false);
    },
    
    CLV: function NES_CPU_Fastendo_CPU2A03$CLV() {
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.overflowMask, false);
    },
    
    _compare: function NES_CPU_Fastendo_CPU2A03$_compare(data) {
        /// <param name="data" type="Number" integer="true">
        /// </param>
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, data > 255);
        this._setZNFlags(data & 255);
    },
    
    CMP: function NES_CPU_Fastendo_CPU2A03$CMP() {
        var data = (this.get_accumulator() + 256 - this._decodeOperand());
        this._compare(data);
    },
    
    CPX: function NES_CPU_Fastendo_CPU2A03$CPX() {
        var data = (this._indexRegisterX + 256 - this._decodeOperand());
        this._compare(data);
    },
    
    CPY: function NES_CPU_Fastendo_CPU2A03$CPY() {
        var data = (this._indexRegisterY + 256 - this._decodeOperand());
        this._compare(data);
    },
    
    NOP: function NES_CPU_Fastendo_CPU2A03$NOP() {
        if (this.curinst_AddressingMode === NES.CPU.Fastendo.AddressingModes.absoluteX) {
            this._decodeAddress();
        }
    },
    
    _branch: function NES_CPU_Fastendo_CPU2A03$_branch() {
        this.curinst_ExtraTiming = 1;
        var addr = this.curinst_Parameters0 & 255;
        if ((addr & 128) === 128) {
            addr = addr - 256;
            this._programCounter += addr;
        }
        else {
            this._programCounter += addr;
        }
        if ((this._programCounter & 255) < addr) {
            this.curinst_ExtraTiming = 2;
        }
    },
    
    BCC: function NES_CPU_Fastendo_CPU2A03$BCC() {
        if ((this._statusRegister & 1) !== 1) {
            this._branch();
        }
    },
    
    BCS: function NES_CPU_Fastendo_CPU2A03$BCS() {
        if ((this._statusRegister & 1) === 1) {
            this._branch();
        }
    },
    
    BPL: function NES_CPU_Fastendo_CPU2A03$BPL() {
        if ((this._statusRegister & 128) !== 128) {
            this._branch();
        }
    },
    
    BMI: function NES_CPU_Fastendo_CPU2A03$BMI() {
        if ((this._statusRegister & 128) === 128) {
            this._branch();
        }
    },
    
    BVC: function NES_CPU_Fastendo_CPU2A03$BVC() {
        if ((this._statusRegister & 64) !== 64) {
            this._branch();
        }
    },
    
    BVS: function NES_CPU_Fastendo_CPU2A03$BVS() {
        if ((this._statusRegister & 64) === 64) {
            this._branch();
        }
    },
    
    BNE: function NES_CPU_Fastendo_CPU2A03$BNE() {
        if ((this._statusRegister & 2) !== 2) {
            this._branch();
        }
    },
    
    BEQ: function NES_CPU_Fastendo_CPU2A03$BEQ() {
        if ((this._statusRegister & 2) === 2) {
            this._branch();
        }
    },
    
    DEX: function NES_CPU_Fastendo_CPU2A03$DEX() {
        this._indexRegisterX = this._indexRegisterX - 1;
        this._indexRegisterX = this._indexRegisterX & 255;
        this._setZNFlags(this._indexRegisterX);
    },
    
    DEY: function NES_CPU_Fastendo_CPU2A03$DEY() {
        this._indexRegisterY = this._indexRegisterY - 1;
        this._indexRegisterY = this._indexRegisterY & 255;
        this._setZNFlags(this._indexRegisterY);
    },
    
    INX: function NES_CPU_Fastendo_CPU2A03$INX() {
        this._indexRegisterX = this._indexRegisterX + 1;
        this._indexRegisterX = this._indexRegisterX & 255;
        this._setZNFlags(this._indexRegisterX);
    },
    
    INY: function NES_CPU_Fastendo_CPU2A03$INY() {
        this._indexRegisterY = this._indexRegisterY + 1;
        this._indexRegisterY = this._indexRegisterY & 255;
        this._setZNFlags(this._indexRegisterY);
    },
    
    TAX: function NES_CPU_Fastendo_CPU2A03$TAX() {
        this._indexRegisterX = this._accumulator;
        this._setZNFlags(this._indexRegisterX);
    },
    
    TXA: function NES_CPU_Fastendo_CPU2A03$TXA() {
        this._accumulator = this._indexRegisterX;
        this._setZNFlags(this._accumulator);
    },
    
    TAY: function NES_CPU_Fastendo_CPU2A03$TAY() {
        this._indexRegisterY = this._accumulator;
        this._setZNFlags(this._indexRegisterY);
    },
    
    TYA: function NES_CPU_Fastendo_CPU2A03$TYA() {
        this._accumulator = this._indexRegisterY;
        this._setZNFlags(this._accumulator);
    },
    
    TXS: function NES_CPU_Fastendo_CPU2A03$TXS() {
        this._stackPointer = this._indexRegisterX;
    },
    
    TSX: function NES_CPU_Fastendo_CPU2A03$TSX() {
        this._indexRegisterX = this._stackPointer;
        this._setZNFlags(this._indexRegisterX);
    },
    
    PHA: function NES_CPU_Fastendo_CPU2A03$PHA() {
        this.pushStack(this._accumulator);
    },
    
    PLA: function NES_CPU_Fastendo_CPU2A03$PLA() {
        this._accumulator = this.popStack();
        this._setZNFlags(this._accumulator);
    },
    
    PHP: function NES_CPU_Fastendo_CPU2A03$PHP() {
        var newStatus = this._statusRegister | 16 | 32;
        this.pushStack(newStatus);
    },
    
    PLP: function NES_CPU_Fastendo_CPU2A03$PLP() {
        this._statusRegister = this.popStack();
    },
    
    JSR: function NES_CPU_Fastendo_CPU2A03$JSR() {
        this.pushStack((this._programCounter >> 8) & 255);
        this.pushStack((this._programCounter - 1) & 255);
        this._programCounter = this._decodeAddress();
    },
    
    ROR: function NES_CPU_Fastendo_CPU2A03$ROR() {
        var data = this._decodeOperand();
        var oldbit = 0;
        if (this.getFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask)) {
            oldbit = 128;
        }
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, (data & 1) === 1);
        data = (data >> 1) | oldbit;
        this._setZNFlags(data);
        if (this.curinst_AddressingMode === NES.CPU.Fastendo.AddressingModes.accumulator) {
            this._accumulator = data;
        }
        else {
            this.setByte(this._decodeAddress(), data);
        }
    },
    
    ROL: function NES_CPU_Fastendo_CPU2A03$ROL() {
        var data = this._decodeOperand();
        var oldbit = 0;
        if (this.getFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask)) {
            oldbit = 1;
        }
        this.setFlag(NES.CPU.Fastendo.CPUStatusMasks.carryMask, (data & 128) === 128);
        data = data << 1;
        data = data & 255;
        data = data | oldbit;
        this._setZNFlags(data);
        if (this.curinst_AddressingMode === NES.CPU.Fastendo.AddressingModes.accumulator) {
            this._accumulator = data;
        }
        else {
            this.setByte(this._decodeAddress(), data);
        }
    },
    
    RTS: function NES_CPU_Fastendo_CPU2A03$RTS() {
        var high, low;
        low = (this.popStack() + 1) & 255;
        high = this.popStack();
        this._programCounter = ((high << 8) | low);
    },
    
    RTI: function NES_CPU_Fastendo_CPU2A03$RTI() {
        this._statusRegister = this.popStack();
        var low = this.popStack();
        var high = this.popStack();
        this._programCounter = ((256 * high) + low);
    },
    
    BRK: function NES_CPU_Fastendo_CPU2A03$BRK() {
        this._programCounter = this._programCounter + 1;
        this.pushStack(this._programCounter >> 8 & 255);
        this.pushStack(this._programCounter & 255);
        var newStatus = this._statusRegister | 16 | 32;
        this.pushStack(newStatus);
        this._statusRegister = this._statusRegister | 20;
        this.set_addressBus(65534);
        var lowByte = this.getCurrentByte();
        this.set_addressBus(65535);
        var highByte = this.getCurrentByte();
        this._programCounter = lowByte + highByte * 256;
    },
    
    _cart: null,
    _pixelWhizzler: null,
    
    get_pixelWhizzler: function NES_CPU_Fastendo_CPU2A03$get_pixelWhizzler() {
        /// <value type="NES.CPU.PixelWhizzlerClasses.IPPU"></value>
        return this._pixelWhizzler;
    },
    set_pixelWhizzler: function NES_CPU_Fastendo_CPU2A03$set_pixelWhizzler(value) {
        /// <value type="NES.CPU.PixelWhizzlerClasses.IPPU"></value>
        this._pixelWhizzler = value;
        set_nmiHandler(this._nmiHandler);
        return value;
    },
    
    _soundBopper: null,
    
    get_soundBopper: function NES_CPU_Fastendo_CPU2A03$get_soundBopper() {
        /// <value type="NES.CPU.Fastendo.IClockedMemoryMappedIOElement"></value>
        return this._soundBopper;
    },
    set_soundBopper: function NES_CPU_Fastendo_CPU2A03$set_soundBopper(value) {
        /// <value type="NES.CPU.Fastendo.IClockedMemoryMappedIOElement"></value>
        this._soundBopper = value;
        this._soundBopper.set_nmiHandler(this._irqUpdater);
        return value;
    },
    
    _nmiHandler: null,
    
    _nmiHandler: function NES_CPU_Fastendo_CPU2A03$_nmiHandler() {
        this._handleNMI = true;
    },
    
    _irqUpdater: null,
    
    _irqUpdater: function NES_CPU_Fastendo_CPU2A03$_irqUpdater() {
        this._handleIRQ = (this._soundBopper.get_irqAsserted() | this._cart.get_irqAsserted()) === 1;
    },
    
    get_cart: function NES_CPU_Fastendo_CPU2A03$get_cart() {
        /// <value type="NES.CPU.Machine.Carts.INESCart"></value>
        return this._cart;
    },
    set_cart: function NES_CPU_Fastendo_CPU2A03$set_cart(value) {
        /// <value type="NES.CPU.Machine.Carts.INESCart"></value>
        this._cart = value;
        this._cart.set_nmiHandler(this._irqUpdater);
        return value;
    },
    
    _stackPointer: 255,
    
    get_stackPointer: function NES_CPU_Fastendo_CPU2A03$get_stackPointer() {
        /// <value type="Number" integer="true"></value>
        return this._stackPointer;
    },
    
    pushStack: function NES_CPU_Fastendo_CPU2A03$pushStack(data) {
        /// <param name="data" type="Number" integer="true">
        /// </param>
        this._rams[this._stackPointer + 256] = data;
        this._stackPointer--;
        if (this._stackPointer < 0) {
            this._stackPointer = 255;
        }
    },
    
    popStack: function NES_CPU_Fastendo_CPU2A03$popStack() {
        /// <returns type="Number" integer="true"></returns>
        this._stackPointer++;
        if (this._stackPointer > 255) {
            this._stackPointer = 0;
        }
        return this._rams[this._stackPointer + 256] & 255;
    },
    
    getCurrentByte: function NES_CPU_Fastendo_CPU2A03$getCurrentByte() {
        /// <returns type="Number" integer="true"></returns>
        this.set_dataBus(this.getByte(this.get_addressBus()));
        return this.get_dataBus();
    },
    
    getByte: function NES_CPU_Fastendo_CPU2A03$getByte(address) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var result = 0;
        switch (address & 61440) {
            case 0:
            case 4096:
                if (address < 2048) {
                    result = this._rams[address];
                }
                else {
                    result = address >> 8;
                }
                break;
            case 8192:
            case 12288:
                result = getByte(this._clock, address);
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
                result = this._cart.getByte(this._clock, address);
                break;
            default:
                throw new Error('Bullshit!');
        }
        return result & 255;
    },
    
    setCurrentByte: function NES_CPU_Fastendo_CPU2A03$setCurrentByte() {
        this.setByte(this.get_addressBus(), this.get_dataBus() & 255);
    },
    
    setByte: function NES_CPU_Fastendo_CPU2A03$setByte(address, data) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <param name="data" type="Number" integer="true">
        /// </param>
        if (address < 2048) {
            this._rams[address & 2047] = data;
            return;
        }
        switch (address & 61440) {
            case 0:
            case 4096:
                this._rams[address & 2047] = data;
                break;
            case 20480:
                this.get_cart().setByte(this._clock, address, data);
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
                this.get_cart().setByte(this._clock, address, data);
                break;
            case 8192:
            case 12288:
                setPPUByte(this._clock, address, data);
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
                        copySprites(this._rams, data * 256);
                        this.curinst_ExtraTiming = this.curinst_ExtraTiming + 512;
                        break;
                    case 16406:
                        break;
                }
                break;
        }
    },
    
    findNextEvent: function NES_CPU_Fastendo_CPU2A03$findNextEvent() {
        this._nextEvent = this._clock + get_nextEventAt();
    },
    
    _handleNextEvent: function NES_CPU_Fastendo_CPU2A03$_handleNextEvent() {
        handleEvent(this.get_clock());
        this.findNextEvent();
    },
    
    _debugging: false,
    
    get_debugging: function NES_CPU_Fastendo_CPU2A03$get_debugging() {
        /// <value type="Boolean"></value>
        return this._debugging;
    },
    set_debugging: function NES_CPU_Fastendo_CPU2A03$set_debugging(value) {
        /// <value type="Boolean"></value>
        this._debugging = value;
        return value;
    },
    
    get_instructionUsage: function NES_CPU_Fastendo_CPU2A03$get_instructionUsage() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return this._instructionUsage;
    },
    
    _instructionHistoryPointer: 255,
    
    get_instructionHistoryPointer: function NES_CPU_Fastendo_CPU2A03$get_instructionHistoryPointer() {
        /// <value type="Number" integer="true"></value>
        return this._instructionHistoryPointer;
    },
    
    get_instructionHistory: function NES_CPU_Fastendo_CPU2A03$get_instructionHistory() {
        /// <value type="Array" elementType="Instruction"></value>
        return this._instructionHistory;
    },
    
    writeInstructionHistoryAndUsage: function NES_CPU_Fastendo_CPU2A03$writeInstructionHistoryAndUsage() {
        this._instructionUsage[this.curinst_OpCode]++;
    },
    
    peekInstruction: function NES_CPU_Fastendo_CPU2A03$peekInstruction(address) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="NES.CPU.Fastendo.Instruction"></returns>
        var inst = new NES.CPU.Fastendo.Instruction();
        inst.opCode = this.getByte(address++);
        inst.addressingMode = this.addressmode[inst.opCode];
        inst.length = 1;
        return inst;
    }
}


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.smallInstruction

NES.CPU.Fastendo.smallInstruction = function NES_CPU_Fastendo_smallInstruction() {
}
NES.CPU.Fastendo.smallInstruction.unpackInstruction = function NES_CPU_Fastendo_smallInstruction$unpackInstruction(instruction) {
    /// <param name="instruction" type="Number" integer="true">
    /// </param>
    /// <returns type="NES.CPU.Fastendo.Instruction"></returns>
    var inst = new NES.CPU.Fastendo.Instruction();
    inst.opCode = instruction & 255;
    inst.parameters0 = (instruction >>> 8) & 255;
    inst.parameters1 = (instruction >>> 16) & 255;
    return inst;
}


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.Instruction

NES.CPU.Fastendo.Instruction = function NES_CPU_Fastendo_Instruction() {
    /// <field name="addressingMode" type="NES.CPU.Fastendo.AddressingModes">
    /// </field>
    /// <field name="address" type="Number" integer="true">
    /// </field>
    /// <field name="opCode" type="Number" integer="true">
    /// </field>
    /// <field name="parameters0" type="Number" integer="true">
    /// </field>
    /// <field name="parameters1" type="Number" integer="true">
    /// </field>
    /// <field name="extraTiming" type="Number" integer="true">
    /// </field>
    /// <field name="length" type="Number" integer="true">
    /// </field>
}
NES.CPU.Fastendo.Instruction.prototype = {
    addressingMode: 0,
    address: 0,
    opCode: 0,
    parameters0: 0,
    parameters1: 0,
    extraTiming: 0,
    length: 0
}


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.Fastendo.CPUStatus

NES.CPU.Fastendo.CPUStatus = function NES_CPU_Fastendo_CPUStatus() {
    /// <field name="statusRegister" type="Number" integer="true">
    /// </field>
    /// <field name="programCounter" type="Number" integer="true">
    /// </field>
    /// <field name="accumulator" type="Number" integer="true">
    /// </field>
    /// <field name="indexRegisterX" type="Number" integer="true">
    /// </field>
    /// <field name="indexRegisterY" type="Number" integer="true">
    /// </field>
}
NES.CPU.Fastendo.CPUStatus.prototype = {
    statusRegister: 0,
    programCounter: 0,
    accumulator: 0,
    indexRegisterX: 0,
    indexRegisterY: 0
}


Type.registerNamespace('NES.CPU.nitenedo');

////////////////////////////////////////////////////////////////////////////////
// NES.CPU.nitenedo.NESMachine

NES.CPU.nitenedo.NESMachine = function NES_CPU_nitenedo_NESMachine(ppu, cart) {
    /// <param name="ppu" type="NES.CPU.PixelWhizzlerClasses.IPPU">
    /// </param>
    /// <param name="cart" type="NES.CPU.Machine.Carts.INESCart">
    /// </param>
    /// <field name="_ppu" type="NES.CPU.PixelWhizzlerClasses.IPPU">
    /// </field>
    /// <field name="_cart" type="NES.CPU.Machine.Carts.INESCart">
    /// </field>
    /// <field name="__soundStatusChanged" type="EventHandler">
    /// </field>
    /// <field name="_enableSound" type="Boolean">
    /// </field>
    /// <field name="_breakpointHit" type="Boolean">
    /// </field>
    /// <field name="_doDraw" type="Boolean">
    /// </field>
    /// <field name="_frameOn" type="Boolean">
    /// </field>
    /// <field name="_frameCount" type="Number" integer="true">
    /// </field>
    /// <field name="__drawscreen" type="EventHandler">
    /// </field>
    this._cart = cart;
    cpuCart = this._cart;
    set_chrRomHandler(this._cart);
    this.initialize();
}

NES.CPU.nitenedo.NESMachine.prototype = {
    _ppu: null,
    _cart: null,
    
    get_cart: function NES_CPU_nitenedo_NESMachine$get_cart() {
        /// <value type="NES.CPU.Machine.Carts.INESCart"></value>
        return this._cart;
    },
    
    add_soundStatusChanged: function NES_CPU_nitenedo_NESMachine$add_soundStatusChanged(value) {
        /// <param name="value" type="Function" />
        this.__soundStatusChanged = ss.Delegate.combine(this.__soundStatusChanged, value);
    },
    remove_soundStatusChanged: function NES_CPU_nitenedo_NESMachine$remove_soundStatusChanged(value) {
        /// <param name="value" type="Function" />
        this.__soundStatusChanged = ss.Delegate.remove(this.__soundStatusChanged, value);
    },
    
    __soundStatusChanged: null,
    _enableSound: true,
    
    get_enableSound: function NES_CPU_nitenedo_NESMachine$get_enableSound() {
        /// <value type="Boolean"></value>
        return this._enableSound;
    },
    set_enableSound: function NES_CPU_nitenedo_NESMachine$set_enableSound(value) {
        /// <value type="Boolean"></value>
        if (this._enableSound !== value) {
            if (this.__soundStatusChanged != null) {
                this.__soundStatusChanged.invoke(this, ss.EventArgs.Empty);
            }
            this._enableSound = value;
        }
        return value;
    },
    
    setupSound: function NES_CPU_nitenedo_NESMachine$setupSound() {
    },
    
    _breakpointHit: false,
    _doDraw: false,
    
    _frameFinished: function NES_CPU_nitenedo_NESMachine$_frameFinished() {
        this._frameOn = false;
    },
    
    _frameOn: false,
    
    initialize: function NES_CPU_nitenedo_NESMachine$initialize() {
        this._frameCount = 0;
        initializePPU();
        this._cart.initializeCart();
        resetCPU();
        powerOn();
    },
    
    reset: function NES_CPU_nitenedo_NESMachine$reset() {
        if (this._cart != null) {
            initializePPU();
            this._cart.initializeCart();
            resetCPU();
            powerOn();
        }
    },
    
    _frameCount: 0,
    
    get_frameCount: function NES_CPU_nitenedo_NESMachine$get_frameCount() {
        /// <value type="Number" integer="true"></value>
        return this._frameCount;
    },
    set_frameCount: function NES_CPU_nitenedo_NESMachine$set_frameCount(value) {
        /// <value type="Number" integer="true"></value>
        this._frameCount = value;
        return value;
    },
    
    add_drawscreen: function NES_CPU_nitenedo_NESMachine$add_drawscreen(value) {
        /// <param name="value" type="Function" />
        this.__drawscreen = ss.Delegate.combine(this.__drawscreen, value);
    },
    remove_drawscreen: function NES_CPU_nitenedo_NESMachine$remove_drawscreen(value) {
        /// <param name="value" type="Function" />
        this.__drawscreen = ss.Delegate.remove(this.__drawscreen, value);
    },
    
    __drawscreen: null,
    
    get_isRunning: function NES_CPU_nitenedo_NESMachine$get_isRunning() {
        /// <value type="Boolean"></value>
        return true;
    }
    

}




Type.registerNamespace('bulbascript');

////////////////////////////////////////////////////////////////////////////////
// bulbascript.bulbascriptApp

bulbascript.bulbascriptApp = function bulbascript_bulbascriptApp() {
    /// <field name="nes" type="NES.CPU.nitenedo.NESMachine" static="true">
    /// </field>
}
bulbascript.bulbascriptApp.main = function bulbascript_bulbascriptApp$main(args) {
    /// <param name="args" type="Object">
    /// </param>
}
bulbascript.bulbascriptApp._nes_Drawscreen = function bulbascript_bulbascriptApp$_nes_Drawscreen(sender, e) {
    /// <param name="sender" type="Object">
    /// </param>
    /// <param name="e" type="ss.EventArgs">
    /// </param>
    drawFrame();
}
bulbascript.bulbascriptApp.loadRom = function bulbascript_bulbascriptApp$loadRom(data) {
    /// <param name="data" type="Array" elementType="Number" elementInteger="true">
    /// </param>
    var whizzler = null;
    var b = new NES.CPU.Machine.ROMLoader.ByteArrayHolder();
    b.set_data(data);
    var cart = NES.CPU.Machine.ROMLoader.iNESFileHandler.loadROM(whizzler, b);
    bulbascript.bulbascriptApp.nes = new NES.CPU.nitenedo.NESMachine(whizzler, cart);
    bulbascript.bulbascriptApp.nes.add_drawscreen(bulbascript.bulbascriptApp._nes_Drawscreen);
}
bulbascript.bulbascriptApp.getVidData = function bulbascript_bulbascriptApp$getVidData() {
    /// <returns type="Array" elementType="Number" elementInteger="true"></returns>
    return bulbascript.bulbascriptApp.nes.get_PPU().get_videoBuffer();
}
bulbascript.bulbascriptApp.getFrameChecksum = function bulbascript_bulbascriptApp$getFrameChecksum() {
    /// <returns type="Number" integer="true"></returns>
    return bulbascript.bulbascriptApp.nes.get_PPU().getFrameChecksum();
}


NES.CPU.Machine.Carts.BaseCart.registerClass('NES.CPU.Machine.Carts.BaseCart', null, NES.CPU.Machine.Carts.INESCart);
NES.CPU.Machine.ROMLoader.ByteArrayHolder.registerClass('NES.CPU.Machine.ROMLoader.ByteArrayHolder');
NES.CPU.Machine.ROMLoader.iNESFileHandler.registerClass('NES.CPU.Machine.ROMLoader.iNESFileHandler');
NES.CPU.NESCart.registerClass('NES.CPU.NESCart', NES.CPU.Machine.Carts.BaseCart);
NES.CPU.Fastendo.CPU2A03.registerClass('NES.CPU.Fastendo.CPU2A03');
NES.CPU.Fastendo.smallInstruction.registerClass('NES.CPU.Fastendo.smallInstruction');
NES.CPU.Fastendo.Instruction.registerClass('NES.CPU.Fastendo.Instruction');
NES.CPU.Fastendo.CPUStatus.registerClass('NES.CPU.Fastendo.CPUStatus');
NES.CPU.nitenedo.NESMachine.registerClass('NES.CPU.nitenedo.NESMachine');
NES.CPU.PPUClasses.NESSprite.registerClass('NES.CPU.PPUClasses.NESSprite');
NES.CPU.PPUClasses.PPUWriteEvent.registerClass('NES.CPU.PPUClasses.PPUWriteEvent');
bulbascript.bulbascriptApp.registerClass('bulbascript.bulbascriptApp');
NES.CPU.Fastendo.CPU2A03._cpuTiming = [ 7, 6, 0, 0, 3, 2, 5, 0, 3, 2, 2, 0, 6, 4, 6, 0, 2, 5, 0, 0, 3, 3, 6, 0, 2, 4, 2, 0, 6, 4, 7, 0, 6, 6, 0, 0, 3, 2, 5, 0, 3, 2, 2, 0, 4, 4, 6, 0, 2, 5, 0, 0, 3, 3, 6, 0, 2, 4, 2, 0, 6, 4, 7, 0, 6, 6, 0, 0, 3, 2, 5, 0, 3, 2, 2, 0, 3, 4, 6, 0, 2, 5, 0, 0, 0, 3, 6, 0, 2, 4, 2, 0, 6, 4, 6, 0, 6, 6, 0, 0, 3, 3, 5, 0, 3, 2, 2, 0, 5, 4, 6, 0, 2, 5, 0, 0, 0, 4, 6, 0, 2, 4, 2, 0, 6, 4, 7, 0, 3, 6, 3, 0, 3, 3, 3, 0, 2, 3, 2, 0, 4, 4, 4, 0, 2, 6, 0, 0, 4, 4, 4, 0, 2, 5, 2, 0, 0, 5, 0, 0, 2, 6, 2, 0, 3, 3, 3, 0, 2, 2, 2, 0, 4, 4, 4, 0, 2, 5, 0, 0, 4, 4, 4, 0, 2, 4, 2, 0, 4, 4, 4, 0, 2, 6, 3, 0, 3, 2, 5, 0, 2, 2, 2, 0, 4, 4, 6, 0, 2, 5, 0, 0, 3, 4, 6, 0, 2, 4, 2, 0, 6, 4, 7, 0, 2, 6, 3, 0, 3, 3, 5, 0, 2, 2, 2, 0, 4, 4, 6, 0, 2, 5, 0, 0, 3, 4, 6, 0, 2, 4, 2, 0, 6, 4, 7, 0 ];

bulbascript.bulbascriptApp.nes = null;

// ---- Do not remove this footer ----
// This script was generated using Script# v0.6.0.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------

}
ss.loader.registerScript('bulbascript', [], executeScript);
})();
