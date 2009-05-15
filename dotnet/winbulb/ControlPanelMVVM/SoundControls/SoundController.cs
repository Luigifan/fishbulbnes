using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo;
using NES.CPU.Machine.BeepsBoops;
using Microsoft.Win32;
using WPFamicom.Configuration;
using System.IO;
using System.ComponentModel;
using VMCommanding;
using System.Windows.Input;
using WPFamicom.Sound;

namespace WPFamicom.ControlPanelMVVM.SoundUI
{
    /// <summary>
    /// something for wpf to bind to
    /// </summary>
    public class SoundController : INotifyPropertyChanged, IDisposable, ICommandSink
    {

        private readonly IAPU apu;
        private readonly NESMachine nes;
        public static readonly RoutedCommand SaveWAVFileCommand = new RoutedCommand();
        IWavStreamer streamer;
        public SoundController(NESMachine nes, IWavStreamer streamer)
        {
            this.streamer = streamer;
            this.apu = nes.SoundBopper;
            this.nes = nes;
            commandSink.RegisterCommand(SaveWAVFileCommand,
                        param => nes.EnableSound,
                        parm => WriteWAVs());

        }

        public void WriteWAVs()
        {
            if (IsWritingWavFile)
            {
                StopWritingWAVFile();
            }
            else
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = "*.mp3"; // Default file extension
                dlg.Filter = "MP3 Files (*.mp3)|*.mp3|WAV Files (*.wav)|*.wav;"; // Filter files by extension
                dlg.InitialDirectory = NESConfigManager.LastWAVFolder;
                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                    StartWAVWrite(filename);
                    NESConfigManager.LastWAVFolder =
                        Directory.GetParent(dlg.FileName).FullName;
                }
            }
        }

        bool isWritingWavFile;

        public bool IsWritingWavFile
        {
            get { return isWritingWavFile; }
            set { isWritingWavFile = value; }
        }

        IWavWriter saver;
        public void StartWAVWrite(string fileName)
        {
            // nes.ThreadStoptendo();
            if (saver != null)
            {
                saver.Dispose();
            }

            if (fileName.ToLower().EndsWith("mp3"))
            {
                saver = new Mp3FileSaver(fileName, (int)44100, 16, 1);
            }
            else
            {
                saver = new WaveFileSaver((int)44100, fileName);
            }
            nes.WriteWAVToFile(saver);
            // nes.ThreadRuntendo();
            isWritingWavFile = true;
            NotifyPropertyChanged("IsWritingWavFile");
        }

        public void StopWritingWAVFile()
        {

            isWritingWavFile = false;
            nes.StopWritingWAV();
            saver = null;
            NotifyPropertyChanged("IsWritingWavFile");
        }


        public bool EnableSound
        {
            get
            {
                return !apu.Muted;
            }
            set
            {
                apu.Muted = !value;
            }
        }
        
        public bool EnableSquareChannel0
        {
            get { return apu.EnableSquare0; }
            set { 
                apu.EnableSquare0 = value; 
            }
        }

        public bool EnableSquareChannel1
        {
            get { return apu.EnableSquare1; }
            set { apu.EnableSquare1 = value; }
        }

        public bool EnableTriangleChannel
        {
            get { return apu.EnableTriangle; }
            set { apu.EnableTriangle = value; }
        }

        public bool EnableNoiseChannel
        {
            get { return apu.EnableNoise; }
            set { apu.EnableNoise = value; }
        }

        private int volume;

        public float Volume
        {
            get { return streamer.Volume; }
            set {

                streamer.Volume = value; 
            }
        }


        public void Dispose()
        {

            if (saver != null)
                saver.Dispose();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        #region ICommandSink Members

        private CommandSink commandSink = new CommandSink();

        public bool CanExecuteCommand(System.Windows.Input.ICommand command, object parameter, out bool handled)
        {
            return commandSink.CanExecuteCommand(command, parameter, out handled);
        }

        public void ExecuteCommand(System.Windows.Input.ICommand command, object parameter, out bool handled)
        {
            commandSink.ExecuteCommand(command, parameter, out handled);
        }

        #endregion
    }
}
