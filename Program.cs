using AoC24.Benchmark;
using AoC24.Days;
using BenchmarkDotNet.Running;
using System.Diagnostics;

//var summary = BenchmarkRunner.Run<Benchmarker>();
//return;
string inp = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "input.txt"));

var day = new Day17();

Console.WriteLine(day.Ch1(inp));
Console.WriteLine(day.Ch2(inp));
Console.ReadLine();