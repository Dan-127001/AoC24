using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AoC24.Days.Day6_Ch1
{
    public class Day6_Ch1
    {
        public string Ch1(string input)
        {
            Board board = InitBoard(input);

            while (board.Is_Guard_Still_In_Area)
            {
                board.GuardMoveOneStep();
            }

            int sum = 0;
            for (int x = 0; x < board.Area.GetLength(0); x++)
            {
                for (int y = 0; y < board.Area.GetLength(1); y++)
                {
                    if (board.Area[x, y].Visited)
                        sum++;
                }
            }

            return sum.ToString();
        }

        private static Board InitBoard(string input)
        {
            Board board = new Board();

            var lines = input.Split(Environment.NewLine);

            board.Area = new Pos[lines[0].Length, lines.Length];

            for (int yPos = 0; yPos < lines.Length; yPos++)
            {
                var line = lines[yPos];

                for (int xPos = 0; xPos < line.Length; xPos++)
                {
                    var el = line[xPos];

                    board.Area[xPos, lines.Length - yPos - 1] = new Pos(el);

                    if (el == '^')
                        board.Guard = new Guard() { Direction = (0, 1), X = xPos, Y = lines.Length - yPos - 1 };
                }
            }

            PrintBoard(board);

            return board;
        }

        private static void PrintBoard(Board board)
        {
            for (int y = board.Area.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < board.Area.GetLength(0); x++)
                {
                    Console.Write(board.Area[x, y].HasObstacle ? "#" : board.Area[x, y].Visited ? "^" : ".");
                }
                Console.WriteLine();
            }
        }
    }

    public class Pos
    {
        public Pos(char x)
        {
            if (x == '#')
            {
                HasObstacle = true;
            }
            else if (x == '^')
            {
                Visited = true;
            }
        }

        public bool HasObstacle { get; set; }
        public bool Visited { get; set; }
    }

    public class Board
    {
        public Pos[,] Area { get; set; }

        public Guard Guard { get; set; }

        public bool Is_Guard_Still_In_Area => IsCoordinatesInArea(Guard.X, Guard.Y);

        private bool IsCoordinatesInArea(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Area.GetLength(0) && y < Area.GetLength(1);
        }

        public bool IsObstacleInFrontOfGuard => IsCoordinatesInArea(Guard.X + Guard.Direction.X, Guard.Y + Guard.Direction.Y) && Area[Guard.X + Guard.Direction.X, Guard.Y + Guard.Direction.Y].HasObstacle;

        public void GuardMoveOneStep()
        {
            if (IsObstacleInFrontOfGuard)
                TurnRight();

            Guard.X += Guard.Direction.X;
            Guard.Y += Guard.Direction.Y;

            if (Is_Guard_Still_In_Area)
                Area[Guard.X, Guard.Y].Visited = true;
        }

        private void TurnRight()
        {
            Guard.Direction = Guard.Direction switch
            {
                (0, 1) => (1, 0),
                (1, 0) => (0, -1),
                (0, -1) => (-1, 0),
                (-1, 0) => (0, 1),
                _ => throw new Exception()
            };
        }
    }

    public class Guard
    {
        public int X { get; set; }
        public int Y { get; set; }

        public (int X, int Y) Direction { get; set; }
    }
}



namespace AoC24.Days.Day6_Ch2
{
    public class Day6_Ch2
    {
        public async Task<string> Ch2(string input)
        {
            Board originalBoard = InitBoard(input);

            int amountOfLoops = 0;

            //for (int y = 0; y < originalBoard.Area.GetLength(0); y++)
            Parallel.For(0, originalBoard.Area.GetLength(1), y =>
            {
                for (int x = 0; x < originalBoard.Area.GetLength(0); x++)
                {
                    if (originalBoard.Area[x, y].HasObstacle || originalBoard.Area[x, y].Visited)
                        continue;

                    var board = originalBoard.DeepCopy();
                    board.Area[x, y].HasObstacle = true;

                    while (board.Is_Guard_Still_In_Area(board.Guard) && board.Is_Guard_Still_In_Area(board.FastGuard))
                    {
                        board.GuardMoveOneStep(board.FastGuard);
                        board.GuardMoveOneStep(board.FastGuard);

                        board.GuardMoveOneStep(board.Guard);

                        if (board.Guard.X == board.FastGuard.X && board.Guard.Y == board.FastGuard.Y && board.Guard.Direction == board.FastGuard.Direction)
                        {
                            amountOfLoops++;
                            break;
                        }
                    }

                    //if (y > 2)
                    //{
                    //    PrintBoard(board);
                    //    await Task.Delay(100);
                    //}
                }
            });

            return amountOfLoops.ToString();
        }

