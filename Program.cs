using AoC24.Benchmark;
using AoC24.Days;
using AoC24.Days25;
using BenchmarkDotNet.Running;
using System.Diagnostics;

//var summary = BenchmarkRunner.Run<Benchmarker>();
//return;
string inp = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "input.txt"));

var day = new Day1_25();

Console.WriteLine(day.Ch1(inp));
Console.WriteLine("MP " + day.Ch2_MartinPascal(inp));
Console.WriteLine("D " + day.Ch2_Daniel(inp));

while (Console.ReadLine() != "q")
{

}