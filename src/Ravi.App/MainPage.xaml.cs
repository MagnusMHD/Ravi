using Ravi.App.ViewModels;

namespace Ravi.App;

public partial class MainPage : ContentPage
{
    private CancellationTokenSource? _animationCancellation;

    public MainPage() { InitializeComponent(); BindingContext = new HomeViewModel(); }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationCancellation = new CancellationTokenSource();
        _ = AnimateRaviAsync(_animationCancellation.Token);
    }

    protected override void OnDisappearing()
    {
        _animationCancellation?.Cancel();
        _animationCancellation?.Dispose();
        _animationCancellation = null;
        base.OnDisappearing();
    }

    private async Task AnimateRaviAsync(CancellationToken cancellationToken)
    {
        var characters = new[] { RaviMascot, RaviProfile, RaviCourse, RaviLesson };
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.WhenAll(characters.Select(character => character.TranslateToAsync(0, -5, 1100, Easing.SinInOut)));
                await Task.WhenAll(characters.Select(character => character.RotateToAsync(1.2, 650, Easing.SinInOut)));
                await Task.WhenAll(characters.Select(character => character.TranslateToAsync(0, 0, 1100, Easing.SinInOut)));
                await Task.WhenAll(characters.Select(character => character.RotateToAsync(-1.2, 650, Easing.SinInOut)));
            }
        }
        catch (OperationCanceledException)
        {
            // The page stopped being visible; the idle animation can end quietly.
        }
    }

    private async void RaviTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Image ravi)
            return;

        ravi.CancelAnimations();
        await ravi.ScaleToAsync(1.1, 120, Easing.CubicOut);
        await ravi.TranslateToAsync(0, -12, 150, Easing.CubicOut);
        await Task.WhenAll(
            ravi.ScaleToAsync(1, 220, Easing.BounceOut),
            ravi.TranslateToAsync(0, 0, 220, Easing.BounceOut),
            ravi.RotateToAsync(0, 220, Easing.SinInOut));
    }

    private async void SpeakClicked(object? sender, EventArgs e)
    {
        if (BindingContext is HomeViewModel vm)
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var english = locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(vm.StepContent, new SpeechOptions { Locale = english });
        }
    }
}
