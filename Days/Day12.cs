using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static AoC24.Days.Day12;

namespace AoC24.Days
{
    public class Day12 : IChallengeYouToADanceOff
    {
        public string Ch1(string input)
        {
            var farm = new Farm(input);

            farm.InitAreas();
            return farm.CalculateCost().ToString();
        }

        public string Ch2(string input)
        {
            var farm = new Farm(input);

            farm.InitAreas();
            return farm.CalculateCostV2().ToString();
        }

        public class Farm
        {
            public FieldCoord[,] Field { get; init; }

            public List<FieldArea> AllFieldAreas = new();
            public Dictionary<(int x, int y), FieldArea> PosFieldMap;

            public Farm(FieldCoord[,] field)
            {
                Field = field;
            }

            public Farm(string input)
            {
                var lines = input.Split(Environment.NewLine);
                Field = new FieldCoord[lines[0].Length, lines.Length];
                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[0].Length; x++)
                    {
                        Field[x, y] = new(lines[y][x]);
                    }
                }
            }

            public List<FieldArea> InitAreas()
            {
                PosFieldMap = new();

                for (int x = 0; x < Field.GetLength(0); x++)
                {
                    for (int y = 0; y < Field.GetLength(1); y++)
                    {
                        var ch = Field[x, y];

                        if (ch.AlreadyAssigned)
                            continue;

                        var area = new FieldArea() { CharPlant = ch.PlantType };
                        AllFieldAreas.Add(area);

                        area.FindAllNeighbours((x, y), Field, PosFieldMap);
                    }
                }

                return AllFieldAreas;
            }

            public long CalculateCost()
            {
                long totalCost = 0;
                foreach (var area in AllFieldAreas)
                {
                    totalCost += area.CalculateCost();
                }
                return totalCost;
            }

