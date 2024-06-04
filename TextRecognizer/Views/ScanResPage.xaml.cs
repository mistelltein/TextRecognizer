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

        var results = await _ocrResultService.GetResultsAsync();
        var displayResults = results.Select(text => new DisplayResult { FullText = text, DisplayText = GetDisplayText(text) }).ToList();
        ResultsCollectionView.ItemsSource = displayResults;
    }

    private string GetDisplayText(string text)
    {
        const int maxLength = 50; // Show only the first 50 characters
        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is DisplayResult selectedResult)
        {
            Clipboard.SetTextAsync(selectedResult.FullText);
            DisplayAlert("Copied", "Text copied to clipboard.", "OK");
        }
    }

    public class DisplayResult
    {
        public string? FullText { get; set; }
        public string? DisplayText { get; set; }
    }
}