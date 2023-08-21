using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VM
{
    internal static class ArithmeticHelper
    {
        private static readonly VMContext Context = new VMContext();

        public static nint VAdd(this nint number1, nint number2)
        {
            if(number1 == 0 || number2 == 0)
            {
                if(number1 == 0)
                    return number2;
                
                if(number2 == 0)
                    return number1;
            }

            unsafe
            {
                nint[] addInstructions = new nint[]
                {
                    VMMnemonic.VMOV, VMMnemonic.VAX, number1,           //Move number1 into VAX
                    VMMnemonic.VMOV, VMMnemonic.VBX, number2,           //Move number2 into VBX

                    VMMnemonic.VADD, VMMnemonic.VAX, VMMnemonic.VBX     //Add VAX and VBX
                };

                Context.Initialize(ref addInstructions);
                Context.Enter();

                nint returnVal = Context.ReturnValue;

                Context.Exit();
                return returnVal;
            }
        }

        public static nint VSubtract(this nint number1, nint number2)
        {
            if(number2 == 0)
                return number1;

            unsafe
            {
                nint[] subInstructions = new nint[]
                {
                    VMMnemonic.VMOV, VMMnemonic.VAX, number1,           //Move number1 into VAX
                    VMMnemonic.VMOV, VMMnemonic.VBX, number2,           //Move number2 into VBX

                    VMMnemonic.VSUB, VMMnemonic.VAX, VMMnemonic.VBX     //Subtract VAX to VBX
                };

                Context.Initialize(ref subInstructions);
                Context.Enter();

                nint returnVal = Context.ReturnValue;

                Context.Exit();
                return returnVal;
            }
        }

        public static nint VMultiply(this nint number1, nint number2)
        {
            if(number1 == 0 || number2 == 0)
                return 0;

            unsafe
            {
                nint[] multiInstructions = new nint[]
                {
                    VMMnemonic.VMOV, VMMnemonic.VAX, number1,           //Move number1 into VAX
                    VMMnemonic.VMOV, VMMnemonic.VBX, number2,           //Move number2 into VBX

                    VMMnemonic.VMULT, VMMnemonic.VAX, VMMnemonic.VBX     //Multiply VAX to VBX
                };

                Context.Initialize(ref multiInstructions);
                Context.Enter();

                nint returnVal = Context.ReturnValue;
                
                Context.Exit();
                return returnVal;
            }
        }

        public static nint VDivide(this nint number1, nint number2)
        {
            if(number1 == 0 || number2 == 0)
                throw new Exception("Cannot divide by zero.");

            unsafe
            {
                nint[] divInstructions = new nint[]
                {
                    VMMnemonic.VMOV, VMMnemonic.VAX, number1,           //Move number1 into VAX
                    VMMnemonic.VMOV, VMMnemonic.VBX, number2,           //Move number2 into VBX

                    VMMnemonic.VDIV, VMMnemonic.VAX, VMMnemonic.VBX     //Divide VAX to VBX
                };

                Context.Initialize(ref divInstructions);
                Context.Enter();

                nint returnVal = Context.ReturnValue;

                Context.Exit();
                return returnVal;
            }
        }
    }
}
