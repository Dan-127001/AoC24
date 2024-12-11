using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day11 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var digits = input.Split(' ').Select(long.Parse).ToList();

            int blinksToDo = 25;

            while (blinksToDo > 0)
            {
                OneBlinkCh1(digits);
                blinksToDo--;
            }

            return digits.Count.ToString();
        }

        private void OneBlinkCh1(List<long> digits)
        {
            for (int i = 0; i < digits.Count; i++)
            {
                var val = digits[i];

                if (val == 0)
                {
                    digits[i] = 1;
                    continue;
                }

                var str = val.ToString();
                if (str.Length % 2 == 0)
                {
                    var val1 = long.Parse(str.Substring(0, str.Length / 2));
                    var val2 = long.Parse(str.Substring(str.Length / 2));

                    digits[i] = val1;
                    digits.Insert(i + 1, val2);
                    i++;
                    continue;
                }

                digits[i] = val * 2024;
            }
        }

        public string Ch2(string input)
        {
            int blinksToDo = 75;

            var rawValDigits = input.Split(' ').Select(long.Parse).ToList();

            foreach (var digit in rawValDigits)
            {
                GetOrCreate(digit, blinksToDo);
                //ToDo.Enqueue((new ADigit() { Value = digit }, blinksToDo));
            }

            while (ToDo.TryDequeue(out var digit))
            {
                OneBlinkCh2(digit.Digit, digit.HowManyToGo - 1);
            }

            long count = 0;
            foreach (var val in rawValDigits)
            {
                var digit = Map[val];

                count += digit.CountDeep(blinksToDo);
            }

            return count.ToString();
        }

        Dictionary<long, ADigit> Map = new();
        Queue<(ADigit Digit, int HowManyToGo)> ToDo = new();
        private void OneBlinkCh2(ADigit digit, int howManyToGo)
        {
            var val = digit.Value;

            if (val == 0)
            {
                digit.NextAfterBlink = [GetOrCreate(1, howManyToGo)];
                return;
            }

            var str = val.ToString();
            if (str.Length % 2 == 0)
            {
                var val1 = long.Parse(str.Substring(0, str.Length / 2));
                var val2 = long.Parse(str.Substring(str.Length / 2));

                digit.NextAfterBlink = [GetOrCreate(val1, howManyToGo), GetOrCreate(val2, howManyToGo)];

                return;
            }

            digit.NextAfterBlink = [GetOrCreate(val * 2024, howManyToGo)];
        }

        private ADigit GetOrCreate(long v, int howManyToGo)
        {
            if (Map.TryGetValue(v, out var el))
            {
                return el;
            }

            var newEl = new ADigit() { Value = v };
            if (howManyToGo > 0)
                ToDo.Enqueue((newEl, howManyToGo));
            Map.Add(v, newEl);
            return newEl;
        }

        public class ADigit
        {
            public long Value { get; set; }
            public List<ADigit> NextAfterBlink { get; set; }
            public Dictionary<int, long> CacheForDeepCounter { get; set; } = new();

            internal long CountDeep(int blinksToDo)
            {
                if (blinksToDo == 0)
                    return 1;

                if (CacheForDeepCounter.ContainsKey(blinksToDo))
                {
                    return CacheForDeepCounter[blinksToDo];
                }

                long count = 0;
                foreach (var next in NextAfterBlink)
                {
                    count += next.CountDeep(blinksToDo - 1);
                }

                CacheForDeepCounter[blinksToDo] = count;

                return count;
            }
        }
    }
}
