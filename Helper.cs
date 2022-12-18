
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

        static void Main(string[] args)
        {
            var dayToRun = args.Where(arg => arg.StartsWith("--day=")).Select(str => int.Parse(str.Substring("--day=".Length))).FirstOrDefault(0);
            if (dayToRun == 0 && !args.Contains("--alldays")) 
            {
                dayToRun = DateTime.Today.Day;
            }
            Console.WriteLine((dayToRun > 0) ? $"Running Day {dayToRun}" : "Running All Days");
            
            var session = args.Where(arg => arg.StartsWith("--session=")).Select(str => str.Substring("--session=".Length)).FirstOrDefault("");
            var testOnly = session == "" || args.Contains("--testonly");

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
                        if (attrs.Length > 0)
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
                            dayResults.Add(day, "!Test not defined");
                        }

                        if (!testOnly && testPassed)
                        {
                            Task<string> getInput = Helper.GetInputForDay(day, session);
                            tasks.Add(getInput.ContinueWith(task => dayResults.Add(day, "Test passed, actual result: " + dayCalc!.Calc(task.Result, false))));
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

            Console.WriteLine("Done");   
        }
    };

    interface IDay
    {
        string Calc(String input, bool test);
    }

    [AttributeUsage(AttributeTargets.Class)]
    class TestDataAttribute : Attribute
    {
        public string? Value;
        public string? Result;
    };
}
