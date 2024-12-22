using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    internal class Day22 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            return "";
            var lines = input.Split(Environment.NewLine);

            ulong total = 0;

            foreach (var line in lines)
            {
                var currVal = ulong.Parse(line);
                var seq = new Sequence(currVal);

                while (seq.SecretNumbers.Count < 2001)
                {
                    seq.AddOneSecretNumber();
                }

                total += seq.SecretNumbers[2000];
            }

            return total.ToString();
        }

        public string Ch2(string input)
        {
            var lines = input.Split(Environment.NewLine);
            Dictionary<(long, long, long, long), long> totalMap = new();

            foreach (var line in lines)
            {
                var currVal = ulong.Parse(line);
                var seq = new Sequence(currVal);

                while (seq.SecretNumbers.Count < 2001)
                {
                    seq.AddOneSecretNumber();
                }

                seq.CountUpTotalMap(totalMap);
            }

            var maxEl = totalMap.MaxBy(x => x.Value);
            return maxEl.ToString();
        }

        public class Sequence
        {
            public Sequence(ulong currVal)
            {
                SecretNumbers = new List<ulong>() { currVal };
                PriceNumbers = new List<long>() { GetPriceFromSecretNumber(currVal) };
            }

            private long GetPriceFromSecretNumber(ulong currVal)
            {
                return long.Parse(currVal.ToString().Last() + "");
            }

            public List<ulong> SecretNumbers { get; set; }
            public List<long> PriceNumbers { get; set; }

            public void AddOneSecretNumber()
            {
                var latestSecretNumber = SecretNumbers.Last();

                var newSecretNumber = CalcSecretNumber(latestSecretNumber);
                SecretNumbers.Add(newSecretNumber);
                PriceNumbers.Add(GetPriceFromSecretNumber(newSecretNumber));
            }

            private ulong CalcSecretNumber(ulong secretNumber)
            {
                var p1 = secretNumber * 64;

                secretNumber = Mix(secretNumber, p1);
                secretNumber = Prune(secretNumber);

                var p2 = secretNumber / 32;
                secretNumber = Mix(secretNumber, p2);
                secretNumber = Prune(secretNumber);

                var p3 = secretNumber * 2048;
                secretNumber = Mix(secretNumber, p3);
                secretNumber = Prune(secretNumber);

                return secretNumber;
            }

            private ulong Prune(ulong curr)
            {
                return curr % 16777216;
            }

            private ulong Mix(ulong curr, ulong latestSecretNumber)
            {
                return curr ^ latestSecretNumber;
            }

            internal void CountUpTotalMap(Dictionary<(long, long, long, long), long> totalMap)
            {
                HashSet<(long, long, long, long)> alreadyDoneKeys = new();

                List<long> latest4 = new();
                for (int i = 0; i < PriceNumbers.Count - 1; i++)
                {
                    latest4.Add(PriceNumbers[i + 1] - PriceNumbers[i]);

                    if (i > 2)
                    {
                        var key = (latest4[0], latest4[1], latest4[2], latest4[3]);

                        if(alreadyDoneKeys.Contains(key))
                        {
                            latest4.RemoveAt(0);
                            continue;
                        }

                        alreadyDoneKeys.Add(key);

                        var val = PriceNumbers[i + 1];

                        if (totalMap.ContainsKey(key))
                        {
                            totalMap[key] += val;
                        }
                        else
                        {
                            totalMap[key] = val;
                        }

                        latest4.RemoveAt(0);
                    }
                }
            }
        }
    }
}
