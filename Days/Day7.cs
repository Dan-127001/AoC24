using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day7 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var lines = input.Split(Environment.NewLine).Select(x => new InstructionLine(x) { AmountOfOperations = 2 }).ToList();

            long possible = 0;
            foreach (var line in lines)
            {
                if (line.IsAnyPossible())
                    possible += line.Result;
            }

            return possible.ToString();
        }

        public string Ch2(string input)
        {
            var lines = input.Split(Environment.NewLine).Select(x => new InstructionLine(x) { AmountOfOperations = 3 }).ToList();

            long possible = 0;
            foreach (var line in lines)
            {
                if (line.IsAnyPossible())
                    possible += line.Result;
            }

            return possible.ToString();
        }


        public class InstructionLine
        {
            public InstructionLine(string input)
            {
                var split = input.Split(": ");
                Result = long.Parse(split[0]);
                Values = split[1].Split(" ").Select(x => long.Parse(x)).ToArray();
            }
            public long Result { get; set; }
            public long[] Values { get; set; }
            public required int AmountOfOperations { get; set; }

            public bool IsAnyPossible()
            {
                return IsAnyPossible(Values[0], Values.AsSpan(1));
            }

            public bool IsAnyPossible(long resSoFar, Span<long> vals)
            {

                for (int i = 0; i < AmountOfOperations; i++)
                {
                    var calc = Calc(resSoFar, vals[0], i);

                    if (vals.Length == 1)
                    {
                        if (calc == Result)
                            return true;
                    }
                    else
                    {
                        if (IsAnyPossible(calc, vals.Slice(1)))
                            return true;
                    }
                }

                return false;
            }

            long Calc(long x, long y, int operationIdx)
            {
                return operationIdx switch
                {
                    0 => Sum(x, y),
                    1 => Multiply(x, y),
                    2 => Concat(x, y),
                    _ => throw new Exception()
                };


                long Sum(long x, long y) => x + y;
                long Multiply(long x, long y) => x * y;
                long Concat(long x, long y) => long.Parse(x.ToString() + y.ToString());

            }
        }
    }
}
