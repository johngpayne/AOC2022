
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
            
            var quadrantSize = (test) ? 4 : 50;

            var map = lines
                .SkipLast(1)
                .Select(line => 
                    Enumerable.Range(0, 4 * quadrantSize)
                    .Select(x => (x >= line.Length) ? ' ' : line[x])
                    .Select(ch => " .#".IndexOf(ch))
                    .ToArray()
                )
                .Concat(Enumerable.Repeat(Enumerable.Repeat(0, 4 * quadrantSize).ToArray(), 4 * quadrantSize))
                .Take(4 * quadrantSize)
                .ToArray();

            var offsets = new (int x, int y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

            int Run(Func<int,int,int,(int x, int y, int facingChange)> f)
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
                                    var posOffset = (x: (quadrantSize + newPos.x) % quadrantSize, y: (quadrantSize + newPos.y) % quadrantSize);
                                    if ((posOffset.x == 0 && facing == 0) || (posOffset.y == 0 && facing == 1) || (posOffset.x == quadrantSize - 1 && facing == 2) || (posOffset.y == quadrantSize - 1 && facing == 3))
                                    {
                                        var quadrantMove = f(x, y, facing);
                                        for (var i = 0; i < quadrantMove.facingChange; i++)
                                        {
                                            posOffset = (quadrantSize - 1 - posOffset.y, posOffset.x);
                                        }
                                        return (
                                            x: quadrantMove.x * quadrantSize + posOffset.x,
                                            y: quadrantMove.y * quadrantSize + posOffset.y,
                                            facing: (facing + quadrantMove.facingChange) % 4
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

            var wrapQuadrant = (int x, int y, int facing) =>
                Enumerable.Range(1, 4)
                    .Select(i => (x: (4 + x / quadrantSize + i * offsets[facing].x) % 4, y: (4 + y / quadrantSize + i * offsets[facing].y) % 4, facingChange: 0))
                    .First(p => map[p.y * quadrantSize][p.x * quadrantSize] != 0);
            
            var makeWrapCube = () =>
            { 
                var coords = 
                    Enumerable.Range(0, 4)
                    .Select(y => 
                        Enumerable.Range(0, 4)
                        .Where(x => map[y * quadrantSize][x * quadrantSize] != 0)
                        .Select(x => (x:x, y:y))
                    )
                    .SelectMany(c => c)
                    .ToArray();
                
                var cubeStrs = (test) ? 
                    new string[] {
                        "F2D0B2C1", // <<<<< test cube isn't used much so haven't checked all the directions
                        "C0E2F3A2",
                        "D0E1B0A1",
                        "F1E0C0A0",
                        "F0B2C1D0",
                        "A2B3E0D1",
                    } :
                    new string[] {
                        "B0C0D2F1",
                        "E2C1A0F4",
                        "B3E0D3A0",
                        "E0F0A2C1",
                        "B2F1D0C0",
                        "E3B4A3D0",
                   };

                var quadMoves = new Dictionary<(int x, int y, int facing),(int x, int y, int facingChange)>();
                for (var faceIndex = 0; faceIndex < 6; ++faceIndex)
                {
                    for (var facing = 0; facing < 4; ++facing)
                    {
                        var to = cubeStrs[faceIndex][facing * 2] - 'A';
                        quadMoves.Add((coords[faceIndex].x, coords[faceIndex].y, facing), (coords[to].x, coords[to].y, cubeStrs[faceIndex][facing * 2 + 1] - '0'));
                    }
                }

                return (int x, int y, int facing) => quadMoves[(x: x / quadrantSize, y: y / quadrantSize, facing)];
            };

            return string.Format("{0}/{1}", Run(wrapQuadrant), Run(makeWrapCube()));
         }
    }
}
 