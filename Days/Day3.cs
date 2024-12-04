using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    internal class Day3 : IChallengeYouToADanceOff
    {

        public string Ch1(string inputLines)
        {
            List<string> validParts = new();
            int numOffset = 4;

            foreach (var input in inputLines.Split(Environment.NewLine))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] != 'm' || input[i + 1] != 'u' || input[i + 2] != 'l' || input[i + 3] != '(')
                        continue;

                    var numIdxAdd = 0;

                    while (char.IsNumber(input[i + numOffset + numIdxAdd]) && numIdxAdd < 3)
                    {
                        numIdxAdd++;
                    }

                    var commaIdx = i + numOffset + numIdxAdd;

                    if (numIdxAdd == 0 || input[commaIdx] != ',')
                        continue;

                    int secNumberIdxAdd = 0;
                    while (char.IsNumber(input[commaIdx + 1 + secNumberIdxAdd]) && secNumberIdxAdd < 3)
                    {
                        secNumberIdxAdd++;
                    }

                    if (secNumberIdxAdd == 0 || input[commaIdx + 1 + secNumberIdxAdd] != ')')
                        continue;

                    var curr = input.Substring(i, commaIdx + secNumberIdxAdd + 2 - i);

                    validParts.Add(curr);
                }
            }

            var total = 0;

            foreach (var item in validParts)
            {
                var split = item.Split("(");
                split = split[1].Split(")").First().Split(',');
                var x = int.Parse(split[0]);
                var y = int.Parse(split[1]);
                total += x * y;
            }

            return total.ToString();
        }

        public string Ch2(string inputLines)
        {
            List<string> validParts = new();
            int numOffset = 4;

            bool isEnabled = true;

            foreach (var input in inputLines.Split(Environment.NewLine))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (!isEnabled)
                    {
                        if (input[i] == 'd' && input[i + 1] == 'o' && input[i + 2] == '(' && input[i + 3] == ')')
                        {
                            i += 3;
                            isEnabled = true;
                        }

                        continue;
                    }
                    else if (isEnabled)
                    {
                        if (input[i] == 'd' && input[i + 1] == 'o' && input[i + 2] == 'n' && input[i + 3] == '\'' && input[i + 4] == 't' && input[i + 5] == '(' && input[i + 6] == ')')
                        {
                            i += 6;
                            isEnabled = false;
                        }
                    }


                    if (input[i] != 'm' || input[i + 1] != 'u' || input[i + 2] != 'l' || input[i + 3] != '(')
                        continue;

                    var numIdxAdd = 0;

                    while (char.IsNumber(input[i + numOffset + numIdxAdd]) && numIdxAdd < 3)
                    {
                        numIdxAdd++;
                    }

                    var commaIdx = i + numOffset + numIdxAdd;

                    if (numIdxAdd == 0 || input[commaIdx] != ',')
                        continue;

                    int secNumberIdxAdd = 0;
                    while (char.IsNumber(input[commaIdx + 1 + secNumberIdxAdd]) && secNumberIdxAdd < 3)
                    {
                        secNumberIdxAdd++;
                    }

                    if (secNumberIdxAdd == 0 || input[commaIdx + 1 + secNumberIdxAdd] != ')')
                        continue;

                    var curr = input.Substring(i, commaIdx + secNumberIdxAdd + 2 - i);

                    validParts.Add(curr);
                }
            }

            var total = 0;

            foreach (var item in validParts)
            {
                var split = item.Split("(");
                split = split[1].Split(")").First().Split(',');
                var x = int.Parse(split[0]);
                var y = int.Parse(split[1]);
                total += x * y;
            }

            return total.ToString();
        }
    }
}
