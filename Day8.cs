
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
        public string Calc(string input, bool test)
        {
            var heights = 
                input
                .Split("\n")
                .Select(line => line.Trim())
                .Where(line => line != "")
                .Select(line => 
                    line.Select(ch => ch - '0').ToArray()
                )   
                .ToArray();

            var results = 
                Enumerable.Range(0, heights.Length).Select(y =>
                    Enumerable.Range(0, heights[y].Length).Select(x =>
                        new (int x, int y)[] {(-1, 0), (1, 0), (0, -1), (0, 1)}
                        .Select(offset =>
                        {
                            var coord = (x, y);
                            var seenMax = 0;
                            var seen = 0;
                            while (true)
                            {
                                if (coord.y < 0 || coord.y >= heights.Length || coord.x < 0 || coord.x >= heights[coord.y].Length) 
                                {
                                    return (canSee: true, dist: seen - 1);
                                }
                                int height = heights[coord.y][coord.x];
                                if (seen > 0 && height >= seenMax)
                                {
                                    return (canSee: false, dist: seen);
                                }
                                seen++;
                                seenMax = int.Max(seenMax, height);
                                coord = (coord.x + offset.x, coord.y + offset.y);
                            }
                        })
                        .Aggregate((canSee: false, dist: 1), (agg, r) => (agg.canSee | r.canSee, agg.dist * r.dist))
                    )
            );
            return String.Format("{0}/{1}", results.Select(row => row.Where(r => r.canSee).Count()).Sum(), results.Select(row => row.Max(r => r.dist)).Max());
       }
    }
}
