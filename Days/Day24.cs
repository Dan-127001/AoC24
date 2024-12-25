using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day24 : IChallengeYouToADanceOff
{
    public Dictionary<string, Gate> Gates { get; set; } = new();

    public string Ch1(string input)
    {
        CaclulateThrough(input);

        var zVals = Gates.Where(x => x.Key.StartsWith("z")).OrderByDescending(x => x.Key).Select(x => x.Value.CurrentValue);
        Debug.Assert(zVals.All(x => x != null));

        //byte to string
        var result = string.Join("", zVals.Select(x => x.Value ? "1" : "0"));
        //byte to decimal number
        var decimalResult = Convert.ToInt64(result, 2);
        Debug.Assert(decimalResult > 0);
        return decimalResult.ToString() + "  " + result;
    }

    public string Ch2(string input)
    {
        var zGates = Gates.Where(x => x.Key.StartsWith("z")).OrderBy(x => x.Key).ToList();


        //var possibleAnchor = GetPossibleAnchor(zGates);
        //var swapped7 = SwapGateForId(zGates, 7);
        //return "";

        PrintLeftRightOperators(zGates);



        //DisplayAllGateConnections(zGates);
        HashSet<(Gate s1, Gate s2)> swapSets = new();

        SwapRec(zGates, swapSets, zGates.Count, 0);

        //DisplayAllGateConnections(zGates);


        return string.Join(",", swapSets.SelectMany(x => new string[] { x.s1.Id, x.s2.Id }).Order());
    }

    private void SwapRec(List<KeyValuePair<string, Gate>> zGates, HashSet<(Gate s1, Gate s2)> swapSets, int latestpossibleAnchorsAmount, int lastAnchor)
    {
        var possibleAnchors = GetPossibleAnchors(zGates, swapSets.Count == 4);
        //if (possibleAnchors.Count > latestpossibleAnchorsAmount)
        //    return;

        foreach (var possibleAnchor in possibleAnchors)
        {
            //if (possibleAnchor < lastAnchor)
            //    continue;

            var swappeds = SwapGateForId(zGates, possibleAnchor);
            foreach (var swapped in swappeds)
            {
                swapSets.Add((swapped.swapped, swapped.swapped1));

                if (AllXY(zGates, zGates.Count - 1) && CalculateCorrect())
                {
                    Console.WriteLine("!!! found " + FormatOutput(swapSets));
                    DisplayAllGateConnections(zGates);
                    return;
                }
                //Debug.Assert(false);

                if (swapSets.Count < 4)
                    SwapRec(zGates, swapSets, possibleAnchors.Count, possibleAnchor);


                UndoSwap(swapped.swapped, swapped.swapped1);

                swapSets.Remove((swapped.swapped, swapped.swapped1));
            }
        }
    }

    private bool FinalAllXY(List<KeyValuePair<string, Gate>> zGates, int anchor)
    {
        if (!AllXY(zGates, anchor))
            return false;

        //for (int anch = 5; anch < anchor; anch++)
        //{
        //    var gateCurrToCheck = zGates[anch].Value;

        //    var leftLeft = gateCurrToCheck.Left.Left;
        //    var leftRight = gateCurrToCheck.Left.Right;

        //    var rightLeft = gateCurrToCheck.Right.Left;
        //    var rightRight = gateCurrToCheck.Right.Right;

        //    if (leftLeft != null && leftLeft.Operation != GateOp.And)
        //        return false;

        //    if (leftRight != null && leftRight.Operation != GateOp.And)
        //        return false;

        //    if (rightLeft != null && rightLeft.Operation != GateOp.And)
        //        return false;

        //    if (rightRight != null && rightRight.Operation != GateOp.And)
        //        return false;
        //}

        if (!CalculateCorrect())
            return false;

        return true;
    }

    private bool CalculateCorrect()
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

        Queue<Gate> Calc = new Queue<Gate>();
        foreach (var xGate in xGates)
        {
            Calc.Enqueue(xGate);
        }

        foreach (var yGate in yGates)
        {
            Calc.Enqueue(yGate);
        }

        while (Calc.TryDequeue(out var gate))
        {
            foreach (var item in gate.CalulateRec())
            {
                if (item.CurrentValue == null)
                    Calc.Enqueue(item);
            }
        }


        string zString = "";
        foreach (var gate in zGates.OrderByDescending(x => x.OriginalId_DontUseNormally))
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

    private string FormatOutput(HashSet<(Gate s1, Gate s2)> swapSets)
    {
        return string.Join(",", swapSets.SelectMany(x => new Gate[] { x.s1, x.s2 }).Select(x => x.Id).Order());
    }

    private static void PrintLeftRightOperators(List<KeyValuePair<string, Gate>> zGates)
    {
        for (int i = 0; i < 15; i++)
        {
            var left = zGates[i].Value.Left;
            var right = zGates[i].Value.Right;
            Console.WriteLine($"{left.Operation} ({left.Id}) ~ {right.Operation} ({right.Id})");

            var leftLeft = left.Left;
            var leftRight = left.Right;

            var rightLeft = right.Left;
            var rightRight = right.Right;
            if (leftLeft != null)
                Console.WriteLine($"{leftLeft.Operation} ({leftLeft.Id}) ~ {leftRight.Operation} ({leftRight.Id})");
            if (rightLeft != null)
                Console.WriteLine($"{rightLeft.Operation} ({rightLeft.Id}) ~ {rightRight.Operation} ({rightRight.Id})");

            Console.WriteLine("+++");
        }
    }

    private HashSet<int> GetPossibleAnchors(List<KeyValuePair<string, Gate>> zGates, bool all)
    {

        HashSet<int> possibleAnchors = new();

        for (int i = 0; i < zGates.Count; i++)
        {
            var el = zGates[i].Value;

            if (!all)
            {
                if (el.Operation != GateOp.Xor)
                    possibleAnchors.Add(i);
            }
            else
            {
                List<Gate> involvedGates = el.GetInvolvedGates(['x', 'y']);
                if (!AllXY(zGates, i))
                    possibleAnchors.Add(i);
                //Console.WriteLine(i + " -> " + string.Join(", ", involvedGates.Select(x => x.Key.Id)));

                foreach (var invGate in involvedGates)
                {
                    if (int.TryParse(invGate.Id.Substring(1), out var num) && (num > i))
                        possibleAnchors.Add(i);
                }
            }
        }

        return possibleAnchors;
    }

    private IEnumerable<(Gate swapped, Gate swapped1)> SwapGateForId(List<KeyValuePair<string, Gate>> zGates, int anchor)
    {
        //List<Gate> involved_TillAnchor = new();
        //for (int i = 0; i < anchor; i++)
        //{
        //    var el = zGates[i].Value;

        //    el.GetInvolvedGates(null, ref involved_TillAnchor, 0);
        //}

        //foreach (var item in involved_TillAnchor.OrderByDescending(x => x.Value))
        //{
        //    Console.WriteLine(item.Key.Id + " -> " + item.Value);
        //}

        //List<KeyValuePair<Gate, int>> tooMuch, missing;
        //GetDiffs(anchor, involved_TillAnchor, involved_Anchor, out tooMuch, out missing);

        //if (!tooMuch.Any())
        //    Console.WriteLine("None too much");
        //foreach (var diff in tooMuch)
        //{
        //    Console.WriteLine("Too much: " + diff.Key.Id);
        //}

        //if (!missing.Any())
        //    Console.WriteLine("None missing");
        //foreach (var miss in missing)
        //{
        //    Console.WriteLine("Missing: " + miss.Key.Id);
        //}

        //Dictionary<Gate, int> possibleSwaps = new();
        //foreach (var gate in involved_TillAnchor.Keys)
        //{
        //    Dictionary<Gate, int> involved = new();
        //    gate.GetInvolvedGates(null, ref involved, 0);

        //    var amountOfMatch = involved_TillAnchor.Where(x => involved.ContainsKey(x.Key)).Count();
        //    possibleSwaps.Add(gate, amountOfMatch);
        //}

        var anchorGate = zGates[anchor].Value;
        var involved_Anchor = anchorGate.GetInvolvedGates(null);

        //swap
        foreach (var gateSw2 in involved_Anchor.Where(x => x.OverrideGate == null).ToList())
        {
            List<Gate> swapList;

            if (anchor > 3 && gateSw2 == anchorGate.Left)
                swapList = Gates.Where(x => x.Value.OverrideGate == null).Select(x => x.Value).ToList();
            else if (anchor > 3 && gateSw2 == anchorGate.Right)
                swapList = Gates.Where(x => x.Value.OverrideGate == null).Select(x => x.Value).ToList();
            else
                swapList = Gates.Where(x => x.Value.OverrideGate == null).Select(x => x.Value).ToList();

            foreach (var possibleSwap in swapList)
            {
                if (possibleSwap == gateSw2)
                    continue;

                Swap(possibleSwap, gateSw2);

                //gate7.GetInvolvedGates(null, ref involved_Anchor, 0);

                //GetDiffs(anchor, involved_TillAnchor, involved_Anchor, out tooMuch, out missing);

                //if (/*!missing.Any() && */AllXY(zGates, anchor))
                //{
                //Console.WriteLine("Swapped: " + possibleSwap.Id + " -> " + gateSw2.Key.Id);
                if (AllXY(zGates, anchor))
                    yield return (gateSw2, possibleSwap);
                else
                {
                    UndoSwap(possibleSwap, gateSw2);
                }
                //}
            }

            //check -> 4 too much is okay and none missing
        }
    }

    private static bool AllXY(List<KeyValuePair<string, Gate>> zGates, int anchor)
    {
        for (int anch = 0; anch <= anchor; anch++)
        {
            var gateCurrToCheck = zGates[anch].Value;

            if (gateCurrToCheck.Locked)
                continue;

            if (gateCurrToCheck.Operation != GateOp.Xor)
                return false;

            if (anch > 2)
            {
                if ((gateCurrToCheck.Left.Operation != GateOp.Xor && gateCurrToCheck.Left.Operation != GateOp.Or) || (gateCurrToCheck.Left.Operation != GateOp.Xor && gateCurrToCheck.Left.Operation != GateOp.Or))
                    return false;
            }


            var involved_Anchor = gateCurrToCheck.GetInvolvedGates(['x', 'y']);

            if (involved_Anchor.Count != (anch + 1) * 2)
                return false;

            HashSet<int> foundX = new();
            HashSet<int> foundY = new();

            foreach (var inv in involved_Anchor)
            {
                if (int.TryParse(inv.Id.Substring(1), out var num))
                {
                    if (inv.Id.StartsWith('x'))
                        foundX.Add(num);
                    else if (inv.Id.StartsWith('y'))
                        foundY.Add(num);
                }
            }

            for (int i = 0; i <= anch; i++)
            {
                if (!foundX.Contains(i))
                    return false;
                if (!foundY.Contains(i))
                    return false;

                foundX.Remove(i);
                foundY.Remove(i);
            }

            if (foundX.Any())
                return false;
            if (foundY.Any())
                return false;

            foreach (var item in involved_Anchor)
            {
                item.Locked = true;
            }
            gateCurrToCheck.Locked = true;
        }

        return true;
    }

    private static void UndoSwap(Gate key1, Gate key2)
    {
        key1.OverrideGate = null;
        key2.OverrideGate = null;
    }

    private static void Swap(Gate key1, Gate key2)
    {
        key1.OverrideGate = key2;
        key2.OverrideGate = key1;
    }

    private static void GetDiffs(int anchor, Dictionary<Gate, int> involvedTillAnchor, Dictionary<Gate, int> involvedAnchor, out List<KeyValuePair<Gate, int>> toMuch, out List<KeyValuePair<Gate, int>> missing)
    {
        toMuch = involvedAnchor.Where(x => !involvedTillAnchor.ContainsKey(x.Key) && !IsExpectedForAnchor(anchor, x.Key)).ToList();
        missing = involvedTillAnchor.Where(x => !involvedAnchor.ContainsKey(x.Key) && !x.Key.Id.StartsWith('z')).ToList();
    }

    private static bool IsExpectedForAnchor(int anchor, Gate key)
    {
        if (int.TryParse(key.Id.Substring(1), out var num))
            return num <= anchor;

        return false;
    }

    private static void DisplayAllGateConnections(List<KeyValuePair<string, Gate>> zGates)
    {

        //display all gates
        for (int i = 0; i < zGates.Count; i++)
        {
            var el = zGates[i].Value;
            List<Gate> involvedGates = el.GetInvolvedGates(['x', 'y']);

            Console.WriteLine(i + " -> " + string.Join(", ", involvedGates.Select(x => x.Id).Order()));
        }
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
                output.CurrentValue = output.GetResult(output.Left.CurrentValue.Value, output.Right.CurrentValue.Value);
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


    public enum GateOp : int { And, Or, Xor };
    public class Gate
    {
        private string id;
        private GateOp operation;
        private Gate left;
        private Gate right;
        private HashSet<Gate> outputs = new();

        public Gate? OverrideGate
        {
            get => overrideGate; set
            {
                overrideGate = value;
                InvolvedGates = null;
                InvolvedGatesXY = null;
            }
        }

        public string OriginalId_DontUseNormally => id;
        public required string Id { get => OverrideGate?.id ?? id; set => id = value; }
        public GateOp Operation { get => OverrideGate?.operation ?? operation; set => operation = value; }

        public Gate Left { get => OverrideGate?.left ?? left; set => left = value; }
        public Gate Right { get => OverrideGate?.right ?? right; set => right = value; }

        public HashSet<Gate> Outputs
        {
            get => OverrideGate?.outputs ?? outputs; set
            {
                outputs = value;
            }
        }
        public bool? CurrentValue
        {
            get => OverrideGate?.currentValue ?? currentValue; set
            {
                if (OverrideGate != null)
                    OverrideGate.currentValue = value;
                else
                    currentValue = value;
            }
        }
        public bool Locked { get => OverrideGate?.locked ?? locked; set => locked = value; }

        public bool? GetResult(bool left, bool right)
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

        internal void AddOutput(Gate outputGate)
        {
            Outputs.Add(outputGate);
        }

        List<Gate>? InvolvedGates;
        List<Gate>? InvolvedGatesXY;
        private bool locked;
        private Gate? overrideGate;
        private bool? currentValue;

        internal List<Gate> GetInvolvedGates(List<char>? toCheck)
        {
            if (toCheck == null)
            {
                if (InvolvedGates == null)
                {
                    var involvedGates = new List<Gate>();
                    GetInvolvedGates(ref involvedGates);
                    InvolvedGates = involvedGates;
                }
                return InvolvedGates;
            }
            else
            {
                if (InvolvedGatesXY == null)
                {
                    var involvedGates = new List<Gate>();
                    GetInvolvedGates(ref involvedGates);
                    InvolvedGatesXY = involvedGates.Where(x => toCheck.Any(y => x.id.StartsWith(y))).ToList();
                }
                return InvolvedGatesXY;
            }
        }

        internal void GetInvolvedGates(ref List<Gate> involvedGates)
        {
            if (involvedGates.Contains(this))
            {
                return;
            }
            else
            {
                involvedGates.Add(this);
            }

            if (Left != null && !involvedGates.Contains(Left))
                Left.GetInvolvedGates(ref involvedGates);

            if (Right != null && !involvedGates.Contains(Right))
                Right.GetInvolvedGates(ref involvedGates);
        }

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

        internal HashSet<Gate> CalulateRec()
        {
            if ((Left?.CurrentValue) != null && (Right?.CurrentValue) != null)
            {
                CurrentValue = GetResult(Left.CurrentValue.Value, Right.CurrentValue.Value);
            }

            return Outputs;
        }
    }
}
