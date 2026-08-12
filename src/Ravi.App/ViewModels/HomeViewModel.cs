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
    private string _studentName = string.Empty;
    private string _ageText = string.Empty;
    private string _gender = string.Empty;
    private string _profileError = string.Empty;
    private LessonStep[] _steps = [];

    private static readonly LessonDefinition[] Grade7 =
    [
        new("01", "Ravi Says Hello", "Greetings", "hello · good morning · thank you · goodbye · friend", "سلام · صبح بخیر · متشکرم · خداحافظ · دوست", "I am → I’m", "Ravi finds a blue door near the school and greets {name}.", "راوی نزدیک مدرسه یک در آبی پیدا می‌کند و به {name} سلام می‌کند.", "What color is the door?", "blue"),
        new("02", "The New Student", "Names and spelling", "name · first name · last name · spell · student", "نام · نام کوچک · نام خانوادگی · هجی کردن · دانش‌آموز", "What’s your name? My name is …", "A new student arrives. Ravi helps {name} introduce themself.", "یک دانش‌آموز جدید می‌آید. راوی به {name} کمک می‌کند خودش را معرفی کند.", "Who helps the new student?", "ravi"),
        new("03", "Meet My Classmates", "Introducing people", "boy · girl · classmate · teacher · meet", "پسر · دختر · همکلاسی · معلم · ملاقات کردن", "This is … / He is … / She is …", "Behind the door, {name} meets Nika and Amir in a bright classroom.", "پشت در، {name} در یک کلاس روشن با نیکا و امیر آشنا می‌شود.", "Where are the children?", "classroom"),
        new("04", "Our Classroom", "School objects", "book · pen · pencil · desk · board", "کتاب · خودکار · مداد · میز · تخته", "This is a … / These are …", "Ravi’s golden key is hidden under a book on the teacher’s desk.", "کلید طلایی راوی زیر کتابی روی میز معلم پنهان شده است.", "Where is the key?", "book"),
        new("05", "Ravi’s Birthday Calendar", "Age and birthday", "age · birthday · year · month · today", "سن · تولد · سال · ماه · امروز", "How old are you? I’m … years old.", "The golden key shows {name} a calendar with a glowing birthday.", "کلید طلایی تقویمی با یک روز تولد درخشان به {name} نشان می‌دهد.", "What does the key show?", "calendar"),
        new("06", "A Week with Ravi", "Days and routines", "Saturday · Sunday · Monday · week · school", "شنبه · یکشنبه · دوشنبه · هفته · مدرسه", "on Monday / every day", "Ravi and {name} have seven days to find the next clue.", "راوی و {name} هفت روز فرصت دارند سرنخ بعدی را پیدا کنند.", "How many days do they have?", "seven"),
        new("07", "Meet the Family", "Family", "mother · father · sister · brother · family", "مادر · پدر · خواهر · برادر · خانواده", "my / your / his / her", "Nika’s family welcomes Ravi and {name} into their home.", "خانواده نیکا از راوی و {name} در خانه‌شان استقبال می‌کنند.", "Whose family welcomes them?", "nika"),
        new("08", "What Do They Do?", "Jobs", "teacher · doctor · nurse · driver · pilot", "معلم · پزشک · پرستار · راننده · خلبان", "What’s his job? He’s a …", "A pilot gives {name} a map that points to the golden path.", "یک خلبان نقشه‌ای به {name} می‌دهد که راه طلایی را نشان می‌دهد.", "Who gives the map?", "pilot"),
        new("09", "The Missing Red Scarf", "Clothes and colors", "scarf · shirt · shoes · red · blue", "روسری · پیراهن · کفش · قرمز · آبی", "He/She is wearing …", "A red scarf disappears beside the golden path. Ravi follows its thread.", "یک روسری قرمز کنار راه طلایی ناپدید می‌شود. راوی نخ آن را دنبال می‌کند.", "What color is the scarf?", "red"),
        new("10", "Who Is It?", "Appearance", "tall · short · young · old · hair", "قدبلند · کوتاه · جوان · پیر · مو", "Who is …? / Which one?", "{name} describes a tall woman who saw the scarf near the market.", "{name} زن قدبلندی را توصیف می‌کند که روسری را نزدیک بازار دیده است.", "Where did she see the scarf?", "market"),
        new("11", "Ravi Visits a House", "Rooms and furniture", "house · bedroom · kitchen · chair · table", "خانه · اتاق خواب · آشپزخانه · صندلی · میز", "in / on / under / next to", "The thread leads Ravi and {name} into a house with a secret room.", "نخ، راوی و {name} را به خانه‌ای با یک اتاق مخفی می‌برد.", "What is inside the house?", "room"),
        new("12", "What Is Everyone Doing?", "Actions now", "reading · cooking · washing · playing · working", "خواندن · آشپزی کردن · شستن · بازی کردن · کار کردن", "Present continuous: is/am/are + -ing", "While everyone is working, Ravi sees the golden light moving upstairs.", "وقتی همه مشغول کارند، راوی نور طلایی را می‌بیند که به طبقه بالا می‌رود.", "Where is the light moving?", "upstairs"),
        new("13", "Finding Ravi’s Address", "Address and phone", "address · street · home · telephone · number", "نشانی · خیابان · خانه · تلفن · شماره", "Where do you live? I live in …", "A note has Ravi’s address, but three numbers are missing.", "روی یک یادداشت نشانی راوی نوشته شده، اما سه شماره آن گم شده است.", "How many numbers are missing?", "three"),
        new("14", "The Clock Tower", "Time", "time · o’clock · morning · afternoon · evening", "زمان · ساعت · صبح · بعدازظهر · عصر", "What time is it? It’s … o’clock.", "At five o’clock, the golden key opens the clock tower.", "ساعت پنج، کلید طلایی برج ساعت را باز می‌کند.", "What time does the tower open?", "five"),
        new("15", "Ravi’s Picnic", "Food and preferences", "bread · rice · fruit · juice · hungry", "نان · برنج · میوه · آبمیوه · گرسنه", "I like … / I’d like … / Let’s …", "Ravi and {name} share fruit and juice before the final mission.", "راوی و {name} پیش از مأموریت نهایی میوه و آبمیوه را با هم تقسیم می‌کنند.", "What do they drink?", "juice"),
        new("16", "The Golden Acorn", "Review adventure", "key · door · clue · friend · adventure", "کلید · در · سرنخ · دوست · ماجراجویی", "Grade 7 grammar review", "{name} places the golden key in the last door. Ravi finds the Golden Acorn.", "{name} کلید طلایی را در آخرین در می‌گذارد. راوی بلوط طلایی را پیدا می‌کند.", "What does Ravi find?", "acorn"),
    ];

    private static readonly LessonDefinition[] Grade8 =
    [
        new("01", "Where Are You From?", "Countries", "country · Iran · France · China · Spain", "کشور · ایران · فرانسه · چین · اسپانیا", "Where are you from? I’m from …", "A letter invites Ravi and {name} to an international festival.", "نامه‌ای راوی و {name} را به یک جشنواره بین‌المللی دعوت می‌کند.", "What arrives?", "letter"),
        new("02", "Our Nationalities", "Nationalities", "Iranian · French · Chinese · Spanish · language", "ایرانی · فرانسوی · چینی · اسپانیایی · زبان", "Are you Iranian? Yes, I am.", "At the festival, {name} meets students from four countries.", "در جشنواره، {name} با دانش‌آموزانی از چهار کشور آشنا می‌شود.", "How many countries?", "four"),
        new("03", "Ravi’s Busy Morning", "Daily activities", "wake up · breakfast · school · study · morning", "بیدار شدن · صبحانه · مدرسه · درس خواندن · صبح", "Simple present with I/you/we/they", "Ravi wakes up early because the festival map is missing.", "راوی زود بیدار می‌شود چون نقشه جشنواره گم شده است.", "What is missing?", "map"),
        new("04", "The Weekly Plan", "Days and schedules", "weekday · weekend · afternoon · evening · every", "روز هفته · آخر هفته · بعدازظهر · عصر · هر", "When do you …? On …", "{name} finds a weekly plan with a clue on Wednesday afternoon.", "{name} برنامه هفتگی‌ای پیدا می‌کند که سرنخی برای بعدازظهر چهارشنبه دارد.", "When is the clue?", "wednesday"),
        new("05", "Ravi Can Help", "Abilities", "draw · swim · ride · climb · speak", "نقاشی کردن · شنا کردن · سوار شدن · بالا رفتن · صحبت کردن", "can / can’t", "Ravi can climb a wall, and {name} can read the hidden sign.", "راوی می‌تواند از دیوار بالا برود و {name} می‌تواند تابلوی مخفی را بخواند.", "What can Ravi do?", "climb"),
        new("06", "The Talent Team", "Questions about ability", "ability · team · question · answer · together", "توانایی · گروه · پرسش · پاسخ · با هم", "Can you …? Who can …?", "The friends combine their abilities to open a silver box.", "دوستان توانایی‌هایشان را با هم ترکیب می‌کنند تا یک جعبه نقره‌ای را باز کنند.", "What do they open?", "box"),
        new("07", "Ravi Has a Headache", "Health problems", "headache · toothache · cold · flu · sick", "سردرد · دندان‌درد · سرماخوردگی · آنفولانزا · بیمار", "I have … / What’s wrong?", "Ravi has a headache after reading the tiny message in the box.", "راوی پس از خواندن پیام ریز داخل جعبه سردرد می‌گیرد.", "What problem does Ravi have?", "headache"),
        new("08", "Helpful Advice", "Health advice", "rest · water · doctor · dentist · medicine", "استراحت · آب · پزشک · دندان‌پزشک · دارو", "Why don’t you …? / You should …", "{name} gives Ravi water and tells him to rest.", "{name} به راوی آب می‌دهد و به او می‌گوید استراحت کند.", "What does Ravi drink?", "water"),
        new("09", "Explore the City", "Places in a city", "museum · mosque · park · metro · center", "موزه · مسجد · پارک · مترو · مرکز", "There is … / There are …", "The message leads Ravi and {name} to a museum in the city center.", "پیام، راوی و {name} را به موزه‌ای در مرکز شهر هدایت می‌کند.", "Where is the museum?", "center"),
        new("10", "Finding the Way", "Directions and descriptions", "north · south · east · west · famous", "شمال · جنوب · شرق · غرب · مشهور", "Where is …? What is it like?", "A guide points west toward a famous old bridge.", "یک راهنما به سمت غرب و یک پل قدیمی مشهور اشاره می‌کند.", "Which direction?", "west"),
        new("11", "The Mountain Village", "Village and nature", "village · mountain · field · river · forest", "روستا · کوه · مزرعه · رودخانه · جنگل", "It is famous for …", "Across the bridge, {name} discovers a mountain village beside a river.", "آن سوی پل، {name} روستایی کوهستانی کنار رودخانه پیدا می‌کند.", "What is beside the village?", "river"),
        new("12", "Weather Watch", "Weather and seasons", "sunny · rainy · snowy · windy · season", "آفتابی · بارانی · برفی · بادی · فصل", "What’s the weather like? It’s …", "Strong wind reveals the festival map under a wooden bench.", "باد شدید نقشه جشنواره را زیر یک نیمکت چوبی آشکار می‌کند.", "What reveals the map?", "wind"),
        new("13", "Ravi’s Hobbies", "Hobbies", "reading · drawing · music · stories · collecting", "کتاب خواندن · نقاشی · موسیقی · داستان‌ها · جمع‌آوری", "Do you like …? Yes, I do.", "Ravi learns that reading stories is {name}’s favorite hobby.", "راوی می‌فهمد که خواندن داستان سرگرمی مورد علاقه {name} است.", "What is the hobby?", "reading"),
        new("14", "The Free-Time Festival", "Free-time activities", "shopping · walking · playing · listening · usually", "خرید کردن · پیاده‌روی · بازی کردن · گوش دادن · معمولاً", "What do you do in your free time?", "Ravi and {name} return the map and celebrate with their friends.", "راوی و {name} نقشه را برمی‌گردانند و با دوستانشان جشن می‌گیرند.", "What do they return?", "map"),
    ];

    public HomeViewModel()
    {
        Grades = new(Enumerable.Range(7, 6));
        Lessons = [];
        SelectRoleCommand = new Command<string>(SelectRole);
        SelectGenderCommand = new Command<string>(SelectGender);
        CreateProfileCommand = new Command(CreateProfile);
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
    public ICommand SelectGenderCommand { get; }
    public ICommand CreateProfileCommand { get; }
    public ICommand SelectGradeCommand { get; }
    public ICommand OpenLessonCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand CheckCommand { get; }
    public ICommand ToggleTranslationCommand { get; }

    public bool IsLogin => _screen == "Login";
    public bool IsProfile => _screen == "Profile";
    public bool IsGrades => _screen == "Grades";
    public bool IsLessons => _screen == "Lessons";
    public bool IsLearning => _screen == "Learning";
    public bool ShowBack => !IsLogin;
    public bool HasProfileError => !string.IsNullOrWhiteSpace(_profileError);
    public bool IsGirl => _gender == "Girl";
    public string GirlButtonColor => IsGirl ? "#FF6B35" : "#EDF8F7";
    public string GirlTextColor => IsGirl ? "White" : "#087F83";
    public string BoyButtonColor => _gender == "Boy" ? "#FF6B35" : "#EDF8F7";
    public string BoyTextColor => _gender == "Boy" ? "White" : "#087F83";
    public string StudentName { get => _studentName; set { _studentName = value; OnPropertyChanged(); } }
    public string AgeText { get => _ageText; set { _ageText = value; OnPropertyChanged(); } }
    public string ProfileError => _profileError;
    public string WelcomeTitle => $"Hello, {_studentName}!";
    public string WelcomeSubtitle => $"Ravi hat deinen Lernweg für dein Alter ({_ageText}) vorbereitet.";
    public string GradeTitle => $"Klasse {_grade}";
    public string StudentBadge => string.IsNullOrWhiteSpace(_studentName) ? "🔥 6" : $"🦊 {_studentName}";
    public string CourseProgressLabel => $"0 von {LessonCount} Lektionen";
    public int LessonCount => _grade == 8 ? Grade8.Length : Grade7.Length;
    public string StepEyebrow => CurrentStep.Eyebrow;
    public string StepTitle => CurrentStep.Title;
    public string StepContent => CurrentStep.Content;
    public string StepTranslation => _showTranslation ? CurrentStep.Translation : "ترجمه فارسی anzeigen";
    public string AnswerPrompt => CurrentStep.AnswerPrompt;
    public string ProgressLabel => $"Schritt {_stepIndex + 1} von {_steps.Length}";
    public double LessonProgress => _steps.Length == 0 ? 0 : (_stepIndex + 1d) / _steps.Length;
    public string NextLabel => _stepIndex == _steps.Length - 1 ? "Lektion abschließen  ✦" : "Weiter  →";
    public string Feedback => _feedback;
    public bool HasFeedback => !string.IsNullOrWhiteSpace(_feedback);
    public string Answer { get => _answer; set { _answer = value; OnPropertyChanged(); } }
    private LessonStep CurrentStep => _steps.Length == 0 ? LessonStep.Empty : _steps[_stepIndex];

    private void SelectRole(string? role) { if (role == "Student") { _screen = "Profile"; NotifyAll(); } }
    private void SelectGender(string? gender) { _gender = gender is "Girl" or "Boy" ? gender : string.Empty; _profileError = string.Empty; NotifyAll(); }

    private void CreateProfile()
    {
        _studentName = _studentName.Trim();
        if (_studentName.Length < 2) _profileError = "Bitte gib deinen Namen ein.";
        else if (!int.TryParse(_ageText, out var age) || age is < 10 or > 20) _profileError = "Bitte gib ein Alter zwischen 10 und 20 ein.";
        else if (string.IsNullOrWhiteSpace(_gender)) _profileError = "Bitte wähle Mädchen oder Junge.";
        else { _ageText = age.ToString(); _profileError = string.Empty; _screen = "Grades"; }
        NotifyAll();
    }

    private void SelectGrade(int grade)
    {
        _grade = grade;
        LoadLessons();
        _screen = "Lessons";
        NotifyAll();
    }

    private void LoadLessons()
    {
        Lessons.Clear();
        var definitions = _grade == 8 ? Grade8 : Grade7;
        if (_grade is not (7 or 8)) return;

        foreach (var item in definitions)
            Lessons.Add(new(item.Number, item.Title, $"{item.Topic} · exam", "25 min", true, false));

        Lessons.Add(new("★", $"Grade {_grade} Final Mission", "Complete level exam · certificate", "45 min", true, true));
    }

    private void OpenLesson(LessonCard? lesson)
    {
        if (lesson?.IsAvailable != true) return;
        _steps = lesson.IsFinalExam ? BuildFinalExam() : BuildLesson(GetDefinition(lesson.Number));
        _stepIndex = 0;
        _screen = "Learning";
        ResetExercise();
    }

    private LessonDefinition GetDefinition(string number) =>
        (_grade == 8 ? Grade8 : Grade7).First(item => item.Number == number);

    private LessonStep[] BuildLesson(LessonDefinition lesson)
    {
        var subject = IsGirl ? "she" : "he";
        var subjectTitle = IsGirl ? "She" : "He";
        var obj = IsGirl ? "her" : "him";
        var story = Personalize(lesson.Story);
        var storyFarsi = Personalize(lesson.StoryFarsi);
        return
        [
            new("01 · WELCOME", $"{lesson.Title}, {_studentName}!", $"Hello, {_studentName}! Today’s mission is {lesson.Topic}.", $"سلام {_studentName}! مأموریت امروز درباره {lesson.Topic} است.", "hello", "Antworte Ravi mit einer englischen Begrüßung."),
            new("02 · VOCABULARY", "New words", lesson.Words, lesson.WordsFarsi, lesson.Words.Split('·')[0].Trim(), "Schreibe eines der neuen englischen Wörter."),
            new("03 · ENGLISH → FARSI", "Translate", lesson.Words, lesson.WordsFarsi, lesson.WordsFarsi.Split('·')[0].Trim(), "Übersetze das erste englische Wort ins Persische."),
            new("04 · FARSI → ENGLISH", "Translate back", lesson.WordsFarsi, lesson.Words, lesson.Words.Split('·')[0].Trim(), "Übersetze das erste persische Wort ins Englische."),
            new("05 · STORY", "Ravi’s continuing adventure", story, storyFarsi, lesson.Expected, lesson.Question),
            new("06 · GRAMMAR", lesson.Grammar, $"Personal example: {_studentName} is {_ageText} years old. {subjectTitle} is Ravi’s friend.", $"مثال شخصی: {_studentName} {_ageText} سال دارد. او دوست راوی است.", subject, $"Complete with he or she: {_studentName} is my friend. ___ is {_ageText}."),
            new("07 · LISTENING", "Listen without reading first", story, storyFarsi, lesson.Expected, lesson.Question),
            new("08 · DICTATION", "Listen and write", $"{_studentName} is Ravi’s friend.", $"{_studentName} دوست راوی است.", _studentName, "Schreibe den gehörten englischen Satz."),
            new("09 · WRITING", "Your turn", $"Write two sentences about {lesson.Topic.ToLowerInvariant()}. Ravi gives {obj} a golden leaf for a complete answer.", $"دو جمله درباره موضوع درس بنویس. راوی برای پاسخ کامل یک برگ طلایی می‌دهد.", _studentName, $"Begin with: My name is {_studentName}."),
            new("10 · LESSON EXAM", "Ravi’s Challenge", $"Exam: vocabulary, English↔Farsi, grammar, listening, dictation, reading and writing. Pass mark: 60%. Story question: {lesson.Question}", $"آزمون: واژگان، ترجمه دوطرفه، دستور زبان، شنیداری، املا، خواندن و نوشتن. حد قبولی: ۶۰٪", lesson.Expected, lesson.Question),
            new("11 · REWARD", $"Well done, {_studentName}!", $"You completed {lesson.Title}. Reward: up to 3 stars, 50 golden leaves and a Ravi badge.", $"آفرین {_studentName}! این درس را تمام کردی. جایزه: تا سه ستاره، ۵۰ برگ طلایی و نشان راوی.", "ravi", "Schreibe: Thank you, Ravi!"),
        ];
    }

    private LessonStep[] BuildFinalExam()
    {
        var gradeName = _grade == 8 ? "Prospect 2" : "Prospect 1";
        var review = _grade == 8 ? "nationality · routines · abilities · health · places · weather · hobbies" : "greetings · family · appearance · home · time · food";
        return
        [
            new("FINAL · INTRO", $"Grade {_grade} Final Mission", $"{_studentName}, this exam reviews all lessons from {gradeName}. You pass with 60%.", $"{_studentName}، این آزمون همه درس‌های این پایه را مرور می‌کند. حد قبولی ۶۰٪ است.", "ready", "Schreibe: I am ready."),
            new("01 · VOCABULARY", "Vocabulary review", review, "مرور واژگان همه درس‌ها", review.Split('·')[0].Trim(), "Schreibe das erste Thema auf Englisch."),
            new("02 · TRANSLATION", "English → Farsi", $"Translate three sentences from the Grade {_grade} story world.", "سه جمله از دنیای داستانی این پایه را ترجمه کن.", "ravi", "Übersetze: Ravi is my friend."),
            new("03 · TRANSLATION", "Farsi → English", "راوی دوست من است.", "Ravi is my friend.", "ravi", "Übersetze den persischen Satz ins Englische."),
            new("04 · GRAMMAR", "Grammar review", $"Use the grammar from every Grade {_grade} lesson in new situations.", "از دستور زبان همه درس‌های این پایه در موقعیت‌های جدید استفاده کن.", IsGirl ? "she" : "he", $"Complete: {_studentName} is my friend. ___ is {_ageText}."),
            new("05 · LISTENING", "Listening mission", $"Ravi and {_studentName} open the final door and find a certificate.", $"راوی و {_studentName} در آخر را باز می‌کنند و یک گواهی پیدا می‌کنند.", "certificate", "What do they find?"),
            new("06 · DICTATION", "Final dictation", $"I finished Grade {_grade} with Ravi.", $"من پایه {_grade} را با راوی تمام کردم.", $"grade {_grade}", "Schreibe den gehörten Satz."),
            new("07 · READING", "Story comprehension", $"After many missions, {_studentName} returns every clue to Ravi. The Golden Acorn shines and the next learning world opens.", $"پس از مأموریت‌های بسیار، {_studentName} همه سرنخ‌ها را به راوی برمی‌گرداند. بلوط طلایی می‌درخشد و دنیای آموزشی بعدی باز می‌شود.", "acorn", "What shines?"),
            new("08 · WRITING", "Final writing", $"Write four sentences about yourself and your adventure with Ravi.", "چهار جمله درباره خودت و ماجراجویی‌ات با راوی بنویس.", _studentName, "Include your name, age, one thing you like and one thing you learned."),
            new("RESULT", $"Grade {_grade} Certificate", $"Congratulations, {_studentName}! A score of 60% unlocks the next grade. Your certificate records vocabulary, grammar, listening, reading and writing results.", $"تبریک {_studentName}! نمره ۶۰٪ پایه بعدی را باز می‌کند. نتیجه مهارت‌های مختلف در گواهی ثبت می‌شود.", "thank", "Schreibe: Thank you, Ravi!"),
        ];
    }

    private string Personalize(string text) => text.Replace("{name}", _studentName, StringComparison.Ordinal);
    private void Next() { if (_stepIndex < _steps.Length - 1) _stepIndex++; else _screen = "Lessons"; ResetExercise(); }
    private void Back() { _screen = _screen switch { "Learning" => "Lessons", "Lessons" => "Grades", "Grades" => "Profile", "Profile" => "Login", _ => "Login" }; NotifyAll(); }

    private void CheckAnswer()
    {
        var expected = CurrentStep.Expected;
        _feedback = _answer.Trim().Contains(expected, StringComparison.OrdinalIgnoreCase)
            ? $"Richtig, {_studentName}! Ravi ist stolz auf dich ✨"
            : $"Fast, {_studentName}! Ravi hilft dir: Die Antwort enthält „{expected}“.";
        NotifyAll();
    }

    private void ResetExercise() { _answer = string.Empty; _feedback = string.Empty; _showTranslation = false; NotifyAll(); }
    private void NotifyAll() => OnPropertyChanged(string.Empty);
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));

    private sealed record LessonDefinition(string Number, string Title, string Topic, string Words, string WordsFarsi, string Grammar, string Story, string StoryFarsi, string Question, string Expected);
    private sealed record LessonStep(string Eyebrow, string Title, string Content, string Translation, string Expected, string AnswerPrompt)
    {
        public static LessonStep Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
    }
}

public sealed record LessonCard(string Number, string Title, string Meta, string Duration, bool IsAvailable, bool IsFinalExam);
