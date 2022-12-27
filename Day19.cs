
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"Blueprint 1:
  Each ore robot costs 4 ore.
  Each clay robot costs 2 ore.
  Each obsidian robot costs 3 ore and 14 clay.
  Each geode robot costs 2 ore and 7 obsidian.

Blueprint 2:
  Each ore robot costs 2 ore.
  Each clay robot costs 3 ore.
  Each obsidian robot costs 3 ore and 8 clay.
  Each geode robot costs 3 ore and 12 obsidian.
", Result = "33/3472")]
   class Day19 : IDay
    {
        public string Calc(string input, bool test)
        {
            var machineTypes = new List<string> { "ore", "clay", "obsidian", "geode" };
            var blueprints = 
                Regex.Matches(input, "Blueprint (?:\\d+):\\s+" + string.Join("", machineTypes.Select(m => $"Each {m} robot costs ([^.]+).\\s*")))
                .Select(m => 
                    m.Groups.Values
                    .Skip(1)
                    .Select(c => c.Value.Split(" and ").Select(p => p.Split(' ')).Select(p => (index: machineTypes.IndexOf(p[1]), amount: int.Parse(p[0]))).ToArray())
                    .Select(c => Enumerable.Range(0, 4).Select(i => c.Where(p => p.index == i).Sum(p => p.amount)).ToArray())
                    .ToList()
                )
                .ToArray();

            int GetMaxGeodes(List<int[]> bp, int maxTime, bool simpleScore)
            {
                var scoreMults = Enumerable.Range(0, 4).Select(i => Enumerable.Range(0, i).Aggregate(1, (agg, i) => agg * bp[i + 1][i]));
                var attempts = Enumerable.Repeat((machines: new int[] {1, 0, 0, 0}, inventory: new int[] {0, 0, 0, 0}, refused:0), 1).ToList();
                for (var min = maxTime - 1; min >= 0; --min)
                {
                    var newAttempts = new List<(int[] machines, int[] inventory, int refused)>();
                    foreach (var attempt in attempts)
                    {
                        var newInventory = attempt.inventory.Zip(attempt.machines, (v,m) => v + m).ToArray();
                        var canBuild = Enumerable.Range(0, 4).Where(i => ((1 << i) & attempt.refused) == 0).Where(i => bp[i].Select((v, i) => attempt.inventory[i] >= v).All(b => b));
                        foreach (var buildIndex in canBuild)
                        {
                            newAttempts.Add((machines: attempt.machines.Select((m, i) => buildIndex == i ? m + 1 : m).ToArray(), inventory: newInventory.Zip(bp[buildIndex], (v,c) => v - c).ToArray(), refused: 0));
                        }
                        newAttempts.Add((machines: attempt.machines, inventory: newInventory, refused: attempt.refused | canBuild.Sum(b => 1 << b)));
                    }     
                    attempts = newAttempts.OrderByDescending(a => a.machines.Zip(a.inventory, (m, i) => m * min + i).Zip(scoreMults, (s, m) => s * m).Sum()).Take(64).ToList();
                }
                return attempts.Max(a => a.inventory[3]);
            }
                
            int ret1 = blueprints.Select((bp,i) => (i + 1) * GetMaxGeodes(bp, 24, true)).Sum();
            long ret2 = blueprints.Take(3).Aggregate((long)1, (agg, bp) => agg * GetMaxGeodes(bp, 32, false));
            return string.Format("{0}/{1}", ret1, ret2);
        }
    }
}