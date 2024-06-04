using TextRecognizer.Services;
using TextRecognizer.Views;

namespace TextRecognizer;

public partial class AppShell : Shell
{
    private readonly IOcrResultService _ocrResultService;

    public AppShell(IOcrResultService ocrResultService)
    {
        InitializeComponent();
        _ocrResultService = ocrResultService;
        Routing.RegisterRoute(nameof(ScanResPage), typeof(ScanResPage));
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (args.Source == ShellNavigationSource.Push)
        {
            await this.FadeTo(0, 250); // Animation of the disappearance
        }
    }

    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        await this.FadeTo(1, 250); // Animation of the appearance
    }
}