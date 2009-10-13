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

            container.RegisterType<IControlPad, SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());
            container.RegisterType<InputHandler>(new ContainerControlledLifetimeManager());
            container.RegisterType<CPU2A03>(new ContainerControlledLifetimeManager());

            container.RegisterInstance<GetFileDelegate>(delegates.BrowseForFile, new ContainerControlledLifetimeManager());
            
            container.RegisterType<NESMachine>( new ContainerControlledLifetimeManager());

            // register views
            container.RegisterType<ControlPanelVM>();
            container.RegisterType<SoundViewModel>();
            container.RegisterType<CheatPanelVM>();
            container.RegisterType<IViewModel, SoundViewModel>("SoundVM", new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModel, ControlPanelVM>("ControlPanel", new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModel, CheatPanelVM>("CheatVM", new ContainerControlledLifetimeManager());

            return container;
        }
    }
}
