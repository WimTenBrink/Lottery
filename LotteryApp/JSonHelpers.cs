using Newtonsoft.Json;

namespace LotteryApp;

public static class JSonHelpers
{
    public static string SaveAsJson<T>(this T data, Formatting format = Formatting.Indented)
    {
        try { return JsonConvert.SerializeObject(data, format); }
        catch (Exception ex) { return ex.Message; }
    }

    public static T LoadFromJson<T>(string json) where T : new() => JsonConvert.DeserializeObject<T>(json) ?? new T();

    public static T Save<T>(this T data, string filename, Formatting format = Formatting.Indented,
        JsonSerializerSettings? settings = null)
    {
        try { File.WriteAllText(filename, data.SaveAsJson(format)); }
        catch (Exception ex) { File.WriteAllText(filename, ex.Message); }
        return data;
    }

    public static T Load<T>(this FileInfo file) where T : new() => JsonConvert.DeserializeObject<T>(File.ReadAllText(file.FullName)) ?? new T();

    public static T Load<T>(string filename) where T : new() => JsonConvert.DeserializeObject<T>(File.ReadAllText(filename)) ?? new T();

}