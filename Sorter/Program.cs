using System.Diagnostics;
using Sorter.Workers;

string rawFile = @"D:\1\gen.txt";
string outputFile = @"D:\1\out.txt";

Stopwatch stopWatch = new Stopwatch();

Console.WriteLine($"Starting BucketSort...");

stopWatch.Start();
BucketSort.Sort(rawFile, outputFile);
stopWatch.Stop();

Console.WriteLine($"BucketSort takes {stopWatch.ElapsedMilliseconds} ms.");

Validator.ValidateSorting(outputFile);
