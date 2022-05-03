using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ZVM
{
    internal class Program
    {
        internal static object[] DataSection;
        internal static object[] TextSection;

        static void Main(string[] args)
        {
            Console.Title = "ZVM";

            //.section .data
            DataSection = new object[0];

            //,section .text
            //.globl _main
            var regSet = new Registers();
            TextSection = new object[]
            {
                //_main:
                /*
                  return _add(3, 2); -> VCALL [VDA]
                  exit(); 
                */
                Instructions.VPUSH, regSet.VAX,
                Instructions.VMOV, regSet.VAX, 3,
                Instructions.VPUSH, regSet.VDX,
                Instructions.VMOV, regSet.VDX, 2,

                Instructions.VMOV, regSet.VDA, 17,
                Instructions.VCALL, regSet.VDA, 2,
                Instructions.VEXIT,

                //_add(int a, int b): 
                Instructions.VADD, regSet.VAX, regSet.VDX,
                Instructions.VPUSH, regSet.VAX,
                Instructions.VRET
            };

            var cpu = new CPU();

            Stopwatch perfCounter = new Stopwatch();
            perfCounter.Start();
            var returnValue = cpu.Execute();
            perfCounter.Stop();

            Console.WriteLine($"\nReturned value: {(int)returnValue} | Took {perfCounter.ElapsedMilliseconds}ms");

            Console.ReadLine();
        }
    }
}
