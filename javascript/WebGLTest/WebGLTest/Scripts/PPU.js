Type.registerNamespace('NES.CPU.PixelWhizzlerClasses');

////////////////////////////////////////////////////////////////////////////////
// NES.CPU.PixelWhizzlerClasses.IPPU

NES.CPU.PixelWhizzlerClasses.IPPU = function() { 
};
NES.CPU.PixelWhizzlerClasses.IPPU.prototype = {
    updatePixelInfo : null,
    get_backgroundVisible : null,
    checkVBlank : null,
    get_chrRomHandler : null,
    set_chrRomHandler : null,
    clearVINT : null,
    copySprites : null,
    get_currentFrame : null,
    get_currentXPosition : null,
    get_currentYPosition : null,
    drawTo : null,
    get_fillRGB : null,
    set_fillRGB : null,
    get_frameFinishHandler : null,
    set_frameFinishHandler : null,
    get_frameOn : null,
    set_frameOn : null,
    get_frames : null,
    getByte : null,
    handleEvent : null,
    get_handleVBlankIRQ : null,
    set_handleVBlankIRQ : null,
    get_hScroll : null,
    initialize : null,
    get_irqAsserted : null,
    set_irqAsserted : null,
    get_isDebugging : null,
    set_isDebugging : null,
    get_isRendering : null,
    get_lastcpuClock : null,
    set_lastcpuClock : null,
    get_maxSpritesPerScanline : null,
    set_maxSpritesPerScanline : null,
    get_nameTableMemoryStart : null,
    set_nameTableMemoryStart : null,
    get_needToDraw : null,
    get_nextEventAt : null,
    get_nmiHandler : null,
    set_nmiHandler : null,
    get_nmiIsThrown : null,
    get_outBuffer : null,
    get_palette : null,
    set_palette : null,
    get_patternTableIndex : null,
    get_pixelWidth : null,
    set_pixelWidth : null,
    get_ppuAddress : null,
    set_ppuAddress : null,
    get_ppuControlByte0 : null,
    set_ppuControlByte0 : null,
    get_ppuControlByte1 : null,
    set_ppuControlByte1 : null,
    get_ppuStatus : null,
    set_ppuStatus : null,
    preloadSprites : null,
    renderScanline : null,
    resetClock : null,
    get_scanlineNum : null,
    get_scanlinePos : null,
    setByte : null,
    setupVINT : null,
    setVideoBuffer : null,
    get_shouldRender : null,
    set_shouldRender : null,
    get_spriteCopyHasHappened : null,
    set_spriteCopyHasHappened : null,
    get_spriteRam : null,
    get_spritesAreVisible : null,
    get_spritesOnLine : null,
    unpackSprites : null,
    get_videoBuffer : null,
    vidRAM_GetNTByte : null,
    get_vScroll : null,
    getFrameChecksum : null
}
NES.CPU.PixelWhizzlerClasses.IPPU.registerInterface('NES.CPU.PixelWhizzlerClasses.IPPU');


Type.registerNamespace('NES.CPU.PPUClasses');

////////////////////////////////////////////////////////////////////////////////
// NES.CPU.PPUClasses.NESSprite

