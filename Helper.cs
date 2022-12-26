
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace AOC
{
    class Helper
    {
        static public async Task<string> GetInputForDay(int day, string session)
        {
            var uri = new Uri($"https://adventofcode.com/2022/day/{day}/input");
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };
            handler.CookieContainer.Add(uri, new Cookie("session", session));
            var client = new HttpClient(handler);
            return await client.GetStringAsync(uri);
        }

        static async Task Main(string[] args)
        {
            var totalWatch = System.Diagnostics.Stopwatch.StartNew();
            var dayToRun = args.Where(arg => arg.StartsWith("--day=")).Select(str => int.Parse(str.Substring("--day=".Length))).FirstOrDefault(0);
            if (dayToRun == 0 && !args.Contains("--alldays")) 
            {
                dayToRun = DateTime.Today.Day;
            }
            Console.WriteLine((dayToRun > 0) ? $"Running Day {dayToRun}" : "Running All Days");
            
            var session = args.Where(arg => arg.StartsWith("--session=")).Select(str => str.Substring("--session=".Length)).FirstOrDefault("");
            var testOnly = session == "" || args.Contains("--testonly");
            var time = args.Contains("--time");
            var singleThread = args.Contains("--singlethread");
            var runTests = !args.Contains("--notest");
            var redact = args.Contains("--redact");

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
                        var dayCalc = Activator.CreateInstance(dayType) as IDay;
                        var attrs = dayType.GetCustomAttributes(typeof(TestDataAttribute), false);
                        var testPassed = true;
                        if (attrs.Length > 0 && runTests)
                        {
                            var attr = (attrs[0] as TestDataAttribute);
                            var testValue = attr!.Value;
                            var expectedResult = attr!.Result;
                            var gotResult = dayCalc!.Calc(testValue!, true);
                            if (expectedResult != gotResult)
                            {
                                testPassed = false;
                                dayResults.Add(day, $"!Test failed. Expected:'{expectedResult}' Got:'{gotResult}'");
                            }
                            else if (testOnly)
                            {
                                dayResults.Add(day, $"Test passed: {gotResult}");
                            }
                        }
                        else if (testOnly)
                        {
                            dayResults.Add(day, runTests ? "!Test not defined" : "!Not running tests");
                        }

                        if (!testOnly && testPassed)
                        {
                            Task<string> getInput = Helper.GetInputForDay(day, session);
                            var calcResult = getInput.ContinueWith(task => 
                            {
                                var watch = System.Diagnostics.Stopwatch.StartNew();
                                string result = dayCalc!.Calc(task.Result, false);
                                watch.Stop();
                                dayResults.Add(day, string.Format(runTests ? "Test passed, actual result: {0}{1}" : "Got result: {0}{1}", (redact) ? new string('*', result.Length) : result, (time && (singleThread || dayToRun != 0)) ? string.Format(" (took {0}ms)", watch.ElapsedMilliseconds) : ""));
                            });
                            if (singleThread)
                            {
                                await calcResult;
                            }
                            else
                            {
                                tasks.Add(calcResult);
                            }
                        }
                    }
                    else
                    {
                        dayResults.Add(day, "!Day Not implemented");
                    }
                }
            }

            if (!singleThread) Task.WaitAll(tasks.ToArray());

            for (int dayIndex = 1; dayIndex <= 25; ++dayIndex)
            {
                var day = dayIndex;
                if ((dayToRun == 0) || (dayToRun == day))
                {
                    Console.Write("Day {0} : ", day.ToString().PadLeft(2));
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

            totalWatch.Stop();
            Console.WriteLine("Done{0}", time ? string.Format(" (took {0}ms)", totalWatch.ElapsedMilliseconds) : "");   
        }
    }

    interface IDay
    {
        string Calc(String input, bool test);
    }

    [AttributeUsage(AttributeTargets.Class)]
    class TestDataAttribute : Attribute
    {
        public string? Value;
        public string? Result;
    }
}
