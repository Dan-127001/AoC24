using AoC24.Helper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day21 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var lines = input.Split(Environment.NewLine).ToArray();
            ulong total = 0;

            foreach (var line in lines)
            {
                var minVal = GetLineInstructions(line, 2);
                var numberInSequence = ulong.Parse(line.Substring(0, line.Length - 1));

                Console.WriteLine(minVal + " " + numberInSequence);
                total += minVal * numberInSequence;
            }

            return total.ToString();
        }

        private string Execute(string instr, Stack<Dictionary<char, Vector2Int>> toDo)
        {
            string currInstr = instr;
            foreach (var machine in toDo)
            {
                currInstr = Pad.Resolve(currInstr, machine);
            }
            return currInstr;
        }


        private ulong GetLineInstructions(string line, int iterations)
        {
            var lineAfterNumpad = "";

            char currKey = 'A';
            foreach (var ch in line)
            {
                lineAfterNumpad += Pad.GetDirKeys(currKey, ch);
                currKey = ch;
            }

            Pad.CurrentMachineChar = Enumerable.Range(0, iterations + 1).ToDictionary(x => x, x => 'A');

            ulong total = 0;

            foreach (var ch in lineAfterNumpad)
            {
                total += Pad.GetDirKeys_Dir(ch, iterations - 1);

                currKey = ch;
            }

            return total;
        }

        public string Ch2(string input)
        {
            var lines = input.Split(Environment.NewLine).ToArray();
            ulong total = 0;

            foreach (var line in lines)
            {
                var minVal = GetLineInstructions(line, 25);
                var numberInSequence = ulong.Parse(line.Substring(0, line.Length - 1));

                Console.WriteLine(minVal + " " + numberInSequence);
                total += minVal * numberInSequence;
            }

            return total.ToString();
        }




        public static class Pad
        {
            public static Dictionary<char, Vector2Int> Numpad = new()
            {
                { '7', new(0,0) },{ '8', new(1,0) },{ '9', new(2,0) },
                { '4', new(0,1) },{ '5', new(1,1) },{ '6', new(2,1) },
                { '1', new(0,2) },{ '2', new(1,2) },{ '3', new(2,2) },
                                  { '0', new(1,3) },{ 'A', new(2,3) },
            };

            public static Dictionary<char, Vector2Int> DirectionPad = new()
            {
                                   { '^', new(1,0) }, { 'A', new(2,0) },
                { '<', new(0,1) }, { 'v', new(1,1) }, { '>', new(2,1) },
            };

            //static Dictionary<char, List<int>>

            public static string GetDirKeys(char latestKey, char newKey)
            {
                StringBuilder stringBuilder = new();
                var newPos = Numpad[newKey];
                var latestPos = Numpad[latestKey];
                var diff = newPos - latestPos;

                if (diff.X < 0)
                {
                    for (int i = 0; i < Math.Abs(diff.X); i++)
                    {
                        stringBuilder.Append('<');
                    }
                }

                if (diff.Y > 0)
                {
                    for (int i = 0; i < diff.Y; i++)
                    {
                        stringBuilder.Append('v');
                    }
                }

                if (diff.Y < 0)
                {
                    for (int i = 0; i < Math.Abs(diff.Y); i++)
                    {
                        stringBuilder.Append('^');
                    }
                }

                if (diff.X > 0)
                {
                    for (int i = 0; i < diff.X; i++)
                    {
                        stringBuilder.Append('>');
                    }
                }

                if ((latestPos.Y == 3 || newPos.Y == 3) && (latestPos.X == 0 || newPos.X == 0))
                {
                    return new string(stringBuilder.ToString().Reverse().ToArray()) + "A";
                }

                stringBuilder.Append('A');

                return stringBuilder.ToString();
            }

            public static Dictionary<int, char> CurrentMachineChar = new();
            public static Dictionary<(char From, char To), Dictionary<int, ulong>> Memorized = new();

            public static ulong GetDirKeys_Dir(char newKey, int toDo)
            {
                try
                {

                    var prevChar = CurrentMachineChar[toDo];
                    //ulong memorizedValue = ulong.MaxValue;


                    if (Memorized.TryGetValue((prevChar, newKey), out var dic))
                    {
                        if (dic.TryGetValue(toDo, out var res))
                        {
                            CurrentMachineChar[toDo] = newKey;
                            return res;
                        }
                    }
                    else
                    {
                        Memorized[(prevChar, newKey)] = new Dictionary<int, ulong>();
                    }

                    var diff = DirectionPad[newKey] - DirectionPad[prevChar];

                    var horKey = diff.X > 0 ? '>' : '<';
                    var verKey = diff.Y > 0 ? 'v' : '^';

                    if (diff.X == 0 && diff.Y == 0)
                    {
                        if (toDo == 0)
                            return 1;

                        var val = GetDirKeys_Dir('A', toDo - 1);

                        //Debug.Assert(memorizedValue == ulong.MaxValue || memorizedValue == val);
                        Memorized[(prevChar, newKey)][toDo] = val;
                        return val;
                    }

                    var manhattan = Math.Abs(diff.X) + Math.Abs(diff.Y);

                    if (toDo == 0)
                        return (ulong)(manhattan + 1);

                    if (manhattan == 1)
                    {
                        var val = GetDirKeys_Dir(diff.Y != 0 ? verKey : horKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);

                        //Debug.Assert(memorizedValue == ulong.MaxValue || memorizedValue == val);
                        Memorized[(prevChar, newKey)][toDo] = val;
                        return val;
                    }

                    if (manhattan == 2)
                    {
                        //if (diff.X == 0)
                        //{
                        //var val1 = GetDirKeys_Dir(verKey, toDo - 1) + GetDirKeys_Dir(verKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);

                        //    //Debug.Assert(memorizedValue == ulong.MaxValue || memorizedValue == val);
                        //    Memorized[(prevChar, newKey)][toDo] = val;
                        //    return val;
                        //}
                        //else if (diff.Y == 0)
                        //{
                        //var val2 = GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);

                        //    //Debug.Assert(memorizedValue == ulong.MaxValue || memorizedValue == val);
                        //    Memorized[(prevChar, newKey)][toDo] = val;
                        //    return val;
                        //}
                        //else
                        //{
                        var val = ulong.MaxValue;
                        
                        if(!(prevChar == '<' && newKey == '^'))
                        {
                            var val1 = GetDirKeys_Dir(verKey, toDo - 1) + GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);
                            if (val1 < val)
                            {
                                val = val1;
                            }
                        }
                        //GetDirKeys_Dir(verKey, toDo - 1) + GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);

                        if (!(prevChar == '^' && newKey == '<'))
                        {
                            var val1 = GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir(verKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);
                            if(val1 < val)
                            {
                                val = val1;
                            }
                        }

                        //Debug.Assert(memorizedValue == ulong.MaxValue || memorizedValue == val);
                        Memorized[(prevChar, newKey)][toDo] = val;
                        return val;
                        //}
                    }

                    if (manhattan == 3)
                    {
                        if (diff.X != -2)
                        {
                            var val = GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir(verKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);

                            //Debug.Assert(memorizedValue == ulong.MaxValue || memorizedValue == val);
                            Memorized[(prevChar, newKey)][toDo] = val;
                            return val;
                        }
                        //return horKey + horKey + verKey;

                        //yield return horKey + verKey + horKey;

                        if (diff.X != 2)
                        {
                            var val = GetDirKeys_Dir(verKey, toDo - 1) + GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir(horKey, toDo - 1) + GetDirKeys_Dir('A', toDo - 1);

                            //Debug.Assert(memorizedValue == ulong.MaxValue || memorizedValue == val);
                            Memorized[(prevChar, newKey)][toDo] = val;
                            return val;
                        }
                        //return verKey + horKey + horKey;
                    }
                }
                finally
                {
                    CurrentMachineChar[toDo] = newKey;
                }

                throw new Exception("Invalid manhattan distance");
            }

            internal static string Resolve(string currInstr, Dictionary<char, Vector2Int> machine)
            {
                Vector2Int currPos = machine['A'];
                string output = "";

                foreach (var instr in currInstr)
                {
                    if (instr == 'A')
                        output += machine.First(x => x.Value == currPos).Key;

                    switch (instr)
                    {
                        case '^':
                            currPos += new Vector2Int(0, -1);
                            break;
                        case 'v':
                            currPos += new Vector2Int(0, 1);
                            break;
                        case '<':
                            currPos += new Vector2Int(-1, 0);
                            break;
                        case '>':
                            currPos += new Vector2Int(1, 0);
                            break;
                    }
                }

                return output;
            }
        }
    }
}
