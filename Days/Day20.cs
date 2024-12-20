using AoC24.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day20 : IChallengeYouToADanceOff
    {
        Pos[,] Map;
        Vector2Int Start, End;
        public string Ch1(string input)
        {
            Init(input);

            var res = GetNormalLength();
            CheckCheatedOptions(2, 100, res);
        }

        private void CheckCheatedOptions(int jumps, int totalStepsSavedMin, int normalSpeed)
        {
            int length = 0;

            var curr = Start;
            while (curr != End)
            {
                Map[curr.X, curr.Y].Visited = true;

                if (curr.X + 1 < Map.GetLength(0) && Map[curr.X + 1, curr.Y] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X + 1, curr.Y);
                }
                else if (curr.X - 1 >= 0 && Map[curr.X - 1, curr.Y] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X - 1, curr.Y);
                }
                else if (curr.Y + 1 < Map.GetLength(1) && Map[curr.X, curr.Y + 1] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X, curr.Y + 1);
                }
                else if (curr.Y - 1 >= 0 && Map[curr.X, curr.Y - 1] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X, curr.Y - 1);
                }

                length++;
            }

            return length;
        }

        private int GetNormalLength()
        {
            int length = 0;

            var curr = Start;
            while (curr != End)
            {
                Map[curr.X, curr.Y].Visited = true;

                if (curr.X + 1 < Map.GetLength(0) && Map[curr.X + 1, curr.Y] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X + 1, curr.Y);
                }
                else if (curr.X - 1 >= 0 && Map[curr.X - 1, curr.Y] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X - 1, curr.Y);
                }
                else if (curr.Y + 1 < Map.GetLength(1) && Map[curr.X, curr.Y + 1] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X, curr.Y + 1);
                }
                else if (curr.Y - 1 >= 0 && Map[curr.X, curr.Y - 1] is { IsWall: false, Visited: false })
                {
                    curr = new Vector2Int(curr.X, curr.Y - 1);
                }

                length++;
            }

            return length;
        }

        private void Init(string input)
        {
            var lines = input.Split(Environment.NewLine);

            Map = new Pos[lines[0].Length, lines.Length];

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    Map[x, y] = new() { IsWall = lines[y][x] == '#', Visited = false, };

                    if (lines[y][x] == 'S')
                    {
                        Start = new Vector2Int(x, y);
                    }
                    else if (lines[y][x] == 'E')
                    {
                        End = new Vector2Int(x, y);
                    }
                }
            }
        }

        public string Ch2(string input)
        {
            return "";
        }

        public class Pos
        {
            public bool IsWall { get; set; }
            public bool Visited { get; set; }

            public int CurrentLength { get; set; }
        }
    }
}
