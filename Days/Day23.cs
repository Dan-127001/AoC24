using BenchmarkDotNet.Helpers;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days
{
    public class Day23 : IChallengeYouToADanceOff
    {
        Dictionary<string, Endpoint> Endpoints = new();
        public string Ch1(string input)
        {
            Init(input);

            var allLoopsOf3 = GetLoopsOf(3, "t");

            return allLoopsOf3.Count.ToString();
        }

        private List<Loop> GetLoopsOf(int loop, string withId)
        {
            HashSet<Loop> allLoops = new();

            foreach (var end in Endpoints)
            {
                if (!end.Key.StartsWith(withId))
                    continue;

                var loops = end.Value.CheckIfExistsInDepth(loop - 1, end.Value, "");

                if (loops == null)
                    continue;

                foreach (var item in loops)
                {
                    allLoops.Add(item);
                }
            }

            var allLoopsList = allLoops.DistinctBy(x => x.Endpoints.Select(x => x.Id).OrderBy(x => x).Aggregate((a, b) => a + b)).ToList();

            return allLoopsList;
        }

        private void Init(string input)
        {
            var connections = input.Split(Environment.NewLine);

            foreach (var connection in connections)
            {
                var con = connection.Split("-");

                var endp = GetOrCreate(con[0]);
                var endp1 = GetOrCreate(con[1]);

                endp.Connections.Add(endp1);
                endp1.Connections.Add(endp);
            }
        }

        private Endpoint GetOrCreate(string con)
        {
            if (Endpoints.TryGetValue(con, out var endpo))
                return endpo;

            var newEnd = new Endpoint() { Id = con };
            Endpoints.Add(con, newEnd);
            return newEnd;
        }

        public string Ch2(string input)
        {
            Init(input);

            var loopsOf3 = GetLoopsOf(3, "");

            Loop best = loopsOf3.First();

            int counter = 0;

            //foreach (var loop in loopsOf3)
            //for (int i = 0; i < loopsOf3.Count; i++)
            Parallel.For(0, loopsOf3.Count, (i) =>
            {
                var loop = loopsOf3[i];

                var bestLoop = ExtendLoop(loop, best.Endpoints.Count);

                lock (this)
                {
                    if (bestLoop != null && bestLoop.Endpoints.Count > best.Endpoints.Count)
                        best = bestLoop;

                    counter++;
                    Console.WriteLine(counter);
                }

            });

            return string.Join(",", best.Endpoints.Select(x => x.Id).Order());
        }

        private Loop ExtendLoop(Loop loop, int currentBest)
        {
            Loop bestLoopSOFar = loop.Copy();

            var connectionsToLoop = loop.Endpoints.First().Connections;

            var sec = loop.Endpoints.Skip(1).First();
            var third = loop.Endpoints.Skip(2).First();
            connectionsToLoop = connectionsToLoop.Where(x => !loop.Endpoints.Contains(x) && sec.Connections.Contains(x) && third.Connections.Contains(x)).ToHashSet();

            if (connectionsToLoop.Count + loop.Endpoints.Count < currentBest)
                return loop;

            foreach (var endp in connectionsToLoop)
            {
                if (loop.Endpoints.Contains(endp))
                    continue;

                if (loop.TryAdd(endp))
                {
                    var nextLoop = ExtendLoop(loop, currentBest);

                    if (nextLoop != null && nextLoop.Endpoints.Count > bestLoopSOFar.Endpoints.Count)
                        bestLoopSOFar = nextLoop.Copy();

                    loop.Endpoints.Remove(endp);
                }
            }

            return bestLoopSOFar;
        }

        public class Endpoint
        {
            public string Id { get; set; }
            public HashSet<Endpoint> Connections { get; set; } = new();

            public HashSet<Loop>? CheckIfExistsInDepth(int depth, Endpoint end, string visited)
            {
                if (depth == 0)
                {
                    if (Connections.Contains(end))
                        return [new(this)];
                    else
                        return null;
                }

                visited += Id + ",";

                HashSet<Loop> count = new();
                foreach (var conn in Connections)
                {
                    if (visited.Contains(conn.Id))
                        continue;

                    var loops = conn.CheckIfExistsInDepth(depth - 1, end, visited);

                    if (loops == null)
                        continue;

                    foreach (var item in loops)
                    {
                        item.Endpoints.Add(this);
                        count.Add(item);
                    }
                }

                return count;
            }

            internal bool HasDirectConOrIsSelf(List<Endpoint> toCheck)
            {
                bool didSmth = false;
                foreach (var item in toCheck.ToArray())
                {
                    if (item == this)
                        continue;
                    if (Connections.Contains(item))
                        continue;

                    didSmth = true;
                    toCheck.Remove(item);
                }
                return didSmth;
            }
        }

        public class Loop
        {
            public Loop(Endpoint end)
            {
                Endpoints.Add(end);
            }

            public Loop(HashSet<Endpoint> endpoints)
            {
                //copy
                Endpoints = endpoints.ToHashSet();
            }

            public Loop(HashSet<Endpoint> endpoints, Endpoint end)
            {
                Endpoints = endpoints.Append(end).ToHashSet();
            }

            public HashSet<Endpoint> Endpoints { get; set; } = new();

            internal Loop Copy()
            {
                return new(Endpoints);
            }

            internal bool TryAdd(Endpoint endp)
            {
                if (Endpoints.All(x => x.Connections.Contains(endp)))
                {
                    Endpoints.Add(endp);
                    return true;
                }

                return false;
            }
        }
    }

}
