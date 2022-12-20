
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"1
2
-3
3
-2
0
4", Result = "3/1623178306")]
   class Day20 : IDay
    {
        public string Calc(string input, bool test)
        {
            var nums = input.Split("\n").Select(line => line.Trim()).Where(line => line != "").Select(line => int.Parse(line)).ToArray();
            var zeroIndex = Array.FindIndex(nums, n => n == 0);
            long DoRuns(long key = 1, int runs = 1)
            {
                var next = nums.Select((v, i) => (i + 1) % nums.Length).ToArray();
                var prev = nums.Select((v, i) => (i + nums.Length - 1) % nums.Length).ToArray();
                var rotateIndex = (int index, int offsetForward) => (offsetForward > nums.Length / 2) ? Enumerable.Range(0, nums.Length - offsetForward).Aggregate(index, (agg, i) => prev[agg]) : Enumerable.Range(0, offsetForward).Aggregate(index, (agg, i) => next[agg]);
                for (int runIndex = 0; runIndex < runs; ++runIndex)
                {    
                    for (int index = 0; index < nums.Length; ++index)
                    {
                        var newIndex = rotateIndex(index, (int)((key * nums[index]) % (nums.Length - 1) + (nums[index] < 0 ? nums.Length - 1 : 0)));
                        if (index != newIndex)
                        {
                            prev[next[index]] = prev[index];
                            next[prev[index]] = next[index];
                            next[index] = next[newIndex];
                            next[newIndex] = index;
                            prev[index] = newIndex;
                            prev[next[index]] = index;
                        }
                    }
                }
                return new int[] { 1000, 2000, 3000 }.Select(offset => key * nums[rotateIndex(zeroIndex, offset % nums.Length)]).Sum();
            }
            return string.Format("{0}/{1}", DoRuns(), DoRuns(811589153, 10));
        }
    }
}
