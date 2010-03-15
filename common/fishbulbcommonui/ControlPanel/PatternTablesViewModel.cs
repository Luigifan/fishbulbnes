using System;
using System.Net;

using Fishbulb.Common.UI;
using fishbulbcommonui;
using FishBulb;

namespace SilverlightCommonUI.ControlPanel
{
    public class PatternTablesViewModel : BaseNESViewModel
    {

        public PatternTablesViewModel(IPlatformDelegates delegates) : base(delegates) 
        {

            Commands.Add("RefreshPatternTable",
                new InstigatorCommand(new CommandExecuteHandler(o => RefreshPatternTables(o as string)),
                    p => true));
            Commands.Add("RefreshNameTable",
                new InstigatorCommand(new CommandExecuteHandler(o => RefreshNameTables(o as string)),
                    p => true));

            _patternTables[0] = new int[128 * 128];
            _patternTables[1] = new int[128 * 128];
            _nameTables[0] = new int[256 * 256];
            _nameTables[1] = new int[256 * 256];
            _nameTables[2] = new int[256 * 256];
            _nameTables[3] = new int[256 * 256];
        }

        protected override void OnAttachTarget()
        {
            TargetMachine.DebugInfoChanged += new EventHandler<NES.CPU.Machine.FastendoDebugging.BreakEventArgs>(TargetMachine_DebugInfoChanged);
        }

        protected override void OnDetachTarget()
        {
            TargetMachine.DebugInfoChanged -= TargetMachine_DebugInfoChanged;
        }


        void TargetMachine_DebugInfoChanged(object sender, NES.CPU.Machine.FastendoDebugging.BreakEventArgs e)
        {
            RefreshPatternTables(null);
        }

        int[][] _patternTables = new int[2][];

        public int[][] PatternTables
        {
            get { return _patternTables; }
            set { _patternTables = value; }
        }

        void RefreshPatternTables(string table)
        {
            _patternTables[0] = TargetMachine.Tiler.DoodlePatternTable(0);
            _patternTables[1] = TargetMachine.Tiler.DoodlePatternTable(0x1000);
            TargetMachine.PPU.SetupBufferForDisplay(ref _patternTables[0]);
            TargetMachine.PPU.SetupBufferForDisplay(ref _patternTables[1]);

            NotifyPropertyChanged("PatternTables");
        }

        int[][] _nameTables = new int[4][];

        public int[][] NameTables
        {
            get { return _nameTables; }
            set { _nameTables = value; }
        }

        void RefreshNameTables(string table)
        {
            _nameTables[0] = TargetMachine.Tiler.DoodleNameTable(0);
            _nameTables[1] = TargetMachine.Tiler.DoodleNameTable(0x400);
            _nameTables[2] = TargetMachine.Tiler.DoodleNameTable(0x800);
            _nameTables[3] = TargetMachine.Tiler.DoodleNameTable(0xc00);

            TargetMachine.PPU.SetupBufferForDisplay(ref _nameTables[0]);
            TargetMachine.PPU.SetupBufferForDisplay(ref _nameTables[1]);
            TargetMachine.PPU.SetupBufferForDisplay(ref _nameTables[2]);
            TargetMachine.PPU.SetupBufferForDisplay(ref _nameTables[3]);
            // NES.CPU.PPUClasses.PixelWhizzler.GetPalABGR();

            NotifyPropertyChanged("PatternTables");
        }


        public override string CurrentView
        {
            get { return "PatternViewer"; }
        }

        public override string CurrentRegion
        {
            get { return null; }
        }

        public override string Header
        {
            get { return "Pattern Tables"; }
        }
    }
}
