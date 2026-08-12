namespace Ravi.Core.Learning;

public sealed record LearningStep(
    LearningStepType Type,
    string Eyebrow,
    string Title,
    string PersianHint,
    string Content,
    string ActionLabel);
