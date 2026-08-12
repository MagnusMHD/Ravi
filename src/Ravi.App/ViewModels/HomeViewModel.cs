using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Ravi.Core.Learning;

namespace Ravi.App.ViewModels;

public sealed class HomeViewModel : INotifyPropertyChanged
{
    private readonly MissionSession _session = new(DemoMissionFactory.CreateWelcomeMission());
    private bool _missionOpen;

    public HomeViewModel()
    {
        PrimaryCommand = new Command(HandlePrimaryAction);
        Refresh();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand PrimaryCommand { get; }
    public string Greeting => "سلام، قهرمان!";
    public string MissionTitle => _session.Mission.Title;
    public string MissionSubtitle => _session.Mission.Subtitle;
    public string StepEyebrow { get; private set; } = string.Empty;
    public string StepTitle { get; private set; } = string.Empty;
    public string PersianHint { get; private set; } = string.Empty;
    public string StepContent { get; private set; } = string.Empty;
    public string PrimaryAction { get; private set; } = string.Empty;
    public string ProgressText { get; private set; } = string.Empty;
    public double Progress { get; private set; }
    public bool IsMissionOpen => _missionOpen;
    public bool IsHomeVisible => !_missionOpen;

    private void HandlePrimaryAction()
    {
        if (!_missionOpen)
        {
            _missionOpen = true;
        }
        else if (_session.IsComplete)
        {
            _session.Restart();
            _missionOpen = false;
        }
        else
        {
            _session.Advance();
        }

        Refresh();
    }

    private void Refresh()
    {
        var step = _session.CurrentStep;
        StepEyebrow = step.Eyebrow;
        StepTitle = step.Title;
        PersianHint = step.PersianHint;
        StepContent = step.Content;
        PrimaryAction = _missionOpen ? step.ActionLabel : "Heutige Mission starten";
        Progress = _missionOpen ? _session.Progress : 0.28;
        ProgressText = _missionOpen
            ? $"Schritt {_session.StepNumber} von {_session.StepCount}"
            : "Wochenziel 2 von 7 Missionen";

        OnPropertyChanged(string.Empty);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
