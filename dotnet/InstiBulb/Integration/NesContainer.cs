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

namespace InstiBulb.Integration
{
    public class NesContainerFactory 
    {
        PlatformDelegates delegates = new PlatformDelegates();

        public IUnityContainer RegisterNesTypes(IUnityContainer container)
        {

            
            // register types needed to build a NES


            // platform specific wavestreamer
            container.RegisterType<InlineWavStreamer>(new ContainerControlledLifetimeManager());
            // make it default
            container.RegisterType<IWavStreamer, InlineWavStreamer>();
            //container.RegisterType<IWavStreamer, OpenALInlineWavStreamer>();
            // the shared buffer between the IWavStreamer and the NES
            container.RegisterType<WavSharer>(new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<WavSharer>(new InjectionConstructor((float)44100.0));

            container.RegisterType<IWavReader, WavSharer>();
            // the component that creates the sound thread
            container.RegisterType<SoundThreader>(new ContainerControlledLifetimeManager());

            // Select and setup the default PPU engine
            //    TODO: add UnsafePixelWhizzler as an option
            container.RegisterType<PixelWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPPU, PixelWhizzler>(new ContainerControlledLifetimeManager());

            // Setup a TileDoodler (used by the debugger)
            container.RegisterType<TileDoodler>(new ContainerControlledLifetimeManager());
            
            container.RegisterType<Bopper>(new ContainerControlledLifetimeManager());

            container.RegisterType<WpfKeyboardControlPad>(new ContainerControlledLifetimeManager()
                , new InjectionProperty("Handler", new ResolvedParameter<MainWindow>() ));
            container.RegisterType<SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());

            container.RegisterType<IControlPad, WpfKeyboardControlPad>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IControlPad, SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());

            container.RegisterType<InputHandler>(new ContainerControlledLifetimeManager());

            //container.RegisterType<WPFNesViewer>(new ContainerControlledLifetimeManager());
            //container.RegisterType<SlimDXNesViewer>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IDisplayContext, SlimDXNesViewer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDisplayContext, SlimDXNesViewer>(new ContainerControlledLifetimeManager());

            container.RegisterType<CPU2A03>(new ContainerControlledLifetimeManager());

            container.RegisterInstance<GetFileDelegate>(delegates.BrowseForFile, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMWriterDelegate>(delegates.WriteSRAM, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMReaderDelegate>(delegates.ReadSRAM, new ContainerControlledLifetimeManager());
            
            container.RegisterType<NESMachine>( new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<NESMachine>(
                    new InjectionProperty("SRAMWriter", new ResolvedParameter<SRAMWriterDelegate>()),
                    new InjectionProperty("SRAMReader", new ResolvedParameter<SRAMReaderDelegate>())
                );


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

            return container;
        }
    }
}
