using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using SlimDXBindings;
using NES.Sound;
using NES.CPU.Machine.BeepsBoops;
using NES.CPU.PPUClasses;
using NES.CPU.Fastendo;
using NES.CPU.Machine;
using NES.CPU.nitenedo;
using Fishbulb.Common.UI;
using GtkNes;
using InstiBulb.WinViewModels;
using InstiBulb.WpfKeyboardInput;
using SlimDXNESViewer;
using NES.CPU.nitenedo.Interaction;
using WpfNESViewer;
using InstiBulb.Sound;
using InstiBulb.Views;
using fishbulbcommonui.SaveStates;
using SlimDXBindings.Viewer10;
using NES.CPU.PixelWhizzlerClasses;

namespace InstiBulb.Integration
{
    public class NesContainerFactory 
    {
        PlatformDelegates delegates = new PlatformDelegates();

        public IUnityContainer RegisterNESCommon(IUnityContainer container)
        {

            container.RegisterType<WavSharer>(new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<WavSharer>(new InjectionConstructor((float)44100.0));
            container.RegisterType<IWavReader, WavSharer>();

            // the component that creates the sound thread
            container.RegisterType<SoundThreader>(new ContainerControlledLifetimeManager());

            container.RegisterType<InputHandler>(new ContainerControlledLifetimeManager());


            container.RegisterType<CPU2A03>(new ContainerControlledLifetimeManager());

            container.RegisterInstance<GetFileDelegate>(delegates.BrowseForFile, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMWriterDelegate>(delegates.WriteSRAM, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMReaderDelegate>(delegates.ReadSRAM, new ContainerControlledLifetimeManager());

            container.RegisterType<NESMachine>(new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>().ConfigureInjectionFor<NESMachine>(
                    new InjectionProperty("SRAMWriter", new ResolvedParameter<SRAMWriterDelegate>()),
                    new InjectionProperty("SRAMReader", new ResolvedParameter<SRAMReaderDelegate>())
                );

            // Setup a TileDoodler (used by the debugger)
            container.RegisterType<TileDoodler>(new ContainerControlledLifetimeManager());
            container.RegisterType<Bopper>(new ContainerControlledLifetimeManager());


            return container;
        }

        public IUnityContainer RegisterHardwareNES(IUnityContainer container)
        {
            container.RegisterType<HardWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPPU, HardWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDisplayContext, DirectX10NesViewer>(new ContainerControlledLifetimeManager());

            return container;
        }

        public IUnityContainer RegisterSoftwareNES(IUnityContainer container)
        {
            container.RegisterType<PixelWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPPU, PixelWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDisplayContext, WPFNesViewer>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IDisplayContext, SlimDXNesViewer>(new ContainerControlledLifetimeManager());

            return container;
        }


        public IUnityContainer RegisterNesTypes(IUnityContainer container)
        {

            RegisterNESCommon(container);
            RegisterHardwareNES(container);
            //RegisterSoftwareNES(container);
            
            // register types needed to build a NES
            // platform specific wavestreamer
            container.RegisterType<InlineWavStreamer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWavStreamer, InlineWavStreamer>();
            // Select and setup the default PPU engine


            //container.RegisterType<WpfKeyboardControlPad>(new ContainerControlledLifetimeManager()
            //    , new InjectionProperty("Handler", new ResolvedParameter<MainWindow>() ));

            container.RegisterType<SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());
            container.RegisterType<IControlPad, SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IControlPad, SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());


            // register view models
            container.RegisterType<ControlPanelVM>();
            container.RegisterType<SoundViewModel>();
            container.RegisterType<WinCheatPanelVM>();
            container.RegisterType<WinDebuggerVM>();
            container.RegisterType<IViewModel, SoundViewModel>("SoundVM", new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModel, ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModel, WinCheatPanelVM>("CheatVM", new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModel, WinDebuggerVM>("DebuggerVM", new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModel, SaveStateVM>("SaveStateVM", new ContainerControlledLifetimeManager());
            
            container.RegisterType<WpfKeyConfigVM>("KeyConfigVM", new ContainerControlledLifetimeManager());

            // register views
            container.RegisterType<NESDisplay>(new ContainerControlledLifetimeManager(),
                new InjectionProperty("Target", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Context", new ResolvedParameter<IDisplayContext>())

                );


            container.RegisterType<ControlPanelView>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "ControlPanel")));
            container.RegisterType<SoundPanelView>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "SoundVM")));
            container.RegisterType<CheatControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "CheatVM")));
            container.RegisterType<CartInfoPanel>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "ControlPanel")));
            container.RegisterType<MachineStatus>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));    
            container.RegisterType<ControllerConfig>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(WpfKeyConfigVM), "KeyConfigVM")));
            container.RegisterType<InstructionRolloutControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            container.RegisterType<InstructionHistoryControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            container.RegisterType<NameTableViewerControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            container.RegisterType<PatternViewerControl>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));
            container.RegisterType<SaveStateView>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "SaveStateVM")));
            container.RegisterType<TileEditor>(new InjectionProperty("DataContext", new ResolvedParameter(typeof(IViewModel), "DebuggerVM")));

            return container;
        }
    }
}
