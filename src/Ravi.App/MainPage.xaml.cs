using Ravi.App.ViewModels;

namespace Ravi.App;

public partial class MainPage : ContentPage
{
    private CancellationTokenSource? _animationCancellation;
    private CancellationTokenSource? _speechCancellation;

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
                await Task.WhenAll(characters.Select(character => character.TranslateToAsync(0, -2, 2200, Easing.SinInOut)));
                await Task.WhenAll(characters.Select(character => character.ScaleToAsync(1.008, 1400, Easing.SinInOut)));
                await Task.WhenAll(characters.Select(character => character.TranslateToAsync(0, 0, 2200, Easing.SinInOut)));
                await Task.WhenAll(characters.Select(character => character.ScaleToAsync(1, 1400, Easing.SinInOut)));
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
        await ravi.ScaleToAsync(1.04, 140, Easing.CubicOut);
        await ravi.TranslateToAsync(0, -5, 150, Easing.CubicOut);
        await Task.WhenAll(
            ravi.ScaleToAsync(1, 220, Easing.CubicOut),
            ravi.TranslateToAsync(0, 0, 220, Easing.CubicOut),
            ravi.RotateToAsync(0, 220, Easing.SinInOut));
    }

    private async void SpeakClicked(object? sender, EventArgs e)
    {
        if (BindingContext is HomeViewModel vm)
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var english = locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(vm.StepContent, new SpeechOptions { Locale = english, Rate = 0.62f });
        }
    }

    private async void SpeakWordClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: VocabularyItem word })
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var english = locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(word.English, new SpeechOptions { Locale = english, Rate = 0.58f });
        }
    }

    private async void SpeakLineClicked(object? sender, EventArgs e)
    {
        var text = sender switch
        {
            Button { BindingContext: VocabularyItem word } => word.English,
            Button { BindingContext: LessonLine line } => line.English,
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(text))
            return;

        var locales = await TextToSpeech.Default.GetLocalesAsync();
        var english = locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
        await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = english, Rate = 0.60f });
    }

    private async void StoryTapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is not HomeViewModel vm)
            return;

        var locales = await TextToSpeech.Default.GetLocalesAsync();
        var english = locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
        _speechCancellation?.Cancel();
        _speechCancellation?.Dispose();
        _speechCancellation = new CancellationTokenSource();
        var token = _speechCancellation.Token;
        var paragraphs = vm.StepContent.Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        try
        {
            foreach (var paragraph in paragraphs)
            {
                var segments = SplitNarrationAndDialogue(paragraph);
                var dialogueIndex = 0;
                foreach (var (text, isDialogue) in segments)
                {
                    var options = isDialogue
                        ? new SpeechOptions { Locale = english, Rate = 0.40f, Pitch = dialogueIndex++ % 2 == 0 ? 0.82f : 1.12f }
                        : new SpeechOptions { Locale = english, Rate = 0.34f, Pitch = 0.96f };
                    await TextToSpeech.Default.SpeakAsync(text, options, token);
                    await Task.Delay(isDialogue ? 600 : 400, token);
                }
                await Task.Delay(1200, token);
            }
        }
        catch (OperationCanceledException)
        {
            // A second tap starts the story again without overlapping voices.
        }
    }

    private static List<(string Text, bool IsDialogue)> SplitNarrationAndDialogue(string paragraph)
    {
        var result = new List<(string Text, bool IsDialogue)>();
        var remaining = paragraph;
        while (remaining.Length > 0)
        {
            var opening = remaining.IndexOf('“');
            if (opening < 0)
            {
                if (!string.IsNullOrWhiteSpace(remaining)) result.Add((remaining.Trim(), false));
                break;
            }

            if (opening > 0) result.Add((remaining[..opening].Trim(), false));
            var closing = remaining.IndexOf('”', opening + 1);
            if (closing < 0)
            {
                result.Add((remaining[(opening + 1)..].Trim(), true));
                break;
            }

            result.Add((remaining[(opening + 1)..closing].Trim(), true));
            remaining = remaining[(closing + 1)..];
        }
        return result.Where(item => !string.IsNullOrWhiteSpace(item.Text)).ToList();
    }
}
