using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZVM
{
    internal class Registers
    {
        internal Registers()
        {
            VIP = 0;
            VSP = -1;
            VFP = -1;
        }

        /// <summary>
        /// Virtual Accumulator Register: Used for storing arithmetic operation and return values
        /// </summary>
        public int VAX = -1;
        /// <summary>
        /// Virtual Counter Register: Used for storing loop index. Otherwise, acts as a secondary purpose register.
        /// </summary>
        public int VCX = -2;
        /// <summary>
        /// Virtual Data Register: Used in conjuction with VAX.
        /// </summary>
        public int VDX = -3;
        /// <summary>
        /// Virtual Index Register: Used for storing a memory address.
        /// </summary>
        public int VIX = -4;

        /// <summary>
        /// Virtual Source Address: Used for storing source address.
        /// </summary>
        public int VSA = -5;
        /// <summary>
        /// Virtual Destination Address: Used for storing destination address.
        /// </summary>
        public int VDA = -6;

        /// <summary>
        /// Virtual Stack Pointer: Used for storing a stack pointer.
        /// </summary>
        public int VSP = -7;
        /// <summary>
        /// Virual Frame Pointer: Used for storing a frame pointer.
        /// </summary>
        public int VFP = -8;
        /// <summary>
        /// Virtual Instruction Pointer: Used for storing an instruction pointer.
        /// </summary>
        public int VIP = -9;
    }
}
