using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day19 : IChallengeYouToADanceOff
    {
        public Dictionary<char, HashSet<string>> PatternOptions { get; set; }
        public List<string> GoalPatterns { get; set; }

        public string Ch1(string input)
        {
            Init(input);

            var total = 0;
            foreach (var pattern in GoalPatterns)
            {
                if (AmountOfPatternPossible(pattern, PatternOptions) > 0)
                {
                    total++;
                }
            }

            return total.ToString();
        }

        private ulong AmountOfPatternPossible(string pattern, Dictionary<char, HashSet<string>> options)
        {
            ulong totalPossible = 0;
            ulong[] possibilitesToIdx = new ulong[pattern.Length];

            for (int i = 0; i < pattern.Length; i++)
            {
                ulong toAdd = possibilitesToIdx[i];

                if (i == 0)
                    toAdd = 1;

                if (options.TryGetValue(pattern[i], out var optionWithCorrectStartingChar))
                {
                    foreach (var option in optionWithCorrectStartingChar)
                    {
                        if (i + option.Length > pattern.Length)
                            continue;


                        if (option.AsSpan().SequenceEqual(pattern.AsSpan(i, option.Length)))
                        {
                            if (i + option.Length == pattern.Length)
                            {
                                totalPossible += toAdd;
                                continue;
                            }

                            possibilitesToIdx[i + option.Length] += toAdd;
                        }

                    }
                }
            }

            return totalPossible;
        }

        private void Init(string input)
        {
            var parts = input.Split(Environment.NewLine + Environment.NewLine);
            PatternOptions = parts[0].Split(", ").GroupBy(x => x[0]).ToDictionary(x => x.Key, x => x.ToHashSet());
            GoalPatterns = parts[1].Split(Environment.NewLine).ToList();
        }

        public string Ch2(string input)
        {
            Init(input);

            ulong total = 0;
            foreach (var pattern in GoalPatterns)
            {
                total += AmountOfPatternPossible(pattern, PatternOptions);

            }

            return total.ToString();
        }
    }

}
