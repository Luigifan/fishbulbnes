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
using WpfNESViewer;
using InstiBulb.Views;
using fishbulbcommonui.SaveStates;
using NES.CPU.PixelWhizzlerClasses;
using SlimDXBindings;
using NES.CPU.PixelWhizzlerClasses.SoftWhizzler;
using SlimDXNESViewer;
using fishbulbcommonui;

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

            //container.RegisterType<InputHandler>(new ContainerControlledLifetimeManager());


            container.RegisterType<CPU2A03>(new ContainerControlledLifetimeManager());



            container.RegisterInstance<GetFileDelegate>(delegates.BrowseForFile, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMWriterDelegate>(delegates.WriteSRAM, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMReaderDelegate>(delegates.ReadSRAM, new ContainerControlledLifetimeManager());

            container.RegisterType<NESMachine>(new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<NESMachine>(
                new InjectionProperty("PadOne", new ResolvedParameter<IControlPad>("keyboard")),
                new InjectionProperty("PadTwo", new ResolvedParameter<SlimDXZapper>("zapper"))
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

        public IUnityContainer RegisterHardwareNES(IUnityContainer container)
        {
            container.RegisterType<HardWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPPU, HardWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDisplayContext, SlimDXNesViewer>(new ContainerControlledLifetimeManager());
            

            return container;
        }

        public IUnityContainer RegisterSoftwareNES(IUnityContainer container)
        {
            container.RegisterType<SoftWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPPU, SoftWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDisplayContext, WPFNesViewer>(new ContainerControlledLifetimeManager());

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
            container.RegisterType<IControlPad, SlimDXKeyboardControlPad>("keyboard", new ContainerControlledLifetimeManager());
            container.RegisterType<IControlPad, SlimDXZapper>("zapper", new ContainerControlledLifetimeManager());


            //// register view models

            container.RegisterType<IViewModel, SoundViewModel>("SoundVM", new ContainerControlledLifetimeManager(), 
                new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, WinCheatPanelVM>("CheatVM", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, WinDebuggerVM>("DebuggerVM", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            container.RegisterType<IViewModel, SaveStateVM>("SaveStateVM", new ContainerControlledLifetimeManager(), new InjectionProperty("TargetMachine", new ResolvedParameter<NESMachine>()));
            

            //container.RegisterType<WpfKeyConfigVM>("KeyConfigVM", new ContainerControlledLifetimeManager());
            // register views
            container.RegisterType<NESDisplay>(new ContainerControlledLifetimeManager(),
                new InjectionProperty("Target", new ResolvedParameter<NESMachine>()),
                new InjectionProperty("Context", new ResolvedParameter<IDisplayContext>())

                );

            InstibulbWpfUI.TypeRegisterer.RegisterWpfUITypes(container);

            return container;
        }
    }
}
