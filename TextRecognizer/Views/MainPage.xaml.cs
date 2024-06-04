using Plugin.Maui.OCR;
using TextRecognizer.Services;

namespace TextRecognizer.Views;

public partial class MainPage : ContentPage
{
    private readonly IOcrResultService _ocrResultService;

    public MainPage()
    {
        InitializeComponent();
        _ocrResultService = new OcrResultService();
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
                using var imageAsStream = await pickResult.OpenReadAsync();
                var imageAsBytes = new byte[imageAsStream.Length];
                await imageAsStream.ReadAsync(imageAsBytes);

                var ocrResult = await OcrPlugin.Default.RecognizeTextAsync(imageAsBytes);

                if (!ocrResult.Success)
                {
                    await DisplayAlert("No success", "No OCR possible", "OK");
                    return;
                }

                await _ocrResultService.SaveResultAsync(ocrResult.AllText);
                await DisplayAlert("OCR Result", ocrResult.AllText, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}