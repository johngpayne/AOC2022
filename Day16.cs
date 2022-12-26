
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
            var tunnelNames = 
                Regex.Matches(input, "Valve ([A-Z]+) has flow rate=(-?\\d+); tunnels? leads? to valves? ([^\n\r]+)")
                .Select(match => match.Groups.Values.Skip(1).Select(value => value.Value).ToArray())
                .Select(values => (name: values[0], vents: int.Parse(values[1]), toNames: values[2].Split(", ")))
                .ToArray();
            var tunnels = 
                tunnelNames
                .Select((tunnel, tunnelIndex) => (vents: tunnel.vents, toIndices: tunnel.toNames.Select(name => Array.FindIndex(tunnelNames, findTunnel => findTunnel.name == name)).ToArray()))
                .ToArray();
            var valuableTunnels = 
                Enumerable.Range(0, tunnels.Length)
                .Where(index => tunnels[index].vents > 0)
                .ToArray();
            
            var startTunnel = 
                Array.FindIndex(tunnelNames, tunnel => tunnel.name == "AA");

            var tunnelDists = 
                Enumerable.Range(0, tunnels.Length)
                .Select(i1 => 
                    Enumerable.Range(0, tunnels.Length)
                    .Select(i2 =>
                        i1 == i2 ? 0 : tunnels[i1].toIndices.Contains(i2) ? 1 : int.MaxValue
                    )
                    .ToArray()
                )
                .ToArray();
            for (var i1 = 0; i1 < tunnels.Length; ++i1)
            {
                for (var i2 = 0; i2 < tunnels.Length; ++i2)
                {
                    for (var i3 = 0; i3 < tunnels.Length; ++i3)
                    {
                        if (tunnelDists[i2][i1] != int.MaxValue && tunnelDists[i1][i3] != int.MaxValue && tunnelDists[i2][i3] > tunnelDists[i2][i1] + tunnelDists[i1][i3])
                        {
                            tunnelDists[i2][i3] = tunnelDists[i2][i1] + tunnelDists[i1][i3];
                        }
                    }
                }
            }
            
            var run = (int time) =>
            {
                var attempts = Enumerable.Repeat((time: time, pos: startTunnel, open: (long)0, vented: 0), 1).ToArray();            
                while (true)
                {
                    var newAttempts = new List<(int time, int pos, long open, int vented)>();
                    foreach (var attempt in attempts)
                    {
                        var targetTunnels = 
                            valuableTunnels
                            .Where(tunnelIndex => (attempt.open & ((long)1 << tunnelIndex)) == 0);

                        foreach(var tunnelIndex in targetTunnels)
                        {
                            var timeToOpenTunnel = tunnelDists[attempt.pos][tunnelIndex] + 1;
                            if (timeToOpenTunnel < attempt.time)
                            {
                                newAttempts.Add(
                                    (
                                        attempt.time - timeToOpenTunnel, 
                                        tunnelIndex, 
                                        attempt.open | ((long)1 << tunnelIndex), 
                                        attempt.vented + tunnels[tunnelIndex].vents * (attempt.time - timeToOpenTunnel)
                                    )
                                );
                            }
                        }
                        newAttempts.Add((0, attempt.pos, attempt.open, attempt.vented));
                    }
                    if (attempts.Length == newAttempts.Count)
                    {
                        break;
                    }
                    attempts = 
                        newAttempts
                        .ToLookup(attempt => attempt.open)
                        .Select(attemptGroup => attemptGroup.OrderByDescending(attempt => attempt.vented).Take(2))
                        .SelectMany(attempt => attempt)
                        .OrderByDescending(attempt => attempt.vented)
                        .ToArray();
                }
                return attempts;
            };

            var part1 = run(30).First().vented;

            var part2Run = run(26);
            var part2 = 
                part2Run
                .Select(attempt =>
                    part2Run
                    .Where(elephantAttempt => (elephantAttempt.open & attempt.open) == 0)
                    .Select(elephantAttempt => attempt.vented + elephantAttempt.vented)
                    .OrderByDescending(vented => vented)
                    .FirstOrDefault(attempt.vented)
                )
                .OrderByDescending(vented => vented)
                .First();

            return string.Format("{0}/{1}", part1, part2); 
      }
    }
}