        private void PrintBoard(Board board)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            for (int y = 0; y < board.Area.GetLength(1); y++)
            {
                for (int x = 0; x < board.Area.GetLength(0); x++)
                {
                    if (board.Area[x, y].HasObstacle)
                        Console.Write("#");
                    else if (board.Area[x, y].Visited)
                        Console.Write("+");
                    else
                        Console.Write(".");
                }
                Console.WriteLine();
            }
            Console.WriteLine("__");
            Console.WriteLine("__");
        }

        private static Board InitBoard(string input)
        {
            Board board = new Board();

            var lines = input.Split(Environment.NewLine);

            board.Area = new Pos[lines[0].Length, lines.Length];

            for (int yPos = 0; yPos < lines.Length; yPos++)
            {
                var line = lines[yPos];

                for (int xPos = 0; xPos < line.Length; xPos++)
                {
                    var el = line[xPos];

                    board.Area[xPos, lines.Length - yPos - 1] = new Pos(el);

                    if (el == '^')
                    {
                        board.Guard = new Guard() { Direction = (0, 1), X = xPos, Y = lines.Length - yPos - 1 };
                        board.FastGuard = new Guard() { Direction = (0, 1), X = xPos, Y = lines.Length - yPos - 1 };
                    }
                }
            }

            return board;
        }
    }

    public class Pos
    {
        public Pos(char x)
        {
            if (x == '#')
            {
                HasObstacle = true;
            }
            else if (x == '^')
            {
                Visited = true;
            }
        }

        public bool HasObstacle { get; set; }
        public bool Visited { get; set; }
    }

    public class Board
    {
        public Pos[,] Area { get; set; }

        public Guard Guard { get; set; }
        public Guard FastGuard { get; set; }

        public bool Is_Guard_Still_In_Area(Guard guard) => IsCoordinatesInArea(guard.X, guard.Y);

        private bool IsCoordinatesInArea(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Area.GetLength(0) && y < Area.GetLength(1);
        }

        public bool IsObstacleInFrontOfGuard(Guard guard) => IsCoordinatesInArea(guard.X + guard.Direction.X, guard.Y + guard.Direction.Y) && Area[guard.X + guard.Direction.X, guard.Y + guard.Direction.Y].HasObstacle;

        public void GuardMoveOneStep(Guard guard)
        {
            while (IsObstacleInFrontOfGuard(guard))
                TurnRight(guard);

            guard.X += guard.Direction.X;
            guard.Y += guard.Direction.Y;

            if (Is_Guard_Still_In_Area(guard))
                Area[guard.X, guard.Y].Visited = true;
        }

        private void TurnRight(Guard guard)
        {
            guard.Direction = guard.Direction switch
            {
                (0, 1) => (1, 0),
                (1, 0) => (0, -1),
                (0, -1) => (-1, 0),
                (-1, 0) => (0, 1),
                _ => throw new Exception()
            };
        }

        public Board DeepCopy()
        {
            Board board = new Board()
            {
                Area = new Pos[Area.GetLength(0), Area.GetLength(1)],
                Guard = new Guard() { Direction = Guard.Direction, X = Guard.X, Y = Guard.Y },
                FastGuard = new Guard() { Direction = FastGuard.Direction, X = FastGuard.X, Y = FastGuard.Y }
            };

            for (int x = 0; x < Area.GetLength(0); x++)
            {
                for (int y = 0; y < Area.GetLength(1); y++)
                {
                    board.Area[x, y] = new Pos(' ');
                    board.Area[x, y].HasObstacle = Area[x, y].HasObstacle;
                    board.Area[x, y].Visited = Area[x, y].Visited;
                }
            }

            return board;
        }
    }

    public class Guard
    {
        public int X { get; set; }
        public int Y { get; set; }

        public (int X, int Y) Direction { get; set; }
    }
}
