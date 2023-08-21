using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VM
{
    internal class VMContext
    {
        internal static nint[] _Instructions;
        private static VMStack _Stack;

        internal nint[] Instructions => _Instructions;
        internal VMStack Stack => _Stack;
        internal VMRegister Registers;

        internal unsafe nint ReturnValue => Registers.VSP - (nint.Size) > (nint)Stack.InitialStackPointer ? Stack.Pop(ref Registers.VSP) : Registers.VAX;
        
        internal void Initialize(ref nint[] istr)
        {
            _Instructions = istr;
            
            //Clear the trace of the original instruction supplier
            istr = null;

            Registers = new VMRegister();

            _Stack = new VMStack();
            _Stack.Initialize(ref Registers.VSP, (nuint)(Instructions.Length * nint.Size));

            if(Registers.VSP == 0)
                Console.WriteLine("Failed to allocate native stack.");
        }

        /// <summary>
        /// Cache native context and enter into VM context.
        /// </summary>
        internal void Enter()
        {
            Dispatcher.DispatchHandlers(this);
        }

        /// <summary>
        /// Restore native context.
        /// </summary>
        internal void Exit()
        {
            Stack.Release();
            _Instructions = null;
            Registers = new VMRegister();
        }
    }
}
