
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"2,2,2
1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3
2,2,4
2,2,6
1,2,5
3,2,5
2,1,5
2,3,5", Result = "64/58")]
    class Day18 : IDay
    {
        public string Calc(string input, bool test)
        {
            VoxelEqual voxelComparer = new VoxelEqual { MaxDim = 4096 };
            var voxels = input.Split("\n").Select(line => line.Trim()).Where(line => line != "").Select(line => line.Split(',').Select(s => int.Parse(s)).ToArray()).ToHashSet(voxelComparer);
            var neighbourOffsets = Enumerable.Range(0, 3).Select(axis => Enumerable.Range(0, 2).Select(i => Enumerable.Range(0, 3).Select(a => (a == axis) ? 2 * i - 1 : 0).ToArray())).SelectMany(m => m).ToArray();
            var voxelNeighbours = (int[] v) => neighbourOffsets.Select(n => n.Select((axisValue, axisIndex) => v[axisIndex] + axisValue).ToArray()); 
            var voxelBorders = voxels.Select(voxel => voxelNeighbours(voxel).Where(v => !voxels.Contains(v))).SelectMany(b => b).ToArray();
            
            var unbounded = new HashSet<int[]>(Enumerable.Repeat(new int[] {0, 0, 0}, 1), voxelComparer);
            var bounded = new HashSet<int[]>(voxelComparer);
            var holeBorders = voxelBorders.Distinct(voxelComparer).Select(coord => 
            {
                if (bounded.Contains(coord))
                {
                    return 0;
                }
                var borders = new int[][]{ coord };
                var used = new HashSet<int[]>(borders, voxelComparer);
                while (true)
                {
                    var newBorders = borders.Select(v => voxelNeighbours(v)).SelectMany(v => v).Distinct(voxelComparer).Where(v => !voxels.Contains(v) && !used.Contains(v)).ToArray();                    
                    if (newBorders.Count() == 0)
                    {
                        bounded.UnionWith(used);
                        return used.Select(c => voxelNeighbours(c).Where(v => !used.Contains(v))).Sum(b => b.Count());
                    }
                    if (newBorders.Any(v => unbounded.Contains(v)))
                    {
                        unbounded.UnionWith(used);
                        return 0;
                    }
                    used.UnionWith(newBorders);
                    borders = newBorders;                
                }
            }).Sum();
            return string.Format("{0}/{1}", voxelBorders.Length, voxelBorders.Length - holeBorders);
        }
        class VoxelEqual : IEqualityComparer<int[]> 
        {
            public int MaxDim = 4096;
            public bool Equals(int[]? v1, int[]? v2) { return v1!.SequenceEqual(v2!); }
            public int GetHashCode(int[]? v1) { return v1!.Aggregate(0, (agg, v) => agg * MaxDim + v); }
        };
    }
}