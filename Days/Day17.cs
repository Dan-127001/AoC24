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
            return "";
            var machine = new Machine();
            SetMachineInits(input, machine);
            machine.RunUntilHalt();

            return string.Join(",", machine.Output);
        }

        private void SetMachineInits(string input, Machine machine, int? AOverride = null)
        {
            var lines = input.Split(Environment.NewLine);
            machine.A_Val = AOverride ?? int.Parse(lines[0].Split(": ")[1]);
            machine.A = AOverride ?? int.Parse(lines[0].Split(": ")[1]);
            machine.B = int.Parse(lines[1].Split(": ")[1]);
            machine.C = int.Parse(lines[2].Split(": ")[1]);

            machine.Instructions = lines[4].Split(": ")[1].Split(",").Select(x => int.Parse(x)).ToArray();
            machine.InstructionsLength = machine.Instructions.Length;
        }

        public string Ch2(string input)
        {
            int amountOfMachines = 15;

            HashSet<int> skip = [715827882, 572662305, 429496729, 286331152, 143165576, 0];

            for (int i = 0; i < amountOfMachines; i++)
            {
                int startingA = startingA = (int)(int.MaxValue * ((double)i / amountOfMachines));

                if (skip.Contains(startingA))
                    continue;

                int endA = (int)(int.MaxValue * ((double)(i + 1) / amountOfMachines));
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
            public int A_Val { get; set; }
            public int B_Val { get; set; }
            public int C_Val { get; set; }

            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }

            public int DesiredOutcomePointer { get; set; }

            public int[] Instructions { get; set; }
            public int InstructionsLength { get; set; }

            public int InstrPointer { get; set; }
            public List<int> Output { get; set; } = new();

            public bool Reset { get; set; }
            public bool CheckIfOutputEqInput { get; set; }
            public int EndA { get; set; }
            public bool Ended { get; set; }

            int StepsTakenWithoutOutput = 0;

            public void RunUntilHalt()
            {
                Debug.Assert(A == A_Val);
                Console.WriteLine("Statring with " + A);
                while (DesiredOutcomePointer < InstructionsLength)
                {
                    while (InstrPointer < InstructionsLength - 1)
                    {
                        Execute(Instructions[InstrPointer], Instructions[InstrPointer + 1]);
                        StepsTakenWithoutOutput++;

                        if(StepsTakenWithoutOutput > 2000)
                        {
                            ResetMachine();
                            Console.WriteLine("loop " + A_Val);
                            continue;
                        }

                        if (CheckIfOutputEqInput && (Reset))
                        {
                            ResetMachine();
                        }
                    }

                    if (DesiredOutcomePointer < InstructionsLength)
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
                    Console.WriteLine(A_Val.ToString());
                }
            }

            private void ResetMachine()
            {
                InstrPointer = 0;
                DesiredOutcomePointer = 0;

                A_Val++;

                A = A_Val;
                B = B_Val;
                C = C_Val;

                StepsTakenWithoutOutput = 0;

                Reset = false;
            }

            public void Execute(int instrType, int operand)
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
            private void _0_Division(int operand)
            {
                var val = (1 << GetCombo(operand));

                if (val == 0)
                {
                    Reset = true;
                    return;
                }

                A = (A / val);
            }

            private void _1_bxl(int operand)
            {
                var val1 = B;
                var val2 = operand;

                B = val1 ^ val2;
            }

            private void _2_bst(int operand)
            {
                B = (GetCombo(operand) % 8);
            }

            private void _3_jnz(int operand, ref bool jump)
            {
                if (A == 0)
                    return;

                InstrPointer = operand;
                jump = false;
            }

            private void _4_bxc(int operand)
            {
                B = B ^ C;
            }

            private void _5_out(int operand)
            {
                var nextVal = GetCombo(operand) % 8;

                if (CheckIfOutputEqInput)
                {
                    if (DesiredOutcomePointer < InstructionsLength && nextVal == Instructions[DesiredOutcomePointer])
                    {
                        DesiredOutcomePointer++;
                        StepsTakenWithoutOutput = 0;
                    }
                    else
                    {
                        Reset = true;
                        return;
                    }
                }
                else
                {
                    Output.Add(nextVal);
                }
            }

            private void _6_bdv(int operand)
            {
                var val = (1 << GetCombo(operand));

                if (val == 0)
                {
                    Reset = true;
                    return;
                }

                B = A / val;
            }

            private void _7_cdv(int operand)
            {
                var val = (1 << GetCombo(operand));

                if (val == 0)
                {
                    Reset = true;
                    return;
                }

                C = A / val;
            }

            private int GetCombo(int operand)
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
