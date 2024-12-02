using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day2 : IChallengeYouToADanceOff
    {
        public static string Ch1(string input)
        {
            var lines = input.Split(Environment.NewLine);

            int safe = 0;

            foreach (var line in lines)
            {
                var lvls = line.Split(" ").Select(x => int.Parse(x)).ToArray();

                int prev = lvls[0];
                bool isIncrease = lvls[1] > prev;

                bool lookingGood = true;

                for (int i = 1; i < lvls.Length; i++)
                {
                    var curr = lvls[i];
                    if (curr > prev != isIncrease)
                    {
                        lookingGood = false;
                        break;
                    }

                    var diff = Math.Abs(curr - prev);
                    if (diff < 1 || diff > 3)
                    {
                        lookingGood = false;
                        break;
                    }

                    prev = curr;
                }

                if (lookingGood)
                    safe++;
            }

            return safe.ToString();
        }

        public static string Ch2(string input)
        {
            var lines = input.Split(Environment.NewLine);

            int safe = 0;

            foreach (var line in lines)
            {
                var lvls = line.Split(" ").Select(x => int.Parse(x)).ToArray();
                if (IsLineSafe_OneErrorKk(lvls, true, true))
                    safe++;

                if (IsLineSafe_OneErrorKk(lvls, false, true))
                    safe++;
            }

            return safe.ToString();
        }

        private static bool IsLineSafe_OneErrorKk(int[] lvls, bool isIncrease, bool checkForSafeByRemovingOneMore)
        {
            int prev = lvls[0];

            HashSet<int> errors = new();

            for (int i = 1; i < lvls.Length; i++)
            {
                var curr = lvls[i];
                if (curr > prev != isIncrease)
                {
                    errors.Add(i);
                    errors.Add(i - 1);
                }

                var diff = Math.Abs(curr - prev);
                if (diff < 1 || diff > 3)
                {
                    errors.Add(i);
                    errors.Add(i - 1);
                }

                prev = curr;
            }

            if (errors.Count == 0)
                return true;

            if (!checkForSafeByRemovingOneMore)
                return false;

            foreach (var error in errors)
            {
                if (IsLineSafe_OneErrorKk(lvls.Where((x, i) => i != error).ToArray(), isIncrease, false))
                    return true;
            }

            return false;
        }
    }
}
