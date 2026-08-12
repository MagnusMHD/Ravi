using Ravi.Core.Learning;

var mission = DemoMissionFactory.CreateWelcomeMission();
var session = new MissionSession(mission);

Ensure(session.StepNumber == 1, "Mission starts at step 1");
Ensure(session.CurrentStep.Type == LearningStepType.Vocabulary, "Vocabulary comes first");
Ensure(session.Progress > 0 && session.Progress < 1, "Initial progress is valid");

for (var i = 1; i < mission.Steps.Count; i++)
    session.Advance();

Ensure(session.IsComplete, "Mission reaches its completion step");
Ensure(Math.Abs(session.Progress - 1) < 0.001, "Completed progress is 100%");

session.Advance();
Ensure(session.StepNumber == mission.Steps.Count, "Advance cannot exceed the final step");

session.Restart();
Ensure(session.StepNumber == 1 && !session.IsComplete, "Restart returns to the first step");

Console.WriteLine("RAVI Core checks passed.");

static void Ensure(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException($"Check failed: {message}");
}
