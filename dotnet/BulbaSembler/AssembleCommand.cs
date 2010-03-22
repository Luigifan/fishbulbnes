using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using FishbulbAssembler;

namespace BulbaSembler
{
    public class AssembleCommand : ICommand
    {

        public AssembleCommand(Assembler assembler)
        {
            this.assembler = assembler;
        }

        Assembler assembler;

        public bool CanExecute(object parameter)
        {
            return (assembler != null);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            assembler.Assemble();
        }
    }
}
