using Newtonsoft.Json;

const string dataFile = "data.json";
if (!File.Exists(dataFile)) throw new FileNotFoundException($"File {dataFile} not found.");
var draws = (JsonConvert.DeserializeObject<List<List<int>>>(File.ReadAllText(dataFile)) ?? throw new InvalidOperationException()).Where(r => r.Count > 0).ToList();
Console.WriteLine($"Loaded {draws.Count} draws.");
var drawPatterns = draws.Select(Pattern).ToList();
var patternsAsync = new List<long>();
for (var x = drawPatterns.Count - 1 - 1; x >= 0; x--)
{
    var tasks = new List<Task>();
    for (var y = drawPatterns.Count - 1; y > x; y--)
    {
        var pattern = drawPatterns[x] & drawPatterns[y];
        tasks.Add(Task.Run(() => patternsAsync.Add(pattern)));
    }

    await Task.WhenAll(tasks);
}
Console.WriteLine($"Generated {patternsAsync.Count} patterns.");
List<long> patterns = patternsAsync.Distinct().OrderBy(r => r).ToList();
Console.WriteLine($"Reduced to {patterns.Count} patterns.");

var patternGroups = patterns.AsParallel().Select(r => new KeyValuePair<int, long>(BitCount(r), r)).GroupBy(r => r.Key).OrderBy(r => r.Key).ToList();
Console.WriteLine($"Generated {patternGroups.Count} pattern groups. These could be hardcoded.");
foreach (var group in patternGroups)
{
    Console.WriteLine();
    Console.WriteLine($"occurrence of {group.Key} bits has {group.Count()} patterns.");
    var matches = group.AsParallel().Select(r => new KeyValuePair<long, List<List<int>>>(r.Value, drawPatterns.Where(p => Match(r.Value, p)).Select(Draw).ToList()))
        .ToList();
    Console.WriteLine($"Number of patterns checked is {matches.Count} patterns. This timing matters.");
    var max = matches.Max(r => r.Value.Count);
    Console.WriteLine($"Highest number of matches is {max} patterns.");
    var maxMatches = matches.Where(r => r.Value.Count == max).OrderBy(r => r.Key);
    foreach (var pair in maxMatches)
    {
        Console.WriteLine($"* Pattern {LongToDraw(pair.Key)} occurs {pair.Value.Count} times.");
        if (maxMatches.Count() <= 5 && pair.Value.Count < 10)
        {
            foreach (var draw in pair.Value) Console.WriteLine($"* * Draw {ListToDraw(draw)}.");
        }
    }
    Console.WriteLine($"Finished for {group.Key} bits.");
}
return;
string ListToDraw(List<int> draw) { return string.Join(", ", draw); }
string LongToDraw(long pattern) { return string.Join(", ", Draw(pattern)); }
bool Match(long pattern, long other) { return (pattern & other) == pattern; }
long Pattern(List<int> draw) { return draw.Aggregate<int, long>(0, (current, i) => current | (1L << i)); }

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