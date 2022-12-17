
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>", Result = "3068/1514285714288")]
    class Day17 : IDay
    {
        public string Calc(string input, bool test)
        {
            var winds = input.Where(ch => ch == '<' || ch == '>').Select(ch => ch == '>').ToArray();
            var shapes = new int[][] { new int[] {15}, new int[] {2, 7, 2}, new int[] {4, 4, 7}, new int[] {1, 1, 1, 1}, new int[] {3, 3}}.Select(shape => shape.Select(row => row << 3).Reverse().ToArray()).ToArray();
            
            List<int> DropRock(List<int> rows, ref int windIndex, int rockIndex)
            {
                var collide = (IEnumerable<(int y, int row)> s) => s.Any(p => (((p.y < rows.Count ? rows[p.y] : 0) | 257) & p.row) != 0);
                var shape = shapes![rockIndex % shapes.Length].Select((row, i) => (y:rows.Count() + 3 + i, row:row)).ToArray();
                while (true)
                {
                    var wind = winds![windIndex++ % winds.Length];
                    var shiftedShape = shape.Select(p => (p.y, wind ? p.row << 1 : p.row >> 1));
                    shape = (!collide(shiftedShape)) ? shiftedShape.ToArray() : shape;
                    var droppedShape = shape.Select(p => (p.y - 1, p.row));
                    if (collide(droppedShape))
                    {
                        break;
                    }
                    shape = droppedShape.ToArray();
                }
                return rows.Select((r, y) => shape.Where(s => s.y == y).Sum(s => s.row) | r).Concat(shape.Where(s => s.y >= rows.Count).Select(s => s.row)).ToList();
            }

            int GetHeight(int numRocks)
            {
                var rows = new List<int>{ 255 };
                int windIndex = 0;
                for (var rockIndex = 0; rockIndex < numRocks; rockIndex++)
                {
                   rows = DropRock(rows, ref windIndex, rockIndex);
                }
                return rows.Count - 1;
            }

            long FindPattern(long num)
            {
                var rows = new List<int>{255}; 
                int windIndex = 0;
                var cycleChecks = new List<(int height, int[] lastRows)>();
                while (true)
                {
                    for (var rockIndex = 0; rockIndex < shapes.Length; rockIndex++)
                    {
                        rows = DropRock(rows, ref windIndex, rockIndex);
                    }     
                    var lastRows = rows.TakeLast(20).ToArray();
                    var cycleStart = cycleChecks.FindLastIndex(cycle => cycle.lastRows.SequenceEqual(lastRows));
                    cycleChecks.Add((rows.Count - 1, lastRows));
                    if (cycleStart >= 0)
                    {
                        var loop = (cycleChecks.Count - cycleStart - 1) * shapes.Length;
                        return num / loop * (cycleChecks[cycleChecks.Count - 1].height - cycleChecks[cycleStart].height) + cycleChecks[(int)(num % loop) / shapes.Length - 1].height;
                    }
                }
            }
            return string.Format("{0}/{1}", GetHeight(2022), FindPattern((long)1000000000000));
        }
    }
}