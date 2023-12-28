using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace LotteryApp;

public class Pattern
{
    public long Value { get; set; } = 0;

    public int BitCount
    {
        get
        {
            int result = 0;
            long value = Value;
            while (value > 0)
            {
                if ((value & 1) == 1) { result++; }
                value >>= 1;
            }
            return result;
        }
    }

    public long Add(int number)
    {
        Value |= 1L << number;
        return Value;
    }

    public long Remove(int number)
    {
        Value &= ~(1L << number);
        return Value;
    }

    public DrawNumbers Draw
    {
        get
        {
            var result = new DrawNumbers();
            for (int i = 0; i < 47; i++)
            {
                if ((Value & (1L << i)) != 0) { result.Add(i); }
            }
            return result;
        }
    }

    public override string ToString() { return Value.ToString("b64"); }
}

public class DrawNumbers : List<int>
{
    public Pattern Pattern
    {
        get
        {
            Pattern result = new Pattern();
            foreach (var i in this) { result.Add(i); }
            return result;
        }
    }

    public override string ToString() { return string.Join(", ", this.Select(i => i.ToString())); }
}

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