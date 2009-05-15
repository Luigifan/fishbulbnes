using System;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.BeepsBoops;
using GtkNes;
using WPFamicom.Sound;

namespace GtkNes
{
	
	
	public class SoundViewModel : IProfileViewModel
	{
		NESMachine nes;
		public SoundViewModel(NESMachine nes, IWavStreamer streamer)
		{
			this.nes = nes;
			
		}
	}
}
