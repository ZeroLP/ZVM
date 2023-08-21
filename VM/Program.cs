using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new VMContext();

            nint[] instructions = new nint[] 
            {
                VMMnemonic.VPUSH, 10,                  //Push 10 onto the stack
                
                VMMnemonic.VMOV, VMMnemonic.VAX, 16,   //Mov 16 into VAX
                VMMnemonic.VADD, VMMnemonic.VAX, 4,    //Add 4 to VAX
                VMMnemonic.VSUB, VMMnemonic.VAX, 1,    //Subtract 1 from VAX

                VMMnemonic.VMULT, VMMnemonic.VAX, 2,   //Multiply 2 with VAX
                VMMnemonic.VDIV, VMMnemonic.VAX, 2,    //Divide 2 with VAX

                VMMnemonic.VPOP, VMMnemonic.VBX,       //Pop the value off of the stack into VBX
                VMMnemonic.VPUSH, 16,                  //Push 16 on to the stack
                VMMnemonic.VPOP, VMMnemonic.VBX        //Pop the stack value into VBX
            };

            context.Initialize(ref instructions);

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            context.Enter();
            sw.Stop();

            Console.WriteLine($"Returned value - Stack/VAX: {context.ReturnValue} | VBX: {context.Registers.VBX} | Took {sw.ElapsedMilliseconds}ms");

            nint firstInstructionsRetVal = context.ReturnValue;
            context.Exit();

            nint[] instructions2 = new nint[]
            {
                VMMnemonic.VPUSH, firstInstructionsRetVal,    //Push firstInstructionsRetVal on to the stack

                VMMnemonic.VPOP, VMMnemonic.VAX,              //Pop the top of the stack into VAX
                VMMnemonic.VADD, VMMnemonic.VAX, 5            //Add 5 into VAX
            };
            context.Initialize(ref instructions2);

            sw.Restart();
            context.Enter();
            sw.Stop();

            Console.WriteLine($"Returned value - Stack/VAX: {context.ReturnValue} | VBX: {context.Registers.VBX} | Took {sw.ElapsedMilliseconds}ms");
            context.Exit();
        }
    }
}