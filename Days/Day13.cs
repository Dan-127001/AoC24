using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day13 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var machines = input.Split(Environment.NewLine + Environment.NewLine).Select(x => new SMachine(x, 0)).ToList();

            long priceTotal = 0;
            foreach (var machine in machines)
            {
                var cheapest = machine.GetCheapest();
                if (cheapest.HasValue)
                    priceTotal += cheapest.Value;
            }

            return priceTotal.ToString();
        }

        public string Ch2(string input)
        {
            var machines = input.Split(Environment.NewLine + Environment.NewLine).Select(x => new SMachine(x, 10000000000000)).ToList();

            long priceTotal = 0;
            foreach (var machine in machines)
            {
                var cheapest = machine.GetCheapest();
                if (cheapest.HasValue)
                    priceTotal += cheapest.Value;
            }

            return priceTotal.ToString();
        }

        public class SMachine
        {
            public double X1 { get; set; }
            public double Y1 { get; set; }

            public double X2 { get; set; }
            public double Y2 { get; set; }

            public double RX { get; set; }
            public double RY { get; set; }


            public long Price1 { get; set; } = 3;
            public long Price2 { get; set; } = 1;


            public SMachine(string x, long add)
            {
                var lines = x.Split(Environment.NewLine);

                (X1, Y1) = LineSplitter(lines[0], "+");
                (X2, Y2) = LineSplitter(lines[1], "+");
                (RX, RY) = LineSplitter(lines[2], "=");

                RX += add;
                RY += add;
            }

            private (long X1, long Y2) LineSplitter(string v, string specialSplitter)
            {
                var longerestingPart = v.Split("X" + specialSplitter)[1];
                var yValue = longerestingPart.Split("Y" + specialSplitter)[1];
                var xValue = longerestingPart.Split(",")[0];

                return (long.Parse(xValue), long.Parse(yValue));
            }

            public long? GetCheapest()
            {
                var a = (RY * X2 - RX * Y2) / (Y1 * X2 - X1 * Y2);
                var b = (RX - a * X1) / X2;

                if(a % 1 == 0 && b % 1 == 0)
                    return (long)a * Price1 + (long)b * Price2;

                return null;
            }
        }
    }
}
