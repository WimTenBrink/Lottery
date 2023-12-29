using System.Diagnostics;
using Newtonsoft.Json;

// Create the report.
using var report = new StreamWriter("report.txt");

// Include a timer to check how fast we go.
var time = new Stopwatch();
time.Start();
long lastElapsed = 0, lastTicks = 0;

// We will now load the draws from the data file.
var dataFile = "data.json";
if (!File.Exists(dataFile)) throw new FileNotFoundException($"File {dataFile} not found.");
var draws = (JsonConvert.DeserializeObject<List<List<int>>>(File.ReadAllText(dataFile)) ?? throw new InvalidOperationException()).Where(r => r.Count > 0)
    .ToList();
WriteLine($"Loaded {draws.Count} draws.");

// We then get all the patterns from the draws.
var drawPatterns = draws.Select(Pattern).ToList();
WriteLine($"Generated {drawPatterns.Count} patterns.");

// We now have X draw patterns. We will now combine all the patterns to get any pattern that occurs at least twice.
List<long> patterns = new();
for (var x = 0; x < drawPatterns.Count - 1; x++)
    for (var y = x + 1; y < drawPatterns.Count; y++)
        patterns.Add(drawPatterns[x] & drawPatterns[y]);
// Reduce and order...
patterns = patterns.Distinct().OrderBy(r => r).ToList();
WriteLine($"Reduced to {patterns.Count} patterns.");

// We now have a lot of patterns. We will now reduce this to unique patterns, grouped by the number of bits.
var patternGroups = patterns.Select(r => new KeyValuePair<int, long>(BitCount(r), r)).GroupBy(r => r.Key).OrderBy(r => r.Key).ToList();
WriteLine($"Generated {patternGroups.Count} pattern groups. These could be hardcoded.");
WriteLine();

// We will now process each group separately.
foreach (var group in patternGroups)
{
    WriteLine($"occurrence of {group.Key} bits has {group.Count()} patterns.");
    // Get all patterns that match the group, grouped by the group patterns.
    var matches = group.Select(r => new KeyValuePair<long, List<List<int>>>(r.Value, drawPatterns.Where(p => Match(r.Value, p)).Select(r => Draw(r)).ToList()))
        .ToList();
    WriteLine($"Number of patterns checked is {matches.Count} patterns. This timing matters.");
    var max = matches.Max(r => r.Value.Count);
    WriteLine($"Highest number of matches is {max} patterns.");
    foreach (var pair in matches.Where(r => r.Value.Count == max).OrderBy(r => r.Key))
    {
        WriteLine($"* Pattern {LongToDraw(pair.Key)} occurs {pair.Value.Count} times:");
        foreach (var draw in pair.Value) WriteLine($"* * Draw {ListToDraw(draw)}.");
    }
    WriteLine($"Finished for {group.Key} bits.");
    WriteLine();
}
report.WriteLine($"Total time: {time.ElapsedMilliseconds,6:D} ms.");
return;

// ------------------------------------------------------------

string ListToDraw(List<int> draw) => string.Join(", ", draw);
string LongToDraw(long pattern) => string.Join(", ", Draw(pattern));
bool Match(long pattern, long other) => (pattern & other) == pattern;

void WriteLine(string? value = "")
{
    Console.WriteLine(value);
    report.WriteLine($"[{time.ElapsedMilliseconds - lastElapsed,4:0} ms, {time.ElapsedTicks - lastTicks,7:0} ticks] {value}");
    lastElapsed = time.ElapsedMilliseconds;
    lastTicks = time.ElapsedTicks;
}

List<int> Draw(long pattern)
{
    var result = new List<int>();
    for (var i = 0; i < 64; i++) if ((pattern & (1L << i)) != 0) result.Add(i);
    return result;
}

long Pattern(List<int> draw)
{
    long result = 0;
    foreach (var i in draw) result |= 1L << i;
    return result;
}

int BitCount(long pattern)
{
    var result = 0;
    while (pattern > 0)
    {
        if ((pattern & 1) == 1) result++;
        pattern >>= 1;
    }
    return result;
}