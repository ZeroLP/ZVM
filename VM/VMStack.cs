using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace VM
{
    internal unsafe class VMStack
    {
        private static nint* _InitialStackPointer;
        private static bool StackEncryptionState;

        internal nint* InitialStackPointer => _InitialStackPointer;

        internal void Initialize(ref nint vsp, nuint stackSize)
        {
            nint* allocStack = NativeMemoryManager.AllocateNewRegion(stackSize);

            if (allocStack == null)
                Console.WriteLine("Failed to allocate a new stack.");
            else
            {
#if DEBUG
                Console.WriteLine($"Stack allocated at: 0x{(nint)allocStack:X} with size: 0x{stackSize:X}");
#endif
                vsp = (nint)allocStack;
            }

            _InitialStackPointer = allocStack;
        }

        /// <summary>
        /// Release the stack memory.
        /// </summary>
        internal bool Release()
        {
            var deallocStatus = NativeMemoryManager.DeallocateRegion(ref _InitialStackPointer);

            if (!deallocStatus)
            {
                Console.WriteLine("Failed to release stack memory.");

                //If for some reason the OS fails to release the stack memory, manually set it to null.
                _InitialStackPointer = null;
            }
            else
            {
#if DEBUG
                Console.WriteLine("Sucessfully released stack memory.");
#endif
            }

            return deallocStatus;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Encrypt(nint vsp)
        {
            unsafe
            {
                for(nint stackIterator = 0; stackIterator < (nint)((vsp - ((nint)_InitialStackPointer)) / nint.Size); stackIterator++)
                {
                    nint valAtAddr = *(_InitialStackPointer + (stackIterator * nint.Size));

                    byte[] xorKey = BitConverter.GetBytes(0x6969);
                    if (!Environment.Is64BitProcess)
                        Array.Reverse(xorKey);

                    byte[] originalValueBytes = BitConverter.GetBytes(valAtAddr);
                    if (!Environment.Is64BitProcess)
                        Array.Reverse(originalValueBytes);

                    for (int i = 0; i < originalValueBytes.Length; i++)
                        originalValueBytes[i] = (byte)(originalValueBytes[i] ^ xorKey[i % xorKey.Length]);

                    *(_InitialStackPointer + (stackIterator * nint.Size)) = (nint)(Environment.Is64BitProcess ? BitConverter.ToInt64(originalValueBytes) : BitConverter.ToInt32(originalValueBytes));

                    //Console.WriteLine($"Processing: {valAtAddr} => {*(_InitialStackPointer + (stackIterator * nint.Size))}");
                }
            }

            StackEncryptionState = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Decrypt(nint vsp)
        {
            Encrypt(vsp);
            StackEncryptionState = false;
        }
        /*{
            unsafe
            {
                for (nint stackIterator = 0; stackIterator < (nint)((vsp - ((nint)StackStartPointer)) / nint.Size); stackIterator++)
                {
                    nint valAtAddr = *(StackStartPointer + (stackIterator * nint.Size));

                    byte[] xorKey = BitConverter.GetBytes(0x6969);
                    if (!Environment.Is64BitProcess)
                        Array.Reverse(xorKey);

                    byte[] encryptedValueBytes = BitConverter.GetBytes(valAtAddr);
                    if (!Environment.Is64BitProcess)
                        Array.Reverse(encryptedValueBytes);

                    for (int i = 0; i < encryptedValueBytes.Length; i++)
                        encryptedValueBytes[i] = (byte)(encryptedValueBytes[i] ^ xorKey[i % xorKey.Length]);

                    *(StackStartPointer + (stackIterator * nint.Size)) = (nint)(Environment.Is64BitProcess ? BitConverter.ToInt64(encryptedValueBytes) : BitConverter.ToInt32(encryptedValueBytes));

                    Console.WriteLine($"Decrypting: {valAtAddr} => {*(StackStartPointer + (stackIterator * nint.Size))}");
                }
            }
        }*/

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Push(nint value, ref nint vsp)
        {
            if (StackEncryptionState)
                Decrypt(vsp);

            *(nint*)vsp = value;

            //Advance the stack
            vsp += nint.Size;

            Encrypt(vsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal nint Pop(ref nint vsp)
        {
            if(StackEncryptionState)
                Decrypt(vsp);

            //Rewind the stack
            vsp -= nint.Size;
            nint retVal = *(nint*)vsp;

            //Properly pop it off
            *(nint*)vsp = 0;

            if (!StackEncryptionState)
                Encrypt(vsp);

            return retVal;
        }
    }
}
