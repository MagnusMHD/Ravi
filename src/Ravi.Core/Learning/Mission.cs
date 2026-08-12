namespace Ravi.Core.Learning;

public sealed record Mission(
    string Id,
    string Title,
    string Subtitle,
    int EstimatedMinutes,
    IReadOnlyList<LearningStep> Steps);
