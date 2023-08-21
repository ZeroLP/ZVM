using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VM
{
    internal class VMMnemonic
    {
#if DEBUG
        internal static readonly Dictionary<nint, string> MnemonicDebugInfoTable = new Dictionary<nint, string>() { };

        private static void LoadFields()
        {
            var fields = typeof(VMMnemonic).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                                        | System.Reflection.BindingFlags.FlattenHierarchy);

            foreach (var fInfo in fields)
            {
                if (fInfo.IsLiteral && !fInfo.IsInitOnly && fInfo.Name.StartsWith("V"))
                    MnemonicDebugInfoTable.Add(Decode((int)fInfo.GetValue(fInfo)), fInfo.Name);
            }
        }

        internal static string GetMnemonicName(nint encodedValue)
        {
            if (MnemonicDebugInfoTable.Count == 0)
                LoadFields();

            if (VMMnemonic.Test(encodedValue))
                return MnemonicDebugInfoTable[Decode(encodedValue)];
            else
                return null;
        }
#endif

        private const nint XORKEY = 0x6969;

        private static readonly Dictionary<nint, nint> EncodedMnemonicTable = new Dictionary<nint, nint>()
        {
            { XORKEY ^ VIP, Encode(XORKEY ^ VIP) },
            { XORKEY ^ VSP, Encode(XORKEY ^ VSP) },

            { XORKEY ^ VAX, Encode(XORKEY ^ VAX) },
            { XORKEY ^ VBX, Encode(XORKEY ^ VBX) },
            { XORKEY ^ VCX, Encode(XORKEY ^ VCX) },

            { XORKEY ^ VMOV, Encode(XORKEY ^ VMOV) },
            { XORKEY ^ VPUSH, Encode(XORKEY ^ VPUSH) },
            { XORKEY ^ VPOP, Encode(XORKEY ^ VPOP) },
            { XORKEY ^ VCALL, Encode(XORKEY ^ VCALL) },

            { XORKEY ^ VADD, Encode(XORKEY ^ VADD) },
            { XORKEY ^ VSUB, Encode(XORKEY ^ VSUB) },
            { XORKEY ^ VMULT, Encode(XORKEY ^ VMULT)},
            { XORKEY ^ VDIV, Encode(XORKEY ^ VDIV) }
        };

        public const nint VIP = XORKEY ^ 0x1;
        public const nint VSP = XORKEY ^ 0x2;

        public const nint VAX = XORKEY ^ 0x3;
        public const nint VBX = XORKEY ^ 0x4;
        public const nint VCX = XORKEY ^ 0x5;

        public const nint VMOV = XORKEY ^ 0x6;
        public const nint VPUSH = XORKEY ^ 0x7;
        public const nint VPOP = XORKEY ^ 0x8;
        public const nint VCALL = XORKEY ^ 0x9;

        public const nint VADD = XORKEY ^ 0x10;
        public const nint VSUB = XORKEY ^ 0x11;
        public const nint VMULT = XORKEY ^ 0x12;
        public const nint VDIV = XORKEY ^ 0x13;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static nint Encode(nint istr)
        {
            //Maybe XOR and bitshift?
            return (istr ^ XORKEY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static nint Decode(nint encodedIstr)
        {
            return (encodedIstr ^ XORKEY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Test(nint istr)
        {             
            return EncodedMnemonicTable.ContainsKey(Decode(istr));
        }
    }
}
