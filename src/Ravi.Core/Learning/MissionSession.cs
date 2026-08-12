namespace Ravi.Core.Learning;

public sealed class MissionSession
{
    private int _stepIndex;

    public MissionSession(Mission mission)
    {
        Mission = mission ?? throw new ArgumentNullException(nameof(mission));
        if (mission.Steps.Count == 0)
            throw new ArgumentException("A mission needs at least one step.", nameof(mission));
    }

    public Mission Mission { get; }
    public LearningStep CurrentStep => Mission.Steps[_stepIndex];
    public int StepNumber => _stepIndex + 1;
    public int StepCount => Mission.Steps.Count;
    public bool IsComplete => CurrentStep.Type == LearningStepType.Complete;
    public double Progress => (double)StepNumber / StepCount;

    public void Advance()
    {
        if (_stepIndex < Mission.Steps.Count - 1)
            _stepIndex++;
    }

    public void Restart() => _stepIndex = 0;
}
