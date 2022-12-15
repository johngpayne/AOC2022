
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
    [TestData(Value = @"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop", Result = "13140/<ffff73><3108e9><0cf3cc><f70e86><10f013><8f0739><f0fc7c><0e01e6>")]
    class Day10 : IDay
    {
        public string Calc(string input, bool test)
        {
            var insts = input.Split('\n').Select(inst => inst.Trim()).Where(inst => inst != "");
            var x = 1;
            var cycles = 1;

            var result1Probes = Enumerable.Range(0, 6).Select(i => i * 40 + 20);
            var result1 = 0;

            var screen = new List<bool>();

            foreach (var inst in insts)
            {
                void PlotAndAdd(int add)
                { 
                    screen.Add(Math.Abs((screen.Count % 40) - x) < 2); 
                    if (result1Probes.Contains(cycles)) result1 += cycles * x;
                    cycles++; 
                    x += add; 
                }
                PlotAndAdd(0);
                if (inst.StartsWith("addx "))
                {
                    PlotAndAdd(int.Parse(inst.Substring(5)));
                }
            }

            var letters = new Dictionary<int, char> { { 0xf1171f, 'E'}, { 0x999f99, 'H'}, { 0xf1248f, 'Z'}, { 0x11171f, 'F'}, { 0x691196, 'C'}, };
            // treat each block of 5 pixels as a 4-bit little endian binary number (ignoring last pixel of block)
            var screenCharBlocks = screen.Chunk(5).Select(row => row.SkipLast(1).Select((b, index) => (b) ? (1 << index) : 0).Sum()).ToArray();
            // collect together the 6 blocks making up each of 8 characters and test them against the alphabet letters
            var result2 = String.Join("", Enumerable.Range(0, 8).Select(chIndex => Enumerable.Range(0, 6).Select(y => (1 << (4 * y)) * screenCharBlocks[y * 8 + chIndex]).Sum()).Select(n => letters.ContainsKey(n) ? letters[n].ToString() : "<" + n.ToString("x6") + ">"));
           
            return string.Format("{0}/{1}", result1.ToString(), result2);
        }
    }
}
