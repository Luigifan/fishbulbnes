using System;
using System.IO;

namespace FishBulb
{
    public class RomLoadedEventArgs : EventArgs
    {

        Stream stream;
        string errorMessage;
        public RomLoadedEventArgs(Stream s, string errorMessage)
            : base()
        {
            this.stream = s;
            this.errorMessage = errorMessage;
        }

        public Stream ResultStream
        {
            get { return stream; }
        }
    }

    public interface IPlatformDelegates
    {
        event EventHandler<RomLoadedEventArgs> RomLoadedEvent;

        string BrowseForFile(string defaultExt, string filter);
        System.IO.Stream LoadFile(string filelocation);
        byte[] ReadSRAM(string romID);
        void WriteSRAM(string romID, byte[] sram);
    }
}