            public long CalculateCostV2()
            {
                long totalCost = 0;
                foreach (var area in AllFieldAreas)
                {
                    totalCost += area.CalculateCostV2(this);
                }
                return totalCost;
            }
        }

        public class FieldArea
        {
            public char CharPlant { get; set; }
            public List<Coord> Coords { get; set; } = new();

            public int CalculateCost()
            {
                var area = Coords.Count;
                var perimeter = CalculatePerimeter();
                return area * perimeter;
            }

            private int CalculatePerimeter()
            {
                int perimeter = 0;
                foreach (var coord in Coords)
                {
                    perimeter += 4 - coord.AmountOf_Neighbours_OfSamePlant;
                }
                return perimeter;
            }

            public long CalculateCostV2(Farm originalFarm)
            {
                var area = Coords.Count;
                var sides = Calculate_All_Sides(originalFarm);
                return area * sides;
            }

            public int Calculate_All_Sides(Farm originalFarm)
            {
                int insideSides = Get_Inside_Sides(originalFarm);
                int outsideSides = Get_Outside_Sides();

                return outsideSides + insideSides;
            }

            int? InsideSidesCache = null;
            public int Get_Inside_Sides(Farm originalFarm)
            {
                if (InsideSidesCache.HasValue)
                    return InsideSidesCache.Value;

                int insideSides = 0;

                var mergedField = GetSelfInnerAndOuter(this, originalFarm.Field.GetLength(0), originalFarm.Field.GetLength(1));
                var newFarm = new Farm(mergedField);
                newFarm.InitAreas();
                var innerAreas = newFarm.AllFieldAreas.Where(x => x.CharPlant == '.');
                insideSides += innerAreas.Sum(x => x.Get_Outside_Sides());

                ResetEdgesThatTouchInnerArea(((int x, int y) val) =>
                {
                    return innerAreas.Any(area => area.Coords.Any(coord => coord.X == val.x && coord.Y == val.y));
                });

                InsideSidesCache = insideSides;
                return insideSides;
            }

            private FieldCoord[,] GetSelfInnerAndOuter(FieldArea field, int xLength, int yLength)
            {
                var allCoords = field.Coords;

                FieldCoord[,] coords = new FieldCoord[xLength, yLength];
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        if (allCoords.Any(c => c.X == x && c.Y == y))
                            coords[x, y] = new FieldCoord('#');
                    }
                }

                for (int x = 0; x < xLength; x++)
                {
                    FlowFill(new([(x, 0)]), coords, 'o');
                    FlowFill(new([(x, yLength - 1)]), coords, 'o');
                }

                for (int y = 0; y < yLength; y++)
                {
                    FlowFill(new([(0, y)]), coords, 'o');
                    FlowFill(new([(xLength - 1, y)]), coords, 'o');
                }

                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        if (coords[x, y] == null)
                            coords[x, y] = new FieldCoord('.');
                    }
                }


                return coords;
            }

            private void FlowFill(Queue<(int x, int y)> toDo, FieldCoord[,] coords, char chType)
            {
                while (toDo.TryDequeue(out var coord))
                {
                    if (coords[coord.x, coord.y] != null)
                        continue;

                    int x = coord.x;
                    int y = coord.y;

                    coords[x, y] = new(chType);

                    if (x > 0 && coords[x - 1, y] == null)
                        toDo.Enqueue((x - 1, y));
                    if (x < coords.GetLength(0) - 1 && coords[x + 1, y] == null)
                        toDo.Enqueue((x + 1, y));
                    if (y > 0 && coords[x, y - 1] == null)
                        toDo.Enqueue((x, y - 1));
                    if (y < coords.GetLength(1) - 1 && coords[x, y + 1] == null)
                        toDo.Enqueue((x, y + 1));
                }
            }

            int? OutsideSidesCache = null;
            public int Get_Outside_Sides()
            {
                if (OutsideSidesCache.HasValue)
                    return OutsideSidesCache.Value;

                int outsideEdges = 0;

                foreach (var coord in Coords)
                {
                    outsideEdges += coord.AmountOfOutsideEdges;
                }

                var outsideSides = (outsideEdges - 4) * 2 + 4;
                OutsideSidesCache = outsideSides;
                return outsideSides;
            }

            internal void FindAllNeighbours((int x, int y) value, FieldCoord[,] field, Dictionary<(int x, int y), FieldArea> posFieldMap)
            {
                HashSet<(int x, int y)> queuedAlready = new();
                Queue<(int x, int y)> toDo = new();
                toDo.Enqueue(value);

                int xLength = field.GetLength(0);
                int yLength = field.GetLength(1);

                while (toDo.TryDequeue(out var coord))
                {
                    queuedAlready.Add(coord);

                    var amountOfNeighbours = 0;

                    var queue = (((int x, int y) newCoord) =>
                    {
                        toDo.Enqueue(newCoord);
                        queuedAlready.Add(newCoord);
                    });

                    int horizontalNeighbours = 0;
                    int verticalNeighbours = 0;

                    if (coord.x > 0 && field[coord.x - 1, coord.y].PlantType == CharPlant)
                    {
                        if (!queuedAlready.Contains((coord.x - 1, coord.y)))
                            queue((coord.x - 1, coord.y));
                        amountOfNeighbours++;
                        horizontalNeighbours++;
                    }

                    if (coord.x < xLength - 1 && field[coord.x + 1, coord.y].PlantType == CharPlant)
                    {
                        if (!queuedAlready.Contains((coord.x + 1, coord.y)))
                            queue((coord.x + 1, coord.y));
                        amountOfNeighbours++;
                        horizontalNeighbours++;
                    }

                    if (coord.y > 0 && field[coord.x, coord.y - 1].PlantType == CharPlant)
                    {
                        if (!queuedAlready.Contains((coord.x, coord.y - 1)))
                            queue((coord.x, coord.y - 1));
                        amountOfNeighbours++;
                        verticalNeighbours++;
                    }

                    if (coord.y < yLength - 1 && field[coord.x, coord.y + 1].PlantType == CharPlant)
                    {
                        if (!queuedAlready.Contains((coord.x, coord.y + 1)))
                            queue((coord.x, coord.y + 1));
                        amountOfNeighbours++;
                        verticalNeighbours++;
                    }

                    int amountOfOutsideEdges = 0; //GetIsEdge(coord, field, amountOfNeighbours);

                    if (verticalNeighbours == 1 && horizontalNeighbours == 1)
                        amountOfOutsideEdges = 1;
                    else if (verticalNeighbours + horizontalNeighbours == 1)
                        amountOfOutsideEdges = 2;
                    else if (verticalNeighbours + horizontalNeighbours == 0)
                        amountOfOutsideEdges = 4;

                    Coords.Add(new Coord { X = coord.x, Y = coord.y, AmountOf_Neighbours_OfSamePlant = amountOfNeighbours, AmountOfOutsideEdges = amountOfOutsideEdges });
                    field[coord.x, coord.y].AlreadyAssigned = true;
                    posFieldMap.Add(coord, this);
                }
            }

            internal void ResetEdgesThatTouchInnerArea(Func<(int x, int y), bool> isInInnerArea)
            {
                foreach (var coord in Coords)
                {
                    if (coord.AmountOfOutsideEdges > 0)
                    {
                        int amountOfInnerAreaTouchingPoints = 0;

                        if (isInInnerArea((coord.X - 1, coord.Y)))
                            amountOfInnerAreaTouchingPoints++;
                        if (isInInnerArea((coord.X + 1, coord.Y)))
                            amountOfInnerAreaTouchingPoints++;
                        if (isInInnerArea((coord.X, coord.Y - 1)))
                            amountOfInnerAreaTouchingPoints++;
                        if (isInInnerArea((coord.X, coord.Y + 1)))
                            amountOfInnerAreaTouchingPoints++;

                        if (amountOfInnerAreaTouchingPoints == 1)
                            coord.AmountOfOutsideEdges--;
                        else if(amountOfInnerAreaTouchingPoints > 1)
                            coord.AmountOfOutsideEdges = 0;
                    }
                }
            }

            public string GetStringInfo(Farm original)
            {
                var outsideSides = Get_Outside_Sides();
                var insideSides = Get_Inside_Sides(original);

                return $"{CharPlant} ~~ Outside: {outsideSides}, Inside: {insideSides}";
            }
        }

        public class Coord
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int AmountOf_Neighbours_OfSamePlant { get; set; }
            public int AmountOfOutsideEdges { get; set; }
        }

        public class FieldCoord
        {
            public FieldCoord(char plantType)
            {
                PlantType = plantType;
            }

            public char PlantType { get; set; }
            public bool AlreadyAssigned { get; set; }
        }
    }
}
