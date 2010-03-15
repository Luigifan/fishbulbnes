using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Threading;
using FishBulb;
using System.Windows.Threading;

namespace SilverBulb
{
    public delegate void CommandExecuteHandler(object parm);
    public delegate bool CommandCanExecuteHandler(object parm);
    public delegate string GetFileDelegate(string defaultExt, string Filter);



    public class PlatformDelegates : IPlatformDelegates
    {


        public event EventHandler<RomLoadedEventArgs> RomLoadedEvent;

        public PlatformDelegates(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public string BrowseForFile(string defaultExt, string filter)
        {
            return null;
        }

        Dispatcher dispatcher;

        public Dispatcher Dispatcher
        {
            get { return dispatcher; }
            set { dispatcher = value; }
        }

        AutoResetEvent loadBlock = new AutoResetEvent(false);
        
        Stream stream;

        public Stream LoadFile(string filelocation)
        {

            if (stream != null) stream.Dispose();


            GetRomStream(filelocation);

            // thread.Join();

            return stream;
        }

        Exception lastException;

        private void GetRomStream(string filelocation)
        {
            bool loadCompleted = false;
            WebClient client = new WebClient();

            client.OpenReadCompleted +=
                delegate(object o, OpenReadCompletedEventArgs e)
                {
                    if (e.Error == null)
                        stream = e.Result;
                    else
                        stream = null;

                    loadCompleted = true;
                    if (RomLoadedEvent != null)
                        RomLoadedEvent(this, new RomLoadedEventArgs(stream, null));
                };
                
            UriKind kind = UriKind.Relative;

            try
            {
                loadCompleted = false;

                client.OpenReadAsync(new Uri(filelocation, kind));

            }
            catch (Exception e)
            {
                lastException = e;
            }
            finally
            {
                client = null;

            }
        }


        public void WriteSRAM(string romID, byte[] sram)
        {

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.DirectoryExists(romID))
                    store.CreateDirectory(romID);

                string fileName = romID + "\\SRAM";

                using (IsolatedStorageFileStream file = store.OpenFile(fileName, System.IO.FileMode.OpenOrCreate))
                {
                    file.Write(sram, 0, sram.Length);
                }
                
            }
        }

        public byte[] ReadSRAM(string romID)
        {

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string fileName = romID + "\\SRAM";
                if (store.DirectoryExists(romID) && store.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream file = store.OpenFile(fileName, System.IO.FileMode.Open))
                    {
                        byte[] ret = new byte[file.Length ];

                        file.Read(ret, 0, ret.Length);
                        return ret;
                    }
                }

                return new byte[0x4000];
            }
        }

    }
}
