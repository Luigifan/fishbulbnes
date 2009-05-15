using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace NES.CPU.Machine.ROMLoader
{
    public static class Hashers
    {
        public static string HashFunction(byte[] nesCart, byte[] chrRom)
        {
            MD5 md5 = MD5.Create();

            byte[] data = (from b in nesCart
                           select b).Union(
                       from c in chrRom select c).ToArray<byte>();

            byte[] hash = md5.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            string s = sb.ToString();
            return s;
        }
    }
}
