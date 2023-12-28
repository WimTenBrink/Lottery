// See https://aka.ms/new-console-template for more information

using LotteryApp;

List<DrawNumbers> draws = Enumerable.Range(0, 600).ToList().Select(i => Helpers.Draw(9, 1, 47)).ToList();
foreach (DrawNumbers draw in draws)
{
    Console.WriteLine($"{draw.Pattern} - {draw.Pattern.BitCount} - {draw.Pattern.Draw}");
}

List<Pattern> drawPatterns = draws.Select(d => d.Pattern).ToList();

List<Pattern> patterns = new();
for (int x = 0; x < drawPatterns.Count - 1; x++)
{
    for (int y = x + 1; y < drawPatterns.Count; y++)
    {
        patterns.Add(drawPatterns[x].Combine(drawPatterns[y]));
    }
}

Console.WriteLine($"We now have {patterns.Count} patterns.");
patterns = patterns.Select(p => p.Value).Distinct().Select(i => new Pattern() { Value = i }).ToList();
Console.WriteLine($"We reduced this to {patterns.Count} patterns.");

var occurances = patterns.Select(p => new KeyValuePair<Pattern, List<Pattern>>(p, drawPatterns.Where(r => p.Match(r)).ToList())).GroupBy(p => p.Key.BitCount).OrderBy(r => r.Key).ToList();

foreach (var occurance in occurances)
{
    int max = occurance.Max(r => r.Value.Count);
    Console.WriteLine($"Occurance of {occurance.Key} bits is {occurance.Count()} times with {max} as the highest occurance.");
    foreach (var item in occurance.Where(r => r.Value.Count == max))
    {
        Console.WriteLine($"  Pattern: {item.Key.Draw} has {item.Value.Count} items");
        if (max <= 2)
        {
            Console.WriteLine($"    We're not showing the draws that have only 2 occurances");
        }
        else
        {
            foreach (var group in item.Value)
            {
                Console.WriteLine($"    Draw: {group.Draw}");
            }
        }
    }
}