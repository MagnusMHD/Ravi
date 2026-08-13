using Ravi.App.ViewModels;
using System.ComponentModel;

namespace Ravi.App;

public partial class MainPage : ContentPage
{
    private CancellationTokenSource? _animationCancellation;
    private CancellationTokenSource? _speechCancellation;
    private string _lastScreen = "Login";
    private int _lastStep = -1;

    public MainPage()
    {
        InitializeComponent();
        var viewModel = new HomeViewModel();
        viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        BindingContext = viewModel;
    }

    private async void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not HomeViewModel vm)
            return;

        if (_lastScreen != vm.ScreenKey)
        {
            _lastScreen = vm.ScreenKey;
            await AnimatePageTransitionAsync();
        }
        else if (vm.IsLearning && _lastStep != vm.StepIndex)
        {
            _lastStep = vm.StepIndex;
            await AnimateLessonTransitionAsync();
        }

        if (e.PropertyName is null or "" && vm.HasFeedback)
            await AnimateFeedbackAsync(vm.LastAnswerCorrect);
    }

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
        var characters = new[] { RaviMascot, RaviProfile, RaviCourse };
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
            await SpeakWithRaviAsync(vm.StepContent, new SpeechOptions { Locale = english, Rate = 0.62f });
        }
    }

    private async void SpeakWordClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: VocabularyItem word })
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var english = locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
            await SpeakWithRaviAsync(word.English, new SpeechOptions { Locale = english, Rate = 0.58f });
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
        await SpeakWithRaviAsync(text, new SpeechOptions { Locale = english, Rate = 0.60f });
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
                    await SpeakWithRaviAsync(text, options, token);
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

    private async Task SpeakWithRaviAsync(string text, SpeechOptions options, CancellationToken cancellationToken = default)
    {
        using var animationCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var pulse = AnimateSpeakingAsync(animationCancellation.Token);
        try
        {
            await TextToSpeech.Default.SpeakAsync(text, options, cancellationToken);
        }
        finally
        {
            animationCancellation.Cancel();
            try { await pulse; } catch (OperationCanceledException) { }
            RaviLesson.CancelAnimations();
            RaviLesson.Scale = 1;
            RaviLesson.TranslationY = 0;
        }
    }

    private async Task AnimateSpeakingAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await RaviLesson.ScaleToAsync(1.025, 280, Easing.SinInOut);
            await RaviLesson.TranslateToAsync(0, -3, 260, Easing.SinInOut);
            await RaviLesson.ScaleToAsync(1, 280, Easing.SinInOut);
            await RaviLesson.TranslateToAsync(0, 0, 260, Easing.SinInOut);
        }
    }

    private async Task AnimatePageTransitionAsync()
    {
        PageContent.CancelAnimations();
        PageContent.Opacity = 0;
        PageContent.TranslationY = 12;
        await Task.WhenAll(
            PageContent.FadeToAsync(1, 260, Easing.CubicOut),
            PageContent.TranslateToAsync(0, 0, 320, Easing.CubicOut));
    }

    private async Task AnimateLessonTransitionAsync()
    {
        LearningContent.CancelAnimations();
        LearningContent.Opacity = 0.35;
        LearningContent.TranslationX = 10;
        await Task.WhenAll(
            LearningContent.FadeToAsync(1, 220, Easing.CubicOut),
            LearningContent.TranslateToAsync(0, 0, 260, Easing.CubicOut));
    }

    private async Task AnimateFeedbackAsync(bool isCorrect)
    {
        FeedbackCard.CancelAnimations();
        RaviLesson.CancelAnimations();
        if (isCorrect)
        {
            await Task.WhenAll(
                FeedbackCard.ScaleToAsync(1.015, 150, Easing.CubicOut),
                RaviLesson.TranslateToAsync(0, -10, 180, Easing.CubicOut));
            await Task.WhenAll(
                FeedbackCard.ScaleToAsync(1, 220, Easing.CubicOut),
                RaviLesson.TranslateToAsync(0, 0, 260, Easing.CubicOut));
        }
        else
        {
            await RaviLesson.TranslateToAsync(-5, 0, 90, Easing.Linear);
            await RaviLesson.TranslateToAsync(5, 0, 90, Easing.Linear);
            await RaviLesson.TranslateToAsync(0, 0, 110, Easing.CubicOut);
        }
    }
}
