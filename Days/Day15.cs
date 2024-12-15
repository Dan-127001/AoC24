using AoC24.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day15 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var split = input.Split(Environment.NewLine + Environment.NewLine);

            var warehouse = new Warehouse(split[0], 1);
            var instructions = split[1].Replace(Environment.NewLine, "").Select(x => new Instruction(x)).ToList();

            foreach (var instr in instructions)
            {
                warehouse.Move(instr);
            }

            long total = 0;
            foreach (var pos in warehouse.Positions)
            {
                if (pos.IsBox)
                    total += pos.Position.Y * 100 + pos.Position.X;
            }

            return total.ToString();
        }

        public string Ch2(string input)
        {
            var split = input.Split(Environment.NewLine + Environment.NewLine);

            var warehouse = new Warehouse(split[0], 2);
            var instructions = split[1].Replace(Environment.NewLine, "").Select(x => new Instruction(x)).ToList();

            foreach (var instr in instructions)
            {
                warehouse.MoveWide(instr);
            }

            //Print(warehouse);

            long total = 0;
            foreach (var pos in warehouse.Positions)
            {
                if (pos.IsBox && pos.PosObstacle.OffsetToOtherPart!.Value.X == 1)
                    total += pos.Position.Y * 100 + pos.Position.X;
            }

            return total.ToString();
        }

        private void Print(Warehouse warehouse)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");

            for (int y = 0; y < warehouse.Positions.GetLength(1); y++)
            {
                for (int x = 0; x < warehouse.Positions.GetLength(0); x++)
                {
                    var pos = warehouse.Positions[x, y];
                    if (pos.IsWall)
                        Console.Write('#');
                    else if (pos.IsBox)
                        Console.Write('O');
                    else if (warehouse.RobotPos.X == x && warehouse.RobotPos.Y == y)
                        Console.Write('@');
                    else
                        Console.Write('.');
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.ReadKey();
        }

        public class Instruction
        {
            public Vector2Int Direction { get; set; }
            public Instruction(char ch)
            {
                Direction = ch switch
                {
                    '^' => Direction = new Vector2Int(0, -1),
                    'v' => Direction = new Vector2Int(0, 1),
                    '<' => Direction = new Vector2Int(-1, 0),
                    '>' => Direction = new Vector2Int(1, 0),
                    _ => throw new Exception("Invalid instruction")
                };
            }
        }

        public class Warehouse
        {
            public Pos[,] Positions { get; set; }
            public Vector2Int RobotPos { get; set; }


            public Warehouse(string wareHouseAreaString, int incrSize)
            {
                var lines = wareHouseAreaString.Split(Environment.NewLine);

                Positions = new Pos[lines[0].Length * incrSize, lines.Length];

                Vector2Int? offsetToOtherPart = incrSize == 2 ? new Vector2Int(1, 0) : null;
                Vector2Int? offsetToOtherPartLeft = incrSize == 2 ? new Vector2Int(-1, 0) : null;

                for (int y = 0; y < lines.Length; y++)
                {
                    var line = lines[y];
                    for (int x = 0; x < line.Length; x++)
                    {
                        Positions[x * incrSize, y] = new Pos(line[x], x * incrSize, y, offsetToOtherPart);
                        if (offsetToOtherPart != null)
                            Positions[x * incrSize + 1, y] = new Pos(line[x], x * incrSize + 1, y, offsetToOtherPartLeft);

                        if (line[x] == '@')
                        {
                            RobotPos = new Vector2Int(x * incrSize, y);
                        }
                    }
                }
            }

            internal void MoveWide(Instruction instr)
            {
                var newPos = RobotPos + instr.Direction;
                var elToPush = Positions[newPos.X, newPos.Y];

                if (elToPush.IsAnyObstacle)
                {
                    var canPush = CanPushObstacleWide(elToPush, instr.Direction);

                    if (canPush)
                    {
                        if (instr.Direction.Y == 0)
                        {
                            Push(elToPush, instr.Direction, null);
                        }
                        else
                        {
                            Debug.Assert(elToPush.PosObstacle.OffsetToOtherPart.HasValue);

                            var nextPos1 = elToPush.Position + elToPush.PosObstacle.OffsetToOtherPart.Value;
                            var nextPosEl1 = Positions[nextPos1.X, nextPos1.Y];

                            HashSet<Vector2Int> alreadyPushed = new();

                            PushWide(elToPush, instr.Direction, null, alreadyPushed);
                            //PushWide(nextPosEl1, instr.Direction, null, alreadyPushed);
                        }
                    }
                    else
                        return;
                }

                RobotPos = newPos;
            }

            internal void Move(Instruction instr)
            {
                var newPos = RobotPos + instr.Direction;
                var newPosEl = Positions[newPos.X, newPos.Y];

                if (newPosEl.IsAnyObstacle)
                {
                    var canPush = CanPushObstacle(newPosEl, instr.Direction);

                    if (canPush)
                        Push(newPosEl, instr.Direction, null);
                    else
                        return;
                }

                RobotPos = newPos;
            }


            private void PushWide(Pos currentPosEl, Vector2Int direction, Pos? switchCurrentWithThis, HashSet<Vector2Int> alreadyPushed)
            {
                if (alreadyPushed.Contains(currentPosEl.Position))
                    return;

                alreadyPushed.Add(currentPosEl.Position);

                Debug.Assert(!currentPosEl.IsWall);

                if (!currentPosEl.IsAnyObstacle && switchCurrentWithThis != null)
                {
                    currentPosEl.PosObstacle = switchCurrentWithThis.PosObstacle;
                    return;
                }

                Debug.Assert(currentPosEl.IsBox);

                var nextPos = currentPosEl.Position + direction;
                var nextPosEl = Positions[nextPos.X, nextPos.Y];

                PushWide(nextPosEl, direction, currentPosEl, alreadyPushed);

                Debug.Assert(currentPosEl.PosObstacle.OffsetToOtherPart.HasValue);

                var nextPos1 = currentPosEl.Position + direction + currentPosEl.PosObstacle.OffsetToOtherPart.Value;
                var nextPosEl1 = Positions[nextPos1.X, nextPos1.Y];

                var currentPos1 = nextPos1 - direction;
                var currentPosEl1 = Positions[currentPos1.X, currentPos1.Y];

                PushWide(nextPosEl1, direction, currentPosEl1, alreadyPushed);

                if (switchCurrentWithThis != null)
                    currentPosEl.PosObstacle = switchCurrentWithThis.PosObstacle;
                else
                    currentPosEl.PosObstacle = new PosObstacle(WarehousePosType.Empty, null);

                currentPosEl1.PosObstacle = new PosObstacle(WarehousePosType.Empty, null);

            }

            private void Push(Pos currentPosEl, Vector2Int direction, Pos? switchCurrentWithThis)
            {
                Debug.Assert(!currentPosEl.IsWall);

                if (!currentPosEl.IsAnyObstacle && switchCurrentWithThis != null)
                {
                    currentPosEl.PosObstacle = switchCurrentWithThis.PosObstacle;
                    return;
                }

                Debug.Assert(currentPosEl.IsBox);

                var nextPos = currentPosEl.Position + direction;
                var nextPosEl = Positions[nextPos.X, nextPos.Y];

                Push(nextPosEl, direction, currentPosEl);

                if (switchCurrentWithThis != null)
                    currentPosEl.PosObstacle = switchCurrentWithThis.PosObstacle;
                else
                    currentPosEl.PosObstacle = new(WarehousePosType.Empty, null);
            }

            private bool CanPushObstacle(Pos newPosEl, Vector2Int direction)
            {
                if (!newPosEl.IsAnyObstacle)
                    return true;

                if (newPosEl.IsWall)
                    return false;

                var nextPos = newPosEl.Position + direction;
                var nextPosEl = Positions[nextPos.X, nextPos.Y];

                return CanPushObstacle(nextPosEl, direction);
            }

            private bool CanPushObstacleWide(Pos currentPosEl, Vector2Int direction)
            {
                if (!currentPosEl.IsAnyObstacle)
                    return true;

                if (currentPosEl.IsWall)
                    return false;

                Debug.Assert(currentPosEl.IsBox);

                var nextPos = currentPosEl.Position + direction;
                var nextPosEl = Positions[nextPos.X, nextPos.Y];

                bool firstCheck = CanPushObstacleWide(nextPosEl, direction);

                if (direction.Y != 0)
                {
                    var nextPos2 = currentPosEl.Position + direction + (currentPosEl.PosObstacle.OffsetToOtherPart ?? new Vector2Int(0, 0));
                    var nextPosEl2 = Positions[nextPos2.X, nextPos2.Y];

                    bool secondCheck = CanPushObstacleWide(nextPosEl2, direction);

                    return firstCheck && secondCheck;
                }

                return firstCheck;
            }

        }

        public enum WarehousePosType : int { Empty = 0, Wall = 1, Box = 2 }
        public record PosObstacle(WarehousePosType WarehousePosType, Vector2Int? OffsetToOtherPart);
        public class Pos
        {
            public Vector2Int Position { get; set; }

            public PosObstacle PosObstacle { get; set; }

            public bool IsWall => PosObstacle.WarehousePosType == WarehousePosType.Wall;
            public bool IsBox => PosObstacle.WarehousePosType == WarehousePosType.Box;

            public bool IsAnyObstacle => IsWall || IsBox;

            public Pos(char ch, int x, int y, Vector2Int? offsetToOtherPart)
            {
                Position = new Vector2Int(x, y);

                if (ch == '#')
                {
                    PosObstacle = new(WarehousePosType.Wall, offsetToOtherPart);
                }
                else if (ch == 'O')
                {
                    PosObstacle = new(WarehousePosType.Box, offsetToOtherPart);
                }
                else
                {
                    PosObstacle = new(WarehousePosType.Empty, offsetToOtherPart);
                }
            }
        }
    }
}
