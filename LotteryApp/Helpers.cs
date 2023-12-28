using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace LotteryApp;

public static class Helpers
{
    private static readonly Random Random = new();

    public static DrawNumbers Draw(int count, int min, int max)
    {
        List<int> range = Enumerable.Range(min, max).ToList();
        DrawNumbers drawNumbers = new DrawNumbers();
        while (count-- > 0)
        {
            var index = Random.Next(range.Count);
            drawNumbers.Add(range[index]);
            range.RemoveAt(index);
        }
        return drawNumbers;
    }

    public static Pattern Combine(this Pattern pattern, Pattern other)
    {
        var result = new Pattern() { Value = pattern.Value };
        result.Value &= other.Value;
        return result;
    }

    public static bool Match(this Pattern pattern, Pattern other)
    {
        return (pattern.Value & other.Value) == pattern.Value;
    }

}