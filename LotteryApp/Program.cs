using System.Diagnostics;
using LotteryApp;
using Newtonsoft.Json.Linq;

// Create the report.
using StreamWriter report = new StreamWriter("report.txt");

// Include a timer to check how fast we go.
var time = new Stopwatch();
time.Start();
long lastElapsed = 0;

void WriteLine(string? value = "")
{
    Console.WriteLine(value);
    report.WriteLine($"[{(time.ElapsedMilliseconds- lastElapsed),4:D} ms] {value}");
    lastElapsed = time.ElapsedMilliseconds;
}


// We will now load the draws from the data file.
string dataFile = "data.json";
if (!File.Exists(dataFile)) throw new FileNotFoundException($"File {dataFile} not found.");
// Remove any empty records.
var draws = JSonHelpers.Load<List<List<int>>>(dataFile).Where(r => r.Count > 0).ToList();
WriteLine($"Loaded {draws.Count} draws.");

// We then get all the patterns from the draws.
List<long> drawPatterns = draws.Select(d => d.Pattern()).ToList();
WriteLine($"Generated {drawPatterns.Count} patterns.");

// We now have X draw patterns. We will now combine all the patterns to get any pattern that occurs at least twice.
List<long> patterns = new();
for (int x = 0; x < drawPatterns.Count - 1; x++)
{
    for (int y = x + 1; y < drawPatterns.Count; y++)
    {
        //Console.WriteLine($"{x} AND {y} = {x & y}");
        patterns.Add(drawPatterns[x] & drawPatterns[y]);
    }
}
// Reduce and order...
patterns = patterns.Distinct().OrderBy(r => r).ToList();
WriteLine($"Reduced to {patterns.Count} patterns.");

// We now have a lot of patterns. We will now reduce this to unique patterns, grouped by the number of bits.
var patternGroups = patterns.Select(r => new KeyValuePair<int, long>(r.BitCount(), r)).GroupBy(r => r.Key).OrderBy(r => r.Key).ToList();
WriteLine($"Generated {patternGroups.Count} pattern groups. These could be hardcoded.");
patternGroups.Save("patterngroups.json");
WriteLine();

// We will now process each group separately.
foreach (var group in patternGroups)
{
    WriteLine($"occurrence of {group.Key} bits has {group.Count()} patterns.");
    // Get all patterns that match the group, grouped by the group patterns.
    var matches = group.Select(r => new KeyValuePair<long, List<List<int>>>(r.Value, drawPatterns.Where(p => r.Value.Match(p)).Select(r=>r.Draw()).ToList())).ToList();
    WriteLine($"Number of patterns checked is {matches.Count} patterns. This timing matters.");

    matches.Save($"matches {group.Key}.json");
    int max = matches.Max(r => r.Value.Count);
    WriteLine($"Highest number of matches is {max} patterns.");
    foreach (var pair in matches.Where(r => r.Value.Count == max).OrderBy(r => r.Key))
    {
        WriteLine($"* Pattern {pair.Key.ToDraw()} occurs {pair.Value.Count} times:");
        foreach (var draw in pair.Value)
        {
            WriteLine($"* * Draw {draw.ToDraw()}.");
        }
    }
    WriteLine($"Finished for {group.Key} bits.");
    WriteLine();
}
report.WriteLine($"Total time: {time.ElapsedMilliseconds,6:D} ms.");