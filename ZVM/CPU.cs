using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZVM
{
    internal class CPU
    {
        private Registers RegistersInUse = new Registers();

        private object[] StackMemory = new object[1000];

        internal object Execute()
        {
            while (RegistersInUse.VIP < Program.TextSection.Length)
            {
                var currentInstruction = Program.TextSection[RegistersInUse.VIP];
                Console.WriteLine($"[CPU]: -> VIP: {RegistersInUse.VIP} | Instruction: {currentInstruction}");
                RegistersInUse.VIP++;

                switch(currentInstruction)
                {
                    case Instructions.VADD:
                        {
                            var firstVal = Program.TextSection[RegistersInUse.VIP];
                            var secVal = Program.TextSection[++RegistersInUse.VIP];
                       
                            var firstValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)firstVal).FirstOrDefault();
                            if (firstValRegObject != null)
                                firstVal = RegistersInUse.GetType().GetField(firstValRegObject.Name).GetValue(RegistersInUse);
                            
                            var secondValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)secVal).FirstOrDefault();
                            if(secondValRegObject != null)
                                secVal = RegistersInUse.GetType().GetField(secondValRegObject.Name).GetValue(RegistersInUse);

                            if (firstValRegObject != null)
                            {
                                Console.WriteLine($"[CPU]: VADD -> Storing into {firstValRegObject.Name} | Result {(int)firstVal + (int)secVal}");
                                RegistersInUse.GetType().GetField(firstValRegObject.Name).SetValue(RegistersInUse, (int)firstVal + (int)secVal);
                            }
                            else
                            {
                                Console.WriteLine($"[CPU]: VADD -> Storing onto the stack | Result {(int)firstVal + (int)secVal}");
                                RegistersInUse.VSP++;
                                StackMemory[RegistersInUse.VSP] = (int)firstVal + (int)secVal; //push added value onto the stack
                            }

                            break;
                        }
                    case Instructions.VSUB:
                        {
                            var firstVal = Program.TextSection[RegistersInUse.VIP];
                            var secVal = Program.TextSection[++RegistersInUse.VIP];

                            var firstValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)firstVal).FirstOrDefault();
                            if (firstValRegObject != null)
                                firstVal = RegistersInUse.GetType().GetField(firstValRegObject.Name).GetValue(RegistersInUse);

                            var secondValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)secVal).FirstOrDefault();
                            if (secondValRegObject != null)
                                secVal = RegistersInUse.GetType().GetField(secondValRegObject.Name).GetValue(RegistersInUse);

                            if (firstValRegObject != null)
                            {
                                Console.WriteLine($"[CPU]: VSUB -> Storing into {firstValRegObject.Name} | Result {(int)firstVal - (int)secVal}");
                                RegistersInUse.GetType().GetField(firstValRegObject.Name).SetValue(RegistersInUse, (int)firstVal - (int)secVal);
                            }
                            else
                            {
                                Console.WriteLine($"[CPU]: VSUB -> Storing onto the stack | Result {(int)firstVal - (int)secVal}");
                                RegistersInUse.VSP++;
                                StackMemory[RegistersInUse.VSP] = (int)firstVal - (int)secVal; //push subtracted value onto the stack
                            }

                            break;
                        }
                    case Instructions.VMUL:
                        {
                            var firstVal = Program.TextSection[RegistersInUse.VIP];
                            var secVal = Program.TextSection[++RegistersInUse.VIP];

                            var firstValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)firstVal).FirstOrDefault();
                            if (firstValRegObject != null)
                                firstVal = RegistersInUse.GetType().GetField(firstValRegObject.Name).GetValue(RegistersInUse);

                            var secondValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)secVal).FirstOrDefault();
                            if (secondValRegObject != null)
                                secVal = RegistersInUse.GetType().GetField(secondValRegObject.Name).GetValue(RegistersInUse);

                            if (firstValRegObject != null)
                            {
                                Console.WriteLine($"[CPU]: VMUL -> Storing into {firstValRegObject.Name} | Result {(int)firstVal * (int)secVal}");
                                RegistersInUse.GetType().GetField(firstValRegObject.Name).SetValue(RegistersInUse, (int)firstVal * (int)secVal);
                            }
                            else
                            {
                                Console.WriteLine($"[CPU]: VMUL -> Storing onto the stack | Result {(int)firstVal * (int)secVal}");
                                RegistersInUse.VSP++;
                                StackMemory[RegistersInUse.VSP] = (int)firstVal * (int)secVal; //push multiplied value onto the stack
                            }

                            break;
                        }
                    case Instructions.VDIV:
                        {
                            var firstVal = Program.TextSection[RegistersInUse.VIP];
                            var secVal = Program.TextSection[++RegistersInUse.VIP];

                            var firstValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)firstVal).FirstOrDefault();
                            if (firstValRegObject != null && firstValRegObject.Name != "VIP")
                                firstVal = RegistersInUse.GetType().GetField(firstValRegObject.Name).GetValue(RegistersInUse);

                            var secondValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)secVal).FirstOrDefault();
                            if (secondValRegObject != null && secondValRegObject.Name != "VIP")
                                secVal = RegistersInUse.GetType().GetField(secondValRegObject.Name).GetValue(RegistersInUse);

                            if ((int)secVal == 0)
                                throw new Exception("[CPU]: VDIV -> Cannot divide by zero.");

                            if (firstValRegObject != null)
                            {
                                Console.WriteLine($"[CPU]: VDIV -> Storing into {firstValRegObject.Name} | Result {(int)firstVal / (int)secVal}");
                                RegistersInUse.GetType().GetField(firstValRegObject.Name).SetValue(RegistersInUse, (int)firstVal / (int)secVal);
                            }
                            else
                            {
                                Console.WriteLine($"[CPU]: VDIV -> Storing onto the stack | Result {(int)firstVal / (int)secVal}");
                                RegistersInUse.VSP++;
                                StackMemory[RegistersInUse.VSP] = (int)firstVal / (int)secVal; //push divided value onto the stack
                            }

                            break;
                        }
                    case Instructions.VPUSH:
                        {
                            var valToPush = Program.TextSection[RegistersInUse.VIP];

                            var regObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)valToPush).FirstOrDefault();
                            if (regObject != null)
                            {
                                Console.WriteLine($"[CPU]: VPUSH -> Pushing {regObject.Name}");

                                if(valToPush == regObject.GetValue(new Registers()))
                                    RegistersInUse.GetType().GetField(regObject.Name).SetValue(RegistersInUse, valToPush);
                                else
                                {
                                    RegistersInUse.VSP++;
                                    StackMemory[RegistersInUse.VSP] = valToPush;
                                }
                            }
                            else
                            {
                                RegistersInUse.VSP++;
                                StackMemory[RegistersInUse.VSP] = valToPush;
                            }

                            break;
                        }
                    case Instructions.VPOP:
                        {
                            var valToPopOff = Program.TextSection[RegistersInUse.VIP];

                            var regObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)valToPopOff).FirstOrDefault();
                            if (regObject != null)
                            {
                                if ((int)RegistersInUse.GetType().GetField(regObject.Name).GetValue(RegistersInUse) == (int)regObject.GetValue(new Registers()))
                                    break;

                                Console.WriteLine($"[CPU]: VPOP -> Popping off {regObject.Name}");
                                RegistersInUse.GetType().GetField(regObject.Name).SetValue(RegistersInUse, regObject.GetValue(new Registers()));
                                Console.WriteLine($"[CPU]: VPOP -> {regObject.Name} is now {regObject.GetValue(new Registers())}");
                            }
                            else
                            {
                                if (StackMemory.Length == 0)
                                    break;

                                if(valToPopOff.GetType() == typeof(Instructions))
                                    valToPopOff = StackMemory[RegistersInUse.VSP];

                                Console.WriteLine($"[CPU]: VPOP -> Popping off {valToPopOff} from the top of the stack.");
                                --RegistersInUse.VSP;
                            }

                            break;
                        }
                    case Instructions.VMOV:
                        {
                            var destAddr = Program.TextSection[RegistersInUse.VIP];
                            var sourceValue = Program.TextSection[++RegistersInUse.VIP];
                            RegistersInUse.VIP++;

                            var regObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)destAddr).FirstOrDefault();
                            if (regObject != null)
                            {
                                Console.WriteLine($"[CPU]: VMOV -> Moving {sourceValue} into {regObject.Name}.");
                                RegistersInUse.GetType().GetField(regObject.Name).SetValue(RegistersInUse, sourceValue);
                            }

                            break;
                        }
                    case Instructions.VRET:
                        {
                            var retVal = StackMemory[RegistersInUse.VSP--];

                            RegistersInUse.VSP = RegistersInUse.VFP;                     //Set to retAddr
                            RegistersInUse.VIP = (int)StackMemory[RegistersInUse.VSP--]; //retAddr
                            RegistersInUse.VFP = (int)StackMemory[RegistersInUse.VSP--]; //Restore frame pointer

                            int numOfArgs = (int)StackMemory[RegistersInUse.VSP--];      //Get numOfArgs from the stack
                            RegistersInUse.VSP -= numOfArgs;                             //Pop arguments off the stack

                            StackMemory[++RegistersInUse.VSP] = retVal;                  //Push return value

                            break;
                        }
                    case Instructions.VCALL:
                        {
                            var subroutAddr = Program.TextSection[RegistersInUse.VIP++];

                            var subroutRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)subroutAddr).FirstOrDefault();
                            if (subroutRegObject != null)
                                subroutAddr = RegistersInUse.GetType().GetField(subroutRegObject.Name).GetValue(RegistersInUse);

                            var numOfArgs = Program.TextSection[RegistersInUse.VIP++];  //Get numOfArgs from the stack

                            StackMemory[++RegistersInUse.VSP] = numOfArgs;              //Push numOfArgs onto the stack
                            StackMemory[++RegistersInUse.VSP] = RegistersInUse.VFP;     //Push frame pointer onto the stack
                            StackMemory[++RegistersInUse.VSP] = RegistersInUse.VIP;     //Push retAddr

                            RegistersInUse.VFP = RegistersInUse.VSP;                    //Set the frame pointer to the retAddr
                            RegistersInUse.VIP = (int)subroutAddr;                      //Set the next instruction to the subroutine address

                            break;
                        }
                    case Instructions.VCMP:
                        {
                            var firstVal = Program.TextSection[RegistersInUse.VIP];
                            var secVal = Program.TextSection[++RegistersInUse.VIP];

                            var firstValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)firstVal).FirstOrDefault();
                            if (firstValRegObject != null)
                                firstVal = RegistersInUse.GetType().GetField(firstValRegObject.Name).GetValue(RegistersInUse);

                            var secondValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)secVal).FirstOrDefault();
                            if (secondValRegObject != null)
                                secVal = RegistersInUse.GetType().GetField(secondValRegObject.Name).GetValue(RegistersInUse);

                            if (firstValRegObject != null)
                            {
                                Console.WriteLine($"[CPU]: VCMP -> Storing into {firstValRegObject.Name} | Result {(((int)firstVal - (int)secVal) == 0 ? 1 : 0)}");
                                RegistersInUse.GetType().GetField(firstValRegObject.Name).SetValue(RegistersInUse, (((int)firstVal - (int)secVal) == 0 ? 1 : 0));
                            }
                            else
                            {
                                Console.WriteLine($"[CPU]: VSUB -> Storing onto the stack | Result {(((int)firstVal - (int)secVal) == 0 ? 1 : 0)}");
                                RegistersInUse.VSP++;
                                StackMemory[RegistersInUse.VSP] = (((int)firstVal - (int)secVal) == 0 ? 1 : 0); //push subtracted value onto the stack
                            }

                            break;
                        }
                    case Instructions.VJMP:
                        {
                            var addToJumpTo = Program.TextSection[RegistersInUse.VIP++];
                            
                            var jumpAddrRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)addToJumpTo).FirstOrDefault();
                            if (jumpAddrRegObject != null)
                                addToJumpTo = RegistersInUse.GetType().GetField(jumpAddrRegObject.Name).GetValue(RegistersInUse);

                            RegistersInUse.VIP = (int)addToJumpTo;

                            break;
                        }
                    case Instructions.VJIE:
                        {
                            var addToJumpTo = Program.TextSection[RegistersInUse.VIP++];

                            int previousInstruction = (int)Program.TextSection[RegistersInUse.VIP-5];
                            if (previousInstruction != (int)Instructions.VCMP)
                                break;

                            var previousCMPOpFirstVal = Program.TextSection[RegistersInUse.VIP - 4];

                            var previousCMPOpFirstValRegObject = typeof(Registers).GetFields().Where(a => (int)a.GetValue(new Registers()) == (int)previousCMPOpFirstVal).FirstOrDefault();
                            if (previousCMPOpFirstValRegObject != null)
                                previousCMPOpFirstVal = RegistersInUse.GetType().GetField(previousCMPOpFirstValRegObject.Name).GetValue(RegistersInUse);
                            else
                                previousCMPOpFirstVal = (int)StackMemory[RegistersInUse.VSP--];

                            if ((int)previousCMPOpFirstVal == 1)
                                RegistersInUse.VIP = (int)addToJumpTo;

                            break;
                        }
                    case Instructions.VEXIT:
                        {
                            Console.WriteLine("[CPU]: VEXIT -> Exiting VM...");
                            return RegistersInUse.VAX != -1 ? RegistersInUse.VAX : (RegistersInUse.VSP < StackMemory.Length ? 0 : StackMemory[RegistersInUse.VSP]);
                        }
                }
            }

            return RegistersInUse.VAX != -1 ? RegistersInUse.VAX : (RegistersInUse.VSP < StackMemory.Length ? 0 : StackMemory[RegistersInUse.VSP]);
        }
    }
}
