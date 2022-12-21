
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"root: pppw + sjmn
dbpl: 5
cczh: sllz + lgvd
zczc: 2
ptdq: humn - dvpt
dvpt: 3
lfqf: 4
humn: 5
ljgn: 2
sjmn: drzm * dbpl
sllz: 4
pppw: cczh / lfqf
lgvd: ljgn * ptdq
drzm: hmdt - zczc
hmdt: 32", Result = "152/301")]
   class Day21 : IDay
    {
        public string Calc(string input, bool test)
        {
            var monkeys = input
                .Split("\n")
                .Select(line => line.Trim())
                .Where(line => line != "")
                .Select(line => line.Split(':'))
                .Select(parts => (name:parts[0], output:parts[1].Trim()))
                .ToLookup(monkey => char.IsAsciiDigit(monkey.output[0]));
            var monkeyNums = monkeys[true].ToDictionary(monkey => monkey.name, monkey => long.Parse(monkey.output));
            var monkeySums = monkeys[false].ToDictionary(monkey => monkey.name, monkey => (op: monkey.output[5], vars: new string[]{monkey.output[0..4], monkey.output[7..11]}));
            
            var ops = new Dictionary<char,Func<long, long,long>> { {'+', (a,b) => a + b}, {'-', (a,b) => a - b}, {'*', (a,b) => a * b}, {'/', (a,b) => a / b} };
            
            long Calc(string name)
            {
                long v;
                if (monkeyNums.TryGetValue(name, out v))
                {
                    return v;
                }
                var monkeySum = monkeySums[name];
                return ops[monkeySum.op](Calc(monkeySum.vars[0]), Calc(monkeySum.vars[1]));
            }

            var opp = new Dictionary<char,char> { {'+', '-'}, {'-', '+'}, {'*', '/'}, {'/', '*'}};
            
            long SolveFor(string name)
            {
                var solve = monkeySums.First(k => k.Value.vars.Contains(name));
                var varIndex = Array.FindIndex(solve.Value.vars, v => v == name);
                if (solve.Key == "root")
                {
                    return Calc(solve.Value.vars[1 - varIndex]);
                }
                else if (varIndex == 0 || solve.Value.op == '+' || solve.Value.op == '*')
                {
                    return ops[opp[solve.Value.op]](SolveFor(solve.Key), Calc(solve.Value.vars[1 - varIndex]));
                }
                else
                {
                    return ops[solve.Value.op](Calc(solve.Value.vars[0]), SolveFor(solve.Key));
                }
            }
            return string.Format("{0}/{1}", Calc("root"), SolveFor("humn"));
        }
    }
}