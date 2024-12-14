using AoC24.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day14 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var floor = new Floor(input, 101, 103);

            int sims = 100;

            for (int i = 0; i < sims; i++)
            {
                floor.OneMove();
            }

            return floor.Ch1Counter().ToString();
        }

        public string Ch2(string input)
        {
            var floor = new Floor(input, 101, 103);

            int sims = 1000000000;

            for (int i = 0; i < sims; i++)
            {
                //if (AnyRowOrColumnFull(floor, 32) || MiddleEmpty(floor, 6))
                if(i == 7569)
                {
                    //Console.Clear();
                    //Console.WriteLine("\x1b[3J");
                    Console.WriteLine(i + ": -------------------------");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    floor.DrawCurrent();

                    Console.ReadKey();
                }

                floor.OneMove();
            }

            return floor.Ch1Counter().ToString();
        }

        private bool MiddleEmpty(Floor floor, int variation)
        {
            var xLength = floor.XLength / 2;
            var yLength = floor.YLength / 2;

            var xStart = xLength - variation;
            var xEnd = xLength + variation;

            var yStart = yLength - variation;
            var yEnd = yLength + variation;

            bool? allSet = null;

            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    var robot = floor.Robots.FirstOrDefault(r => r.Pos.X == x && r.Pos.Y == y);
                    if (robot == null)
                    {
                        if (allSet == true)
                        {
                            return false;
                        }
                        allSet = false;
                    }
                    else
                    {
                        if (allSet == false)
                        {
                            return false;
                        }
                        allSet = true;
                    }
                }
            }

            return true;
        }

        private bool AnyRowOrColumnFull(Floor floor, int amount)
        {
            Dictionary<int, int> xCount = new();
            Dictionary<int, int> yCount = new();

            foreach (var robot in floor.Robots)
            {
                if (!xCount.ContainsKey(robot.Pos.X))
                {
                    xCount.Add(robot.Pos.X, 0);
                }
                xCount[robot.Pos.X]++;
                if (!yCount.ContainsKey(robot.Pos.Y))
                {
                    yCount.Add(robot.Pos.Y, 0);
                }
                yCount[robot.Pos.Y]++;
            }

            return xCount.Any(x => x.Value > amount) || yCount.Any(x => x.Value > amount);
        }

        private bool CountInWithYEquals(Floor floor, int yVal, int amount)
        {
            return floor.Robots.Count(r => r.Pos.Y == yVal) >= amount;
        }
        private bool CountInWithXEquals(Floor floor, int xVal, int amount)
        {
            return floor.Robots.Count(r => r.Pos.X == xVal) >= amount;
        }

        public class Floor
        {
            public int XLength { get; set; }
            public int YLength { get; set; }

            public List<Robot> Robots { get; set; }

            public Floor(string input, int xLength, int yLength)
            {
                XLength = xLength;
                YLength = yLength;
                var lines = input.Split(Environment.NewLine);
                Robots = lines.Select(x => new Robot(x, XLength, YLength)).ToList();
            }

            public void OneMove()
            {
                foreach (var robot in Robots)
                {
                    robot.OneMove();
                }
            }

            public int Ch1Counter()
            {
                var xQuadrantSize = XLength / 2;
                var yQuadrantSize = YLength / 2;

                int[] counter = new int[4];

                foreach (var robot in Robots)
                {
                    var x = robot.Pos.X;
                    var y = robot.Pos.Y;

                    if (x < xQuadrantSize && y < yQuadrantSize)
                    {
                        counter[0]++;
                    }
                    else if (x > xQuadrantSize && y < yQuadrantSize)
                    {
                        counter[1]++;
                    }
                    else if (x < xQuadrantSize && y > yQuadrantSize)
                    {
                        counter[2]++;
                    }
                    else if (x > xQuadrantSize && y > yQuadrantSize)
                    {
                        counter[3]++;
                    }
                }

                var res = 1;
                foreach (var el in counter)
                {
                    res *= el;
                }

                return res;
            }

            internal void DrawCurrent()
            {
                for (int y = 0; y < YLength; y++)
                {
                    string toDrawLine = "";
                    for (int x = 0; x < XLength; x++)
                    {
                        var robot = Robots.FirstOrDefault(robot => robot.Pos.X == x && robot.Pos.Y == y);
                        if (robot != null)
                        {
                            toDrawLine += "#";
                        }
                        else
                        {
                            toDrawLine += ".";
                        }
                    }
                    Console.WriteLine(toDrawLine);
                }
                Console.WriteLine();
            }
        }

        public class Robot
        {
            public int XLength { get; set; }
            public int YLength { get; set; }

            public Vector2Int Pos { get; set; }
            public Vector2Int Direction { get; set; }

            public Robot(string instr, int xLength, int yLength)
            {
                XLength = xLength;
                YLength = yLength;

                var split = instr.Split(" v=");

                var dirSplit = split[1].Split(",");

                Direction = new(int.Parse(dirSplit[0]), int.Parse(dirSplit[1]));

                var posSplit = split[0].Split("=")[1].Split(",");

                Pos = new(int.Parse(posSplit[0]), int.Parse(posSplit[1]));
            }

            public void OneMove()
            {
                Pos += Direction;

                if (Pos.X < 0 || Pos.Y < 0 || Pos.X >= XLength || Pos.Y >= YLength)
                {
                    var newX = (Pos.X + XLength) % XLength;
                    var newY = (Pos.Y + YLength) % YLength;
                    Pos = new(newX, newY);
                }
            }
        }
    }
}
