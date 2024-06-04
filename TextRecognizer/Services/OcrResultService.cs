namespace TextRecognizer.Services;

public interface IOcrResultService
{
    Task SaveResultAsync(string text);
    Task<IReadOnlyList<string>> GetResultsAsync();
}

public class OcrResultService : IOcrResultService
{
    private readonly List<string> _results;

    public OcrResultService()
    {
        _results = new List<string>();
    }

    public Task SaveResultAsync(string text)
    {
        _results.Add(text);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<string>> GetResultsAsync()
    {
        return Task.FromResult((IReadOnlyList<string>)_results.AsReadOnly());
    }
}