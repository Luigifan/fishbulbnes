using System;
namespace InstiBulb.WpfKeyboardInput
{
    public interface IBindToDisplayContext
    {
        NES.CPU.nitenedo.Interaction.IDisplayContext DisplayContext { get; set; }
    }
}
