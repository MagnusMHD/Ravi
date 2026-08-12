namespace Ravi.Core.Learning;

public static class DemoMissionFactory
{
    public static Mission CreateWelcomeMission() => new(
        "grade7-welcome-demo",
        "The Hidden Message",
        "پیام پنهان • Demo-Mission",
        12,
        [
            new(
                LearningStepType.Vocabulary,
                "1 • WORD POWER",
                "Hello, friend!",
                "سلام، دوست من!",
                "hello  •  friend  •  name  •  student",
                "Wörter gelernt"),
            new(
                LearningStepType.Story,
                "2 • STORY",
                "Ravi finds a message",
                "راوی یک پیام پیدا می‌کند",
                "“Hello! My name is Ravi. What is your name?”",
                "Weiter zur Geschichte"),
            new(
                LearningStepType.Grammar,
                "3 • GRAMMAR",
                "My name is …",
                "برای معرفی خودمان",
                "I am Ravi.  •  My name is Ravi.  •  What is your name?",
                "Muster verstanden"),
            new(
                LearningStepType.Listening,
                "4 • LISTEN",
                "Listen for the name",
                "به اسم گوش کن",
                "🔊  Hello! My name is Sara. I am a student.",
                "Audio verstanden"),
            new(
                LearningStepType.Complete,
                "MISSION COMPLETE",
                "The first clue is yours!",
                "اولین سرنخ برای توست!",
                "+ 20 XP   •   4 new words   •   Story clue unlocked",
                "Zurück zur Übersicht")
        ]);
}
