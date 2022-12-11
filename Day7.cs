
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC
{
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
                    cd = cd.Parent!;
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
            public Dir? Parent;
        }
    }
}
