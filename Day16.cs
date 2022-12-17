
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II", Result = "1651/1707")]
    class Day16 : IDay
    {
        public string Calc(string input, bool test)
        {
            var tunnelNames = Regex.Matches(input, "Valve ([A-Z]+) has flow rate=(-?\\d+); tunnels? leads? to valves? ([^\n\r]+)")
                .Select(m => m.Groups.Values.Skip(1).Select(v => v.Value).ToArray())
                .Select(m => (n:m[0], f:int.Parse(m[1]), tn:m[2].Split(", ")))
                .ToList();
            var tunnels = tunnelNames
                .Select(m => (f:m.f, t:m.tn.Select(n => tunnelNames.FindIndex(m => m.n == n))))
                .ToList();
            
            int Run(int maxMins, int numPos)
            {
                var attempts = Enumerable.Repeat((pos:Enumerable.Repeat(tunnelNames.FindIndex(m => m.n == "AA"), numPos).ToArray(), open: tunnels.Select((t, i) => (t.f == 0) ? (long)1 << i : (long)0).Sum(), vented: 0, venting: 0, visited: Enumerable.Repeat((long)0, numPos).ToArray()), 1).ToList();
                for (var min = maxMins - 1; min > 0; --min)
                {
                    for (var posIndex = 0; posIndex < numPos; ++posIndex)
                    {
                        var newAttempts = new List<(int[] pos, long open, int vented, int venting, long[] visited)>();
                        foreach (var attempt in attempts)
                        {
                            T[] ReplaceAtIndex<T>(T[] a, Func<T,T> f) { return a.Select((v, i) => i == posIndex ? f(v) : v).ToArray(); }
                            if (attempt.open == ((long)1 << (tunnels.Count())) - 1)
                            {
                                newAttempts.Add(attempt);
                            }
                            else
                            {
                                var bit = (long)1 << attempt.pos[posIndex];
                                if ((attempt.open & bit) == 0)
                                {
                                    newAttempts.Add((attempt.pos, attempt.open | bit, attempt.vented, attempt.venting + tunnels[attempt.pos[posIndex]].f, ReplaceAtIndex(attempt.visited, v => 0)));
                                }
                                foreach(int to in tunnels[attempt.pos[posIndex]].t)
                                {
                                    var tBit = (long)1 << to;
                                    if ((attempt.visited[posIndex] & tBit) == 0)
                                    {
                                        newAttempts.Add((ReplaceAtIndex(attempt.pos, p => to), attempt.open, attempt.vented, attempt.venting, ReplaceAtIndex(attempt.visited, v => v | tBit)));
                                    }
                                }
                            }
                        }
                        attempts = newAttempts;
                    }
                    var limitAttempts = (IEnumerable<(int[], long, int vented, int, long[])> ar, int n) => (ar.Count() > n) ? ar.OrderByDescending(a => a.vented).Take(n) : ar;
                    attempts = limitAttempts(attempts.Select(m => (m.pos, m.open, m.vented + m.venting, m.venting, m.visited)), min * 1000).ToList();
                }
                return attempts.Max(a => a.vented);
            }
            return string.Format("{0}/{1}", Run(30, 1), Run(26, 2));
       }
    }
}