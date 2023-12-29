using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;

namespace LotteryApp;

public static class Helpers
{
    public static long Add(this long pattern, int number)
    {
        pattern |= 1L << number;
        return pattern;
    }

    public static long Remove(this long pattern, int number)
    {
        pattern &= ~(1L << number);
        return pattern;
    }

    public static long Combine(this long pattern, long other)
    {
        return pattern & other;
    }

    public static bool Match(this long pattern, long other)
    {
        return (pattern & other) == pattern;
    }

    public static int BitCount(this long pattern)
    {
        int result = 0;
        while (pattern > 0)
        {
            if ((pattern & 1) == 1) { result++; }
            pattern >>= 1;
        }
        return result;
    }

    public static long Pattern(this List<int> draw)
    {
        long result = 0;
        foreach (var i in draw) { result = result.Add(i); }
        return result;
    }

    public static List<int> Draw(this long pattern)
    {
        var result = new List<int>();
        for (int i = 0; i < 64; i++)
        {
            if ((pattern & (1L << i)) != 0) { result.Add(i); }
        }
        return result;
    }

    public static string ToDraw(this long pattern)
    {
        return string.Join(", ", pattern.Draw());
    }

   public static string ToDraw(this List<int> draw)
    {
        return string.Join(", ", draw);
    }
}