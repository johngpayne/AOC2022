
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;

namespace AOC
{
    [TestData(Value=@"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000", Result = "24000/45000")]
    class Day1 : IDay
    {
        public string Calc(String input)
        {
            return String.Join('/', new int[]{1, 3}.Select(n => input.Split('\n').Aggregate(new List<Tuple<int,int>>{new Tuple<int,int>(0,0)}, (acc,line) => acc.Concat(new Tuple<int,int>[]{(line.Trim() == "") ? new Tuple<int,int>(acc.Last().Item1 + 1, 0) : new Tuple<int,int>(acc.Last().Item1, acc.Last().Item2 + int.Parse(line))}).ToList()).GroupBy(t => t.Item1).Select(g => g.Last().Item2).OrderByDescending(i => i).Take(n).Sum().ToString()));
        }
    }

    [TestData(Value = @"A Y
B X
C Z", Result = "15/12")]
    class Day2 : IDay
    {
        public string Calc(string input)
        {
            return string.Join('/', Enumerable.Range(0, 2).Select(i => 
            {
                var outcomes = new Dictionary<string,int>(Enumerable.Range(0, 3).Select(otherPick => Enumerable.Range(0, 3).Select(offset => new KeyValuePair<string,int>("ABC"[otherPick] + " " + "XYZ"[(i == 1) ? (offset + 1) % 3 : (otherPick + offset) % 3], 1 + ((otherPick + offset) % 3) + 3 * ((offset + 1) % 3)))).SelectMany(e => e));
                return input.Split('\n').Sum(line => outcomes.GetValueOrDefault(line.Trim())).ToString();
            }));
        }
    };

    [TestData(Value = @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw", Result = "157/70")]
    class Day3 : IDay
    {
        public string Calc(string input)
        {
            var lines = input.Split('\n').Where(line => line.Trim() != "");
            var scoreChar = (char c) => { if (Char.IsAsciiLetterLower(c)) return (1 + (c - 'a')); else if (Char.IsAsciiLetterUpper(c)) return (27 + (c - 'A')); else return 0; };
            int scoreParts<T>(IEnumerable<T> parts) where T : IEnumerable<char> { return parts.ElementAt(0).Where(ch => parts.Skip(1).All(part => part.Contains(ch))).Distinct().Sum(ch => scoreChar(ch)); }
            return String.Format("{0}/{1}", lines.Select(line => scoreParts(line.Trim().Chunk(line.Length / 2))).Sum(), lines.Chunk(3).Sum(chunk => scoreParts(chunk)));
        }
    };

    [TestData(Value = @"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8", Result = "2/4")]
    class Day4 : IDay
    {
        public string Calc(string input)
        {
            var lines = input.Split('\n').Where(line => line != "").Select(line => line.Split(',').Select(part => { var vals = part.Split('-').Select(str => int.Parse(str)); return new Range(vals.ElementAt(0), vals.ElementAt(1)); }));
            var result1 = lines.Count(ranges => { var r0 = ranges.ElementAt(0); var r1 = ranges.ElementAt(1); return r0.Contains(r1) || r1.Contains(r0); });
            var result2 = lines.Count(ranges => Range.Overlap(ranges.ElementAt(0), ranges.ElementAt(1)));
            return String.Format("{0}/{1}", result1, result2);
        }

        struct Range
        {
            int _low;
            int _high;
            public Range(int low, int high) { this._low = low; this._high = high; }
            public bool Contains(Range r) => _low <= r._low && _high >= r._high;
            public static bool Overlap(Range r1, Range r2) => r1._high >= r2._low && r1._low <= r2._high;
        };
    };

    [TestData(Value = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2", Result = "CMZ/MCD")]
    class Day5 : IDay
    {
        public string Calc(string input)
        {
            var lines = input.Split('\n');
            var lineNotEmpty = (string line) => line.Trim() != "";
            var linesInit = lines.TakeWhile(lineNotEmpty).SkipLast(1);
            var linesCommands = lines.SkipWhile(lineNotEmpty).Where(lineNotEmpty);
            var numStacks = linesInit.Max(line => (line.Length + 1) / 4);
            return string.Join('/', new Func<IEnumerable<char>,IEnumerable<char>>[] {a => a.Reverse(), a => a}.Select(
                f => 
                {
                    var stacks = Enumerable.Range(0, numStacks).Select(stackIndex => linesInit.Select(line => line.PadRight(numStacks * 4)[4 * stackIndex + 1]).SkipWhile(ch => ch == ' ').Reverse().ToList()).ToArray();
                    foreach (var command in linesCommands)
                    {
                        var vals = command.Split(' ').Where((x, i) => (i & 1) == 1).Select((x, i) => int.Parse(x) - ((i != 0) ? 1 : 0)).ToArray();
                        stacks[vals[2]].AddRange(f(stacks[vals[1]].TakeLast(vals[0])));
                        stacks[vals[1]].RemoveRange(stacks[vals[1]].Count() - vals[0], vals[0]);
                    }
                    return new String(stacks.Select(stack => stack.Last()).ToArray());
                }
            ));
        }
    }

    [TestData(Value = "mjqjpqmgbljsphdztnvjfqwrcgsmlb", Result = "7/19")]
    class Day6 : IDay
    {
        public string Calc(string input)
        {
            return String.Join('/', new int[] {4, 14}.Select(len => (len + Enumerable.Range(0, input.Length - len).First(index => input.Substring(index, len).Distinct().Count() == len)).ToString()));
        }
    }

    [TestData(Value = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k", Result = "95437/24933642")]
    class Day7 : IDay
    {
        public string Calc(string input)
        {
            var lines = input.Split('\n').Select(s => s.Trim()).Where(s => s != "").Skip(1);
            var root = new Dir();
            var cd = root;
            foreach (var line in lines)
            {
                if (line == "$ cd ..")
                {
                    cd = cd.Parent;
                }
                else if (line.StartsWith("$ cd ")) 
                {
                    cd = cd.Sub[line.Substring(5)];
                }
                else if (line == "$ ls") 
                {

                }
                else if (line.StartsWith("dir "))
                {
                    cd.Sub.Add(line.Substring(4), new Dir() { Parent = cd });
                }
                else 
                {
                    var split = line.Split(' ');
                    cd.Files.Add(split[1], int.Parse(split[0]));
                }
            }

            var sizeCaches = new Dictionary<Dir, int>();
            int CacheSizes(Dir dir)
            {
                int size = dir.Sub.Sum(sub => CacheSizes(sub.Value)) + dir.Files.Sum(file => file.Value);
                sizeCaches[dir] = size;
                return size;
            }
            CacheSizes(root);

            IEnumerable<Dir> GetAllDirs(Dir dir)
            {
                return new Dir[] { dir }.Concat(dir.Sub.Select(sub => GetAllDirs(sub.Value)).SelectMany(d => d));
            }
            var allDirs = GetAllDirs(root);

            return String.Format("{0}/{1}", 
                allDirs.Where(dir => sizeCaches[dir] <= 100000).Sum(dir => sizeCaches[dir]), 
                sizeCaches[allDirs.OrderBy(dir => sizeCaches[dir]).Where(dir => sizeCaches[dir] > sizeCaches[root] - 40000000).First()]);
        }

        class Dir
        {
            public Dictionary<string,Dir> Sub = new Dictionary<string, Dir>();
            public Dictionary<string,int> Files = new Dictionary<string, int>();
            public Dir Parent;
        }
    }

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
noop", Result = "13140/        ")]
    class Day10 : IDay
    {
        public string Calc(string input)
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
            var result2 = new string(Enumerable.Range(0, 8).Select(chIndex => letters.GetValueOrDefault(Enumerable.Range(0, 6).Select(y => (1 << (4 * y)) * screenCharBlocks[y * 8 + chIndex]).Sum(), ' ')).ToArray());
           
            return string.Format("{0}/{1}", result1.ToString(), result2);
        }
    }

    [TestData(Value = @"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1", Result="10605/2713310158")]
    class Day11 : IDay
    {
        public string Calc(string input)
        {
            var monkeys = input.Split("\n").Select(line => line.Trim()).Where(line => line != "").Chunk(6).Select((setup, index) =>
            {
                return new Monkey()
                {
                    Index = index,
                    StartItems = setup[1].Substring(16).Split(", ").Select(s => int.Parse(s)).ToArray(),
                    Quadratic = (setup[2][21] == '+') ? (0, 1, int.Parse(setup[2].Substring(23))) : ((setup[2].Substring(23) == "old") ? (1, 0, 0) : (0, int.Parse(setup[2].Substring(23)), 0)),
                    Divisible = int.Parse(setup[3].Substring(19)),
                    PassTo = setup.Skip(4).Select(s => s.Last() - '0').ToArray()
                };
            }).ToArray();
            var gcd = monkeys.Aggregate(1, (agg, m) => agg * m.Divisible);
            return string.Join('/', new (int,int)[] { (20, 3), (10000, 1)}.Select(
                tryPair =>
                {
                    var inspections = new int[monkeys.Length];
                    var items = monkeys.Select(monkey => monkey.StartItems.ToList()).ToArray();
                    for (var round = 0; round < tryPair.Item1; round++)
                    {
                        foreach (var monkey in monkeys)
                        {
                            foreach (var worryStart in items[monkey.Index])
                            {
                                inspections[monkey.Index]++;
                                int worry = (int)(((monkey.Quadratic.Item1 * worryStart * worryStart + monkey.Quadratic.Item2 * (long)worryStart + monkey.Quadratic.Item3) / tryPair.Item2) % gcd);
                                items[monkey.PassTo[(worry % monkey.Divisible == 0) ? 0 : 1]].Add(worry);
                            }
                            items[monkey.Index].Clear();
                        }
                    }

                    return inspections.OrderByDescending(i => i).Take(2).Aggregate((long)1, (agg, i) => agg * i);
                }
            ));
        }

        public class Monkey
        {
            public int Index;
            public int[] StartItems;
            public (long,long,long) Quadratic;
            public int Divisible;
            public int[] PassTo;
        }
    }

    class Helper
    {
        static public async Task<string> GetInputForDay(int day)
        {
            var uri = new Uri($"https://adventofcode.com/2022/day/{day}/input");
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };
            handler.CookieContainer.Add(uri, new Cookie("_ga", "GA1.2.1394107465.1669998200"));
            handler.CookieContainer.Add(uri, new Cookie("_gid", "GA1.2.134386260.1669998200"));
            handler.CookieContainer.Add(uri, new Cookie("session", "53616c7465645f5f49faa53ad92d0e50882440fa3d341ab87d881ecc64a7cb100222d571a8eefd3d01535f93b16293d76682d83d1f65f5794060382fcd058a38"));
            var client = new HttpClient(handler);
            return await client.GetStringAsync(uri);
        }

        static void Main(string[] args)
        {
            var dayToRun = args.Where(arg => arg.StartsWith("--day=")).Select(str => int.Parse(str.Substring("--day=".Length))).FirstOrDefault(0);
            if (dayToRun == 0 && !args.Contains("--alldays")) 
            {
                dayToRun = DateTime.Today.Day;
            }
            Console.WriteLine((dayToRun > 0) ? $"Running Day {dayToRun}" : "Running All Days");
            
            var testOnly = args.Contains("--testonly");

            var dayResults = new Dictionary<int, string>();
            var tasks = new List<Task>();
            for (int dayIndex = 1; dayIndex <= 25; ++dayIndex)
            {
                var day = dayIndex;
                if ((dayToRun == 0) || (dayToRun == day))
                {
                    var dayType = Type.GetType($"AOC.Day{day}");
                    if (dayType != null)
                    {
                        var dayCalc = (IDay)Activator.CreateInstance(dayType);
                        var attrs = dayType.GetCustomAttributes(typeof(TestDataAttribute), false);
                        var testPassed = true;
                        if (attrs.Length > 0)
                        {
                            var attr = (attrs[0] as TestDataAttribute);
                            var testValue = attr.Value;
                            var expectedResult = attr.Result;
                            var gotResult = dayCalc.Calc(testValue);
                            if (expectedResult != gotResult)
                            {
                                testPassed = false;
                                dayResults.Add(day, $"!Test failed. Expected:'{expectedResult}' Got:'{gotResult}'");
                            }
                            else if (testOnly)
                            {
                                dayResults.Add(day, "Test passed");
                            }
                        }
                        else if (testOnly)
                        {
                            dayResults.Add(day, "!Test not defined");
                        }

                        if (!testOnly && testPassed)
                        {
                            Task<string> getInput = Helper.GetInputForDay(day);
                            tasks.Add(getInput.ContinueWith(task => dayResults.Add(day, "Test passed, actual result: " + dayCalc.Calc(task.Result))));
                        }
                    }
                    else
                    {
                        dayResults.Add(day, "!Day Not implemented");
                    }
                }
            }

            Task.WaitAll(tasks.ToArray());            
            
            for (int dayIndex = 1; dayIndex <= 25; ++dayIndex)
            {
                var day = dayIndex;
                if ((dayToRun == 0) || (dayToRun == day))
                {
                    Console.Write($"DAY {day}:\t");
                    if (dayResults[day].StartsWith('!'))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(dayResults[day].Substring(1));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(dayResults[day]);
                    }
                    Console.ResetColor();
                }
            }

            Console.WriteLine("done");   
        }
    };

    interface IDay
    {
        string Calc(String input);
    }

    [AttributeUsage(AttributeTargets.Class)]
    class TestDataAttribute : Attribute
    {
        public string Value;
        public string Result;
    };
}
