using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace VM
{
    internal class Dispatcher
    {
        /// <summary>
        /// Function pointer table to handlers (Opcode executor)
        /// Acts as a native jump table to handlers
        /// </summary>
        private static readonly unsafe Dictionary<byte[], nint> HandlerTable = new Dictionary<byte[], nint>();

        static Dispatcher()
        {
            InitializeHandlers();
        }

        private static void InitializeHandlers()
        {
            unsafe
            {
                EncryptAndAddHandlerToTable(&Handle_VMOV, VMMnemonic.VMOV);
                EncryptAndAddHandlerToTable(&Handle_VPUSH, VMMnemonic.VPUSH);
                EncryptAndAddHandlerToTable(&Handle_VPOP, VMMnemonic.VPOP);

                EncryptAndAddHandlerToTable(&Handle_VADD, VMMnemonic.VADD);
                EncryptAndAddHandlerToTable(&Handle_VSUB, VMMnemonic.VSUB);
                EncryptAndAddHandlerToTable(&Handle_VMULT, VMMnemonic.VMULT);
                EncryptAndAddHandlerToTable(&Handle_VDIV, VMMnemonic.VDIV);
            }
        }

        internal static void DispatchHandlers(VMContext context)
        {
            while (context.Registers.VIP != -1 &&
               context.Registers.VIP <= (context.Instructions.Length - 1))
            {
                nint currentVIP = context.Registers.VIP;
                nint currentInstruction = context.Instructions[currentVIP];

#if DEBUG
                string mnemonicName = VMMnemonic.GetMnemonicName(currentInstruction);
                Console.WriteLine($"[{currentVIP}] Instruction: {(mnemonicName != null ? mnemonicName : "Integer")} ({currentInstruction:X})");
#endif

                DecryptAndDispatchHandler(ref context, currentInstruction, currentVIP);
                context.Registers.VIP++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void DecryptAndDispatchHandler(ref VMContext context, nint currentInstruction, nint currentVIP)
        {
            using (SHA256 shaEngine = SHA256.Create())
            {
                var mnemonicHash = shaEngine.ComputeHash(BitConverter.GetBytes(currentInstruction));
                var hashKeys = HandlerTable.Keys.Where(k => k.SequenceEqual(mnemonicHash));

                if (hashKeys.Any())
                {
                    unsafe
                    {
                        delegate* managed<ref VMContext, nint, void> handler = (delegate* managed<ref VMContext, nint, void>)Decrypt((nint)HandlerTable[hashKeys.First()]);
                        handler(ref context, currentVIP);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void Handle_VMOV(ref VMContext context, nint currentVIP)
        {
            nint dest = context.Instructions[currentVIP + 1];
            nint source = context.Instructions[currentVIP + 2];

            if ((!VMMnemonic.Test(dest) && !VMMnemonic.Test(source)) || !VMMnemonic.Test(dest))
                throw new Exception("Invalid destination on VMOV operation.");

            if (VMMnemonic.Test(dest) && !VMMnemonic.Test(source))
                AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), source);

            if (VMMnemonic.Test(source) && VMMnemonic.Test(dest))
                AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), source);

            context.Registers.VIP += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void Handle_VPUSH(ref VMContext context, nint currentVIP)
        {
            nint pushVal = context.Instructions[currentVIP + 1];

            context.Stack.Push(pushVal, ref context.Registers.VSP);

            context.Registers.VIP++;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void Handle_VPOP(ref VMContext context, nint currentVIP)
        {
            nint dest = context.Instructions[currentVIP + 1];

            if (!VMMnemonic.Test(dest))
                throw new Exception("Cannot load a stack value into a non register.");

            nint popValue = context.Stack.Pop(ref context.Registers.VSP);
            AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), popValue);

            context.Registers.VIP++;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void Handle_VADD(ref VMContext context, nint currentVIP)
        {
            nint dest = context.Instructions[currentVIP + 1];
            nint source = context.Instructions[currentVIP + 2];

            if (VMMnemonic.Test(dest))
            {
                nint destRegValue = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(dest));

                if (VMMnemonic.Test(source))
                {
                    nint sourceRegVal = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(source));
                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue + sourceRegVal);
                }
                else
                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue + source);
            }
            else
                context.Stack.Push(dest + source, ref context.Registers.VSP);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void Handle_VSUB(ref VMContext context, nint currentVIP)
        {
            nint dest = context.Instructions[currentVIP + 1];
            nint source = context.Instructions[currentVIP + 2];

            if (VMMnemonic.Test(dest))
            {
                nint destRegValue = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(dest));

                if (VMMnemonic.Test(source))
                {
                    nint sourceRegVal = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(source));
                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue - sourceRegVal);
                }
                else
                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue - source);
            }
            else
                context.Stack.Push(dest - source, ref context.Registers.VSP);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void Handle_VMULT(ref VMContext context, nint currentVIP)
        {
            nint dest = context.Instructions[currentVIP + 1];
            nint source = context.Instructions[currentVIP + 2];

            if (VMMnemonic.Test(dest))
            {
                nint destRegValue = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(dest));

                if (VMMnemonic.Test(source))
                {
                    nint sourceRegVal = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(source));
                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue * sourceRegVal);
                }
                else
                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue * source);
            }
            else
                context.Stack.Push(dest * source, ref context.Registers.VSP);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void Handle_VDIV(ref VMContext context, nint currentVIP)
        {
            nint dest = context.Instructions[currentVIP + 1];
            nint source = context.Instructions[currentVIP + 2];

            if (VMMnemonic.Test(dest))
            {
                nint destRegValue = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(dest));

                if (VMMnemonic.Test(source))
                {
                    nint sourceRegVal = RetrieveValueFromRegister(ref context, VMMnemonic.Decode(source));

                    if (sourceRegVal == 0)
                        throw new Exception("Cannot divide by zero.");

                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue / sourceRegVal);
                }
                else
                {
                    if (source == 0)
                        throw new Exception("Cannot divide by zero.");

                    AssignValueIntoDestination(ref context, VMMnemonic.Decode(dest), destRegValue / source);
                }
            }
            else
            {
                if (source == 0)
                    throw new Exception("Cannot divide by zero.");

                context.Stack.Push(dest / source, ref context.Registers.VSP);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static unsafe void EncryptAndAddHandlerToTable(delegate* managed<ref VMContext, nint, void> handler, nint mnemonicValue)
        {
            using (SHA256 shaEngine = SHA256.Create())
            {
                byte[] mnemonicHash = shaEngine.ComputeHash(BitConverter.GetBytes(mnemonicValue));
                delegate* managed<ref VMContext, nint, void> handlerPtr = handler;

                HandlerTable.Add(mnemonicHash, Encrypt((nint)handlerPtr));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        private static nint Encrypt(nint encryptionValue)
        {
            unsafe
            {
                byte[] xorKey = BitConverter.GetBytes(0x6969);
                if (!Environment.Is64BitProcess)
                    Array.Reverse(xorKey);

                byte[] originalValueBytes = BitConverter.GetBytes(encryptionValue);
                if (!Environment.Is64BitProcess)
                    Array.Reverse(originalValueBytes);

                for (int i = 0; i < originalValueBytes.Length; i++)
                    originalValueBytes[i] = (byte)(originalValueBytes[i] ^ xorKey[i % xorKey.Length]);

                return (nint)(Environment.Is64BitProcess ? BitConverter.ToInt64(originalValueBytes) : BitConverter.ToInt32(originalValueBytes));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static nint Decrypt(nint decryptionValue)
        {
            return Encrypt(decryptionValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static nint RetrieveValueFromRegister(ref VMContext context, nint decodedMnem)
        {
            nint retrievedValue = 0;

            switch (VMMnemonic.Decode(decodedMnem))
            {
                case VMMnemonic.VAX:
                    retrievedValue = context.Registers.VAX;
                    break;
                case VMMnemonic.VBX:
                    retrievedValue = context.Registers.VBX;
                    break;
                case VMMnemonic.VCX:
                    retrievedValue = context.Registers.VCX;
                    break;
            }

            return retrievedValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        private static void AssignValueIntoDestination(ref VMContext ctx, nint decodedMnem, nint source)
        {
            switch (VMMnemonic.Encode(decodedMnem))
            {
                case VMMnemonic.VAX:
                    {
                        if (VMMnemonic.Test(source))
                        {
                            switch (source)
                            {
                                case VMMnemonic.VBX:
                                    ctx.Registers.VAX = ctx.Registers.VBX;
                                    break;
                                case VMMnemonic.VCX:
                                    ctx.Registers.VAX = ctx.Registers.VCX;
                                    break;
                            }
                        }
                        else
                            ctx.Registers.VAX = source;

                        break;
                    }

                case VMMnemonic.VBX:
                    {
                        if (VMMnemonic.Test(source))
                        {
                            switch (VMMnemonic.Decode(source))
                            {
                                case VMMnemonic.VAX:
                                    ctx.Registers.VBX = ctx.Registers.VAX;
                                    break;
                                case VMMnemonic.VCX:
                                    ctx.Registers.VBX = ctx.Registers.VCX;
                                    break;
                            }
                        }
                        else
                            ctx.Registers.VBX = source;

                        break;
                    }

                case VMMnemonic.VCX:
                    {
                        if (VMMnemonic.Test(source))
                        {
                            switch (VMMnemonic.Decode(source))
                            {
                                case VMMnemonic.VAX:
                                    ctx.Registers.VCX = ctx.Registers.VAX;
                                    break;
                                case VMMnemonic.VBX:
                                    ctx.Registers.VCX = ctx.Registers.VBX;
                                    break;
                            }
                        }
                        else
                            ctx.Registers.VCX = source;

                        break;
                    }
            }
        }
    }
}
