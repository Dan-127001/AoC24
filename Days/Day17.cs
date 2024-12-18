using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static AoC24.Days.Day17;

namespace AoC24.Days
{
    public class Day17 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var machine = new Machine();
            SetMachineInits(input, machine);
            machine.RunUntilHalt();

            bool isSameAsInput = machine.Output.SequenceEqual(machine.Instructions);

            if (isSameAsInput)
            {
                Console.WriteLine("MATCH");
            }

            Console.WriteLine(string.Join(",", machine.Instructions));
            Console.WriteLine(string.Join(",", machine.Output));

            return "";
        }

        private void SetMachineInits(string input, Machine machine, ulong? AOverride = null)
        {
            var lines = input.Split(Environment.NewLine);

            var aVal = AOverride ?? ulong.Parse(lines[0].Split(": ")[1]);
            //var aVal = AOverride ?? Convert.ToUInt64(lines[0].Split(": ")[1], 2);

            machine.A_Val = aVal;
            machine.A = aVal;
            machine.B = ulong.Parse(lines[1].Split(": ")[1]);
            machine.C = ulong.Parse(lines[2].Split(": ")[1]);

            machine.Instructions = lines[4].Split(": ")[1].Split(",").Select(x => ulong.Parse(x)).ToArray();
            machine.InstructionsLength = machine.Instructions.Length;
        }

        public string Ch2(string input)
        {
            //var ints = input.Split(Environment.NewLine).Select(x => int.Parse(x)).ToArray();

            ////print every int int dez, oct, bin
            //foreach (var integ in ints)
            //{
            //    Console.WriteLine(Convert.ToString(integ, 2) + "  \t" + integ.ToString("X") + "\t" + integ.ToString());
            //}

            //return "";
            int amountOfMachines = 15;

            for (int i = 0; i < amountOfMachines; i++)
            {
                var startingA = (ulong)(ulong.MaxValue * ((double)i / amountOfMachines));

                //if (skip.Contains(startingA))
                //    continue;

                var endA = (ulong)(ulong.MaxValue * ((double)(i + 1) / amountOfMachines));
                Task.Run(() =>
                {
                    try
                    {
                        var machine = new Machine();
                        machine.CheckIfOutputEqInput = true;
                        SetMachineInits(input, machine, startingA);
                        machine.EndA = endA;
                        machine.RunUntilHalt();

                        Console.WriteLine("Machine done " + startingA);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                });
            }


            return "";
        }


        public class Machine
        {
            public ulong A_Val { get; set; }
            public ulong B_Val { get; set; }
            public ulong C_Val { get; set; }

            public ulong A { get; set; }
            public ulong B { get; set; }
            public ulong C { get; set; }

            public int DesiredOutcomeCheckPointer { get; set; }

            public ulong[] Instructions { get; set; }
            public int InstructionsLength { get; set; }

            public int InstrPointer { get; set; }
            public List<ulong> Output { get; set; } = new();

            public bool Reset { get; set; }
            public bool CheckIfOutputEqInput { get; set; }
            public ulong EndA { get; set; }
            public bool Ended { get; set; }

            public void RunUntilHalt()
            {
                Debug.Assert(A == A_Val);
                Console.WriteLine("Statring with " + A);

                while (DesiredOutcomeCheckPointer < InstructionsLength)
                {
                    while (InstrPointer < InstructionsLength - 1)
                    {
                        Execute(Instructions[InstrPointer], Instructions[InstrPointer + 1]);
                        if (CheckIfOutputEqInput && (Reset))
                        {
                            ResetMachine();
                        }
                    }

                    if (DesiredOutcomeCheckPointer < InstructionsLength)
                    {
                        ResetMachine();
                        if (A_Val > EndA)
                        {
                            Ended = true;
                            break;
                        }
                    }
                }

                if (Ended)
                {
                    Console.WriteLine("Ended " + EndA);
                }
                else
                {
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine(A_Val.ToString());
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                    Console.WriteLine("FOUND IT");
                }
            }

            //ulong hasToHave = 0x35528BB1;
            //ulong skip = 0xffffffff;

            //ulong hasToHave = 0x49C35528BB1;
            //ulong skip = 0xffffffffffff;

            //ulong hasToHave = 0xC35528BB1;
            //ulong skip = 0xfffffffff;

            //ulong hasToHave = 0x528BB1;
            //ulong skip = 0xffffff;

            ulong hasToHave = 0xF5D3C0F;
            //ulong hasToHave = 0xF93C0F;
            //ulong hasToHave = 0x053C0F;

            //ulong hasToHave = 0x3C0F;
            ulong skip = 0xfffffff;

            //ulong hasToHave = 0xE49C35528BB1;
            //ulong skip = 0xffffffffffff;

            private void ResetMachine()
            {
                if (DesiredOutcomeCheckPointer >= InstructionsLength - 1)
                {
                    if ((A_Val & 0x3C0F) != 0x3C0F && (A_Val & 0x49C35528BB1) != 0x49C35528BB1)
                        Console.WriteLine("+++++++++Here+++++++++");

                    //Console.WriteLine(A_Val + "," + DesiredOutcomeCheckPointer);

                    //bin oct dez
                    Console.WriteLine(UlongToBinaryString(A_Val) + "  \t" + (A_Val).ToString("X") + "  \t" + Conversion.Oct(A_Val) + "  \t" + (A_Val).ToString() + "  \t" + DesiredOutcomeCheckPointer);
                }

                InstrPointer = 0;
                DesiredOutcomeCheckPointer = 0;

                A_Val += skip + 1;

                while ((A_Val & hasToHave) != hasToHave)
                {
                    A_Val = (A_Val & ~skip) | hasToHave;
                }

                A = A_Val;
                B = B_Val;
                C = C_Val;

                Reset = false;
            }
            static string UlongToBinaryString(ulong value)
            {
                char[] bits = new char[64];
                // Initialize all bits to '0'
                for (int i = 0; i < 64; i++)
                {
                    bits[i] = '0';
                }

                int index = 63;
                while (value != 0)
                {
                    bits[index--] = (value & 1) == 1 ? '1' : '0';
                    value >>= 1;
                }

                // Return the full 64-bit binary string
                return new string(bits);
            }
            public void Execute(ulong instrType, ulong operand)
            {
                bool jump = true;

                switch (instrType)
                {
                    case 0: // InstrType.Division:
                        _0_Division(operand);
                        break;
                    case 1: // InstrType.bxl:
                        _1_bxl(operand);
                        break;
                    case 2: // InstrType.bst:
                        _2_bst(operand);
                        break;
                    case 3: // InstrType.jnz:
                        _3_jnz(operand, ref jump);
                        break;
                    case 4: // InstrType.bxc:
                        _4_bxc(operand);
                        break;
                    case 5: // InstrType._out:
                        _5_out(operand);
                        break;
                    case 6: // InstrType.bdv:
                        _6_bdv(operand);
                        break;
                    case 7: // InstrType.cdv:
                        _7_cdv(operand);
                        break;
                    default:
                        break;
                }

                if (jump)
                    InstrPointer += 2;
            }
            private void _0_Division(ulong operand)
            {
                var val = (1UL << (int)GetCombo(operand));

                if (val == 0)
                {
                    Reset = true;
                    return;
                }

                A = (A / val);
            }

            private void _1_bxl(ulong operand)
            {
                var val1 = B;
                var val2 = operand;

                B = val1 ^ val2;
            }

            private void _2_bst(ulong operand)
            {
                B = (GetCombo(operand) % 8);
            }

            private void _3_jnz(ulong operand, ref bool jump)
            {
                if (A == 0)
                    return;

                InstrPointer = (int)operand;
                jump = false;
            }

            private void _4_bxc(ulong operand)
            {
                B = B ^ C;
            }

            private void _5_out(ulong operand)
            {
                var nextVal = GetCombo(operand) % 8UL;

                if (CheckIfOutputEqInput)
                {
                    if (DesiredOutcomeCheckPointer < InstructionsLength && nextVal == Instructions[DesiredOutcomeCheckPointer])
                    {
                        DesiredOutcomeCheckPointer++;
                    }
                    else
                    {
                        if (DesiredOutcomeCheckPointer > InstructionsLength)
                            Console.WriteLine(DesiredOutcomeCheckPointer);

                        Reset = true;
                        return;
                    }
                }
                else
                {
                    Output.Add(nextVal);
                }
            }

            private void _6_bdv(ulong operand)
            {
                var val = (1UL << (int)GetCombo(operand));

                if (val == 0)
                {
                    Reset = true;
                    return;
                }

                B = A / val;
            }

            private void _7_cdv(ulong operand)
            {
                var val = (1UL << (int)GetCombo(operand));

                if (val == 0)
                {
                    Reset = true;
                    return;
                }

                C = A / val;
            }

            private ulong GetCombo(ulong operand)
            {
                switch (operand)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return operand;
                    case 4:
                        return A;
                    case 5:
                        return B;
                    case 6:
                        return C;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public enum InstrType : int { Division, bxl, bst, jnz, bxc, _out, bdv, cdv, }
        public class Instruction
        {
            public InstrType InstrType { get; set; }
            public int Operand { get; set; }
        }
    }
}
