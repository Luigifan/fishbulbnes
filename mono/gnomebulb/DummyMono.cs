using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//this whole thing is just to compile on windows without including 
// Mono.Unix.  Stetic likes this stuff by default 
// If you can't beat 'em join em
namespace Mono.Unix
{
	static class Catalog
	{
        public static string GetString(string s)
        {
            return s;
        }
	}
}