NES.CPU.PPUClasses.NESSprite = function NES_CPU_PPUClasses_NESSprite() {
    /// <field name="yPosition" type="Number" integer="true">
    /// </field>
    /// <field name="xPosition" type="Number" integer="true">
    /// </field>
    /// <field name="spriteNumber" type="Number" integer="true">
    /// </field>
    /// <field name="foreground" type="Boolean">
    /// </field>
    /// <field name="isVisible" type="Boolean">
    /// </field>
    /// <field name="tileIndex" type="Number" integer="true">
    /// </field>
    /// <field name="attributeByte" type="Number" integer="true">
    /// </field>
    /// <field name="flipX" type="Boolean">
    /// </field>
    /// <field name="flipY" type="Boolean">
    /// </field>
    /// <field name="changed" type="Boolean">
    /// </field>
}
NES.CPU.PPUClasses.NESSprite.prototype = {
    yPosition: 0,
    xPosition: 0,
    spriteNumber: 0,
    foreground: false,
    isVisible: false,
    tileIndex: 0,
    attributeByte: 0,
    flipX: false,
    flipY: false,
    changed: false
}


    var _p32 = new Int32Array(256);
    var _framePalette = new Int32Array(256);
    var rgb32OutBuffer = new Int32Array(256 * 256);
    var outBuffer = new Int32Array(256 * 256);
    var _drawInfo = new Int32Array(256 * 256);
    var _palette = new Int32Array(32);
    var sprite0scanline = -1;
    var sprite0x = -1;
    var _spriteRAM = new Int32Array(256);
    var _spritesOnLine = new Int32Array(256 * 2);
    var _vBuffer = new Int32Array(240 * 256);

    var _spriteAddress = 0;
    var _maxSpritesPerScanline = 8;
    var currentSprites = new Array(8);
    var _unpackedSprites = new Array(64);


    initSprites();



    var currentXPosition =  0;
    var currentYPosition = 0;
    
    function get_currentYPosition() {
        /// <value type="Number" integer="true"></value>
        return currentYPosition;
    }
    
    function get_currentXPosition() {
        /// <value type="Number" integer="true"></value>
        return currentXPosition;
    }
    
    var _scanlineNum = 0;
    var _scanlinePos = 0;
    
    function get_scanlinePos() {
        /// <value type="Number" integer="true"></value>
        return _scanlinePos;
    }
    
    function get_scanlineNum() {
        /// <value type="Number" integer="true"></value>
        return _scanlineNum;
    }
    
    var _isDebugging = false;
    
    function get_isDebugging() {
        /// <value type="Boolean"></value>
        return _isDebugging;
    }

    function set_isDebugging(value) {
        /// <value type="Boolean"></value>
        _isDebugging = value;
        return value;
    }
    
    function initializePPU() {
        _PPUAddress = 0;
        _PPUStatus = 0;
        _PPUControlByte0 = 0;
        _PPUControlByte1 = 0;
        _hScroll = 0;
        _scanlineNum = 0;
        _scanlinePos = 0;
        _spriteAddress = 0;

        initSprites();
    }
    
    var _shouldRender = false;
    
    function get_shouldRender() {
        /// <value type="Boolean"></value>
        return _shouldRender;
    }

    function set_shouldRender(value) {
        /// <value type="Boolean"></value>
        _shouldRender = value;
        return value;
    }
    
    var _vBuffer = null;
    var _frames = 0;
    
    function get_frames() {
        /// <value type="Number" integer="true"></value>
        return _frames;
    }
    
    function vidRAM_GetNTByte(address) {
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var result = 0;
        if (address >= 8192 && address < 12288) {
            result = getPPUByte(0, address);
        }
        else {
            result = getPPUByte(0, address);
        }
        return result;
    }
    
    var hitSprite = false;
    var _handleVBlankIRQ = false;
    
    function get_handleVBlankIRQ() {
        /// <value type="Boolean"></value>
        return _handleVBlankIRQ;
    }

    function set_handleVBlankIRQ(value) {
        /// <value type="Boolean"></value>
        _handleVBlankIRQ = value;
        return value;
    }
    
    var _PPUControlByte0 = 0;
    var _PPUControlByte1 = 0;
    
    function get_ppuControlByte0() {
        /// <value type="Number" integer="true"></value>
        return _PPUControlByte0;
    }

    function set_ppuControlByte0(value) {
        /// <value type="Number" integer="true"></value>
        if (_PPUControlByte0 !== value) {
            _PPUControlByte0 = value;
            _updatePPUControlByte0();
        }
        return value;
    }
    
    function _updatePPUControlByte0() {
        if ((_PPUControlByte0 & 16) === 16) {
            _backgroundPatternTableIndex = 4096;
        }
        else {
            _backgroundPatternTableIndex = 0;
        }
    }
    
    function get_nmiIsThrown() {
        /// <value type="Boolean"></value>
        return (_PPUControlByte0 & 128) === 128;
    }
    
    function get_ppuControlByte1() {
        /// <value type="Number" integer="true"></value>
        return _PPUControlByte1;
    }

    function set_ppuControlByte1(value) {
        /// <value type="Number" integer="true"></value>
        _PPUControlByte1 = value;
        return value;
    }
    
    function get_backgroundVisible() {
        /// <value type="Boolean"></value>
        return _tilesAreVisible;
    }
    
    var _spritesAreVisible = false;
    var _tilesAreVisible = false;
    
    function get_spritesAreVisible() {
        /// <value type="Boolean"></value>
        return _spritesAreVisible;
    }
    
    var _PPUStatus = 0;
    
    function get_ppuStatus() {
        /// <value type="Number" integer="true"></value>
        return _PPUStatus;
    }

    function set_ppuStatus(value) {
        /// <value type="Number" integer="true"></value>
        _PPUStatus = value;
        return value;
    }
    
    var _PPUAddress = 0;
    
    function get_ppuAddress() {
        /// <value type="Number" integer="true"></value>
        return _PPUAddress;
    }

    function set_ppuAddress(value) {
        /// <value type="Number" integer="true"></value>
        _PPUAddress = value;
        return value;
    }
    
    var _ppuReadBuffer = 0;
    var _ppuAddressLatchIsHigh = true;
    var _backgroundPatternTableIndex = 0;
    
    function get_patternTableIndex() {
        /// <value type="Number" integer="true"></value>
        return _backgroundPatternTableIndex;
    }
    
    var _needToDraw  = true;
    
    function get_needToDraw() {
        /// <value type="Boolean"></value>
        return _needToDraw;
    }
    
    var _isRendering = true;
    
    function get_isRendering() {
        /// <value type="Boolean"></value>
        return _isRendering;
    }
    
    var frameClock = 0;
    var frameEnded = false;
    var _frameOn = false;
    
    function get_frameOn() {
        /// <value type="Boolean"></value>
        return _frameOn;
    }

    function set_frameOn(value) {
        /// <value type="Boolean"></value>
        _frameOn = value;
        return value;
    }

    var _nameTableMemoryStart = 0;
    
    function get_nameTableMemoryStart() {
        /// <value type="Number" integer="true"></value>
        return _nameTableMemoryStart;
    }

    function set_nameTableMemoryStart(value) {
        /// <value type="Number" integer="true"></value>
        _nameTableMemoryStart = value;
        return value;
    }
    
    function get_currentFrame() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return _vBuffer;
    }
    
    function renderScanline(scanlineNum) {
        /// <param name="scanlineNum" type="Number" integer="true">
        /// </param>
        throw new Error('Not Implemented');
    }
    
    var lastcpuClock = 0;
    
    function get_lastcpuClock() {
        /// <value type="Number" integer="true"></value>
        return lastcpuClock;
    }

    function set_lastcpuClock(value) {
        /// <value type="Number" integer="true"></value>
        lastcpuClock = value;
        return value;
    }
    
    function drawTo(cpuClockNum) {
        /// <summary>
        /// draws from the lastcpuClock to the current one
        /// </summary>
        /// <param name="cpuClockNum" type="Number" integer="true">
        /// </param>
        var frClock = (cpuClockNum - lastcpuClock) * 3;
        if (frameClock < 6820) {
            if (frameClock + frClock < 6820) {
                frameClock += frClock;
                frClock = 0;
            }
            else {
                frClock += frameClock - 6820;
                frameClock = 6820;
            }
        }
        bumpScanline(frClock);
        lastcpuClock = cpuClockNum;
    }
    
    function bumpScanline(frClock) {
        /// <param name="frClock" type="Number" integer="true">
        /// </param>
        do {
            switch (frameClock++) {
                case 0:
                    break;
                case 6820:
                    clearVINT();
                    _frameOn = true;
                    frameOn = true;
                    if (spriteChanges) {
                        unpackSprites();
                        spriteChanges = false;
                    }
                    break;
                case 7125:
                    break;
                case 7161:
                    vbufLocation = 0;
                    xNTXor = 0;
                    yNTXor = 0;
                    currentXPosition = 0;
                    currentYPosition = 0;
                    break;
                case frameClockEnd:
                    drawCurrentFrame();
                    
                    _shouldRender = true;
                    frameOn = false;
                    setupVINT();
                    _frameOn = false;
                    frameClock = 0;
                    break;
            }
            if (frameClock >= 7161 && frameClock <= 89342) {
                if (currentXPosition < 256 && vbufLocation < 61440) {
                    xPosition = currentXPosition + lockedHScroll;
                    if ((xPosition & 7) === 0) {
                        xNTXor = ((xPosition & 256) === 256) ? 1024 : 0;
                        xPosition &= 255;
                        _fetchNextTile();
                    }
                    var tilePixel = (_tilesAreVisible) ? getNameTablePixel() : 0;
                    var spritePixel = (_spritesAreVisible) ? getSpritePixel() : 0;
                    if (!hitSprite && spriteZeroHit && tilePixel !== 0) {
                        hitSprite = true;
                        _PPUStatus = _PPUStatus | 64;
                    }
                    var pixel = (spriteIsForegroundPixel || (tilePixel === 0 && spritePixel !== 0)) ? spritePixel : tilePixel;
                    
                    //writePixel(vbufLocation, pixel);
                    
                    var palIndex = _palette[pixel];
                    writePixel(vbufLocation, palIndex);

                    vbufLocation++;
                }
                currentXPosition++;
                if (currentXPosition > 340) {
                    currentXPosition = 0;
                    currentYPosition++;
                    preloadSprites(currentYPosition);
                    if (_spritesOnThisScanline >= 7) {
                        _PPUStatus = _PPUStatus | 32;
                    }
                    lockedHScroll = _hScroll;
                    updatePixelInfo();
                    runNewScanlineEvents();
                }
            }
        } while (frClock-- > 0);
    }
    
    function get_outBuffer() {
        return outBuffer;
    }
    
    function get_videoBuffer() {
        return rgb32OutBuffer;
    }
    
    function setVideoBuffer(inBuffer) {
        /// <param name="inBuffer" type="Array" elementType="Number" elementInteger="true">
        /// </param>
        rgb32OutBuffer = inBuffer;
    }
    
    var _frameEnded = false;
    
    function checkVBlank() {
        /// <summary>
        /// Checks if NMI needs to be reasserted during vblank
        /// </summary>
        if (!_nmiHasBeenThrownThisFrame && !_frameOn && get_nmiIsThrown() && get__nmiOccurred()) {
            _nmiHandler.invoke();
            set_handleVBlankIRQ(true);
            _nmiHasBeenThrownThisFrame = true;
        }
    }
    
    var vbufLocation = 0;
    var pixelWidth = 32;
    
    function get_pixelWidth() {
        /// <value type="Number" integer="true"></value>
        return pixelWidth;
    }

    function set_pixelWidth(value) {
        /// <value type="Number" integer="true"></value>
        pixelWidth = value;
        return value;
    }
    
    var _fillRGB = false;
    
    function get_fillRGB() {
        /// <value type="Boolean"></value>
        return _fillRGB;
    }

    function set_fillRGB(value) {
        /// <value type="Boolean"></value>
        _fillRGB = value;
        return value;
    }
    
    function updatePixelInfo() {
        _nameTableMemoryStart = nameTableBits * 1024;
    }
    
    var _clipTiles = false;
    var _clipSprites = false;
    
    function _clippingTilePixels() {
        /// <returns type="Boolean"></returns>
        return _clipTiles;
    }
    
    function _clippingSpritePixels() {
        /// <returns type="Boolean"></returns>
        return _clipSprites;
    }
    
    var nameTableBits = 0;
    var _vidRamIsRam = true;
    
    function get_palette() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return _palette;
    }

    function set_palette(value) {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        _palette = value;
        return value;
    }
    
    function setPPUByte(Clock, address, data) {
        /// <param name="Clock" type="Number" integer="true">
        /// </param>
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <param name="data" type="Number" integer="true">
        /// </param>
        _needToDraw = true;
        switch (address & 7) {
            case 0:
                drawTo(Clock);
                _PPUControlByte0 = data;
                nameTableBits = _PPUControlByte0 & 3;
                _backgroundPatternTableIndex = ((_PPUControlByte0 & 16) >> 4) * 4096;
                updatePixelInfo();
                break;
            case 1:
                drawTo(Clock);
                _isRendering = (data & 24) !== 0;
                _PPUControlByte1 = data;
                _spritesAreVisible = (_PPUControlByte1 & 16) === 16;
                _tilesAreVisible = (_PPUControlByte1 & 8) === 8;
                _clipTiles = (_PPUControlByte1 & 2) !== 2;
                _clipSprites = (_PPUControlByte1 & 4) !== 4;
                updatePixelInfo();
                break;
            case 2:
                _ppuReadBuffer = data;
                break;
            case 3:
                _spriteAddress = data & 255;
                break;
            case 4:
                _spriteRAM[_spriteAddress] = data;
                _spriteAddress = (_spriteAddress + 1) & 255;
                _unpackedSprites[_spriteAddress / 4].changed = true;
                spriteChanges = true;
                break;
            case 5:
                if (_ppuAddressLatchIsHigh) {
                    drawTo(Clock);
                    _hScroll = data;
                    lockedHScroll = _hScroll & 7;
                    updatePixelInfo();
                    _ppuAddressLatchIsHigh = false;
                }
                else {
                    drawTo(Clock);
                    _vScroll = data;
                    if (data > 240) {
                        _vScroll = data - 256;
                    }
                    if (!_frameOn || (_frameOn && !_isRendering)) {
                        lockedVScroll = _vScroll;
                    }
                    _ppuAddressLatchIsHigh = true;
                    updatePixelInfo();
                }
                break;
            case 6:
                if (_ppuAddressLatchIsHigh) {
                    _PPUAddress = (_PPUAddress & 255) | ((data & 63) << 8);
                    _ppuAddressLatchIsHigh = false;
                }
                else {
                    _PPUAddress = (_PPUAddress & 32512) | data & 255;
                    _ppuAddressLatchIsHigh = true;
                    drawTo(Clock);
                    _hScroll = ((_PPUAddress & 31) << 3);
                    _vScroll = (((_PPUAddress >> 5) & 31) << 3);
                    _vScroll |= ((_PPUAddress >> 12) & 3);
                    nameTableBits = ((_PPUAddress >> 10) & 3);
                    if (_frameOn) {
                        lockedHScroll = _hScroll;
                        lockedVScroll = _vScroll;
                        lockedVScroll -= currentYPosition;
                    }
                    updatePixelInfo();
                }
                break;
            case 7:
                if ((_PPUAddress & 65280) === 16128) {
                    drawTo(Clock);
                    writeToNESPalette(_PPUAddress, data);
                    updatePixelInfo();
                }
                else {
                    if ((_PPUAddress & 61440) === 8192) {
                        chrRomHandler.setPPUByte(Clock, _PPUAddress, data);
                    }
                    else {
                        if (_vidRamIsRam) {
                            chrRomHandler.setPPUByte(Clock, _PPUAddress, data);
                        }
                    }
                }
                if ((get_ppuControlByte0() & 4) === 4) {
                    _PPUAddress = (_PPUAddress + 32);
                }
                else {
                    _PPUAddress = (_PPUAddress + 1);
                }
                _ppuAddressLatchIsHigh = true;
                set_ppuAddress((get_ppuAddress() & 16383));
                break;
        }
    }
    
    function getPPUByte(Clock, address) {
        /// <param name="Clock" type="Number" integer="true">
        /// </param>
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        switch (address & 7) {
            case 0:
            case 1:
            case 5:
            case 6:
                return _ppuReadBuffer;
            case 2:
                var ret;
                _ppuAddressLatchIsHigh = true;
                ret = (_ppuReadBuffer & 31) | _PPUStatus;
                drawTo(Clock);
                if ((ret & 128) === 128) {
                    _PPUStatus = _PPUStatus & ~128;
                }
                updatePixelInfo();
                return ret;
            case 4:
                var tmp = _spriteRAM[_spriteAddress];
                return tmp;
            case 7:
                if ((get_ppuAddress() & 65280) === 16128) {
                    tmp = _palette[get_ppuAddress() & 31];
                    _ppuReadBuffer = chrRomHandler.getPPUByte(Clock, _PPUAddress - 4096);
                }
                else {
                    tmp = _ppuReadBuffer;
                    if ((_PPUAddress >= 8192 & _PPUAddress <= 12287) === 1) {
                        _ppuReadBuffer = chrRomHandler.getPPUByte(Clock, _PPUAddress);
                    }
                    else {
                        _ppuReadBuffer = chrRomHandler.getPPUByte(Clock, _PPUAddress & 16383);
                    }
                }
                if ((_PPUControlByte0 & 4) === 4) {
                    _PPUAddress = _PPUAddress + 32;
                }
                else {
                    _PPUAddress = _PPUAddress + 1;
                }
                _PPUAddress = (_PPUAddress & 16383);
                return tmp;
        }
        return 0;
    }
    
    var _nmiHandler = null;
    
    function get_nmiHandler() {
        /// <value type="NES.CPU.Fastendo.MachineEvent"></value>
        return _nmiHandler;
    }
    
    function set_nmiHandler(value) {
        /// <value type="NES.CPU.Fastendo.MachineEvent"></value>
        _nmiHandler = value;
        return value;
    }
    
    function get_irqAsserted() {
        /// <summary>
        /// ppu doesnt throw irq's
        /// </summary>
        /// <value type="Boolean"></value>
        return false;
    }

    function set_irqAsserted(value) {
        /// <summary>
        /// ppu doesnt throw irq's
        /// </summary>
        /// <value type="Boolean"></value>
        return value;
    }
    
    function get_nextEventAt() {
        /// <value type="Number" integer="true"></value>
        if (frameClock < 6820) {
            return (6820 - frameClock) / 3;
        }
        else {
            return (((89345 - frameClock) / 341) / 3);
        }
    }
    
    function handleEvent(Clock) {
        /// <param name="Clock" type="Number" integer="true">
        /// </param>
        drawTo(Clock);
    }
    
    function resetClock(Clock) {
        /// <param name="Clock" type="Number" integer="true">
        /// </param>
        lastcpuClock = Clock;
    }
    
    var _frameFinished = null;
    
    function get_frameFinishHandler() {
        /// <value type="NES.CPU.Fastendo.MachineEvent"></value>
        return _frameFinished;
    }

    function set_frameFinishHandler(value) {
        /// <value type="NES.CPU.Fastendo.MachineEvent"></value>
        _frameFinished = value;
        return value;
    }
    
    function get__nmiOccurred() {
        /// <value type="Boolean"></value>
        return (_PPUStatus & 128) === 128;
    }
    
    var _nmiHasBeenThrownThisFrame = false;
    
    function setupVINT() {
        _PPUStatus = _PPUStatus | 128;
        _nmiHasBeenThrownThisFrame = false;
        _frames = _frames + 1;
        _needToDraw = false;
        if (get_nmiIsThrown()) {
            cpuHandleNMI = true;
            set_handleVBlankIRQ(true);
            _nmiHasBeenThrownThisFrame = true;
        }
    }
    
    function clearVINT() {
        _PPUStatus = 0;
        hitSprite = false;
        spriteSize = ((_PPUControlByte0 & 32) === 32) ? 16 : 8;
        if ((_PPUControlByte1 & 24) !== 0) {
            _isRendering = true;
        }
    }
    
    function _runEndOfScanlineRenderEvents() {
    }
    
    function runNewScanlineEvents() {
        yPosition = currentYPosition + lockedVScroll;
        if (yPosition < 0) {
            yPosition += 240;
        }
        if (yPosition >= 240) {
            yPosition -= 240;
            yNTXor = 2048;
        }
        else {
            yNTXor = 0;
        }
    }
    
    function _updateSprites() {
    }
    
    function _updateTiles() {
    }
    
    var _hScroll =  0;
    var _vScroll = 0;
    var lockedHScroll = 0;
    var lockedVScroll = 0;
    
    function get_hScroll() {
        /// <value type="Number" integer="true"></value>
        return lockedHScroll;
    }
    
    function get_vScroll() {
        /// <value type="Number" integer="true"></value>
        return lockedVScroll;
    }
    
    var spriteChanges = false;
    var _spriteCopyHasHappened = false;
    
    function get_spriteCopyHasHappened() {
        /// <value type="Boolean"></value>
        return _spriteCopyHasHappened;
    }

    function set_spriteCopyHasHappened(value) {
        /// <value type="Boolean"></value>
        _spriteCopyHasHappened = value;
        return value;
    }
    
    var spriteZeroHit = false;
    var spriteIsForegroundPixel = false;


    
    function get_maxSpritesPerScanline() {
        /// <value type="Number" integer="true"></value>
        return _maxSpritesPerScanline;
    }

    function set_maxSpritesPerScanline(value) {
        /// <value type="Number" integer="true"></value>
        _maxSpritesPerScanline = value;
        return value;
    }
    

    function get_spriteRam() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return _spriteRAM;
    }
    
    function copySprites(source, copyFrom) {
        /// <param name="source" type="Array" elementType="Number" elementInteger="true">
        /// </param>
        /// <param name="copyFrom" type="Number" integer="true">
        /// </param>
        for (var i = 0; i < 256; ++i) {
            var spriteLocation = (_spriteAddress + i) & 255;
            if (_spriteRAM[spriteLocation] !== source[copyFrom + i]) {
                _spriteRAM[spriteLocation] = source[copyFrom + i];
                var spriteId = spriteLocation >> 2;
                _unpackedSprites[spriteId].changed = true;
            }
        }
        _spriteCopyHasHappened = true;
        spriteChanges = true;
    }
    
    function initSprites() {
        for (var i = 0; i < _maxSpritesPerScanline; ++i) {
            currentSprites[i] = new NES.CPU.PPUClasses.NESSprite();
        }
        
        for (var i = 0; i < 64; ++i) {
            _unpackedSprites[i] = new NES.CPU.PPUClasses.NESSprite();
        }
    }
    
    function getSpritePixel() {
        /// <returns type="Number" integer="true"></returns>
        spriteIsForegroundPixel = false;
        spriteZeroHit = false;
        var result = 0;
        var yLine = 0;
        var xPos = 0;
        var tileIndex = 0;
        for (var i = 0; i < _spritesOnThisScanline; ++i) {
            var currSprite = currentSprites[i];
            if (currSprite.xPosition > 0 && currentXPosition >= currSprite.xPosition && currentXPosition < currSprite.xPosition + 8) {
                var spritePatternTable = 0;
                if ((_PPUControlByte0 & 8) === 8) {
                    spritePatternTable = 4096;
                }
                xPos = currentXPosition - currSprite.xPosition;
                yLine = currentYPosition - currSprite.yPosition - 1;
                yLine = yLine & (spriteSize - 1);
                tileIndex = currSprite.tileIndex;
                if ((_PPUControlByte0 & 32) === 32) {
                    if ((tileIndex & 1) === 1) {
                        spritePatternTable = 4096;
                        tileIndex = tileIndex ^ 1;
                    }
                    else {
                        spritePatternTable = 0;
                    }
                }
                result = whissaSpritePixel(spritePatternTable, xPos, yLine, currSprite, tileIndex);
                if (result !== 0) {
                    if (currSprite.spriteNumber === 0) {
                        spriteZeroHit = true;
                    }
                    spriteIsForegroundPixel = currSprite.foreground;
                    return (result | currSprite.attributeByte);
                }
            }
        }
        return 0;
    }
    
    function whissaSpritePixel(patternTableIndex, x, y, sprite, tileIndex) {
        /// <param name="patternTableIndex" type="Number" integer="true">
        /// </param>
        /// <param name="x" type="Number" integer="true">
        /// </param>
        /// <param name="y" type="Number" integer="true">
        /// </param>
        /// <param name="sprite" type="NES.CPU.PPUClasses.NESSprite">
        /// </param>
        /// <param name="tileIndex" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var patternEntry;
        var patternEntryBit2;
        if (sprite.flipY) {
            y = spriteSize - y - 1;
        }
        if (y >= 8) {
            y += 8;
        }
        patternEntry = chrRomHandler.getPPUByte(0, patternTableIndex + tileIndex * 16 + y);
        patternEntryBit2 = chrRomHandler.getPPUByte(0, patternTableIndex + tileIndex * 16 + y + 8);
        return ((sprite.flipX) ? ((patternEntry >> x) & 1) | (((patternEntryBit2 >> x) << 1) & 2) : ((patternEntry >> 7 - x) & 1) | (((patternEntryBit2 >> 7 - x) << 1) & 2));
    }
    
    var _spritesOnThisScanline = 0;
    var spriteSize = 0;
    
    function get_spritesOnLine() {
        /// <value type="Array" elementType="Number" elementInteger="true"></value>
        return _spritesOnLine;
    }
    
    function preloadSprites(scanline) {
        /// <summary>
        /// populates the currentSpritesXXX arrays with the first 8 visible sprites on the
        /// denoted scanline.
        /// </summary>
        /// <param name="scanline" type="Number" integer="true">
        /// the scanline to preload sprites for
        /// </param>
        _spritesOnThisScanline = 0;
        sprite0scanline = -1;
        var yLine = currentYPosition - 1;
        for (var spriteNum = 0; spriteNum < 256; spriteNum += 4) {
            var spriteID = ((spriteNum + _spriteAddress) & 255) >> 2;
            var y = _unpackedSprites[spriteID].yPosition + 1;
            if (scanline >= y && scanline < y + spriteSize) {
                if (spriteID === 0) {
                    sprite0scanline = scanline;
                    sprite0x = _unpackedSprites[spriteID].xPosition;
                }
                var spId = spriteNum >> 2;
                currentSprites[_spritesOnThisScanline] = _unpackedSprites[spriteID];
                currentSprites[_spritesOnThisScanline].isVisible = true;
                _spritesOnThisScanline++;
                if (_spritesOnThisScanline === _maxSpritesPerScanline) {
                    break;
                }
            }
        }
        if (_spritesOnThisScanline > 7) {
            _PPUStatus = _PPUStatus | 32;
        }
    }
    
    function unpackSprites() {
        var len = _unpackedSprites.length;
        for (var currSprite = 0; currSprite < len; ++currSprite) {
            if (_unpackedSprites[currSprite].changed) {
                _unpackSprite(currSprite);
            }
        }
    }
    
    function _unpackSprite(currSprite) {
        /// <param name="currSprite" type="Number" integer="true">
        /// </param>
        var attrByte = _spriteRAM[currSprite * 4 + 2];
        _unpackedSprites[currSprite].isVisible = true;
        _unpackedSprites[currSprite].attributeByte = ((attrByte & 3) << 2) | 16;
        _unpackedSprites[currSprite].yPosition = _spriteRAM[currSprite * 4];
        _unpackedSprites[currSprite].xPosition = _spriteRAM[currSprite * 4 + 3];
        _unpackedSprites[currSprite].spriteNumber = currSprite;
        _unpackedSprites[currSprite].foreground = (attrByte & 32) !== 32;
        _unpackedSprites[currSprite].flipX = (attrByte & 64) === 64;
        _unpackedSprites[currSprite].flipY = (attrByte & 128) === 128;
        _unpackedSprites[currSprite].tileIndex = _spriteRAM[currSprite * 4 + 1];
        _unpackedSprites[currSprite].changed = false;
    }
    
    var _patternEntry = 0;
    var _patternEntryByte2 = 0;
    var _currentAttributeByte = 0;
    var _currentTileIndex = 0;
    var xNTXor = 0;
    var yNTXor = 0;
    var _fetchTile = true;
    var xPosition = 0;
    var yPosition = 0;
    
    function getNameTablePixel() {
        /// <returns type="Number" integer="true"></returns>
        var result = ((_patternEntry & 128) >> 7) + ((_patternEntryByte2 & 128) >> 6);
        _patternEntry = _patternEntry << 1;
        _patternEntryByte2 = _patternEntryByte2 << 1;
        if (result > 0) {
            result += _currentAttributeByte;
        }
        return result;
    }
    
    function _fetchNextTile() {
        var ppuNameTableMemoryStart = _nameTableMemoryStart ^ xNTXor ^ yNTXor;
        var xTilePosition = xPosition >> 3;
        var tileRow = ((yPosition >> 3) % 30) << 5;
        var tileNametablePosition = 8192 + ppuNameTableMemoryStart + xTilePosition + tileRow;
        var TileIndex = chrRomHandler.getPPUByte(0, tileNametablePosition);
        var patternTableYOffset = yPosition & 7;
        var patternID = _backgroundPatternTableIndex + (TileIndex * 16) + patternTableYOffset;
        _patternEntry = chrRomHandler.getPPUByte(0, patternID);
        _patternEntryByte2 = chrRomHandler.getPPUByte(0, patternID + 8);
        _currentAttributeByte = _getAttributeTableEntry(ppuNameTableMemoryStart, xTilePosition, yPosition >> 3);
    }
    
    function _getAttributeTableEntry(ppuNameTableMemoryStart, i, j) {
        /// <param name="ppuNameTableMemoryStart" type="Number" integer="true">
        /// </param>
        /// <param name="i" type="Number" integer="true">
        /// </param>
        /// <param name="j" type="Number" integer="true">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var LookUp = chrRomHandler.getPPUByte(0, 8192 + ppuNameTableMemoryStart + 960 + (i >> 2) + ((j >> 2) << 3));
        switch ((i & 2) | ((j & 2) * 2)) {
            case 0:
                return (LookUp << 2) & 12;
            case 2:
                return LookUp & 12;
            case 4:
                return (LookUp >> 2) & 12;
            case 6:
                return (LookUp >> 4) & 12;
        }
        return 0;
    }
    
    var chrRomHandler = null;
    
    function get_chrRomHandler() {
        /// <value type="NES.CPU.Machine.Carts.INESCart"></value>
        return chrRomHandler;
    }
    
    function set_chrRomHandler(value) {
        /// <value type="NES.CPU.Machine.Carts.INESCart"></value>
        chrRomHandler = value;
        return value;
    }
    
    function writeToNESPalette(address, data) {
        /// <summary>
        /// Called by SetByte() for writes to palette ram
        /// </summary>
        /// <param name="address" type="Number" integer="true">
        /// </param>
        /// <param name="data" type="Number" integer="true">
        /// </param>
        
        var palAddress = address & 31;
        
        
        
        _palette[palAddress] = data;
        updatePaletteEntry(palAddress, data);
        if ((_PPUAddress & 65519) === 16128) {
            updatePaletteEntry((palAddress ^ 16) & 31, data);
            _palette[(palAddress ^ 16) & 31] = data;
        }
    }
    
    function getFrameChecksum() {
        /// <returns type="Number" integer="true"></returns>
        var sum = 0;
        var $enum1 = ss.IEnumerator.getEnumerator(rgb32OutBuffer);
        while ($enum1.moveNext()) {
            var i = $enum1.get_current();
            sum += i;
            sum &= 268435455;
        }
        return sum;
    }


