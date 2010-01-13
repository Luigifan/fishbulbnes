using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using NES.CPU.Fastendo;
using NES.Sound;
using NES.CPU.Machine.BeepsBoops;
using NES.CPU.nitenedo;
using NES.CPU.Machine;
using NES.CPU.PPUClasses;

namespace NES.CPU.Unity
{
    public static class NESTypeRegisterer
    {
        public static IUnityContainer RegisterNESTypes(this IUnityContainer container)
        {

            container.RegisterType<WavSharer>(new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<WavSharer>(new InjectionConstructor((float)44100.0));
            container.RegisterType<IWavReader, WavSharer>();

            // the component that creates the sound thread
            container.RegisterType<SoundThreader>(new ContainerControlledLifetimeManager());
            container.RegisterType<CPU2A03>(new ContainerControlledLifetimeManager());
            container.RegisterType<NESMachine>(new ContainerControlledLifetimeManager());
            container.Configure<InjectedMembers>().ConfigureInjectionFor<NESMachine>(
                    new InjectionProperty("SRAMWriter", new ResolvedParameter<SRAMWriterDelegate>()),
                    new InjectionProperty("SRAMReader", new ResolvedParameter<SRAMReaderDelegate>())
                );

            // Setup a TileDoodler (used by the debugger)
            container.RegisterType<TileDoodler>(new ContainerControlledLifetimeManager());
            container.RegisterType<Bopper>(new ContainerControlledLifetimeManager());

            container.RegisterType<IControlPad, NullControlPad>();

            return container;
        }
    }
}
