using ConvertToJson;

string filePath = "data.csv"; // Replace with the actual path to your CSV file

List<List<int>> numbers = new List<List<int>>();

using StreamReader reader = new StreamReader(filePath);
while (!reader.EndOfStream)
{
    string line = reader.ReadLine() ?? string.Empty;
    var tokens = line.Split(',').Where(r => !string.IsNullOrWhiteSpace(r));
    List<int> intValues = tokens.Select(int.Parse).ToList();
    numbers.Add(intValues);
}

numbers.Save("data.json");