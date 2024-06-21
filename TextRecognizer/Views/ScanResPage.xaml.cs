using TextRecognizer.Services;

namespace TextRecognizer.Views;

public partial class ScanResPage : ContentPage
{
    private readonly IOcrResultService _ocrResultService;

    public ScanResPage(IOcrResultService ocrResultService)
    {
        InitializeComponent();
        _ocrResultService = ocrResultService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadResultsAsync();
    }

    private async Task LoadResultsAsync()
    {
        var results = await _ocrResultService.GetResultsAsync();
        var displayResults = results.Select(text => new DisplayResult { FullText = text, DisplayText = GetDisplayText(text) }).ToList();
        ResultsCollectionView.ItemsSource = displayResults;
    }

    private string GetDisplayText(string text)
    {
        const int maxLength = 100; // Show only the first 100 characters
        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is DisplayResult selectedResult)
        {
            try
            {
                await CopyAsync(selectedResult.FullText!);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Clipboard error: {ex.Message}", "OK");
            }

            await Task.Delay(300);
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    public async Task CopyAsync(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Clipboard.Default.SetTextAsync(text);
                });
                await DisplayAlert("Copied", "Text copied to clipboard!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to copy text: {ex.Message}", "OK");
            }
        }
    }

    private async void ClearResults_Clicked(object sender, EventArgs e)
    {
        await _ocrResultService.ClearResultsAsync();
        await LoadResultsAsync();
    }

    public class DisplayResult
    {
        public string? FullText { get; set; }
        public string? DisplayText { get; set; }
    }
}