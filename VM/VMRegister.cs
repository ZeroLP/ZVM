using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM
{
    internal struct VMRegister
    {
        /// <summary>
        /// Virtual Stack Pointer
        /// </summary>
        internal nint VSP;

        /// <summary>
        /// Virtual Instruction Pointer
        /// </summary>
        internal nint VIP;


        /// <summary>
        /// Virtual Accumulator Register
        /// </summary>
        internal nint VAX;

        /// <summary>
        /// Virtual Base Register
        /// </summary>
        internal nint VBX;

        /// <summary>
        /// Virtual Counter Register
        /// </summary>
        internal nint VCX;
    }
}
