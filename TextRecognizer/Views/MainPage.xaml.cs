using Plugin.Maui.OCR;
using TesseractOcrMaui;
using TextRecognizer.Services;

namespace TextRecognizer.Views;

public partial class MainPage : ContentPage
{
    private readonly IOcrResultService _ocrResultService;
    private readonly ITesseract _tesseract;

    public MainPage(IOcrResultService ocrResultService, ITesseract tesseract)
    {
        InitializeComponent();
        _ocrResultService = ocrResultService;
        _tesseract = tesseract;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await OcrPlugin.Default.InitAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Initialization Error", ex.Message, "OK");
        }
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        await PerformOcrAsync(async () => await MediaPicker.Default.PickPhotoAsync());
    }

    private async void PictureBtn_Clicked(object sender, EventArgs e)
    {
        await PerformOcrAsync(async () => await MediaPicker.Default.CapturePhotoAsync());
    }

    private async Task PerformOcrAsync(Func<Task<FileResult>> getFileAsync)
    {
        try
        {
            var pickResult = await getFileAsync();

            if (pickResult != null)
            {
                var result = await _tesseract.RecognizeTextAsync(pickResult.FullPath);

                await _ocrResultService.SaveResultAsync(result.RecognisedText);
                await DisplayAlert("OCR Result", "The text was successfully scanned. Please check the results page", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}