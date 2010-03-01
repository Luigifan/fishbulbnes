using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine.Carts;
using System.IO;
using NES.CPU.Fastendo;
using NES.CPU.PPUClasses;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.Security.Cryptography;
using NES.CPU.PixelWhizzlerClasses;

namespace NES.CPU.Machine.ROMLoader
{
    public static class iNESFileHandler
    {
        public static INESCart GetCart(string fileName, IPPU ppu)
        {
            INESCart _cart = null;
            if (fileName.IndexOf(".zip") > 0)
            {
                return GetZippedCart(fileName, ppu);
            }

            if (fileName.IndexOf(".nsf") > 0)
            {
                throw new CartLoadException("NSF SUPPORT IS NOT SUPPORTED.  please fund development the ultimate edition for full nsf support.");
                //using (FileStream stream = File.Open(fileName, FileMode.Open))
                //{
                //    _cart = LoadNSF(stream);
                //}
                //return _cart;
            }

            using (FileStream stream = File.Open(fileName, FileMode.Open))
            {
                _cart = LoadROM(ppu, stream);
            }
            return _cart;
        }

        public static INESCart GetZippedCart(string fileName, IPPU ppu)
        {
            INESCart _cart = null;

            FileStream stream = File.Open(fileName, FileMode.Open);
            
            ZipInputStream zipStream = new ZipInputStream(stream);
            
            ZipEntry entry = zipStream.GetNextEntry();
            try
            {
                
                if (entry.Name.IndexOf(".nes") > 0)
                {
                    Console.WriteLine("Loading " + entry.Name + " " + entry.Size.ToString() + " bytes");
                    byte[] data;//= new byte[entry.Size];

                    BinaryReader reader = new BinaryReader(zipStream);
                    data = reader.ReadBytes((int)entry.Size);

                    //int len = zipStream.Read(data, 0, (int)entry.Size);
                    Console.WriteLine("BodyRead " + data.Length.ToString() + " bytes");
                    //reader.Close();
                    MemoryStream mstream = new MemoryStream(data);
                    _cart = LoadROM(ppu, mstream);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failure reading zip file.", e);
            }
            finally
            {
                zipStream.CloseEntry();
                zipStream.Close();
                stream.Close();
            }

            return _cart;
        }

        public static INESCart LoadROM(IPPU ppu, Stream zipStream)
        {
            INESCart _cart = null;
            byte[] iNesHeader = new byte[16];
            int bytesRead = zipStream.Read(iNesHeader, 0, 16);
            /*
 .NES file format
---------------------------------------------------------------------------
0-3      String "NES^Z" used to recognize .NES files.
4        Number of 16kB ROM banks.
5        Number of 8kB VROM banks.
6        bit 0     1 for vertical mirroring, 0 for horizontal mirroring
         bit 1     1 for battery-backed RAM at $6000-$7FFF
         bit 2     1 for a 512-byte trainer at $7000-$71FF
         bit 3     1 for a four-screen VRAM layout 
         bit 4-7   Four lower bits of ROM Mapper Type.
7        bit 0-3   Reserved, must be zeroes!
         bit 4-7   Four higher bits of ROM Mapper Type.
8-15     Reserved, must be zeroes!
16-...   ROM banks, in ascending order. If a trainer is present, its
         512 bytes precede the ROM bank contents.
...-EOF  VROM banks, in ascending order.
---------------------------------------------------------------------------
*/
            int mapperId = (iNesHeader[6] & 0xF0);
            mapperId = mapperId / 16;
            mapperId += iNesHeader[7];

            int prgRomCount = iNesHeader[4];
            int chrRomCount = iNesHeader[5];

            byte[] theRom = new byte[prgRomCount * 0x4000];
            byte[] chrRom = new byte[chrRomCount * 0x4000];

            int chrOffset = 0;

            bytesRead = zipStream.Read(theRom, 0, theRom.Length);
            chrOffset = (int)zipStream.Position;
            bytesRead = zipStream.Read(chrRom, 0, chrRom.Length);
            Console.WriteLine("{0} chr rom bytes read", bytesRead);
            switch (mapperId)
            {
                case 0:
                case 2:
                case 3:
                case 7:
                    _cart = new NESCart();

                    break;
                case 1:
                    _cart = new NesCartMMC1();
                    break;
                case 4:
                    _cart = new NesCartMMC3();
                    
                    break;
            }

            if (_cart != null)
            {
                _cart.Whizzler = ppu;
                ppu.ChrRomHandler = _cart;
                _cart.ROMHashFunction = Hashers.HashFunction;
                _cart.LoadiNESCart(iNesHeader, prgRomCount, chrRomCount, theRom, chrRom, chrOffset);
            }

            return _cart;

        }

        //private static INESCart LoadNSF(Stream zipStream)
        //{
        //    INESCart _cart = null;
        //    byte[] iNesHeader = new byte[0x7F];
        //    int bytesRead = zipStream.Read(iNesHeader, 0, 0x7F);

        //    byte[] theRom = new byte[zipStream.Length - 0x7F];


        //    bytesRead = zipStream.Read(theRom, 0, theRom.Length);

        //    _cart = new NSFCart();
        //    _cart.LoadiNESCart(iNesHeader, 0, 0, theRom, null, 0);
            
        //    if (_cart != null)
        //    {
        //        _cart.Whizzler = null;
        //        _cart.ROMHashFunction = Hashers.HashFunction;
        //    }

        //    return _cart;

        //}



    }
}
