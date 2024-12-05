using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day5 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var splitInputs = input.Split(Environment.NewLine + Environment.NewLine);

            var rulesStr = splitInputs[0].Split(Environment.NewLine);
            var updatesStr = splitInputs[1].Split(Environment.NewLine);

            ConcurrentDictionary<int, HashSet<int>> TheyMustBe_Before_TheKey = new();


            foreach (var rule in rulesStr)
            {
                var rStr = rule.Split('|');

                var n1 = int.Parse(rStr[0]);
                var n2 = int.Parse(rStr[1]);

                TheyMustBe_Before_TheKey.AddOrUpdate(n2, [n1], (key, list) =>
                {
                    list.Add(n1);
                    return list;
                });
            }

            List<int[]> validLines = new();

            var updates = updatesStr.Select(x => x.Split(',').Select(y => int.Parse(y)).ToArray()).ToArray();

            foreach (var updateLine in updates)
            {
                bool isLineValid = true;

                for (int i = 0; i < updateLine.Length; i++)
                {
                    if (TheyMustBe_Before_TheKey.TryGetValue(updateLine[i], out var elsThatCannotBeAfter))
                    {
                        for (int j = i + 1; j < updateLine.Length; j++)
                        {
                            if (elsThatCannotBeAfter.Contains(updateLine[j]))
                            {
                                isLineValid = false;
                                break;
                            }
                        }
                    }

                    if (!isLineValid)
                        break;
                }

                if (isLineValid)
                    validLines.Add(updateLine);
            }

            int sum = 0;

            foreach (var validLine in validLines)
            {
                var middle = validLine.Length / 2;
                sum += validLine[middle];
            }

            return sum.ToString();
        }

        public string Ch2(string input)
        {
            var splitInputs = input.Split(Environment.NewLine + Environment.NewLine);

            var rulesStr = splitInputs[0].Split(Environment.NewLine);
            var updatesStr = splitInputs[1].Split(Environment.NewLine);

            ConcurrentDictionary<int, HashSet<int>> TheyMustBe_Before_TheKey = new();


            foreach (var rule in rulesStr)
            {
                var rStr = rule.Split('|');

                var n1 = int.Parse(rStr[0]);
                var n2 = int.Parse(rStr[1]);

                TheyMustBe_Before_TheKey.AddOrUpdate(n2, [n1], (key, list) =>
                {
                    list.Add(n1);
                    return list;
                });
            }

            List<List<int>> invalidLines = new();

            var updates = updatesStr.Select(x => x.Split(',').Select(y => int.Parse(y)).ToList()).ToList();

            foreach (var updateLine in updates)
            {
                bool isLineValid = true;

                for (int i = 0; i < updateLine.Count; i++)
                {
                    if (TheyMustBe_Before_TheKey.TryGetValue(updateLine[i], out var elsThatCannotBeAfter))
                    {
                        for (int j = i + 1; j < updateLine.Count; j++)
                        {
                            if (elsThatCannotBeAfter.Contains(updateLine[j]))
                            {
                                isLineValid = false;
                                break;
                            }
                        }
                    }

                    if (!isLineValid)
                        break;
                }

                if (!isLineValid)
                    invalidLines.Add(updateLine);
            }

            foreach (var updateLine in invalidLines)
            {
                for (int i = 0; i < updateLine.Count; i++)
                {
                    if (TheyMustBe_Before_TheKey.TryGetValue(updateLine[i], out var elsThatCannotBeAfter))
                    {
                        for (int j = i + 1; j < updateLine.Count; j++)
                        {
                            if (elsThatCannotBeAfter.Contains(updateLine[j]))
                            {
                                var el = updateLine[i];
                                updateLine.Insert(j + 1, el);
                                updateLine.RemoveAt(i);
                                i = -1;
                                break;
                            }
                        }
                    }
                }
            }



            int sum = 0;

            foreach (var invalidLine in invalidLines)
            {
                var middle = invalidLine.Count / 2;
                sum += invalidLine[middle];
            }

            return sum.ToString();
        }
    }
}
