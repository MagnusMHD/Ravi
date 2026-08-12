using Ravi.App.ViewModels;

namespace Ravi.App;

public partial class MainPage : ContentPage
{
    public MainPage() { InitializeComponent(); BindingContext = new HomeViewModel(); }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        while (IsVisible)
        {
            await RaviMascot.ScaleToAsync(1.04, 900, Easing.SinInOut);
            await RaviMascot.ScaleToAsync(0.97, 900, Easing.SinInOut);
        }
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
