using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day10 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var lines = input.Split(Environment.NewLine);

            var map = new TrailMap();
            map.Map = new int[lines[0].Length, lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    map.Map[j, i] = int.Parse(lines[i][j].ToString());
                }
            }

            var startingPoints = map.GetTrackStartingPoints();
            var amountOfTrails = map.CountTrialAmountOfTrailsForStartingPoints_Individual(startingPoints);

            return amountOfTrails.ToString();
        }

        public string Ch2(string input)
        {
            var lines = input.Split(Environment.NewLine);

            var map = new TrailMap();
            map.Map = new int[lines[0].Length, lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    map.Map[j, i] = int.Parse(lines[i][j].ToString());
                }
            }

            var startingPoints = map.GetTrackStartingPoints();
            var amountOfTrails = map.CountTrialAmountOfTrailsForStartingPoints(startingPoints);

            return amountOfTrails.ToString();
        }

        public class TrailMap
        {
            public int[,] Map { get; set; }

            internal int CountTrialAmountOfTrailsForStartingPoints_Individual(List<(int X, int Y)> startingPoints)
            {
                int count = 0;

                foreach (var pos in startingPoints)
                {
                    var endPoints = new HashSet<(int X, int Y)>();
                    CountWayUp_Individual(pos, ref endPoints);
                    count += endPoints.Count;
                }

                return count;
            }

            private void CountWayUp_Individual((int X, int Y) pos, ref HashSet<(int X, int Y)> endPoints)
            {
                var currVal = Map[pos.X, pos.Y];
                if (currVal == 9)
                {
                    endPoints.Add(pos);
                    return;
                }

                var currValPlus1 = currVal + 1;

                if (pos.X > 0 && Map[pos.X - 1, pos.Y] == currValPlus1)
                    CountWayUp_Individual((pos.X - 1, pos.Y), ref endPoints);
                if (pos.Y > 0 && Map[pos.X, pos.Y - 1] == currValPlus1)
                    CountWayUp_Individual((pos.X, pos.Y - 1), ref endPoints);
                if (pos.X < Map.GetLength(0) - 1 && Map[pos.X + 1, pos.Y] == currValPlus1)
                    CountWayUp_Individual((pos.X + 1, pos.Y), ref endPoints);
                if (pos.Y < Map.GetLength(1) - 1 && Map[pos.X, pos.Y + 1] == currValPlus1)
                    CountWayUp_Individual((pos.X, pos.Y + 1), ref endPoints);
            }
            internal int CountTrialAmountOfTrailsForStartingPoints(List<(int X, int Y)> startingPoints)
            {
                int count = 0;
                foreach (var pos in startingPoints)
                {
                    var trailScore = CountWayUp(pos);
                    count += trailScore;
                }
                return count;
            }

            private int CountWayUp((int X, int Y) pos)
            {
                var currVal = Map[pos.X, pos.Y];
                if (currVal == 9)
                    return 1;

                var currValPlus1 = currVal + 1;

                int count = 0;

                if (pos.X > 0 && Map[pos.X - 1, pos.Y] == currValPlus1)
                    count += CountWayUp((pos.X - 1, pos.Y));
                if (pos.Y > 0 && Map[pos.X, pos.Y - 1] == currValPlus1)
                    count += CountWayUp((pos.X, pos.Y - 1));
                if (pos.X < Map.GetLength(0) - 1 && Map[pos.X + 1, pos.Y] == currValPlus1)
                    count += CountWayUp((pos.X + 1, pos.Y));
                if (pos.Y < Map.GetLength(1) - 1 && Map[pos.X, pos.Y + 1] == currValPlus1)
                    count += CountWayUp((pos.X, pos.Y + 1));

                return count;
            }

            internal List<(int X, int Y)> GetTrackStartingPoints()
            {
                var startingPoints = new List<(int X, int Y)>();
                for (int i = 0; i < Map.GetLength(0); i++)
                {
                    for (int j = 0; j < Map.GetLength(1); j++)
                    {
                        if (Map[i, j] == 0)
                            startingPoints.Add((i, j));
                    }
                }
                return startingPoints;
            }
        }
    }
}
