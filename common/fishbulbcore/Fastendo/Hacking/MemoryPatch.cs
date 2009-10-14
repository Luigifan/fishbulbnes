using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Fastendo.Hacking
{
    public interface IMemoryPatch
    {
        bool Activated { get; set; }
        int Address { get; set; }
        int GetData(int data);
    }

    public class MemoryPatch : IMemoryPatch
    {

        int _data;

        public MemoryPatch(int Address, int Data)
        {
            _data = Data;
            this.Address = Address;
        }
        public bool Activated
        {
            get; set;
        }

        public int Address
        {
            get;
            set;
        }

        public int GetData(int data)
        {
            return _data;
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}  Activated: {2}", Address, _data, Activated);
        }

    }

    public class ComparedMemoryPatch : IMemoryPatch
    {
        
        byte _CompareData,_ReplaceData;

        public ComparedMemoryPatch(int Address, byte CompareToData, byte ReplaceWithData)
        {
            _CompareData = CompareToData;
            _ReplaceData = ReplaceWithData;
            this.Address = Address;
        }
        public bool Activated
        {
            get; set;
        }

        public int Address
        {
            get;
            set;
        }

        public int GetData(int data)
        {
            return (data == _CompareData)  ?_ReplaceData : data;
        }

        public override string ToString()
        {
            return string.Format("{0} = {1} if {2} Activated: {3}", Address, _ReplaceData, _CompareData, Activated);
        }

    }
}
