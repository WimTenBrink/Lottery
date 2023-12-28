using Newtonsoft.Json;

namespace LotteryApp;

public class Pattern
{
    [JsonIgnore]
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