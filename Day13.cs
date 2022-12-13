
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AOC
{
    [TestData(Value = @"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]
", Result = "13/140")]
    class Day13 : IDay
    {
        public string Calc(string input)
        {
            var parseNumber = (Stack<char> buffer) => buffer.PopWhile(ch => char.IsAsciiDigit(ch)).Aggregate(0, (agg, ch) => 10 * agg + (ch - '0'));

            List<object> ParseGroup(Stack<char> buffer)
            {
                var group = new List<object>();
                buffer.Pop();
                while (true)
                {
                    if (char.IsAsciiDigit(buffer.Peek())) 
                    {
                        group.Add(parseNumber(buffer));
                    }
                    else if (buffer.Peek() == '[') 
                    {
                        group.Add(ParseGroup(buffer));
                    }
                    if (buffer.Pop() == ']') 
                    {
                        return group;
                    }
                }
            }

            var parseStr = (string str) => ParseGroup(new Stack<char>(str.Reverse()));
            var pairs = input.Split("\n").Chunk(3).Select(c => c.SkipLast(1).Select(line => parseStr(line)).ToArray());

            int Compare(IEnumerable<object> p0, IEnumerable<object> p1)
            {
                if (p0.Count() * p1.Count() == 0)
                {
                    return p0.Count().CompareTo(p1.Count());
                }
                var compare = (p0.First() is int && p1.First() is int) ? ((int)p0.First()).CompareTo((int)p1.First()) : Compare((p0.First() is List<object>) ? p0.First() as List<object> : new List<object>{ (int)p0.First() }, (p1.First() is List<object>) ? p1.First() as List<object> : new List<object>{ (int)p1.First() });
                return (compare != 0) ? compare : Compare(p0.Skip(1), p1.Skip(1));
            }

            var ret1 = pairs.Select((pair, index) => (Compare(pair[0], pair[1]) == -1) ? index + 1 : 0).Sum().ToString();
        
            var extras = new List<List<object>>{ parseStr("[[2]]"), parseStr("[[6]]")};
            var sortedLines = pairs.SelectMany(s => s).Concat(extras).ToList();
            sortedLines.Sort((p0, p1) => Compare(p0, p1));
            var ret2 = sortedLines.Select((line, index) => (line, index + 1)).Where(pair => extras.Any(extra => Compare(extra, pair.Item1) == 0)).Aggregate(1, (agg, pair) => agg * pair.Item2).ToString();
            
            return string.Format("{0}/{1}", ret1, ret2);
        }
   }

   static class Extensions
   {
        public static List<T> PopWhile<T>(this Stack<T> s, Func<T, bool> f) { var r = new List<T>(); while (f(s.Peek())) r.Add(s.Pop()); return r; }
   }
}
