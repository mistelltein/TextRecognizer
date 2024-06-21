using TextRecognizer.Services;

namespace TextRecognizer.Views;

public partial class ScanResPage : ContentPage
{
    private readonly IOcrResultService _ocrResultService;
    private bool _isResultsLoading = false;

    public ScanResPage(IOcrResultService ocrResultService)
    {
        InitializeComponent();
        _ocrResultService = ocrResultService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isResultsLoading)
        {
            _isResultsLoading = true;
            await LoadResultsAsync();
            _isResultsLoading = false;
        }
    }

    private async Task LoadResultsAsync()
    {
        try
        {
            var results = await _ocrResultService.GetResultsAsync();
            var displayResults = results.Select(text => new DisplayResult { FullText = text, DisplayText = GetDisplayText(text) }).ToList();
            ResultsListView.ItemsSource = displayResults;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading results: {ex.Message}");
            await DisplayAlert("Error", $"Error loading results: {ex.Message}", "OK");
        }
    }

    private string GetDisplayText(string text)
    {
        const int maxLength = 100; // Show only the first 100 characters
        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }

    private async void OnSelectionChanged(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is DisplayResult selectedResult)
        {
            Console.WriteLine($"Selected result: {selectedResult.FullText}");
            if (selectedResult.FullText != null)
            {
                try
                {
                    await CopyAsync(selectedResult.FullText);
                    Console.WriteLine("Text copied successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in OnSelectionChanged: {ex}");
                    await DisplayAlert("Error", $"Clipboard error: {ex.Message}", "OK");
                }
            }
            else
            {
                Console.WriteLine("Selected result FullText is null");
            }
            await Task.Delay(300);
            ResultsListView.SelectedItem = null;
        }
        else
        {
            Console.WriteLine("No item selected");
        }
    }

    public async Task CopyAsync(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            try
            {
                Console.WriteLine("Copying text to clipboard");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Clipboard.Default.SetTextAsync(text);
                });
                Console.WriteLine("Text copied to clipboard successfully");
                await DisplayAlert("Copied", "Text copied to clipboard!", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to copy text: {ex.Message}");
                await DisplayAlert("Error", $"Failed to copy text: {ex.Message}", "OK");
            }
        }
        else
        {
            Console.WriteLine("Text is empty, nothing to copy");
        }
    }

    private async void ClearResults_Clicked(object sender, EventArgs e)
    {
        try
        {
            await _ocrResultService.ClearResultsAsync();
            await LoadResultsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing results: {ex.Message}");
            await DisplayAlert("Error", $"Error clearing results: {ex.Message}", "OK");
        }
    }

    public class DisplayResult
    {
        public string? FullText { get; set; }
        public string? DisplayText { get; set; }
    }
}