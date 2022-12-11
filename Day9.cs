
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2", Result = "13/1")]
    class Day9 : IDay
    {
        public string Calc(string input)
        {
            var insts = input.Split('\n').Select(line => line.Split(' ')).Where(p => p.Length == 2).Select(p => (p[0][0], int.Parse(p[1])));
            var dirs = new Dictionary<char,(int,int)>() { {'U', (0, -1)}, {'D', (0, 1)}, {'L', (-1,0)}, {'R', (1,0)} };
            var rope = Enumerable.Repeat((0,0), 10).ToArray();
            var allTailPos = Enumerable.Range(0, 9).Select(n => new List<(int,int)>() {(0,0)}).ToArray();
            foreach (var inst in insts)
            {
                var dir = dirs[inst.Item1];
                for (var index = 0; index < inst.Item2; index++)
                {
                    rope[0] = (rope[0].Item1 + dir.Item1, rope[0].Item2 + dir.Item2);
                    for (var tailIndex = 1; tailIndex < 10; ++tailIndex)
                    {
                        var offset = (rope[tailIndex - 1].Item1 - rope[tailIndex].Item1, rope[tailIndex - 1].Item2 - rope[tailIndex].Item2);
                        if (Math.Max(Math.Abs(offset.Item1), Math.Abs(offset.Item2)) == 2)
                        {
                            rope[tailIndex] = (rope[tailIndex].Item1 + Math.Sign(offset.Item1), rope[tailIndex].Item2 + Math.Sign(offset.Item2));
                            allTailPos[tailIndex - 1].Add(rope[tailIndex]);
                        }
                    }
                }
            }
            return String.Format("{0}/{1}", allTailPos[0].Distinct().Count(), allTailPos[8].Distinct().Count());
        }
    }
}
