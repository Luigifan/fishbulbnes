using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SlimDXBindings.Viewer10;
using Microsoft.Practices.Unity;
using InstibulbWpfUI;
using NES.CPU.nitenedo;
using NES.CPU.Machine;
using SlimDXBindings;
using InstiBulb;
using Fishbulb.Common.UI;
using NES.CPU.Unity;
using fishbulbcommonui.Unity;
using NES.CPU.PPUClasses;
using NES.CPU.PixelWhizzlerClasses;
using NES.CPU.nitenedo.Interaction;
using NES.Sound;

namespace _10NES
{
    static class Program
    {

        static PlatformDelegates delegates;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IUnityContainer container = MakeNESContainer();
            D3D10Host host = container.Resolve<D3D10Host>();
            System.Windows.Application application = new System.Windows.Application();
            host.Container = container;
            host.QuadUp();
            Application.Run();
        }

        static IUnityContainer MakeNESContainer()
        {
            delegates = new PlatformDelegates();


            IUnityContainer container = new UnityContainer()
                .RegisterWpfUITypes()
                .RegisterNESTypes()
                .RegisterCommonUITypes()
                ;
            // whats left is to setup the delegates for file handling, configure the nes machine and go

            container.RegisterInstance<GetFileDelegate>(delegates.BrowseForFile, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMWriterDelegate>(delegates.WriteSRAM, new ContainerControlledLifetimeManager());
            container.RegisterInstance<SRAMReaderDelegate>(delegates.ReadSRAM, new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>().ConfigureInjectionFor<NESMachine>(
                new InjectionProperty("PadOne", new ResolvedParameter<IControlPad>("keyboard")),
                new InjectionProperty("PadTwo", new ResolvedParameter<IControlPad>())
            );

            // set up a whizzlar
            container.RegisterType<HardWhizzler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPPU, HardWhizzler>(new ContainerControlledLifetimeManager());

            // set up some beeps
            container.RegisterType<InlineWavStreamer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWavStreamer, InlineWavStreamer>();

            // register control pads and zappers
            container.RegisterType<SlimDXKeyboardControlPad>(new ContainerControlledLifetimeManager());
            container.RegisterType<IControlPad, SlimDXKeyboardControlPad>("keyboard", new ContainerControlledLifetimeManager());
            container.RegisterType<IControlPad, SlimDXZapper>("zapper", new ContainerControlledLifetimeManager());

            return container;
        }

    }
}
