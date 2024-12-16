using AoC24.Helper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day16 : IChallengeYouToADanceOff
    {
        public Element[,] Elements { get; set; }

        public Vector2Int Right = new Vector2Int(1, 0);
        public Vector2Int Left = new Vector2Int(-1, 0);
        public Vector2Int Up = new Vector2Int(0, -1);
        public Vector2Int Down = new Vector2Int(0, 1);

        public string Ch1(string input)
        {
            return "";
            Vector2Int end = FlowInYou(input);

            var fastest = Elements[end.Y, end.X].Direction_SoFarTrackerMap.Min(x => x.Value.Points);

            return fastest.ToString();
        }

        public string Ch2(string input)
        {
            Vector2Int end = FlowInYou(input);

            var fastest = Elements[end.Y, end.X].Direction_SoFarTrackerMap.Min(x => x.Value.Points);

            var allPos = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

            foreach (var tracker in Elements[end.Y, end.X].Direction_SoFarTrackerMap.Where(x => x.Value.Points == fastest))
            {
                tracker.Value.BeforeLoopAny(allPos);
            }

            return allPos.Keys.Count.ToString();
        }

        private Vector2Int FlowInYou(string input)
        {
            var lines = input.Split(Environment.NewLine);

            Elements = new Element[lines.Length, lines[0].Length];

            Vector2Int start = new Vector2Int(0, 0);
            Vector2Int end = new Vector2Int(0, 0);

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];

                for (int x = 0; x < line.Length; x++)
                {
                    var ch = line[x];
                    Elements[y, x] = new Element
                    {
                        Pos = new Vector2Int(x, y),
                        IsWall = ch == '#',
                    };
                    if (ch == 'E')
                        end = new Vector2Int(x, y);
                    if (ch == 'S')
                        start = new Vector2Int(x, y);
                }
            }

            foreach (var element in Elements)
            {
                var xDiff = Math.Abs(element.Pos.X - end.X);
                var yDiff = Math.Abs(element.Pos.Y - end.Y);

                var atLeastOneEdge = xDiff > 0 && yDiff > 0;
                element.Heuristic = xDiff + yDiff + (atLeastOneEdge ? 1000 : 0);
            }

            List<PosDir> toDo = new();
            toDo.Add(new PosDir(start, Right, 0));
            var startEl = Elements[start.Y, start.X];
            startEl.Direction_SoFarTrackerMap[Right] = new SoFarTracker { Points = 0, Amount = 1, SelfElement = startEl, Dir = Right };

            while (TryGetNextStep(toDo, out var step))
            {
                var el = Elements[step.Pos.Y, step.Pos.X];
                var soFar = el.Direction_SoFarTrackerMap[step.Dir];

                Debug.Assert(soFar != null);

                var nextStraight = step.Pos + step.Dir;
                var currPoints = soFar.Points;
                var currAmount = soFar.Amount;


                AddToQueueIfPossible(step.Pos + Right, Right, currPoints + 1 + Add1kIfNotEq(Right, step.Dir), currAmount, soFar, toDo);
                AddToQueueIfPossible(step.Pos + Left, Left, currPoints + 1 + Add1kIfNotEq(Left, step.Dir), currAmount, soFar, toDo);
                AddToQueueIfPossible(step.Pos + Up, Up, currPoints + 1 + Add1kIfNotEq(Up, step.Dir), currAmount, soFar, toDo);
                AddToQueueIfPossible(step.Pos + Down, Down, currPoints + 1 + Add1kIfNotEq(Down, step.Dir), currAmount, soFar, toDo);
            }

            return end;
        }

        private int Add1kIfNotEq(Vector2Int right, Vector2Int dir)
        {
            return right == dir ? 0 : 1000;
        }

        private bool TryGetNextStep(List<PosDir> toDo, out PosDir step)
        {
            if (toDo.Count == 0)
            {
                step = null!;
                return false;
            }

            step = toDo.MinBy(x => x.PointsSoFar)!;

            toDo.Remove(step);

            return true;
        }

        private void AddToQueueIfPossible(Vector2Int toCheckPos, Vector2Int toCheckDir, int thisPoints, int thisAmount, SoFarTracker before, List<PosDir> toDo)
        {
            if (toCheckPos.X < 0 || toCheckPos.X >= Elements.GetLength(1) || toCheckPos.Y < 0 || toCheckPos.Y >= Elements.GetLength(0))
                return;

            var nextEl = Elements[toCheckPos.Y, toCheckPos.X];

            if (nextEl.IsWall)
                return;

            var nextEl_SoFarTracker = nextEl.Direction_SoFarTrackerMap.GetOrAdd(toCheckDir, new SoFarTracker() { SelfElement = nextEl, Dir = toCheckDir });


            if (thisPoints > nextEl_SoFarTracker.Points)
            {
                return;
            }
            else if (thisPoints == nextEl_SoFarTracker.Points)
            {
                nextEl_SoFarTracker.Amount += thisAmount;
                nextEl_SoFarTracker.Before.Add(before);
            }
            else
            {
                nextEl_SoFarTracker.Points = thisPoints;
                nextEl_SoFarTracker.Amount = thisAmount;
                nextEl_SoFarTracker.Before = [before];
            }

            toDo.Add(new PosDir(toCheckPos, toCheckDir, thisPoints));
        }


        public class Element
        {
            public Vector2Int Pos { get; set; }

            public bool IsWall { get; set; }

            public int Heuristic { get; set; }


            public ConcurrentDictionary<Vector2Int, SoFarTracker> Direction_SoFarTrackerMap { get; set; } = new();
        }

        public class SoFarTracker
        {
            public required Element SelfElement { get; set; }
            public required Vector2Int Dir { get; set; }
            public int Points { get; set; } = int.MaxValue;
            public int Amount { get; set; }

            public List<SoFarTracker> Before { get; set; } = new();

            public void BeforeLoopAny(Dictionary<Vector2Int, HashSet<Vector2Int>> beforePositions)
            {
                var beforePos = beforePositions.GetValueOrDefault(SelfElement.Pos);

                if (beforePos != null)
                {
                    if (beforePos.Contains(Dir))
                        return;

                    beforePos.Add(Dir);
                }
                else
                {
                    beforePositions.Add(SelfElement.Pos, [Dir]);
                }


                foreach (var elBefore in Before)
                {
                    elBefore.BeforeLoopAny(beforePositions);
                }
            }
        }

        public record PosDir(Vector2Int Pos, Vector2Int Dir, int PointsSoFar);
    }


}
