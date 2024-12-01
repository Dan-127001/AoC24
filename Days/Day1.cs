using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public static class Day1
    {
        public static string Ch1(string input)
        {
            List<int> left = new();
            List<int> right = new();

            foreach (var line in input.Split(Environment.NewLine))
            {
                var split = line.Split("   ");
                left.Add(int.Parse(split[0]));
                right.Add(int.Parse(split[1]));
            }

            left = left.Order().ToList();
            right = right.Order().ToList();

            int totalDiff = 0;
            for (int i = 0; i < left.Count; i++)
            {
                totalDiff += Math.Abs(left[i] - right[i]);
            }

            return totalDiff.ToString();
        }

        public static string Ch2(string input)
        {
            List<int> left = new();
            List<int> right = new();

            foreach (var line in input.Split(Environment.NewLine))
            {
                var split = line.Split("   ");
                left.Add(int.Parse(split[0]));
                right.Add(int.Parse(split[1]));
            }

            int similarityScore = 0;
            for (int i = 0; i < left.Count; i++)
            {
                var left_i = left[i];
                similarityScore += right.Count(x => x == left_i) * left_i;
            }

            return similarityScore.ToString();
        }
    }
}
