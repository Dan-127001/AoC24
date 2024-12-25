using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day25 : IChallengeYouToADanceOff
    {
        public Dictionary<int, List<Lock>> Locks { get; set; }
        public Dictionary<int, List<Key>> Keys { get; set; }
        public string Ch1(string input)
        {
            var blocks = input.Split(Environment.NewLine + Environment.NewLine);

            Locks = new();
            Keys = new();

            foreach (var block in blocks)
            {
                var lines = block.Split(Environment.NewLine);

                if (lines[0].All(x => x == '.'))
                {
                    Lock @lock = new();
                    for (int x = 0; x < lines[0].Length; x++)
                    {
                        for (int y = 1; y < 6; y++)
                        {
                            if (lines[y][x] == '#')
                            {
                                @lock.Heights[x] = 6 - y;
                                break;
                            }
                        }
                    }
                    var lockHeightSum = @lock.Heights.Sum();
                    if (!Locks.TryGetValue(lockHeightSum, out var list))
                    {
                        list = new();
                        Locks.Add(lockHeightSum, list);
                    }

                    list.Add(@lock);
                }
                else
                {
                    Key key = new();
                    for (int i = 0; i < lines[0].Length; i++)
                    {
                        for (int j = 5; j > 0; j--)
                        {
                            if (lines[j][i] == '#')
                            {
                                key.Heights[i] = j;
                                break;
                            }
                        }
                    }
                    int keyHeightSum = key.Heights.Sum();
                    if (!Keys.TryGetValue(keyHeightSum, out var list))
                    {
                        list = new();
                        Keys.Add(keyHeightSum, list);
                    }

                    list.Add(key);
                }
            }

            int counter = 0;
            foreach (var loc in Locks.SelectMany(x => x.Value))
            {
                var locSum = loc.Heights.Sum();
                foreach (var keyOptions in Keys)
                {
                    if (keyOptions.Key + locSum > 25)
                        continue;

                    foreach (var key in keyOptions.Value)
                    {
                        bool allMatch = true;
                        for (int i = 0; i < 5; i++)
                        {
                            if (loc.Heights[i] + key.Heights[i] > 5)
                            {
                                allMatch = false;
                                break;
                            }
                        }

                        if (allMatch)
                            counter++;
                    }
                }
            }

            return counter.ToString();
        }

        public string Ch2(string input)
        {
            return "";
        }

        public class Lock
        {
            public int[] Heights { get; set; } = new int[5];
        }

        public class Key
        {
            public int[] Heights { get; set; } = new int[5];
        }
    }
}
