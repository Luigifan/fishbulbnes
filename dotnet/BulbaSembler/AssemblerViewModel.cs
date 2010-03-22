using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FishbulbAssembler;
using System.Windows.Input;

namespace BulbaSembler
{
    public class AssemblerViewModel
    {

        public AssemblerViewModel()
        {
            this.Assembler = new Assembler();
            assembleCommand = new AssembleCommand(this.Assembler);
        }

        public Assembler Assembler { get; set; }

        ICommand assembleCommand;

        public ICommand AssembleCommand
        {
            get { return assembleCommand; }
        }
    }
}
