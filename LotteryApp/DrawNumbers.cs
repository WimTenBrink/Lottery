using Newtonsoft.Json;

namespace LotteryApp;

public class DrawNumbers : List<int>
{
    [JsonIgnore]
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