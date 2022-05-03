using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZVM
{
    internal enum Instructions : int
    {
        /// <summary>
        /// Virtual Add instruction: Adds two numbers and pushes the result into the source number.
        /// </summary>
        VADD = 0,
        /// <summary>
        /// Virtual Subtract instruction: Substracts two numbers and pushes the result into the source number.
        /// </summary>
        VSUB = 1,
        /// <summary>
        /// Virtual Multiply instruction: Multiplies two numbers and pushes the result into the source number.
        /// </summary>
        VMUL = 2,
        /// <summary>
        /// Virtual Divide instruction: Divides two numbers and pushes the result into the source number.
        /// </summary>
        VDIV = 3,
        
        /// <summary>
        /// Virtual Push instruction: Pushes a value onto the stack or into a register.
        /// </summary>
        VPUSH = 4,
        /// <summary>
        /// Virtual Pop instruction: Pops a value off of the stack or off of a register.
        /// </summary>
        VPOP = 5,
        /// <summary>
        /// Virtual Move instruction: Moves the source value into the destination register.
        /// </summary>
        VMOV = 6,
        /// <summary>
        /// Virtual Return instruction: Returns a value from a register or from the stack.
        /// </summary>
        VRET = 7,
        /// <summary>
        /// Virtual Call instruction: Calls a subroutine by the instruction address.
        /// </summary>
        VCALL = 8,

        /// <summary>
        /// Virtual Compare instruction: Compares two values by subtracting both, and returns true/false bit.
        /// </summary>
        VCMP = 9,
        /// <summary>
        /// Virtual Jump instruction: Jumps immediately to the destination instruction address.
        /// </summary>
        VJMP = 10,
        /// <summary>
        /// Virtual Jump If Equal instruction: Jumps to the destination instruction address after verifying the previous instruction(VCMP) returned true bit value.
        /// </summary>
        VJIE = 11,

        /// <summary>
        /// Virtual Exit instruction: Exits immediately out of the execution context.
        /// </summary>
        VEXIT = 12
    }
}
