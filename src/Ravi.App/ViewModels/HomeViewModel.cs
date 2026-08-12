using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Ravi.App.ViewModels;

public sealed class HomeViewModel : INotifyPropertyChanged
{
    private string _screen = "Login";
    private int _grade = 7;
    private int _stepIndex;
    private string _answer = string.Empty;
    private string _feedback = string.Empty;
    private bool _showTranslation;

    private readonly LessonStep[] _steps =
    [
        new("01 · NEW WORDS", "Vocabulary", "journey", "سفر", "journey"),
        new("02 · STORY", "Ravi's first journey", "Ravi packed his little blue bag and followed the morning sun.", "راوی کیف آبی کوچکش را بست و خورشید صبح را دنبال کرد.", "ravi"),
        new("03 · GRAMMAR", "Past simple", "Ravi packed his bag. We use the past simple for completed actions.", "برای کارهای تمام‌شده در گذشته استفاده می‌کنیم.", "packed"),
        new("04 · LISTENING", "Listen & understand", "Where did Ravi go? Listen carefully, then answer.", "راوی کجا رفت؟ با دقت گوش کن.", "school"),
        new("05 · WRITING", "Your turn", "Write one English sentence about your own journey.", "یک جمله انگلیسی درباره سفر خودت بنویس.", "journey")
    ];

    public HomeViewModel()
    {
        Grades = new(Enumerable.Range(7, 6));
        Lessons = new()
        {
            new("01", "A new journey", "12 words · 6 activities", "18 min", true),
            new("02", "At school", "10 words · 5 activities", "15 min", true),
            new("03", "Friends & family", "14 words · 7 activities", "22 min", true),
            new("04", "Around the city", "Coming soon", "", false)
        };
        SelectRoleCommand = new Command<string>(SelectRole);
        SelectGradeCommand = new Command<int>(SelectGrade);
        OpenLessonCommand = new Command<LessonCard>(OpenLesson);
        NextCommand = new Command(Next);
        BackCommand = new Command(Back);
        CheckCommand = new Command(CheckAnswer);
        ToggleTranslationCommand = new Command(() => { _showTranslation = !_showTranslation; NotifyAll(); });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<int> Grades { get; }
    public ObservableCollection<LessonCard> Lessons { get; }
    public ICommand SelectRoleCommand { get; }
    public ICommand SelectGradeCommand { get; }
    public ICommand OpenLessonCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand CheckCommand { get; }
    public ICommand ToggleTranslationCommand { get; }
    public bool IsLogin => _screen == "Login";
    public bool ShowBack => !IsLogin;
    public bool IsGrades => _screen == "Grades";
    public bool IsLessons => _screen == "Lessons";
    public bool IsLearning => _screen == "Learning";
    public string GradeTitle => $"Klasse {_grade}";
    public string StepEyebrow => _steps[_stepIndex].Eyebrow;
    public string StepTitle => _steps[_stepIndex].Title;
    public string StepContent => _steps[_stepIndex].Content;
    public string StepTranslation => _showTranslation ? _steps[_stepIndex].Translation : "Übersetzung anzeigen";
    public string ProgressLabel => $"Schritt {_stepIndex + 1} von {_steps.Length}";
    public double LessonProgress => (_stepIndex + 1d) / _steps.Length;
    public string NextLabel => _stepIndex == _steps.Length - 1 ? "Lektion abschließen  ✦" : "Weiter  →";
    public string Feedback => _feedback;
    public bool HasFeedback => !string.IsNullOrWhiteSpace(_feedback);
    public string Answer { get => _answer; set { _answer = value; OnPropertyChanged(); } }

    private void SelectRole(string? role) { if (role == "Student") { _screen = "Grades"; NotifyAll(); } }
    private void SelectGrade(int grade) { _grade = grade; _screen = "Lessons"; NotifyAll(); }
    private void OpenLesson(LessonCard? lesson) { if (lesson?.IsAvailable != true) return; _stepIndex = 0; _screen = "Learning"; ResetExercise(); }
    private void Next() { if (_stepIndex < _steps.Length - 1) _stepIndex++; else _screen = "Lessons"; ResetExercise(); }
    private void Back() { _screen = _screen switch { "Learning" => "Lessons", "Lessons" => "Grades", "Grades" => "Login", _ => "Login" }; NotifyAll(); }
    private void CheckAnswer()
    {
        var expected = _steps[_stepIndex].Expected;
        _feedback = _answer.Trim().Contains(expected, StringComparison.OrdinalIgnoreCase)
            ? "Richtig! Ravi ist stolz auf dich ✨" : $"Fast! Tipp: Die Antwort enthält „{expected}“.";
        NotifyAll();
    }
    private void ResetExercise() { _answer = ""; _feedback = ""; _showTranslation = false; NotifyAll(); }
    private void NotifyAll() => OnPropertyChanged(string.Empty);
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
    private sealed record LessonStep(string Eyebrow, string Title, string Content, string Translation, string Expected);
}

public sealed record LessonCard(string Number, string Title, string Meta, string Duration, bool IsAvailable);
