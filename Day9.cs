
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
        public string Calc(string input, bool test)
        {
            var insts = input.Split('\n').Select(line => line.Split(' ')).Where(p => p.Length == 2).Select(p => (dir: p[0][0], count: int.Parse(p[1])));
            var dirs = new Dictionary<char,(int x,int y)>() { {'U', (0, -1)}, {'D', (0, 1)}, {'L', (-1,0)}, {'R', (1,0)} };
            var rope = Enumerable.Repeat((x: 0, y: 0), 10).ToArray();
            var allTailPos = Enumerable.Range(0, 9).Select(n => new List<(int x,int y)>() {(0,0)}).ToArray();
            foreach (var inst in insts)
            {
                var dir = dirs[inst.dir];
                for (var index = 0; index < inst.count; index++)
                {
                    rope[0] = (rope[0].x + dir.x, rope[0].y + dir.y);
                    for (var tailIndex = 1; tailIndex < 10; ++tailIndex)
                    {
                        var offset = (x: rope[tailIndex - 1].x - rope[tailIndex].x, y: rope[tailIndex - 1].y - rope[tailIndex].y);
                        if (Math.Max(Math.Abs(offset.x), Math.Abs(offset.y)) == 2)
                        {
                            rope[tailIndex] = (rope[tailIndex].x + Math.Sign(offset.x), rope[tailIndex].y + Math.Sign(offset.y));
                            allTailPos[tailIndex - 1].Add(rope[tailIndex]);
                        }
                    }
                }
            }
            return String.Format("{0}/{1}", allTailPos[0].Distinct().Count(), allTailPos[8].Distinct().Count());
        }
    }
}
