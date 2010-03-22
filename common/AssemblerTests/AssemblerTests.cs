using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FishbulbAssembler;

namespace AssemblerTests
{
    [TestClass]
    public class AssemblerTests
    {
        [TestMethod]
        public void AssemblerShouldDecodeImmediateAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("LDA #$04");
            bld.AppendLine("LDX #$04");
            bld.AppendLine("LDY #$04");
            bld.AppendLine("ADC #$44");
            bld.AppendLine("AND #$44");
            bld.AppendLine("CMP #$44");
            bld.AppendLine("CPX #$44");
            bld.AppendLine("CPY #$44");
            bld.AppendLine("EOR #$44");
            bld.AppendLine("ORA #$44");
            bld.AppendLine("SBC #$44");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 169, 4 }, assembler.Output[0]);
            CompareByteArrays(new byte[] { 162, 4 }, assembler.Output[1]);
            CompareByteArrays(new byte[] { 160, 4 }, assembler.Output[2]);
            CompareByteArrays(new byte[] { 0x69, 0x44 }, assembler.Output[3]);
            CompareByteArrays(new byte[] { 0x29, 0x44 }, assembler.Output[4]);
            CompareByteArrays(new byte[] { 0xC9, 0x44 }, assembler.Output[5]);
            CompareByteArrays(new byte[] { 0xE0, 0x44 }, assembler.Output[6]);
            CompareByteArrays(new byte[] { 0xC0, 0x44 }, assembler.Output[7]);
            CompareByteArrays(new byte[] { 0x49, 0x44 }, assembler.Output[8]);
            CompareByteArrays(new byte[] { 0x09, 0x44 }, assembler.Output[9]);

            CompareByteArrays(new byte[] { 0xE9, 0x44 }, assembler.Output[10]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeImplicitAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("BRK");
            bld.AppendLine("NOP");
            bld.AppendLine("PHA");
            bld.AppendLine("PHP");
            bld.AppendLine("PLA");
            bld.AppendLine("PLP");
            bld.AppendLine("RTS");
            bld.AppendLine("SEC");
            bld.AppendLine("SED");
            bld.AppendLine("TAX");
            bld.AppendLine("TAY");
            bld.AppendLine("TSX");
            bld.AppendLine("TXA");
            bld.AppendLine("TXS");
            bld.AppendLine("TYA");
            bld.AppendLine("SEI");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x00 }, assembler.Output[0]);
            CompareByteArrays(new byte[] { 0xEA }, assembler.Output[1]);
            CompareByteArrays(new byte[] { 0x48 }, assembler.Output[2]);
            CompareByteArrays(new byte[] { 0x08 }, assembler.Output[3]);
            CompareByteArrays(new byte[] { 0x68 }, assembler.Output[4]);
            CompareByteArrays(new byte[] { 0x28 }, assembler.Output[5]);
            CompareByteArrays(new byte[] { 0x60 }, assembler.Output[6]);
            CompareByteArrays(new byte[] { 0x38 }, assembler.Output[7]);
            CompareByteArrays(new byte[] { 0xF8 }, assembler.Output[8]);
            CompareByteArrays(new byte[] { 0xAA }, assembler.Output[9]);
            CompareByteArrays(new byte[] { 0xA8 }, assembler.Output[10]);
            CompareByteArrays(new byte[] { 0xBA }, assembler.Output[11]);
            CompareByteArrays(new byte[] { 0x8A }, assembler.Output[12]);
            CompareByteArrays(new byte[] { 0x9A }, assembler.Output[13]);
            CompareByteArrays(new byte[] { 0x98 }, assembler.Output[14]);
            CompareByteArrays(new byte[] { 0x78 }, assembler.Output[15]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeAccumulatorAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ASL A");
            bld.AppendLine("LSR A");
            bld.AppendLine("ROL A");
            bld.AppendLine("ROR A");


            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x0A }, assembler.Output[0]);
            CompareByteArrays(new byte[] { 0x4A }, assembler.Output[1]);
            CompareByteArrays(new byte[] { 0x2A }, assembler.Output[2]);
            CompareByteArrays(new byte[] { 0x6A }, assembler.Output[3]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeZeroPageAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ADC $44");
            bld.AppendLine("AND $44");
            bld.AppendLine("ASL $44");
            bld.AppendLine("BIT $44");
            bld.AppendLine("CMP $44");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x65, 0x44 }, assembler.Output[0]);
            CompareByteArrays(new byte[] { 0x25, 0x44 }, assembler.Output[1]);
            CompareByteArrays(new byte[] { 0x06, 0x44 }, assembler.Output[2]);
            CompareByteArrays(new byte[] { 0x24, 0x44 }, assembler.Output[3]);
            CompareByteArrays(new byte[] { 0xc5, 0x44 }, assembler.Output[4]);

        }

        [TestMethod]
        public void AssemblerShouldDecodeZeroPageXAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ADC $44,X");
            bld.AppendLine("AND $44,X");
            bld.AppendLine("ASL $44,X");
            bld.AppendLine("CMP $44,X");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x75, 0x44 }, assembler.Output[0]);
            CompareByteArrays(new byte[] { 0x35, 0x44 }, assembler.Output[1]);
            CompareByteArrays(new byte[] { 0x16, 0x44 }, assembler.Output[2]);
            CompareByteArrays(new byte[] { 0xD5, 0x44 }, assembler.Output[3]);

        }

        [TestMethod]
        public void AssemblerShouldDecodeZeroPageYAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("LDX $44,Y");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0xB6, 0x44 }, assembler.Output[0]);
        }



        [TestMethod]
        public void AssemblerShouldDecodeAbsoluteAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("LDX $4400");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0xAE, 0x00, 0x44 }, assembler.Output[0]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeAbsoluteYAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("LDX $4400,Y");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0xBE, 0x00, 0x44 }, assembler.Output[0]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeAbsoluteXAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("LDY $4400,X");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0xBC, 0x00, 0x44 }, assembler.Output[0]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeIndirectXAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ADC ($44,X)");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x61, 0x44 }, assembler.Output[0]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeIndirectYAddresses()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ADC ($44),Y");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x71, 0x44 }, assembler.Output[0]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeHexValues()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ADC ($44),Y");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x71, 0x44 }, assembler.Output[0]);
        }

        [TestMethod]
        public void AssemblerShouldDecodeBinaryValues()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ADC (%01000100),Y");
            // make sure it pads zeroes, this should be the same result (0x44)
            bld.AppendLine("ADC (%1000100),Y");
            // should be immediate
            bld.AppendLine ("ADC #%1000100");
            // should be zeropage
            bld.AppendLine("ADC %1000100");
            // should be absolute $4400
            bld.AppendLine("ADC %100010000000000");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x71, 0x44 }, assembler.Output[0]);
            CompareByteArrays(new byte[] { 0x71, 0x44 }, assembler.Output[1]);
            CompareByteArrays(new byte[] { 0x69, 0x44 }, assembler.Output[2]);
            CompareByteArrays(new byte[] { 0x65, 0x44 }, assembler.Output[3]);
            CompareByteArrays(new byte[] { 0x6D, 0x00, 0x44 }, assembler.Output[4]);
        }



        [TestMethod]
        public void AssemblerShouldRecognizeBadCode()
        {
            StringBuilder bld = new StringBuilder();
            // invalid zeropage, x instruction
            bld.AppendLine("BIT $44,X");
        }

        [TestMethod]
        public void AssemblerShouldDecodeWithValidLabels()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("FART ADC (%01000100),Y");
            // make sure it pads zeroes, this should be the same result (0x44)
            bld.AppendLine("LODA ADC (%1000100),Y");
            // should be immediate
            bld.AppendLine("POOF ADC #%1000100");
            // should be zeropage
            bld.AppendLine("SPANG ADC %1000100");
            // should be absolute $4400
            bld.AppendLine("ROG ADC %100010000000000 ; comments");

            Assembler assembler = new Assembler();
            assembler.Text = bld.ToString();
            assembler.Assemble();

            CompareByteArrays(new byte[] { 0x71, 0x44 }, assembler.Output[0]);
            CompareByteArrays(new byte[] { 0x71, 0x44 }, assembler.Output[1]);
            CompareByteArrays(new byte[] { 0x69, 0x44 }, assembler.Output[2]);
            CompareByteArrays(new byte[] { 0x65, 0x44 }, assembler.Output[3]);
            CompareByteArrays(new byte[] { 0x6D, 0x00, 0x44 }, assembler.Output[4]);
        }

        private static void CompareByteArrays(byte[] expected, byte[] actual)
        {
            Assert.AreEqual<int>(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; ++i)
                Assert.AreEqual<byte>(expected[i], actual[i]);
        }
    }
}

