using AoC24.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day18 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var field = InitField(input, 1024);

            var start = new Vector2Int(0, 0);
            var end = new Vector2Int(field.Area.GetLength(0) - 1, field.Area.GetLength(1) - 1);

            var path = field.FindShortestPathSteps(start, end);

            return path.ToString();
        }

        public string Ch2(string input)
        {
            int currAmountOfLines = 1024;
            while (true)
            {
                var field = InitField(input, currAmountOfLines);

                var start = new Vector2Int(0, 0);
                var end = new Vector2Int(field.Area.GetLength(0) - 1, field.Area.GetLength(1) - 1);

                var path = field.FindShortestPathSteps(start, end);

                if (path == int.MaxValue)
                    break;

                currAmountOfLines++;
            }

            return GetCoordinatesAtLine(input, currAmountOfLines - 1).ToString();
        }

        private string GetCoordinatesAtLine(string input, int index)
        {
            var lines = input.Split(Environment.NewLine);
            return lines[index];
        }

        private Field InitField(string input, int? maxLines = null)
        {
            var lines = input.Split(Environment.NewLine);

            lines = lines.Take(maxLines ?? lines.Length).ToArray();

            int sizeOfGrid = lines.Length > 40 ? 71 : 7;

            var hashSet = new HashSet<Vector2Int>();
            foreach (var line in lines)
            {
                var split = line.Split(",");
                hashSet.Add(new Vector2Int(int.Parse(split[0]), int.Parse(split[1])));
            }

            Field field = new Field();
            field.Area = new PosElement[sizeOfGrid, sizeOfGrid];

            for (int x = 0; x < sizeOfGrid; x++)
            {
                for (int y = 0; y < sizeOfGrid; y++)
                {
                    var pos = new Vector2Int(x, y);
                    field.Area[x, y] = new PosElement { Position = pos, IsWall = hashSet.Contains(pos) };
                }
            }

            return field;
        }

    }

    public class Field
    {
        public PosElement[,] Area { get; set; }

        Vector2Int[] AllDirections = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        internal int FindShortestPathSteps(Vector2Int start, Vector2Int end)
        {
            var startingPoint = Area[start.X, start.Y];
            startingPoint.CurrentSteps = 0;

            HashSet<PosElement> toDo = [startingPoint];

            while (TryGetNextToCheck(toDo, end, out var nextToCheck))
            {
                Debug.Assert(nextToCheck != null);

                if (nextToCheck.Position == end)
                    break;

                foreach (var dir in AllDirections)
                {
                    int x = nextToCheck.Position.X + dir.X;
                    int y = nextToCheck.Position.Y + dir.Y;

                    if (x < 0 || x >= Area.GetLength(0) || y < 0 || y >= Area.GetLength(1))
                        continue;

                    var el = Area[x, y];

                    if (el.IsWall || el.CurrentSteps <= nextToCheck.CurrentSteps + 1)
                        continue;

                    el.CurrentSteps = nextToCheck.CurrentSteps + 1;
                    toDo.Add(el);
                }
            }

            var endPoint = Area[end.X, end.Y];
            return endPoint.CurrentSteps;
        }

        private bool TryGetNextToCheck(HashSet<PosElement> toDo, Vector2Int end, out PosElement? nextToCheck)
        {
            if (toDo.Count == 0)
            {
                nextToCheck = null;
                return false;
            }

            nextToCheck = toDo.MinBy(x => x.CurrentSteps + x.Heuristic(end));
            toDo.Remove(nextToCheck!);
            return true;
        }
    }

    public class PosElement
    {
        public Vector2Int Position { get; set; }
        public bool IsWall { get; set; }

        public int CurrentSteps { get; set; } = int.MaxValue;
        public int Heuristic(Vector2Int end)
        {
            return Math.Abs(Position.X - end.X) + Math.Abs(Position.Y - end.Y);
        }
    }
}
