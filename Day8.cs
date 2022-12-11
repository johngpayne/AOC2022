
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"30373
25512
65332
33549
35390", Result = "21/8")]
    class Day8 : IDay
    {
        public string Calc(string input)
        {
            var heights = input.Split("\n").Select(line => line.Trim()).Where(line => line != "").Select(line => line.Select(ch => ch - '0').ToArray()).ToArray();
            var results = Enumerable.Range(0, heights.Length).Select(
                y =>
                    Enumerable.Range(0, heights[y].Length).Select(
                        x =>
                            new (int, int)[] {(-1, 0), (1, 0), (0, -1), (0, 1)}.Select(
                                offset =>
                                {
                                    var coord = (x, y);
                                    var seenMax = 0;
                                    var seen = 0;
                                    while (true)
                                    {
                                        if (coord.Item2 < 0 || coord.Item2 >= heights.Length || coord.Item1 < 0 || coord.Item1 >= heights[coord.Item2].Length) 
                                        {
                                            return (true, seen - 1);
                                        }
                                        int height = heights[coord.Item2][coord.Item1];
                                        if (seen > 0 && height >= seenMax)
                                        {
                                            return (false, seen);
                                        }
                                        seen++;
                                        seenMax = int.Max(seenMax, height);
                                        coord = (coord.Item1 + offset.Item1, coord.Item2 + offset.Item2);
                                    }
                                }
                            ).Aggregate((false, 1), (agg, r) => (agg.Item1 | r.Item1, agg.Item2 * r.Item2))
                    )
            );
            return String.Format("{0}/{1}", results.Select(row => row.Where(r => r.Item1).Count()).Sum(), results.Select(row => row.Max(r => r.Item2)).Max());
       }
    }
}
