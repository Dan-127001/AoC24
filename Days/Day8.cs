using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day8 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var map = new Map();
            var lines = input.Split(Environment.NewLine);

            map.Area = new Pos[lines.Length, lines[0].Length];

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    map.Area[x, y] = new Pos(line[x], x, y);
                }
            }


            var res = map.CalcAntinodes(false);

            //PrintArea(map.Area);

            return res;
        }

        public string Ch2(string input)
        {
            var map = new Map();
            var lines = input.Split(Environment.NewLine);

            map.Area = new Pos[lines.Length, lines[0].Length];

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    map.Area[x, y] = new Pos(line[x], x, y);
                }
            }


            var res = map.CalcAntinodes(true);

            //PrintArea(map.Area);

            return res;
        }

        private void PrintArea(Pos[,] area)
        {
            for (int y = 0; y < area.GetLength(0); y++)
            {
                for (int x = 0; x < area.GetLength(1); x++)
                {
                    if (area[x, y].Antinodes.Any())
                        Console.Write(area[x, y].Antinodes.Count);
                    else if (area[x, y].AntenntaType != null)
                        Console.Write(area[x, y].AntenntaType);
                    else
                        Console.Write('.');
                }
                Console.WriteLine();
            }
        }



        public class Map
        {
            public Pos[,] Area { get; set; }

            internal string CalcAntinodes(bool multipleTimes)
            {
                var grouped = Area.Cast<Pos>().GroupBy(x => x.AntenntaType);

                foreach (var item in grouped)
                {
                    var arr = item.ToArray();

                    if (arr.Length == 1)
                        continue;

                    for (int i = 0; i < arr.Length; i++)
                    {
                        for (int j = 0; j < arr.Length; j++)
                        {
                            var el = arr[i];
                            if (i == j || el.AntenntaType == null)
                                continue;

                            var rel = el.PosVec - arr[j].PosVec;
                            int run = 1;

                            while (multipleTimes || run == 1)
                            {
                                var newPos = el.PosVec + (rel * run);

                                run++;
                                if (newPos.X < 0 || newPos.Y < 0 || newPos.X >= Area.GetLength(0) || newPos.Y >= Area.GetLength(1))
                                    break;

                                Area[newPos.X, newPos.Y].Antinodes.Add(el.AntenntaType.Value);
                            }
                        }
                    }
                }

                int uniquePosCount = 0;
                for (int y = 0; y < Area.GetLength(0); y++)
                {
                    for (int x = 0; x < Area.GetLength(1); x++)
                    {
                        if (Area[x, y].Antinodes.Any())
                            uniquePosCount++;
                        else if (multipleTimes && Area[x, y].AntenntaType != null)
                            uniquePosCount++;
                    }
                }

                return uniquePosCount.ToString();
            }
        }

        public class Pos
        {
            public Pos(char v, int x, int y)
            {
                if (v == '.')
                    return;

                AntenntaType = v;

                X = x;
                Y = y;
                PosVec = new Vector2Int(x, y);
            }

            public char? AntenntaType { get; set; }
            public List<char> Antinodes { get; set; } = new();
            public int X { get; set; }
            public int Y { get; set; }

            public Vector2Int PosVec { get; set; }
        }

        public struct Vector2Int
        {
            public Vector2Int(int x, int y)
            {
                X = x;
                Y = y;
            }
            public int X { get; set; }
            public int Y { get; set; }
            public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new Vector2Int(a.X + b.X, a.Y + b.Y);
            public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new Vector2Int(a.X - b.X, a.Y - b.Y);

            public static Vector2Int operator *(Vector2Int a, int b) => new Vector2Int(a.X * b, a.Y * b);
        }
    }
}
