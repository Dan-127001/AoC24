using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AoC24.Days
{
    public class Day24_Alt : IChallengeYouToADanceOff
    {
        public Dictionary<string, Gate> Gates { get; set; } = new();

        public string Ch1(string input)
        {
            CaclulateThrough(input);

            return "";
        }

        public string Ch2(string input)
        {
            List<Gate> gatesToSwap = PrintAndGetGatesToSwap();
            List<Gate> swappedThis = new();

            var amountOfX = Gates.Count(x => x.Key.StartsWith('x'));
            var amountOfY = Gates.Count(x => x.Key.StartsWith('y'));
            var amountOfZ = Gates.Count(x => x.Key.StartsWith('z'));

            foreach (var gate in gatesToSwap.ToList())
            {
                var possibleGates = GetPossibleGates(gate);
                if (possibleGates.Count == 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Swapping");
                    Console.WriteLine();

                    SwapGates(gate, possibleGates.First());
                    swappedThis.Add(gate);
                    swappedThis.Add(possibleGates.First());

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();

                    gatesToSwap = PrintAndGetGatesToSwap();
                }
                else
                {
                    Console.WriteLine("1 left");
                }
            }

            return "";
            gatesToSwap = PrintAndGetGatesToSwap(true);
            Debug.Assert(gatesToSwap.Count == 1);
            var possibleSwaps = Gates.Values.ToList();

            //foreach (var swapMe in Gates.Values)
            for (int i = 0; i < possibleSwaps.Count; i++)
            {
                var swapMe = possibleSwaps[i];
                //var swapMe = gatesToSwap.First();

                //while (gatesToSwap.Count > 0)
                //foreach (var el in possibleSwaps)
                for (int j = 0; j < possibleSwaps.Count; j++)
                {
                    var el = possibleSwaps[j];
                    //var el = possibleSwaps.First();
                    //possibleSwaps.Remove(el);

                    SwapGates(swapMe, el);

                    gatesToSwap = PrintAndGetGatesToSwap(false);

                    if (gatesToSwap.Count > 0)
                    {
                        SwapGates(el, swapMe);
                    }
                    else
                    {
                        swappedThis.Add(el);
                        swappedThis.Add(swapMe);
                        break;
                    }
                }

                Console.WriteLine(i + "/" + possibleSwaps.Count);

                if (swappedThis.Count == 8)
                    break;
            }

            gatesToSwap = PrintAndGetGatesToSwap(true);
            return "";
        }

        private List<Gate> PrintAndGetGatesToSwap(bool print = true)
        {
            List<Gate> gatesToSwap = new();
            var zGates = Gates.Where(x => x.Key.StartsWith('z')).OrderBy(x => x.Key).ToList();

            for (int i = 2; i < zGates.Count - 1; i++)
            {
                var gate = zGates[i];
                var prevGate = zGates[i - 1];
                if (print)
                {
                    Console.WriteLine(gate.Key + " -> " + gate.Value.Operation);

                    var left = gate.Value.Left;
                    var right = gate.Value.Right;

                    if (left != null)
                        Console.WriteLine("_" + left.Id + " -> " + left.Operation);

                    var leftLeft = left?.Left;
                    var leftRight = left?.Right;

                    if (leftLeft != null)
                        Console.WriteLine("__" + leftLeft.Id + " -> " + leftLeft.Operation);
                    if (leftRight != null)
                        Console.WriteLine("__" + leftRight.Id + " -> " + leftRight.Operation);

                    if (right != null)
                        Console.WriteLine("_" + right.Id + " -> " + right.Operation);

                    var rightLeft = right?.Left;
                    var rightRight = right?.Right;

                    if (rightLeft != null)
                        Console.WriteLine("__" + rightLeft.Id + " -> " + rightLeft.Operation);
                    if (rightRight != null)
                        Console.WriteLine("__" + rightRight.Id + " -> " + rightRight.Operation);

                    Console.WriteLine("+++");
                }

                var par = IsParentsMet(gate.Value, i);

                if (gate.Value.Operation != GateOp.Xor || !par)
                {
                    gatesToSwap.Add(gate.Value);
                }
            }
            gatesToSwap = gatesToSwap.OrderBy(x => x.Id).ToList();
            if (print)
                Console.WriteLine("Gates to swap: " + string.Join(", ", gatesToSwap.Select(x => x.Id)));
            return gatesToSwap;
        }

        public void SwapGates(Gate gate1, Gate gate2)
        {
            var leftTemp = gate1.Left;
            var rightTemp = gate1.Right;

            gate1.Left.Outputs.Remove(gate1);
            gate1.Left.Outputs.Add(gate2);
            gate1.Left = gate2.Left;

            gate1.Right.Outputs.Remove(gate1);
            gate1.Right.Outputs.Add(gate2);
            gate1.Right = gate2.Right;

            gate2.Left.Outputs.Remove(gate2);
            gate2.Left.Outputs.Add(gate1);
            gate2.Left = leftTemp;

            gate2.Right.Outputs.Remove(gate2);
            gate2.Right.Outputs.Add(gate1);
            gate2.Right = rightTemp;


            //var outputTemp = gate1.Outputs;

            //gate1.Outputs = gate2.Outputs;
            //gate2.Outputs = outputTemp;

            var operationTemp = gate1.Operation;
            gate1.Operation = gate2.Operation;
            gate2.Operation = operationTemp;

            //var id = gate1.Id;
            //gate1.Id = gate2.Id;
            //gate2.Id = id;
        }

        private HashSet<Gate> GetPossibleGates(Gate toSwap)
        {
            Debug.Assert(toSwap.Id_AsNumber != null);

            HashSet<Gate> gates = new();

            foreach (var gate in Gates)
            {
                if (gate.Value.Operation == GateOp.Xor)
                {
                    if (gate.Value.Left.Operation is GateOp.Or or GateOp.Xor)
                    {
                        if (gate.Value.Right.Operation is GateOp.Or or GateOp.Xor && gate.Value.Right.Id != gate.Value.Left.Id)
                        {
                            if (IsParentsMet(gate.Value, toSwap.Id_AsNumber.Value))
                                gates.Add(gate.Value);
                        }
                    }
                }
            }

            return gates;
        }

        private bool IsParentsMet(Gate gate, int index)
        {
            var parentsXY = gate.GetXYParents();

            if (parentsXY.Count != (index + 1) * 2)
                return false;

            for (int i = 0; i < index + 1; i++)
            {
                if (parentsXY.Count(x => (x.Id.StartsWith('x') || x.Id.StartsWith('y')) && x.Id_AsNumber == i) != 2)
                    return false;
            }

            return true;
        }

        public void CaclulateThrough(string input)
        {
            var split = input.Split(Environment.NewLine + Environment.NewLine);
            var gateLines = split[1].Split(Environment.NewLine);
            foreach (var gateLine in gateLines)
            {
                var lineSplit = gateLine.Split(" -> ");
                var outputGateId = lineSplit[1];
                var outputGate = GetOrCreateOutputGate(outputGateId);

                var inputsSplit = lineSplit[0].Split(" ");

                var op = inputsSplit[1];

                var leftGateId = inputsSplit[0];
                var rightGateId = inputsSplit[2];

                var leftGate = GetOrCreateOutputGate(leftGateId);
                var rightGate = GetOrCreateOutputGate(rightGateId);

                outputGate.SetOperator(op);
                outputGate.SetInputs(leftGate, rightGate);

                leftGate.AddOutput(outputGate);
                rightGate.AddOutput(outputGate);
            }

            var initialSets = split[0].Split(Environment.NewLine);
            foreach (var line in initialSets)
            {
                var lineSplit = line.Split(": ");
                var gateId = lineSplit[0];
                var isSet = lineSplit[1] == "1";

                var setGate = Gates[gateId];
                setGate.CurrentValue = isSet;
                EnqueueNexts(setGate);
            }
        }


        private void EnqueueNexts(Gate setGate)
        {
            foreach (var output in setGate.Outputs)
            {
                if (output.Left.CurrentValue != null && output.Right.CurrentValue != null)
                {
                    output.CurrentValue = output.CalculateCurrentValue(output.Left.CurrentValue.Value, output.Right.CurrentValue.Value);
                    EnqueueNexts(output);
                }
            }
        }

        private Gate GetOrCreateOutputGate(string gateId)
        {
            if (!Gates.TryGetValue(gateId, out var gate))
            {
                gate = new Gate() { Id = gateId };
                Gates.Add(gateId, gate);
            }

            return gate;
        }

        public bool CalculationTest()
        {
            List<Gate> xGates = new();
            List<Gate> yGates = new();
            List<Gate> zGates = new();

            foreach (var gate in Gates)
            {
                gate.Value.CurrentValue = null;

                if (gate.Key.StartsWith("x"))
                    xGates.Add(gate.Value);
                else if (gate.Key.StartsWith("y"))
                    yGates.Add(gate.Value);
                else if (gate.Key.StartsWith("z"))
                    zGates.Add(gate.Value);
            }

            xGates = xGates.OrderBy(x => x.Id).ToList();
            yGates = yGates.OrderBy(x => x.Id).ToList();
            zGates = zGates.OrderBy(x => x.Id).ToList();

            Random r = new Random(123);

            string valueX = "";
            string valueY = "";

            for (int i = 0; i < xGates.Count; i++)
            {
                valueX += r.Next(0, 2);
                xGates[i].CurrentValue = valueX[i] == '1';

                valueY += r.Next(0, 2);
                yGates[i].CurrentValue = valueY[i] == '1';
            }

            long x = Convert.ToInt64(valueX, 2);
            long y = Convert.ToInt64(valueY, 2);

            long z = x + y;

            foreach (var zGate in zGates)
            {
                zGate.CalulateRec();
            }

            //(Calc.TryDequeue(out var gate))
            //{
            //    foreach (var item in gate.CalulateRec())
            //    {
            //        if (item.CurrentValue == null)
            //            Calc.Enqueue(item);
            //    }
            //}


            string zString = "";
            foreach (var gate in zGates.OrderByDescending(x => x.Id_AsNumber))
            {
                if (gate.CurrentValue == null)
                    return false;
                zString += gate.CurrentValue!.Value ? "1" : "0";
            }

            long zFromMachine = Convert.ToInt64(zString, 2);

            if (zFromMachine != z)
            {
                string expectedBinary = Convert.ToString(z, 2);
                Console.WriteLine(expectedBinary + " Expected: " + z + " -> ");
                Console.WriteLine(zString + " Wrong: " + zFromMachine + " -> ");
                return false;
            }

            return true;
        }

        public enum GateOp : int { And, Or, Xor };
        public class Gate
        {
            private string id;

            public required string Id
            {
                get => id; set
                {
                    id = value;
                    Id_AsNumber = int.TryParse(value[1..], out var number) ? number : null;
                }
            }
            public int? Id_AsNumber { get; set; }
            public GateOp Operation { get; set; }
            public Gate Left { get; set; }
            public Gate Right { get; set; }
            public HashSet<Gate> Outputs { get; set; } = new();


            public bool? CurrentValue { get; set; }

            internal void SetInputs(Gate leftGate, Gate rightGate)
            {
                Left = leftGate;
                Right = rightGate;
            }

            internal void SetOperator(string op)
            {
                Operation = op switch
                {
                    "AND" => GateOp.And,
                    "OR" => GateOp.Or,
                    "XOR" => GateOp.Xor,
                    _ => throw new NotImplementedException()
                };
            }

            public bool CalculateCurrentValue(bool left, bool right)
            {
                switch (Operation)
                {
                    case GateOp.And:
                        return left && right;
                    case GateOp.Or:
                        return left || right;
                    case GateOp.Xor:
                        return left ^ right;
                }

                throw new NotImplementedException();
            }

            internal void CalulateRec()
            {
                if (Left.CurrentValue == null)
                    Left.CalulateRec();
                if (Right.CurrentValue == null)
                    Right.CalulateRec();

                if ((Left?.CurrentValue) != null && (Right?.CurrentValue) != null)
                {
                    CurrentValue = CalculateCurrentValue(Left.CurrentValue.Value, Right.CurrentValue.Value);
                }
            }

            internal void AddOutput(Gate outputGate)
            {
                Outputs.Add(outputGate);
            }

            internal HashSet<Gate> GetXYParents()
            {
                Queue<Gate> toDo = new();

                if (Left != null)
                    toDo.Enqueue(Left);

                if (Right != null)
                    toDo.Enqueue(Right);

                HashSet<Gate> parents = new();
                HashSet<Gate> visited = new();

                while (toDo.TryDequeue(out var parentGate))
                {
                    visited.Add(parentGate);
                    if (parentGate.Id.StartsWith('x') || parentGate.Id.StartsWith('y'))
                    {
                        parents.Add(parentGate);
                    }
                    else
                    {
                        if (parentGate.Left != null && !visited.Contains(parentGate.Left))
                            toDo.Enqueue(parentGate.Left);
                        if (parentGate.Right != null && !visited.Contains(parentGate.Right))
                            toDo.Enqueue(parentGate.Right);
                    }
                }

                return parents;
            }
        }
    }
}
