using AoC24.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day20 : IChallengeYouToADanceOff
    {
        Pos[,] Map;
        Vector2Int Start;
        Vector2Int End;

        public string Ch1(string input)
        {
            Init(input);

            NormalRun();

            var totalGoodCheatRuns = CheckShortcuts(2, 100);

            return totalGoodCheatRuns.ToString();
        }

        private int CheckShortcuts(int maxRangeOfShortcuts, int minTimeSafed)
        {
            var currPos = Start;
            int totalGoodCheatRuns = 0;
            
            while (currPos != End)
            {
                var possible = GetPossiblePositionsWithShortcuts(currPos, maxRangeOfShortcuts, minTimeSafed);

                totalGoodCheatRuns += possible;

                Map[currPos.X, currPos.Y].VisitedCheatRun = true;

                if (currPos.X > 0 && !Map[currPos.X - 1, currPos.Y].IsWall && !Map[currPos.X - 1, currPos.Y].VisitedCheatRun)
                {
                    currPos = new Vector2Int(currPos.X - 1, currPos.Y);
                }
                else if (currPos.Y > 0 && !Map[currPos.X, currPos.Y - 1].IsWall && !Map[currPos.X, currPos.Y - 1].VisitedCheatRun)
                {
                    currPos = new Vector2Int(currPos.X, currPos.Y - 1);
                }
                else if (currPos.X < Map.GetLength(0) && !Map[currPos.X + 1, currPos.Y].IsWall && !Map[currPos.X + 1, currPos.Y].VisitedCheatRun)
                {
                    currPos = new Vector2Int(currPos.X + 1, currPos.Y);
                }
                else if (currPos.Y < Map.GetLength(1) && !Map[currPos.X, currPos.Y + 1].IsWall && !Map[currPos.X, currPos.Y + 1].VisitedCheatRun)
                {
                    currPos = new Vector2Int(currPos.X, currPos.Y + 1);
                }
            }

            return totalGoodCheatRuns;
        }

        private int GetPossiblePositionsWithShortcuts(Vector2Int currPos, int maxRangeOfShortcuts, int minTimeSafed)
        {
            var pathStepAtCurrentPos = Map[currPos.X, currPos.Y].PathStep;
            int amountOfPossibleTimeSafes = 0;

            for (int x = -maxRangeOfShortcuts; x <= maxRangeOfShortcuts; x++)
            {
                for (int y = -maxRangeOfShortcuts; y <= maxRangeOfShortcuts; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    if (currPos.X + x < 0 || currPos.X + x >= Map.GetLength(0) || currPos.Y + y < 0 || currPos.Y + y >= Map.GetLength(1))
                        continue;
                    var manhatten = Math.Abs(x) + Math.Abs(y);
                    if (manhatten > maxRangeOfShortcuts)
                        continue;


                    var el1 = Map[currPos.X + x, currPos.Y + y];
                    if (!el1.IsWall && el1.PathStep - pathStepAtCurrentPos - manhatten >= minTimeSafed)
                    {
                        amountOfPossibleTimeSafes++;
                    }

                }
            }

            return amountOfPossibleTimeSafes;
        }

        private void NormalRun()
        {
            var currPos = Start;
            int currentStepCounter = 0;

            while (currPos != End)
            {
                Map[currPos.X, currPos.Y].VisitedNormalRun = true;
                Map[currPos.X, currPos.Y].PathStep = currentStepCounter;

                if (currPos.X > 0 && !Map[currPos.X - 1, currPos.Y].IsWall && !Map[currPos.X - 1, currPos.Y].VisitedNormalRun)
                {
                    currPos = new Vector2Int(currPos.X - 1, currPos.Y);
                }
                else if (currPos.Y > 0 && !Map[currPos.X, currPos.Y - 1].IsWall && !Map[currPos.X, currPos.Y - 1].VisitedNormalRun)
                {
                    currPos = new Vector2Int(currPos.X, currPos.Y - 1);
                }
                else if (currPos.X < Map.GetLength(0) && !Map[currPos.X + 1, currPos.Y].IsWall && !Map[currPos.X + 1, currPos.Y].VisitedNormalRun)
                {
                    currPos = new Vector2Int(currPos.X + 1, currPos.Y);
                }
                else if (currPos.Y < Map.GetLength(1) && !Map[currPos.X, currPos.Y + 1].IsWall && !Map[currPos.X, currPos.Y + 1].VisitedNormalRun)
                {
                    currPos = new Vector2Int(currPos.X, currPos.Y + 1);
                }

                currentStepCounter++;
            }

            Map[End.X, End.Y].PathStep = currentStepCounter;
            Map[End.X, End.Y].VisitedNormalRun = true;
        }

        void Init(string input)
        {
            var lines = input.Split(Environment.NewLine);
            Map = new Pos[lines[0].Length, lines.Length];

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    Map[x, y] = new Pos()
                    {
                        IsWall = line[x] == '#',
                        PathStep = int.MaxValue,
                        VisitedNormalRun = false,
                    };

                    if (line[x] == 'S')
                        Start = new Vector2Int(x, y);
                    else if (line[x] == 'E')
                        End = new Vector2Int(x, y);
                }
            }
        }

        public string Ch2(string input)
        {
            Init(input);

            NormalRun();

            var totalGoodCheatRuns = CheckShortcuts(20, 100);

            return totalGoodCheatRuns.ToString();
        }


        public class Pos
        {
            public bool IsWall { get; set; }
            public int PathStep { get; set; }

            public bool VisitedNormalRun { get; set; }
            public bool VisitedCheatRun { get; set; }
        }
    }
}
