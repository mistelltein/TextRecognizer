using System.Text.Json;

namespace TextRecognizer.Services;

public interface IOcrResultService
{
    Task SaveResultAsync(string text);
    Task<IReadOnlyList<string>> GetResultsAsync();
}

public class OcrResultService : IOcrResultService
{
    private const string FileName = "ocr_results.json";
    private readonly string _filePath;
    private readonly List<string> _results;

    public OcrResultService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
        _results = LoadResultsFromFile();
    }

    public async Task SaveResultAsync(string text)
    {
        _results.Insert(0, text);
        await SaveResultsToFile();
    }

    public Task<IReadOnlyList<string>> GetResultsAsync()
    {
        return Task.FromResult((IReadOnlyList<string>)_results.AsReadOnly());
    }

    private List<string> LoadResultsFromFile()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        return new List<string>();
    }

    private async Task SaveResultsToFile()
    {
        var json = JsonSerializer.Serialize(_results);
        await File.WriteAllTextAsync(_filePath, json);
    }
}