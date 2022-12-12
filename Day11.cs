﻿
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1", Result="10605/2713310158")]
    class Day11 : IDay
    {
        public string Calc(string input)
        {
            var monkeys = input.Split("\n").Select(line => line.Trim()).Where(line => line != "").Chunk(6).Select((setup, index) =>
            {
                return new Monkey()
                {
                    Index = index,
                    StartItems = setup[1].Substring(16).Split(", ").Select(s => int.Parse(s)).ToArray(),
                    Quadratic = (setup[2][21] == '+') ? (0, 1, int.Parse(setup[2].Substring(23))) : ((setup[2].Substring(23) == "old") ? (1, 0, 0) : (0, int.Parse(setup[2].Substring(23)), 0)),
                    Divisible = int.Parse(setup[3].Substring(19)),
                    PassTo = setup.Skip(4).Select(s => s.Last() - '0').ToArray()
                };
            }).ToArray();
            var gcd = monkeys.Aggregate(1, (agg, m) => agg * m.Divisible);
            return string.Join('/', new (int,int)[] { (20, 3), (10000, 1)}.Select(
                tryPair =>
                {
                    var inspections = new int[monkeys.Length];
                    var items = monkeys.Select(monkey => monkey.StartItems!.ToList()).ToArray();
                    for (var round = 0; round < tryPair.Item1; round++)
                    {
                        foreach (var monkey in monkeys)
                        {
                            foreach (var worryStart in items[monkey.Index])
                            {
                                inspections[monkey.Index]++;
                                int worry = (int)(((monkey.Quadratic.Item1 * worryStart * worryStart + monkey.Quadratic.Item2 * (long)worryStart + monkey.Quadratic.Item3) / tryPair.Item2) % gcd);
                                items[monkey.PassTo![(worry % monkey.Divisible == 0) ? 0 : 1]].Add(worry);
                            }
                            items[monkey.Index].Clear();
                        }
                    }

                    return inspections.OrderByDescending(i => i).Take(2).Aggregate((long)1, (agg, i) => agg * i);
                }
            ));
        }

        public class Monkey
        {
            public int Index;
            public int[]? StartItems;
            public (long,long,long) Quadratic;
            public int Divisible;
            public int[]? PassTo;
        }
    }
}