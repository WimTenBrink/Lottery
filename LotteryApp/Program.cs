﻿using System.Diagnostics;
using LotteryApp;

using StreamWriter report = new StreamWriter("report.txt");

const int numDraws = 2500;
List<DrawNumbers> draws = new();

// Include a timer to check how fast we go.
var time = new Stopwatch();
time.Start();
string dataFile = "data.json";
string filename = $"draw {numDraws}.json";

if (File.Exists(dataFile))
{
    draws = JSonHelpers.Load<List<DrawNumbers>>(dataFile);
}
else
if (File.Exists(filename))
{
    draws = JSonHelpers.Load<List<DrawNumbers>>(filename);
}

// We first get all the drawn results from the file, or create new draws.
if (draws.Count == 0)
{
    draws = Enumerable.Range(0, numDraws).ToList().Select(i => Helpers.Draw(9, 1, 47)).ToList();
    draws.Save(filename);
}
report.WriteLine($"Time elapsed to draw {draws.Count} times: {time.Elapsed}");

// We then get all the patterns from the draws.
List<Pattern> drawPatterns = draws.Select(d => d.Pattern).ToList();

// We now have 600 patterns. We will now combine all the patterns to get any pattern that occurs at least twice.
List<Pattern> patterns = new();
for (int x = 0; x < drawPatterns.Count - 1; x++)
{
    for (int y = x + 1; y < drawPatterns.Count; y++)
    {
        patterns.Add(drawPatterns[x].Combine(drawPatterns[y]));
    }
}
report.WriteLine($"Time elapsed to make patterns: {time.Elapsed}");

// We now have 179700 patterns. We will now reduce this to unique patterns. And save the patterns as information.
report.WriteLine($"We now have {patterns.Count} patterns.");
patterns = patterns.Select(p => p.Value).Distinct().Select(i => new Pattern() { Value = i }).OrderByDescending(r => r.BitCount).ThenBy(r => r.Value).ToList();
report.WriteLine($"We reduced this to {patterns.Count} patterns.");
patterns.Save("patterns.json");
report.WriteLine($"Time elapsed to reduce patterns: {time.Elapsed}");

// We will now group all draws by the patterns that they match, and group these patterns based on the number of bits. And save this as information.
var occurrences = patterns.Select(p => new KeyValuePair<Pattern, List<Pattern>>(p, drawPatterns.Where(r => p.Match(r)).ToList())).GroupBy(p => p.Key.BitCount).OrderBy(r => r.Key).ToList();
occurrences.Save("occurrences.json");
report.WriteLine($"Time elapsed to count occurrences: {time.Elapsed}");

// We will now show the occurrences of the patterns, and the draws that match these patterns with the highest number of draws.
foreach (var occurrence in occurrences)
{
    occurrence.Save($"occurrence {occurrence.Key}.json");
    int max = occurrence.Max(r => r.Value.Count);
    report.WriteLine($"occurrence of {occurrence.Key} bits is {occurrence.Count()} times with {max} as the highest occurrence.");
    foreach (var group in occurrence.Where(r => r.Value.Count == max).OrderBy(r=>r.Value.Count))
    //foreach (var group in occurrence.Where(r => r.Value.Count > 2).OrderBy(r=>r.Value.Count))
    {
        report.WriteLine($"* Pattern {group.Key.BitCount} bits: value(s) {group.Key.Draw} has {group.Value.Count} items");
        if (group.Value.Count < 0)
        {
            foreach (var item in group.Value)
            {
                report.WriteLine($"    Draw: {item.Draw}");
            }
        }
    }
}
report.WriteLine($"Time elapsed to finish: {time.Elapsed}");
