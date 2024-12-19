using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day19 : IChallengeYouToADanceOff
    {
        public Dictionary<char, string> PatternOptions { get; set; }
        public List<string> GoalPatterns { get; set; }

        public string Ch1(string input)
        {
            Init(input);

            var total = 0;
            foreach (var pattern in GoalPatterns)
            {
                if (IsPatternPossible(pattern, PatternOptions))
                {

                }
            }
        }

        private bool IsPatternPossible(string pattern, Dictionary<string, string> options)
        {

        }

        private void Init(string input)
        {
            var parts = input.Split(Environment.NewLine + Environment.NewLine);
            PatternOptions = parts[0].Split(Environment.NewLine).ToDictionary(x => x[0], x => x);
            GoalPatterns = parts[1].Split(Environment.NewLine).ToList();
        }

        public string Ch2(string input)
        {
            throw new NotImplementedException();
        }
    }

}
