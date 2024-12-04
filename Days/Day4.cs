using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    internal class Day4 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            char[][] chars = input.Split(Environment.NewLine).Select(x => x.ToCharArray()).ToArray();

            int count = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                for (int j = 0; j < chars[0].Length; j++)
                {
                    if (chars[i][j] != 'X')
                        continue;

                    count += CheckIncrements(chars, i, j, 1, 0);
                    count += CheckIncrements(chars, i, j, -1, 0);

                    count += CheckIncrements(chars, i, j, 0, 1);
                    count += CheckIncrements(chars, i, j, 0, -1);

                    count += CheckIncrements(chars, i, j, 1, 1);
                    count += CheckIncrements(chars, i, j, -1, -1);

                    count += CheckIncrements(chars, i, j, -1, 1);
                    count += CheckIncrements(chars, i, j, 1, -1);
                }
            }

            return count.ToString();
        }

        private int CheckIncrements(char[][] chars, int i, int j, int xInc, int yInc)
        {
            var lastX = i + (xInc * 3);
            var lastY = j + (yInc * 3);

            if (lastX < chars.Length && lastX >= 0 && lastY < chars[0].Length && lastY >= 0 && chars[i + xInc][j + yInc] == 'M' && chars[i + xInc * 2][j + yInc * 2] == 'A' && chars[i + xInc * 3][j + yInc * 3] == 'S')
            {
                return 1;
            }
            return 0;
        }

        public string Ch2(string input)
        {
            char[][] chars = input.Split(Environment.NewLine).Select(x => x.ToCharArray()).ToArray();

            int count = 0;

            for (int i = 1; i < chars.Length - 1; i++)
            {
                for (int j = 1; j < chars[0].Length - 1; j++)
                {
                    if (chars[i][j] != 'A')
                        continue;

                    count += Check_X_Shape(chars, i, j);
                }
            }

            return count.ToString();
        }


        private int Check_X_Shape(char[][] chars, int i, int j)
        {
            var topRight = chars[i + 1][j + 1];
            if (topRight == 'M')
            {
                if (chars[i - 1][j - 1] != 'S')
                    return 0;
            }
            else if (topRight == 'S')
            {
                if (chars[i - 1][j - 1] != 'M')
                    return 0;
            }
            else
            {
                return 0;
            }

            var topLeft = chars[i - 1][j + 1];
            if (topLeft == 'M')
            {
                if (chars[i + 1][j - 1] != 'S')
                    return 0;
            }
            else if (topLeft == 'S')
            {
                if (chars[i + 1][j - 1] != 'M')
                    return 0;
            }
            else
            {
                return 0;
            }

            return 1;
        }
    }
}
