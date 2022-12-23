
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

10R5L5R10L4R5L5", Result = "6032/5031")]
   class Day22 : IDay
    {
        public string Calc(string input, bool test)
        {
            var lines = input
                .Split('\n')
                .Select(line => line.TrimEnd())
                .Where(line => line != "");
            
            var instructions = Regex.Matches(lines.Last(), "(\\d+)([LR])?")
                .Select(m => (steps: int.Parse(m.Groups[1].Value), turn: Array.IndexOf(new string[] { "L", "", "R" }, m.Groups[2].Value) - 1));
            
            var map = lines
                .SkipLast(1)
                .Select(line => 
                    Enumerable.Range(0, lines.SkipLast(1).Max(l => l.Length))
                    .Select(x => x >= line.Length ? 0 : " .#".IndexOf(line[x]))
                    .ToArray()
                )
                .ToArray();

            var faceSize = Math.Min(map.Length, map[0].Length) / 3;
            var offsets = new (int x, int y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

            int Run(Func<int,int,int,(int x, int y, int facingChange)> calcFaceMove)
            {
                var mapNext = Enumerable.Range(0, 4)
                    .Select(facing => 
                        map
                        .Select((row, y) =>
                            row
                            .Select((v, x) => 
                            {
                                var oldPos = (x: x, y: y, facing: facing);
                                if (v != 1)
                                {
                                    return oldPos;
                                }

                                Func<(int x, int y, int facing)> getNewPos = () =>
                                {
                                    var newPos = (x: x + offsets[facing].x, y: y + offsets[facing].y, facing: facing);
                                    if ((x + faceSize) / faceSize != (newPos.x + faceSize) / faceSize || (y + faceSize) / faceSize != (newPos.y + faceSize) / faceSize)
                                    {
                                        var faceMove = calcFaceMove(x / faceSize, y / faceSize, facing);
                                        
                                        var posOffset = Enumerable.Range(0, faceMove.facingChange).Aggregate(
                                            (x: (faceSize + newPos.x) % faceSize, y: (faceSize + newPos.y) % faceSize),
                                            (agg, i) => (faceSize - 1 - agg.y, agg.x));
                    
                                        return (
                                            x: faceSize * faceMove.x + posOffset.x,
                                            y: faceSize * faceMove.y + posOffset.y,
                                            facing: (facing + faceMove.facingChange) % 4
                                        );  
                                    }
                                    else
                                    {
                                        return newPos;
                                    }
                                };

                                var newPos = getNewPos();
                                return (map[newPos.y][newPos.x] == 1) ? newPos : oldPos;
                                
                            })
                            .ToArray()
                        )
                        .ToArray()
                    )
                    .ToArray();
                
                var move = ((int x, int y, int facing) pos, int steps) => Enumerable.Range(0, steps).Aggregate(pos, (agg, i) => mapNext[agg.facing][agg.y][agg.x]);
                var turn = ((int x, int y, int facing) pos, int turn) => (x: pos.x, y: pos.y, facing: (4 + pos.facing + turn) % 4);
                
                var pos = instructions.Aggregate((x: Array.FindIndex(map[0], m => m == 1), y: 0, facing: 0), (agg, instruction) => turn(move(agg, instruction.steps), instruction.turn));       
                return (pos.y + 1) * 1000 + (pos.x + 1) * 4 + pos.facing;
            }

            var wrapPartA = (int sx, int sy, int facing) =>
                Enumerable.Range(1, 4)
                    .Select(i => (x: (4 + sx + i * offsets[facing].x) % 4, y: (4 + sy + i * offsets[facing].y) % 4, facingChange: 0))
                    .First(p => { var x = faceSize * p.x; var y = faceSize * p.y; return y < map.Length && x < map[y].Length && map[y][x] != 0; });

            var makeWrapCube = () =>
            {
                var sides = 
                    Enumerable.Range(0, map.Length / faceSize).Select(y => Enumerable.Range(0, map[0].Length / faceSize).Select(x => (x, y))).SelectMany(c => c)
                    .Where(c => map[faceSize * c.y][faceSize * c.x] != 0)
                    .Select(pos => (pos, verts: new (int x, int y)[] { (1,0), (1,1), (0,1), (0,0) }.Select(offset => (x: offset.x + pos.x, y: offset.y + pos.y, z: 0)).ToArray()))
                    .ToArray();

                void Fold((int x, int y) foldDir, int foldPos, int skip)
                {
                    var sidesToFold = sides.Where(side => side.pos.x * foldDir.x + side.pos.y * foldDir.y == foldPos && sides.Where(s => s.pos.x == side.pos.x + foldDir.x && s.pos.y == side.pos.y + foldDir.y).Count() == 1);
                    var vertsToFold = sidesToFold.Select(side => side.verts.Skip(skip)).SelectMany(v => v).Take(2).ToArray();
                    if (vertsToFold.Length == 2)
                    {
                        var axis = (x: vertsToFold[1].x - vertsToFold[0].x, y: vertsToFold[1].y - vertsToFold[0].y, z: vertsToFold[1].z - vertsToFold[0].z);
                        var rotations = new Dictionary<(int x, int y, int z),int[]> {
                            { (1, 0, 0), new int[] {1,0,0, 0,0,-1, 0,1,0} },
                            { (-1, 0, 0), new int[] {1,0,0, 0,0,1, 0,-1,0} },
                            { (0, 1, 0), new int[] {0,0,1, 0,1,0, -1,0,0} },
                            { (0, -1, 0), new int[] {0,0,-1, 0,1,0, 1,0,0} },
                            { (0, 0, 1), new int[] {0,-1,0, 1,0,0, 0,0,1} },
                            { (0, 0, -1), new int[] {1,1,0, -1,0,0, 0,0,1} },
                        };
                        var rot = rotations[axis];
                        sides = sides.Select(side => 
                            (foldDir.x * side.pos.x + foldDir.y * side.pos.y <= foldPos) ? 
                                side : 
                                (
                                    pos: side.pos, 
                                    verts: 
                                        side.verts
                                        .Select(v => (x: v.x - vertsToFold[0].x, y: v.y - vertsToFold[0].y, z: v.z - vertsToFold[0].z))
                                        .Select(v => (x: rot[0] * v.x + rot[1] * v.y + rot[2] * v.z, y: rot[3] * v.x + rot[4] * v.y + rot[5] * v.z, z: rot[6] * v.x + rot[7] * v.y + rot[8] * v.z))
                                        .Select(v => (x: v.x + vertsToFold[0].x, y: v.y + vertsToFold[0].y, z: v.z + vertsToFold[0].z))
                                        .ToArray()
                                )
                        )
                        .ToArray();
                    }
                }

                for (var foldIndex = 0; foldIndex < 3; foldIndex++)
                {
                    Fold(offsets[0], foldIndex, 0);
                    Fold(offsets[1], foldIndex, 1);
                }

                var faceMoves = sides.Select(sideA => 
                    Enumerable.Range(0, 4)
                    .Select(facing =>
                    {
                        var vertsA = new int[] { facing, (facing + 1) % 4 };
                        (var sideB, var vertsB) = 
                            sides
                            .Where(s => s != sideA)
                            .Select(s => (side: s, matches: vertsA.Select(v => Array.IndexOf(s.verts, sideA.verts[v])).ToArray()))
                            .Where(p => p.matches.All(i => i >= 0))
                            .First();
                       
                        var getFacingChange = () =>
                        {
                            if (vertsA[0] == vertsB[1] && vertsA[1] == vertsB[0]) return 2;
                            else if (vertsA[0] != vertsB[0] && vertsA[1] != vertsB[1]) return 0;
                            else if (vertsA[0] == vertsB[0]) return 1;
                            else return 3;
                        };

                        return (from: (x: sideA.pos.x, y: sideA.pos.y, facing: facing), to: (x: sideB.pos.x, y: sideB.pos.y, facingChange: getFacingChange()));
                    })
                )
                .SelectMany(p => p)
                .ToDictionary(p => p.from, p => p.to);

                return (int x, int y, int facing) => faceMoves[(x, y, facing)]; 
            };

            return string.Format("{0}/{1}", Run(wrapPartA), Run(makeWrapCube()));
         }
    }
}
 