////////////////////////////////////////////////////////////////////////////////
// NES.CPU.PPUClasses.PPUWriteEvent

NES.CPU.PPUClasses.PPUWriteEvent = function NES_CPU_PPUClasses_PPUWriteEvent() {
    /// <field name="_isWrite" type="Boolean">
    /// </field>
    /// <field name="_scanlineNum" type="Number" integer="true">
    /// </field>
    /// <field name="_scanlinePos" type="Number" integer="true">
    /// </field>
    /// <field name="_frameClock" type="Number" integer="true">
    /// </field>
    /// <field name="_registerAffected" type="Number" integer="true">
    /// </field>
    /// <field name="_dataWritten" type="Number" integer="true">
    /// </field>
}
NES.CPU.PPUClasses.PPUWriteEvent.prototype = {
    _isWrite: false,
    
    get_isWrite: function NES_CPU_PPUClasses_PPUWriteEvent$get_isWrite() {
        /// <value type="Boolean"></value>
        return this._isWrite;
    },
    set_isWrite: function NES_CPU_PPUClasses_PPUWriteEvent$set_isWrite(value) {
        /// <value type="Boolean"></value>
        this._isWrite = value;
        return value;
    },
    
    _scanlineNum: 0,
    
    get_scanlineNum: function NES_CPU_PPUClasses_PPUWriteEvent$get_scanlineNum() {
        /// <value type="Number" integer="true"></value>
        return this._scanlineNum;
    },
    set_scanlineNum: function NES_CPU_PPUClasses_PPUWriteEvent$set_scanlineNum(value) {
        /// <value type="Number" integer="true"></value>
        this._scanlineNum = value;
        return value;
    },
    
    _scanlinePos: 0,
    
    get_scanlinePos: function NES_CPU_PPUClasses_PPUWriteEvent$get_scanlinePos() {
        /// <value type="Number" integer="true"></value>
        return this._scanlinePos;
    },
    set_scanlinePos: function NES_CPU_PPUClasses_PPUWriteEvent$set_scanlinePos(value) {
        /// <value type="Number" integer="true"></value>
        this._scanlinePos = value;
        return value;
    },
    
    _frameClock: 0,
    
    get_frameClock: function NES_CPU_PPUClasses_PPUWriteEvent$get_frameClock() {
        /// <value type="Number" integer="true"></value>
        return this._frameClock;
    },
    set_frameClock: function NES_CPU_PPUClasses_PPUWriteEvent$set_frameClock(value) {
        /// <value type="Number" integer="true"></value>
        this._frameClock = value;
        return value;
    },
    
    _registerAffected: 0,
    
    get_registerAffected: function NES_CPU_PPUClasses_PPUWriteEvent$get_registerAffected() {
        /// <value type="Number" integer="true"></value>
        return this._registerAffected;
    },
    set_registerAffected: function NES_CPU_PPUClasses_PPUWriteEvent$set_registerAffected(value) {
        /// <value type="Number" integer="true"></value>
        this._registerAffected = value;
        return value;
    },
    
    _dataWritten: 0,
    
    get_dataWritten: function NES_CPU_PPUClasses_PPUWriteEvent$get_dataWritten() {
        /// <value type="Number" integer="true"></value>
        return this._dataWritten;
    },
    set_dataWritten: function NES_CPU_PPUClasses_PPUWriteEvent$set_dataWritten(value) {
        /// <value type="Number" integer="true"></value>
        this._dataWritten = value;
        return value;
    },
    
    get_text: function NES_CPU_PPUClasses_PPUWriteEvent$get_text() {
        /// <value type="String"></value>
        return this.toString();
    },
    
    toString: function NES_CPU_PPUClasses_PPUWriteEvent$toString() {
        /// <returns type="String"></returns>
        return String.format(' {0:x2} written to {1:x4} at {2}, {3}', this._registerAffected, this._dataWritten, this._scanlineNum, this._scanlinePos);
    }
}


_scanlinePreRenderDummyScanline = 20;
_scanlineRenderingStartsOn = 21;
_scanlineRenderingEndsOn = 260;
_scanlineLastRenderedPixel = 255;
_scanlineTotalLength = 340;
_scanlineEventPPUXIncremented = 3;
_scanlineEventPPUXReset = 257;
_scanlineEventPPUYIncremented = 251;
_vBufferWidth = 256;
pal = new Array(256 * 4);
frameClockEnd = 89342;