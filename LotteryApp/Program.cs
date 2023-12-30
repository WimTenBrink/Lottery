using System.Diagnostics;
using Newtonsoft.Json;

using var report = new StreamWriter("report.txt");
long lastElapsed = 0;
long lastTicks = 0;
var time = new Stopwatch();
const string dataFile = "data.json";
time.Start();
if (!File.Exists(dataFile)) throw new FileNotFoundException($"File {dataFile} not found.");
var draws = (JsonConvert.DeserializeObject<List<List<int>>>(File.ReadAllText(dataFile)) ?? throw new InvalidOperationException()).Where(r => r.Count > 0)
    .ToList();
WriteLine($"Loaded {draws.Count} draws.");
var drawPatterns = draws.Select(Pattern).ToList();
WriteLine($"Generated {drawPatterns.Count} patterns.");
List<long> patterns = [];
for (var x = drawPatterns.Count - 1 - 1; x >= 0; x--)
for (var y = drawPatterns.Count - 1; y > x; y--)
    patterns.Add(drawPatterns[x] & drawPatterns[y]);
patterns = patterns.Distinct().OrderBy(r => r).ToList();
WriteLine($"Reduced to {patterns.Count} patterns.");
var patternGroups = patterns.Select(r => new KeyValuePair<int, long>(BitCount(r), r)).GroupBy(r => r.Key).OrderBy(r => r.Key).ToList();
WriteLine($"Generated {patternGroups.Count} pattern groups. These could be hardcoded.");
foreach (var group in patternGroups)
{
    WriteLine();
    WriteLine($"occurrence of {group.Key} bits has {group.Count()} patterns.");
    var matches = group.Select(r => new KeyValuePair<long, List<List<int>>>(r.Value, drawPatterns.Where(p => Match(r.Value, p)).Select(Draw).ToList()))
        .ToList();
    WriteLine($"Number of patterns checked is {matches.Count} patterns. This timing matters.");
    var max = matches.Max(r => r.Value.Count);
    WriteLine($"Highest number of matches is {max} patterns.");
    foreach (var pair in matches.Where(r => r.Value.Count == max).OrderBy(r => r.Key))
    {
        WriteLine($"* Pattern {LongToDraw(pair.Key)} occurs {pair.Value.Count} times.");
        foreach (var draw in pair.Value) WriteLine($"* * Draw {ListToDraw(draw)}.");
    }
    WriteLine($"Finished for {group.Key} bits.");
}
report.WriteLine($"Total time: {time.ElapsedMilliseconds,6:D} ms.");
return;
string ListToDraw(List<int> draw) { return string.Join(", ", draw); }
string LongToDraw(long pattern) { return string.Join(", ", Draw(pattern)); }
bool Match(long pattern, long other) { return (pattern & other) == pattern; }
long Pattern(List<int> draw) { return draw.Aggregate<int, long>(0, (current, i) => current | (1L << i)); }

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
    for (var i = 0; i < 64; i++)
        if ((pattern & (1L << i)) != 0)
            result.Add(i);
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