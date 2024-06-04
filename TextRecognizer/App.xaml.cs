using TextRecognizer.Services;

namespace TextRecognizer;

public partial class App : Application
{
    public App(IOcrResultService ocrResultService)
    {
        InitializeComponent();

        MainPage = new AppShell(ocrResultService);
    }
}