using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using NES.Sound;
using NES.CPU.Machine.BeepsBoops;
using NES.CPU.PPUClasses;
using NES.CPU.Fastendo;
using NES.CPU.Machine;
using NES.CPU.nitenedo;
using Fishbulb.Common.UI;
using InstiBulb.WinViewModels;
using InstiBulb.WpfKeyboardInput;
using NES.CPU.nitenedo.Interaction;

using InstiBulb.Views;
using fishbulbcommonui.SaveStates;
using NES.CPU.PixelWhizzlerClasses;
using SlimDXBindings;
using NES.CPU.PixelWhizzlerClasses.SoftWhizzler;
using fishbulbcommonui;
using InstiBulb.WpfNESViewer;
using SlimDXBindings.Viewer10;
using System.Windows;
using FishBulb;
using _10NES2;


namespace InstiBulb.Integration
{
    public class NesContainerFactory 
    {
        PlatformDelegates delegates = new PlatformDelegates();

         IUnityContainer RegisterNESCommon(IUnityContainer container)
        {

            container.RegisterType<WavSharer>(new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<WavSharer>(new InjectionConstructor((float)44100.0));
            container.RegisterType<IWavReader, WavSharer>();

            // the component that creates the sound thread
            container.RegisterType<SoundThreader>(new ContainerControlledLifetimeManager());
            container.RegisterType<CPU2A03>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPlatformDelegates, PlatformDelegates>();
            container.RegisterInstance<GetFileDelegate>(delegates.BrowseForFile, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMWriterDelegate>(delegates.WriteSRAM, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMReaderDelegate>(delegates.ReadSRAM, new ContainerControlledLifetimeManager());

            container.RegisterType<NESMachine>(new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<NESMachine>(
                new InjectionProperty("PadOne", new ResolvedParameter<IControlPad>("padone")),
                new InjectionProperty("PadTwo", new ResolvedParameter<IControlPad>("padtwo"))
            );
            container.Configure<InjectedMembers>().ConfigureInjectionFor<NESMachine>(
                    new InjectionProperty("SRAMWriter", new ResolvedParameter<SRAMWriterDelegate>()),
                    new InjectionProperty("SRAMReader", new ResolvedParameter<SRAMReaderDelegate>())
                );

            // Setup a TileDoodler (used by the debugger)
            container.RegisterType<TileDoodler>(new ContainerControlledLifetimeManager());
            container.RegisterType<Bopper>(new ContainerControlledLifetimeManager());


            return container;
        }

        // IUnityContainer RegisterHardwareNES(IUnityContainer container)
        //{
        //    container.RegisterType<IPPU, NES.CPU.PixelWhizzlerClasses.HardWhizzler>(new ContainerControlledLifetimeManager());
        //    container.RegisterType<IViewModel, D3D10DisplayViewModel>("DisplayViewModel", new ContainerControlledLifetimeManager());
        //    container.RegisterType<IDisplayContext, D3D10NesViewer>(new ContainerControlledLifetimeManager()
        //        , new InjectionProperty("AttachedMachine", new ResolvedParameter<NESMachine>())
        //        , new InjectionProperty("ViewModel", new ResolvedParameter<IViewModel>("DisplayViewModel"))
        //        );

        //    return container;
        //}

         IUnityContainer RegisterSoftwareNES(IUnityContainer container)
        {
            container.RegisterType<IPPU, SoftWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDisplayContext, WPFNesViewer>(new ContainerControlledLifetimeManager()
                , new InjectionProperty("AttachedMachine", new ResolvedParameter<NESMachine>())
                );
            return container;
        }

        public IUnityContainer RegisterNesTypes(IUnityContainer container, string nesType)
        {

            RegisterNESCommon(container);
            
            //if (nesType.ToLower() == "hard")
            //    RegisterHardwareNES(container);
            //else
                RegisterSoftwareNES(container);
            
            // register types needed to build a NES
            // platform specific wavestreamer
            //container.RegisterType<InlineWavStreamer>(new ContainerControlledLifetimeManager());
                container.RegisterType<IWavStreamer, XNAtWavStreamer>(new ContainerControlledLifetimeManager());
            // Select and setup the default PPU engine



            container.RegisterType<IControlPad, WpfKeyboardControlPad>("padone", new ContainerControlledLifetimeManager(),
            new InjectionProperty("Handler", new ResolvedParameter<Window>("MainWindow"))
            );
            
            //container.RegisterType<IControlPad, SlimDXKeyboardControlPad>("padone", new ContainerControlledLifetimeManager()
            //    );
            //container.RegisterType<IKeyBindingConfigTarget, SlimDXKeyboardControlPad>("padone");

            container.RegisterType<IControlPad, NullControlPad>("padtwo", new ContainerControlledLifetimeManager());

            //// register view models
            
            container.RegisterType<IViewModel, SoundViewModel>("SoundPanel", new ContainerControlledLifetimeManager(),  
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Streamer", new ResolvedParameter<IWavStreamer>())
                );


            container.RegisterType<IViewModel, ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, WinCheatPanelVM>("CheatPanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, WinDebuggerVM>("DebugPanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, SaveStateVM>("SaveStatePanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            
            container.RegisterType<IViewModel, WpfKeyConfigVM>("ControllerConfigPanel", new ContainerControlledLifetimeManager()
                , new InjectionConstructor(new ResolvedParameter<IKeyBindingConfigTarget>("padone"), new ResolvedParameter<IPlatformDelegates>())
                , new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>())
                );


            return container;
        }
    }
}
