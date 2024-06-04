using Microsoft.Extensions.Logging;
using Plugin.Maui.OCR;
using TextRecognizer.Services;
using TextRecognizer.Views;

namespace TextRecognizer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseOcr()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IOcrService>(OcrPlugin.Default);
        builder.Services.AddSingleton<IOcrResultService, OcrResultService>();

        builder.Services.AddTransient<App>();
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<ScanResPage>();

        return builder.Build();
    }
}