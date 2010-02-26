using System;
namespace InstiBulb.WpfKeyboardInput
{
    public interface IKeyBindingConfigTarget
    {
        System.Collections.Generic.Dictionary<System.Windows.Input.Key, InstiBulb.WpfKeyboardInput.PadValues> NesKeyBindings { get; set; }
        void SetKeyBinding(InstiBulb.WpfKeyboardInput.NesKeyBinding binding);

    }
}
