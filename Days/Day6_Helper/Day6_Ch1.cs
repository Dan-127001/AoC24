using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days.Day6_Helper.Ch1
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
