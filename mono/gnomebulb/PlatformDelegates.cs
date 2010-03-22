using FishBulb;
using System;
using System.IO;

namespace GtkNes
{
	
	
	public class PlatformDelegates : IPlatformDelegates
	{
		
		public PlatformDelegates()
		{
		}
		
		public event EventHandler<RomLoadedEventArgs> RomLoadedEvent;

        public string BrowseForFile(string defaultExt, string filter)
		{
			return null;	
		}
        
		public System.IO.Stream LoadFile(string filelocation)
		{
			Stream s = null;
			try{
				s = File.Open(filelocation, FileMode.Open);
			} catch{}
			return s;
		}
		
        public byte[] ReadSRAM(string romID)
		{
			return new byte[4000];
		}
        public void WriteSRAM(string romID, byte[] sram)
		{}

	}
}
