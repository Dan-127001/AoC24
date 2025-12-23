using AoC24.Days;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days25
{
    public class Day1_25 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            int currDialPos = 50;
            int countZeroes = 0;

            foreach (var line in lines)
            {
                var isLeft = line.First() == 'L';
                var number = int.Parse(line[1..]);

                number %= 100;

                if (isLeft)
                    currDialPos = (currDialPos - number + 100) % 100;
                else
                    currDialPos = (currDialPos + number) % 100;

                if (currDialPos == 0)
                    countZeroes++;
            }

            return countZeroes.ToString();
        }

        public string Ch2(string input)
        {
            return Ch2_Daniel(input);
        }

        public string Ch2_Daniel(string input)
        {
            var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            int currDialPos = 50;
            int countZeroes = 0;

            foreach (var line in lines)
            {
                var isLeft = line.First() == 'L';
                var number = int.Parse(line[1..]);

                countZeroes += number / 100;

                number %= 100;

                if (isLeft && number > currDialPos && currDialPos != 0)
                    countZeroes++;
                else if (!isLeft && number > (100 - currDialPos) && currDialPos != 0)
                    countZeroes++;

                if (isLeft)
                    currDialPos = (currDialPos - number + 100) % 100;
                else
                    currDialPos = (currDialPos + number) % 100;

                if (currDialPos == 0)
                    countZeroes++;
            }

            return countZeroes.ToString();
        }

        public string Ch2_MartinPascal(string input)
        {
            var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            int currDialPos = 50;
            int countZeroes = 0;

            foreach (var line in lines)
            {
                var isLeft = line.First() == 'L';
                var number = int.Parse(line[1..]);

                bool startedOnZero = currDialPos == 0;

                if (isLeft)
                    number = -number;

                currDialPos += number;

                var passedCount = 0;

                passedCount = currDialPos / 100;
                passedCount = Math.Abs(passedCount);

                //if (passedCount != 0 && startedOnZero && currDialPos < 0)
                //    passedCount--;

                currDialPos %= 100;

                if ((passedCount == 0 || number < 0) && currDialPos == 0)
                    passedCount++;

                if (currDialPos < 0)
                {
                    if (!startedOnZero)
                        passedCount++;
                    currDialPos += 100;
                }

                countZeroes += passedCount;
            }

            return countZeroes.ToString();
        }
    }
}
