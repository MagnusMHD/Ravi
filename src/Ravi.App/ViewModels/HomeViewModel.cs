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
    private string _currentLessonNumber = string.Empty;
    private LessonStep[] _steps = [];
    public bool LastAnswerCorrect { get; private set; }

    private static readonly VocabularyItem[] Grade7Lesson1Vocabulary =
    [
        new(1, "hello", "/həˈləʊ/", "hə-LOU", "سلام"),
        new(2, "hi", "/haɪ/", "hai", "سلام"),
        new(3, "good morning", "/ɡʊd ˈmɔːnɪŋ/", "gud MOR-ning", "صبح بخیر"),
        new(4, "good afternoon", "/ɡʊd ˌɑːftəˈnuːn/", "gud af-tə-NUN", "بعدازظهر بخیر"),
        new(5, "good evening", "/ɡʊd ˈiːvnɪŋ/", "gud IW-ning", "عصر بخیر"),
        new(6, "goodbye", "/ˌɡʊdˈbaɪ/", "gud-BAI", "خداحافظ"),
        new(7, "see you", "/siː juː/", "sii ju", "می‌بینمت / به امید دیدار"),
        new(8, "please", "/pliːz/", "pliis", "لطفاً"),
        new(9, "thank you", "/ˈθæŋk juː/", "THÄNK ju", "متشکرم"),
        new(10, "fine", "/faɪn/", "fain", "خوب"),
        new(11, "great", "/ɡreɪt/", "greit", "عالی"),
        new(12, "tired", "/ˈtaɪəd/", "TAI-əd", "خسته"),
        new(13, "today", "/təˈdeɪ/", "tə-DEI", "امروز"),
        new(14, "name", "/neɪm/", "neim", "نام"),
        new(15, "friend", "/frend/", "frend", "دوست")
    ];

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
        Grades =
        [
            new(7, "هفتم", "Start the adventure"),
            new(8, "هشتم", "Build your confidence"),
            new(9, "نهم", "Explore new worlds"),
            new(10, "دهم", "Master new skills"),
            new(11, "یازدهم", "Speak with confidence"),
            new(12, "دوازدهم", "Complete the journey")
        ];
        Lessons = [];
        VocabularyItems = [];
        LessonLines = [];
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
    public ObservableCollection<GradeChoice> Grades { get; }
    public ObservableCollection<LessonCard> Lessons { get; }
    public ObservableCollection<VocabularyItem> VocabularyItems { get; }
    public ObservableCollection<LessonLine> LessonLines { get; }
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
    public string ScreenKey => _screen;
    public int StepIndex => _stepIndex;
    public bool ShowBack => !IsLogin;
    public bool HasProfileError => !string.IsNullOrWhiteSpace(_profileError);
    public bool IsGirl => _gender == "Girl";
    public string GirlButtonColor => IsGirl ? "#173A69" : "White";
    public string GirlTextColor => IsGirl ? "White" : "#173A69";
    public string BoyButtonColor => _gender == "Boy" ? "#173A69" : "White";
    public string BoyTextColor => _gender == "Boy" ? "White" : "#173A69";
    public string StudentName { get => _studentName; set { _studentName = value; OnPropertyChanged(); } }
    public string AgeText { get => _ageText; set { _ageText = value; OnPropertyChanged(); } }
    public string ProfileError => _profileError;
    public string WelcomeTitle => $"Hello, {_studentName}!";
    public string WelcomeSubtitle => $"Ravi has prepared a personal learning path for age {_ageText}.";
    public string GradeTitle => $"Grade {_grade}";
    public string StudentBadge => string.IsNullOrWhiteSpace(_studentName) ? "STUDENT" : _studentName;
    public string CourseProgressLabel => $"0 of {LessonCount} lessons completed";
    public int LessonCount => _grade == 8 ? Grade8.Length : Grade7.Length;
    public string StepEyebrow => CurrentStep.Eyebrow;
    public string StepTitle => CurrentStep.Title;
    public string StepContent => CurrentStep.Content;
    public string StepTranslation => _showTranslation ? CurrentStep.Translation : "Show Persian translation  فارسی";
    public string StoryTranslation => CurrentStep.Translation;
    public string StoryOpeningImage => IsGirl ? "lesson1_secret_door.jpg" : "lesson1_secret_door_boy.jpg";
    public string StoryEndingImage => IsGirl ? "lesson1_magic_passage.jpg" : "lesson1_magic_passage_boy.jpg";
    public string AnswerPrompt => CurrentStep.AnswerPrompt;
    public string ProgressLabel => $"Step {_stepIndex + 1} of {_steps.Length}";
    public double LessonProgress => _steps.Length == 0 ? 0 : (_stepIndex + 1d) / _steps.Length;
    public string NextLabel => _stepIndex == _steps.Length - 1 ? "Complete lesson" : "Continue  →";
    public string Feedback => _feedback;
    public bool HasFeedback => !string.IsNullOrWhiteSpace(_feedback);
    public string Answer { get => _answer; set { _answer = value; OnPropertyChanged(); } }
    private LessonStep CurrentStep => _steps.Length == 0 ? LessonStep.Empty : _steps[_stepIndex];
    public bool IsVocabularyStep => CurrentStep.Eyebrow.EndsWith("VOCABULARY", StringComparison.Ordinal);
    public bool IsPhraseStep => _grade == 7 && _currentLessonNumber == "01" && CurrentStep.Eyebrow.StartsWith("02 · PRONUNCIATION", StringComparison.Ordinal);
    public bool IsStoryStep => CurrentStep.Eyebrow.Contains("STORY", StringComparison.Ordinal);
    public bool IsLessonOneStory => IsStoryStep && _grade == 7 && _currentLessonNumber == "01";
    public bool IsRegularStep => !IsVocabularyStep && !IsPhraseStep && !IsStoryStep;
    public bool ShowExercise => !IsVocabularyStep && !IsPhraseStep && !IsStoryStep;
    public bool HasTranslation => !string.IsNullOrWhiteSpace(CurrentStep.Translation);
    public bool CanSpeakContent => CurrentStep.Eyebrow.Contains("LISTENING", StringComparison.Ordinal) || CurrentStep.Eyebrow.Contains("DICTATION", StringComparison.Ordinal);
    public string RaviTip => _grade == 7
        ? IsVocabularyStep ? "روی واژه انگلیسی بزن تا راوی آن را تلفظ کند."
        : IsPhraseStep ? "روی هر جمله بزن، گوش کن و سپس با صدای بلند تکرار کن."
        : IsStoryStep ? "داستان را بخوان. با لمس متن انگلیسی، راوی کل داستان را برایت می‌خواند."
        : "راوی کنار توست؛ راهنمای فارسی را بخوان و پاسخ را بنویس."
        : "Ravi is here to help you at every step.";

    private void SelectRole(string? role) { if (role == "Student") { _screen = "Profile"; NotifyAll(); } }
    private void SelectGender(string? gender) { _gender = gender is "Girl" or "Boy" ? gender : string.Empty; _profileError = string.Empty; NotifyAll(); }

    private void CreateProfile()
    {
        _studentName = _studentName.Trim();
        if (_studentName.Length < 2) _profileError = "Please enter your name.";
        else if (!int.TryParse(_ageText, out var age) || age is < 10 or > 20) _profileError = "Please enter an age between 10 and 20.";
        else if (string.IsNullOrWhiteSpace(_gender)) _profileError = "Please choose girl or boy.";
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
            Lessons.Add(new(item.Number, item.Title, $"{item.Topic} · vocabulary · story · practice", "25 min", true, false));

        Lessons.Add(new("★", $"Grade {_grade} Final Mission", "Complete level exam · certificate", "45 min", true, true));
    }

    private void OpenLesson(LessonCard? lesson)
    {
        if (lesson?.IsAvailable != true) return;
        _currentLessonNumber = lesson.Number;
        _steps = lesson.IsFinalExam ? BuildFinalExam() : BuildLesson(GetDefinition(lesson.Number));
        _stepIndex = 0;
        _screen = "Learning";
        ResetExercise();
    }

    private LessonDefinition GetDefinition(string number) =>
        (_grade == 8 ? Grade8 : Grade7).First(item => item.Number == number);

    private LessonStep[] BuildLesson(LessonDefinition lesson)
    {
        if (_grade == 7 && lesson.Number == "01")
            return BuildGrade7Lesson1();

        var subject = IsGirl ? "she" : "he";
        var subjectTitle = IsGirl ? "She" : "He";
        var obj = IsGirl ? "her" : "him";
        var story = BuildStory(lesson);
        var storyFarsi = BuildStoryFarsi(lesson);
        return
        [
            new("01 · WELCOME", $"{lesson.Title}, {_studentName}!", $"Hello, {_studentName}! Today’s mission is {lesson.Topic}.", $"سلام {_studentName}! مأموریت امروز درباره {lesson.Topic} است.", "hello", "یک سلام انگلیسی برای راوی بنویس."),
            new("02 · VOCABULARY", "New vocabulary", lesson.Words, lesson.WordsFarsi, lesson.Words.Split('·')[0].Trim(), "Tap each speaker, listen, and learn every word."),
            new("03 · ENGLISH → FARSI", "English to Persian", lesson.Words, lesson.WordsFarsi, lesson.WordsFarsi.Split('·')[0].Trim(), "معنی فارسی اولین واژه انگلیسی را بنویس."),
            new("04 · FARSI → ENGLISH", "Persian to English", lesson.WordsFarsi, lesson.Words, lesson.Words.Split('·')[0].Trim(), "معادل انگلیسی اولین واژه فارسی را بنویس."),
            new("05 · STORY", "Ravi’s continuing adventure", story, storyFarsi, lesson.Expected, "پرسش داستان را با یک واژه انگلیسی پاسخ بده. پاسخ در متن است."),
            new("06 · GRAMMAR", lesson.Grammar, $"در این بخش یاد می‌گیریم چگونه جمله را بسازیم. به نمونه نگاه کن:\n{_studentName} is {_ageText} years old.\n{subjectTitle} is Ravi’s friend.\nکلمهٔ «{subjectTitle.ToLowerInvariant()}» برای اشاره دوباره به نام به کار می‌رود.", "", subject, $"جای خالی را با he یا she پر کن:\n{_studentName} is my friend. ___ is {_ageText}."),
            new("07 · LISTENING", "Listen without reading first", story, storyFarsi, lesson.Expected, "ابتدا فقط گوش کن. سپس پاسخ پرسش داستان را با یک واژه انگلیسی بنویس."),
            new("08 · DICTATION", "Listen and write", $"{_studentName} is Ravi’s friend.", $"{_studentName} دوست راوی است.", _studentName, "بدون نگاه کردن به متن گوش کن و جمله انگلیسی را بنویس."),
            new("09 · WRITING", "Your turn", $"Write two sentences about {lesson.Topic.ToLowerInvariant()}. Ravi gives {obj} a golden leaf for a complete answer.", $"دو جمله درباره موضوع درس بنویس. راوی برای پاسخ کامل یک برگ طلایی می‌دهد.", _studentName, $"دو جمله انگلیسی بنویس. با این جمله شروع کن: My name is {_studentName}."),
            new("10 · LESSON EXAM", "Ravi’s Challenge", $"Exam: vocabulary, English↔Farsi, grammar, listening, dictation, reading and writing. Pass mark: 60%. Story question: {lesson.Question}", $"آزمون: واژگان، ترجمه دوطرفه، دستور زبان، شنیداری، املا، خواندن و نوشتن. حد قبولی: ۶۰٪", lesson.Expected, lesson.Question),
            new("11 · REWARD", $"Well done, {_studentName}!", $"You completed {lesson.Title}. Reward: up to 3 stars, 50 golden leaves and a Ravi badge.", $"آفرین {_studentName}! این درس را تمام کردی. جایزه: تا سه ستاره، ۵۰ برگ طلایی و نشان راوی.", "ravi", "Write: Thank you, Ravi!"),
        ];
    }

    private LessonStep[] BuildGrade7Lesson1()
    {
        var companionName = IsGirl ? "Nika" : "Amir";
        var child = IsGirl ? "girl" : "boy";
        var childFarsi = IsGirl ? "دختری" : "پسری";
        var childTitleFarsi = IsGirl ? "دختر" : "پسر";
        var story = $"It is morning. A young fox is standing near a school. His name is Ravi.\n\nRavi sees a blue door under a tree. The door opens slowly. A {child} comes out.\n\n“Hello,” says the {child}. “My name is {companionName}. What’s your name?”\n\n“Hi! I’m Ravi,” says the fox.\n\n“How are you today?” asks {companionName}.\n\n“I’m great, thank you. How are you?”\n\n“I’m fine.”\n\n{companionName} looks at the blue door. A small golden light is behind it.\n\n“Nice to meet you, Ravi. I need your help.”\n\nRavi smiles. “Nice to meet you, too. Let’s go!”\n\nTogether, they walk through the secret door.";
        var storyFarsi = $"صبح است. یک روباه جوان نزدیک یک مدرسه ایستاده است. نام او راوی است.\n\nراوی زیر یک درخت، یک درِ آبی می‌بیند. در به‌آرامی باز می‌شود. {childFarsi} بیرون می‌آید.\n\n{childTitleFarsi} می‌گوید: «سلام. نام من {companionName} است. نام تو چیست؟»\n\nروباه می‌گوید: «سلام! من راوی هستم.»\n\n{companionName} می‌پرسد: «امروز حالت چطور است؟»\n\nراوی می‌گوید: «عالی‌ام، متشکرم. تو چطوری؟»\n\n{companionName} می‌گوید: «خوبم.»\n\n{companionName} به درِ آبی نگاه می‌کند. پشت آن نور طلایی کوچکی دیده می‌شود.\n\n{companionName} می‌گوید: «از آشنایی با تو خوشحالم، راوی. به کمک تو نیاز دارم.»\n\nراوی لبخند می‌زند: «من هم از آشنایی با تو خوشحالم. بیا برویم!»\n\nآن‌ها با هم از درِ مخفی عبور می‌کنند.";

        return
        [
            new("01 · NEW VOCABULARY", "Words and useful expressions", "hello · hi · good morning · good afternoon · good evening · goodbye · see you · please · thank you · fine · great · tired · today · name · friend", "سلام · سلام · صبح بخیر · بعدازظهر بخیر · عصر بخیر · خداحافظ · به امید دیدار · لطفاً · متشکرم · خوب · عالی · خسته · امروز · نام · دوست", "hello", "Listen to every word before you continue."),
            new("02 · PRONUNCIATION", "Ravi’s Echo", "1. Hello!\n2. Good morning.\n3. How are you today?\n4. I’m fine, thank you.\n5. Nice to meet you.\n6. Goodbye. See you!", "۱. سلام!\n۲. صبح بخیر.\n۳. امروز حالت چطور است؟\n۴. خوبم، متشکرم.\n۵. از آشنایی با شما خوشحالم.\n۶. خداحافظ. به امید دیدار!", "hello", "Listen and repeat each line aloud."),
            new("03 · VOCABULARY PRACTICE", "English → Persian", "واژهٔ انگلیسی را ببین و معنی فارسی آن را بنویس:\nhello", "", "سلام", "معنی فارسی hello چیست؟ فقط یک پاسخ بنویس."),
            new("04 · VOCABULARY PRACTICE", "Persian → English", "معنی فارسی را ببین و واژهٔ انگلیسی آن را بنویس:\nمتشکرم", "", "thank you", "معادل انگلیسی «متشکرم» چیست؟"),
            new("05 · STORY", "The Secret Door", story, storyFarsi, "blue", "درِ مخفی چه رنگی است؟ پاسخ را به انگلیسی بنویس."),
            new("06 · READING", "Story comprehension", $"درست یا نادرست را مشخص کن:\n1. Ravi is a fox.\n2. It is evening.\n3. The door is blue.\n4. The {child}’s name is Sara.\n5. {companionName} needs Ravi’s help.\n\nسپس به پرسش‌ها پاسخ بده:\n• راوی کجاست؟\n• پشت در چیست؟\n• راوی چه احساسی دارد؟", "", "school", "راوی کجاست؟ پاسخ کوتاه انگلیسی بنویس: Near a ..."),
            new("07 · TRANSLATION", "English → Persian", "1. Hello.\n2. My name is Ravi.\n3. How are you today?\n4. I’m fine, thank you.\n5. Nice to meet you.\n6. Goodbye. See you!", "۱. سلام.\n۲. نام من راوی است.\n۳. امروز حالت چطور است؟\n۴. خوبم، متشکرم.\n۵. از آشنایی با شما خوشحالم.\n۶. خداحافظ. به امید دیدار!", "سلام", "جمله Hello را به فارسی ترجمه کن."),
            new("08 · TRANSLATION", "Persian → English", $"1. سلام.\n2. نام من {companionName} است.\n3. حالت چطور است؟\n4. من عالی‌ام.\n5. متشکرم.\n6. از آشنایی با شما خوشحالم.", $"1. Hello. / Hi.\n2. My name is {companionName}.\n3. How are you?\n4. I’m great.\n5. Thank you.\n6. Nice to meet you.", "hello", "«سلام» را به انگلیسی ترجمه کن."),
            new("09 · GRAMMAR", "The verb “to be”", "فعل to be یعنی «بودن». در جمله‌های ساده بعد از فاعل می‌آید:\n\nI am → I’m  یعنی «من هستم»\nyou are → you’re  یعنی «تو هستی»\nhe is → he’s  یعنی «او (مذکر) هست»\nshe is → she’s  یعنی «او (مونث) هست»\nit is → it’s  یعنی «آن هست»\n\nشکل کوتاه در گفت‌وگوی روزمره بسیار رایج است.\n\nنمونه‌ها:\nI am Ravi. → I’m Ravi.\nI am fine. → I’m fine.\nI am tired. → I’m tired.", "", "am", "جای خالی را با شکل درست فعل to be پر کن:\nI ___ Ravi."),
            new("10 · LISTENING", "Listen carefully", $"Good morning. My name is {companionName}. I’m fine today.", $"صبح بخیر. نام من {companionName} است. امروز خوبم.", companionName, $"نام {childTitleFarsi} چیست؟ پاسخ را به انگلیسی بنویس."),
            new("11 · DICTATION", "Listen and write", "Hello.\nMy name is Ravi.\nHow are you today?\nI’m fine, thank you.\nNice to meet you.\nGoodbye. See you!", "هر جمله را گوش کن و به انگلیسی بنویس.", "hello", "گوش کن و سپس اولین جمله انگلیسی را بنویس."),
            new("12 · WRITING", "Introduce yourself", $"نمونه:\nHello. My name is {_studentName}.\nI’m great today.\nNice to meet you.", "", _studentName, "سه جمله انگلیسی درباره خودت بنویس: سلام کن، نامت را بگو و احساست را بیان کن."),
            new("13 · SPEAKING", "Speak with Ravi", $"Hello. My name is {_studentName}. I’m fine today. Nice to meet you.", $"سلام. نام من {_studentName} است. امروز خوبم. از آشنایی با شما خوشحالم.", _studentName, "متن معرفی را با صدای بلند بخوان و سپس یک بار به انگلیسی بنویس."),
            new("14 · LESSON EXAM", "Ravi’s First Challenge", $"20 points · Pass mark: 12\n\nA. Vocabulary (4)\nB. Sentences (4)\nC. Grammar (3)\nD. Listening (3)\nE. Dictation (2)\nF. Story (2)\nG. Writing (2)\n\nStory question: Who needs Ravi’s help?", "۲۰ امتیاز · حد قبولی: ۱۲\nآزمون واژگان، جمله‌ها، دستور زبان، شنیداری، املا، داستان و نوشتن", companionName, "چه کسی به کمک راوی نیاز دارد؟ نام او را به انگلیسی بنویس."),
            new("15 · REWARD", "Ravi’s New Friend", "You earned up to three stars, 50 golden leaves, the first picture of the secret door, and access to Lesson 2: The New Student.", "تو تا سه ستاره، ۵۰ برگ طلایی، اولین تصویر در مخفی و دسترسی به درس دوم را به دست آوردی.", "thank", "به انگلیسی بنویس: متشکرم، راوی!"),
        ];
    }

    private LessonStep[] BuildFinalExam()
    {
        var gradeName = _grade == 8 ? "Prospect 2" : "Prospect 1";
        var review = _grade == 8 ? "nationality · routines · abilities · health · places · weather · hobbies" : "greetings · family · appearance · home · time · food";
        return
        [
            new("FINAL · INTRO", $"Grade {_grade} Final Mission", $"{_studentName}, this exam reviews all lessons from {gradeName}. You pass with 60%.", $"{_studentName}، این آزمون همه درس‌های این پایه را مرور می‌کند. حد قبولی ۶۰٪ است.", "ready", "Write: I am ready."),
            new("01 · VOCABULARY", "Vocabulary review", review, "مرور واژگان همه درس‌ها", review.Split('·')[0].Trim(), "Write the first topic in English."),
            new("02 · TRANSLATION", "English → Persian", $"Translate three sentences from the Grade {_grade} story world.", "سه جمله از دنیای داستانی این پایه را ترجمه کن.", "ravi", "Translate: Ravi is my friend."),
            new("03 · TRANSLATION", "Persian → English", "راوی دوست من است.", "Ravi is my friend.", "ravi", "Translate the Persian sentence into English."),
            new("04 · GRAMMAR", "Grammar review", $"Use the grammar from every Grade {_grade} lesson in new situations.", "از دستور زبان همه درس‌های این پایه در موقعیت‌های جدید استفاده کن.", IsGirl ? "she" : "he", $"Complete: {_studentName} is my friend. ___ is {_ageText}."),
            new("05 · LISTENING", "Listening mission", $"Ravi and {_studentName} open the final door and find a certificate.", $"راوی و {_studentName} در آخر را باز می‌کنند و یک گواهی پیدا می‌کنند.", "certificate", "What do they find?"),
            new("06 · DICTATION", "Final dictation", $"I finished Grade {_grade} with Ravi.", $"من پایه {_grade} را با راوی تمام کردم.", $"grade {_grade}", "Write the sentence you hear."),
            new("07 · READING", "Story comprehension", $"After many missions, {_studentName} returns every clue to Ravi. The Golden Acorn shines and the next learning world opens.", $"پس از مأموریت‌های بسیار، {_studentName} همه سرنخ‌ها را به راوی برمی‌گرداند. بلوط طلایی می‌درخشد و دنیای آموزشی بعدی باز می‌شود.", "acorn", "What shines?"),
            new("08 · WRITING", "Final writing", $"Write four sentences about yourself and your adventure with Ravi.", "چهار جمله درباره خودت و ماجراجویی‌ات با راوی بنویس.", _studentName, "Include your name, age, one thing you like and one thing you learned."),
            new("RESULT", $"Grade {_grade} Certificate", $"Congratulations, {_studentName}! A score of 60% unlocks the next grade. Your certificate records vocabulary, grammar, listening, reading and writing results.", $"تبریک {_studentName}! نمره ۶۰٪ پایه بعدی را باز می‌کند. نتیجه مهارت‌های مختلف در گواهی ثبت می‌شود.", "thank", "Write: Thank you, Ravi!"),
        ];
    }

    private string Personalize(string text) => text.Replace("{name}", _studentName, StringComparison.Ordinal);
    private string BuildStory(LessonDefinition lesson)
    {
        if (lesson.Number == "01" && _grade == 7)
            return $"Ravi is standing near a school when he sees {_studentName}. “Hello, {_studentName}!” says Ravi. {_studentName} smiles and says, “Good morning!” Suddenly, a blue door appears under an old tree. Ravi finds a golden key in the grass. “Are you ready for an adventure?” he asks. Together, they walk toward the mysterious door.";

        var (challenge, choice, ending) = GetStoryBeats(lesson.Number);
        return $"{Personalize(lesson.Story)} The air shimmers, and a new part of the adventure begins.\n\n“Stay close, {_studentName},” says Ravi. {challenge}\n\n{_studentName} takes a careful breath. “I know what to do,” {_studentName} says. {choice}\n\n{ending} Ravi smiles and says, “Every English word helps us find the way.” Together, they follow the golden light toward their next adventure.";
    }

    private string BuildStoryFarsi(LessonDefinition lesson)
    {
        if (lesson.Number == "01" && _grade == 7)
            return $"راوی نزدیک مدرسه ایستاده است که {_studentName} را می‌بیند. راوی می‌گوید: «سلام {_studentName}!» {_studentName} لبخند می‌زند و می‌گوید: «صبح بخیر!» ناگهان زیر یک درخت قدیمی دری آبی ظاهر می‌شود. راوی کلیدی طلایی در چمن پیدا می‌کند. او می‌پرسد: «برای یک ماجراجویی آماده‌ای؟» آن‌ها با هم به سوی در اسرارآمیز می‌روند.";

        var (challenge, choice, ending) = GetStoryBeatsFarsi(lesson.Number);
        return $"{Personalize(lesson.StoryFarsi)} هوا می‌درخشد و بخش تازه‌ای از ماجراجویی آغاز می‌شود.\n\nراوی می‌گوید: «نزدیک من بمان، {_studentName}.» {challenge}\n\n{_studentName} نفس عمیقی می‌کشد و می‌گوید: «می‌دانم چه کار کنم.» {choice}\n\n{ending} راوی لبخند می‌زند و می‌گوید: «هر واژه انگلیسی کمک می‌کند راه را پیدا کنیم.» آن‌ها با هم نور طلایی را تا ماجراجویی بعدی دنبال می‌کنند.";
    }

    private (string Challenge, string Choice, string Ending) GetStoryBeats(string number) => (_grade, number) switch
    {
        (7, "02") => ("A nervous new student stands at the classroom door, unable to say his name.", $"{_studentName} slowly spells the name, and Ravi repeats every letter with him.", "The student finally introduces himself, and the class welcomes him warmly."),
        (7, "03") => ("Nika and Amir are waiting, but a magical mist has hidden the name cards on their desks.", $"{_studentName} introduces each classmate, and every correct name makes one card glow.", "The mist disappears, and the three classmates shake hands."),
        (7, "04") => ("The classroom door locks, and a riddle on the board asks them to find five school objects.", $"{_studentName} names the book, pen, pencil, desk and board while Ravi searches carefully.", "Under the last book, they discover Ravi’s warm golden key."),
        (7, "05") => ("The calendar pages spin so quickly that Ravi cannot see the glowing birthday.", $"{_studentName} says the age, month and date aloud, and the pages begin to slow down.", "The correct birthday shines like a small star and reveals a silver number."),
        (7, "06") => ("Seven stepping-stones appear above a dark stream, one for each day of the week.", $"{_studentName} names the days in order while Ravi jumps from stone to stone.", "On the final stone, Monday’s clue opens like a tiny golden flower."),
        (7, "07") => ("A family portrait has lost four faces, and the empty spaces whisper for their names.", $"{_studentName} identifies the mother, father, sister and brother as Ravi holds the frame.", "The family picture becomes whole again, and Nika’s home fills with light."),
        (7, "08") => ("Five visitors offer help, but only one knows the route above the clouds.", $"{_studentName} asks about every job until the pilot steps forward with a folded map.", "The pilot opens the map, and a golden path appears across the sky."),
        (7, "09") => ("A red thread races through the market, slipping beneath shoes and around blue doors.", $"{_studentName} describes every colour and item of clothing while Ravi follows the moving thread.", "The thread leads them to the missing red scarf caught on a rose bush."),
        (7, "10") => ("The market is crowded, and three people match part of the witness’s description.", $"{_studentName} carefully describes height, age and hair until Ravi recognises the right person.", "The tall woman remembers a small house at the end of the street."),
        (7, "11") => ("Inside the quiet house, each room changes place whenever they choose the wrong door.", $"{_studentName} uses in, on, under and next to to guide Ravi through the rooms.", "Behind a chair in the secret room, the golden thread begins to glow."),
        (7, "12") => ("Everyone in the house is busy, but a moving golden light is trying to escape upstairs.", $"{_studentName} tells Ravi who is reading, cooking, washing and playing right now.", "They reach the stairs just as the light turns into a shining address card."),
        (7, "13") => ("Rain has washed three numbers from Ravi’s address, leaving only a street name.", $"{_studentName} compares the telephone numbers on nearby doors and restores the missing digits.", "The completed address points toward an old clock tower across the square."),
        (7, "14") => ("The clock hands spin backwards, and the tower door will open at only one exact time.", $"{_studentName} listens to each bell and calls out five o’clock when the hands align.", "The golden key turns, and warm evening light pours from the tower."),
        (7, "15") => ("Ravi is hungry, but the picnic basket opens only for a kind and complete request.", $"{_studentName} politely asks for bread, fruit and juice, then shares the food with Ravi.", "At the bottom of the basket, they find the final piece of the golden map."),
        (7, "16") => ("The last door has no handle; it is covered with pictures from every earlier mission.", $"{_studentName} remembers the clues, names the objects and completes the final sentences.", "The pictures join together, the door opens, and the Golden Acorn rises into Ravi’s hands."),
        (8, "01") => ("The invitation has no country written on it, only four coloured stamps.", $"{_studentName} identifies Iran, France, China and Spain while Ravi studies the map.", "The stamps form a compass pointing toward an international festival."),
        (8, "02") => ("Four students speak at once, and the welcome board has mixed up their nationalities.", $"{_studentName} listens to each language and matches every student to the correct country.", "The board lights up, and all four students teach Ravi a friendly greeting."),
        (8, "03") => ("Before sunrise, Ravi discovers that the festival map has vanished from his desk.", $"{_studentName} retraces Ravi’s morning routine from waking up to leaving for school.", "A breakfast crumb leads them to a note hidden beside the empty map case."),
        (8, "04") => ("The note contains a weekly plan, but its days drift around like loose puzzle pieces.", $"{_studentName} rebuilds the schedule and finds the marked Wednesday afternoon.", "At that exact time, a silver arrow appears on the classroom window."),
        (8, "05") => ("The arrow points above a high wall where a hidden sign flashes in the wind.", $"{_studentName} reads the sign while Ravi uses his strong paws to climb safely.", "Together they copy the message before the glowing letters disappear."),
        (8, "06") => ("A silver box asks for three different abilities before it will unlock.", $"{_studentName} organises the friends: one draws, one reads and Ravi climbs to press the final symbol.", "The team succeeds, and the box opens with a clear musical note."),
        (8, "07") => ("The message inside is so tiny that Ravi reads too long and develops a painful headache.", $"{_studentName} notices Ravi’s tired eyes and asks exactly what is wrong.", "Ravi admits he needs help, and they carry the message to a quiet room."),
        (8, "08") => ("Ravi wants to continue immediately, although his head still hurts.", $"{_studentName} gives sensible advice: drink water, rest and ask a doctor if the pain continues.", "After a peaceful rest, Ravi feels better and thanks his thoughtful friend."),
        (8, "09") => ("The recovered message shows five city places, but only one hides the next clue.", $"{_studentName} compares the museum, mosque, park, metro and city centre on the map.", "A golden museum symbol begins to pulse in the centre of the city."),
        (8, "10") => ("Busy streets turn the map around, and Ravi can no longer tell east from west.", $"{_studentName} asks a local guide for directions and follows the route toward the west.", "Beyond the final corner, a famous old bridge appears above the river."),
        (8, "11") => ("Across the bridge, fog hides a mountain village and the path divides near a forest.", $"{_studentName} follows the sound of the river while Ravi watches the fields for landmarks.", "The fog lifts, revealing warm village lights beneath the mountain."),
        (8, "12") => ("Sun, rain and wind change within minutes, scattering papers around the village square.", $"{_studentName} describes each change in the weather while Ravi catches the flying pages.", "A strong gust lifts a bench cover and reveals the missing festival map."),
        (8, "13") => ("The map is torn, and each missing piece belongs to someone with a different hobby.", $"{_studentName} asks about reading, drawing, music and collecting until every piece returns.", "Ravi joins the pieces and discovers a route shaped like an open book."),
        (8, "14") => ("At the festival gate, the map asks how the friends spend their free time together.", $"{_studentName} describes walking, playing, listening to music and visiting the shops.", "The gate opens, music fills the square, and Ravi returns the map to its grateful owner."),
        _ => ("A hidden clue begins to glow, but its meaning is not clear.", $"{_studentName} uses the new words to help Ravi understand the message.", "The clue opens a safe path forward."),
    };

    private (string Challenge, string Choice, string Ending) GetStoryBeatsFarsi(string number)
    {
        var english = GetStoryBeats(number);
        return (_grade, number) switch
        {
            (7, "02") => ("دانش‌آموزی خجالتی کنار در کلاس ایستاده و نمی‌تواند نامش را بگوید.", $"{_studentName} نام را آهسته هجی می‌کند و راوی هر حرف را همراه او تکرار می‌کند.", "دانش‌آموز سرانجام خودش را معرفی می‌کند و کلاس با مهربانی از او استقبال می‌کند."),
            (7, "03") => ("مهی جادویی کارت نام‌های روی میز نیکا و امیر را پنهان کرده است.", $"{_studentName} هر همکلاسی را معرفی می‌کند و با هر نام درست، یک کارت می‌درخشد.", "مه ناپدید می‌شود و سه همکلاسی با هم دست می‌دهند."),
            (7, "04") => ("در کلاس قفل می‌شود و معمای روی تخته از آن‌ها می‌خواهد پنج وسیله مدرسه را پیدا کنند.", $"{_studentName} کتاب، خودکار، مداد، میز و تخته را نام می‌برد و راوی با دقت می‌گردد.", "زیر آخرین کتاب، کلید طلایی و گرم راوی را پیدا می‌کنند."),
            (7, "05") => ("صفحه‌های تقویم آن‌قدر سریع می‌چرخند که راوی روز تولد درخشان را نمی‌بیند.", $"{_studentName} سن، ماه و تاریخ را بلند می‌گوید و صفحه‌ها آرام می‌شوند.", "تاریخ درست مانند ستاره می‌درخشد و یک عدد نقره‌ای را نشان می‌دهد."),
            (7, "06") => ("هفت سنگ برای هفت روز هفته روی رودخانه‌ای تاریک ظاهر می‌شود.", $"{_studentName} روزها را به ترتیب می‌گوید و راوی از سنگی به سنگ دیگر می‌پرد.", "روی آخرین سنگ، سرنخ دوشنبه مانند گلی طلایی باز می‌شود."),
            (7, "07") => ("چهار چهره از قاب خانوادگی ناپدید شده‌اند و جای خالی نام آن‌ها را می‌خواهد.", $"{_studentName} مادر، پدر، خواهر و برادر را معرفی می‌کند و راوی قاب را نگه می‌دارد.", "تصویر خانواده کامل می‌شود و خانه نیکا پر از نور می‌گردد."),
            (7, "08") => ("پنج مهمان پیشنهاد کمک می‌دهند، اما فقط یکی راه بالای ابرها را می‌داند.", $"{_studentName} درباره شغل‌ها می‌پرسد تا خلبان با نقشه‌ای تاخورده جلو می‌آید.", "خلبان نقشه را باز می‌کند و راهی طلایی در آسمان ظاهر می‌شود."),
            (7, "09") => ("نخی قرمز با سرعت از بازار می‌گذرد، زیر کفش‌ها و دور درهای آبی می‌پیچد.", $"{_studentName} رنگ‌ها و لباس‌ها را توصیف می‌کند و راوی نخ را دنبال می‌کند.", "نخ آن‌ها را به شال قرمز گمشده روی بوته گل می‌رساند."),
            (7, "10") => ("بازار شلوغ است و سه نفر بخشی از توصیف شاهد را دارند.", $"{_studentName} قد، سن و مو را دقیق توصیف می‌کند تا راوی فرد درست را بشناسد.", "زن قدبلند خانه کوچکی در انتهای خیابان را به یاد می‌آورد."),
            (7, "11") => ("در خانه ساکت، هر بار که در اشتباهی انتخاب می‌شود اتاق‌ها جابه‌جا می‌شوند.", $"{_studentName} با واژه‌های مکانی راوی را از اتاق‌ها عبور می‌دهد.", "پشت صندلی اتاق مخفی، نخ طلایی شروع به درخشیدن می‌کند."),
            (7, "12") => ("همه در خانه مشغول‌اند، اما نور طلایی متحرکی می‌خواهد به طبقه بالا فرار کند.", $"{_studentName} می‌گوید چه کسی در حال خواندن، آشپزی، شستن و بازی است.", "در بالای پله‌ها، نور به کارت نشانی درخشانی تبدیل می‌شود."),
            (7, "13") => ("باران سه عدد را از نشانی راوی پاک کرده و فقط نام خیابان باقی مانده است.", $"{_studentName} شماره‌های درهای اطراف را مقایسه و عددهای گمشده را کامل می‌کند.", "نشانی کامل به برج ساعت قدیمی آن سوی میدان اشاره می‌کند."),
            (7, "14") => ("عقربه‌ها برعکس می‌چرخند و در برج فقط در یک ساعت دقیق باز می‌شود.", $"{_studentName} به زنگ‌ها گوش می‌دهد و هنگام تنظیم عقربه‌ها ساعت پنج را اعلام می‌کند.", "کلید طلایی می‌چرخد و نور گرم عصر از برج بیرون می‌ریزد."),
            (7, "15") => ("راوی گرسنه است، اما سبد پیک‌نیک فقط با یک درخواست مؤدبانه باز می‌شود.", $"{_studentName} نان، میوه و آبمیوه می‌خواهد و غذا را با راوی تقسیم می‌کند.", "ته سبد، آخرین قطعه نقشه طلایی را پیدا می‌کنند."),
            (7, "16") => ("در آخر دستگیره ندارد و با تصویر همه مأموریت‌های گذشته پوشیده شده است.", $"{_studentName} سرنخ‌ها را به یاد می‌آورد و جمله‌های نهایی را کامل می‌کند.", "تصویرها به هم می‌پیوندند، در باز می‌شود و بلوط طلایی در دست راوی قرار می‌گیرد."),
            _ when _grade == 8 => ($"سرنخ تازه‌ای از ماجرای «{number}» ظاهر می‌شود و راه را مبهم می‌کند.", $"{_studentName} با واژه‌های تازه به راوی کمک می‌کند پیام را بفهمد و انتخاب درستی انجام دهد.", "آن‌ها مشکل را حل می‌کنند و سرنخ بعدی ماجرای جشنواره آشکار می‌شود."),
            _ => ("سرنخی پنهان می‌درخشد، اما معنای آن روشن نیست.", $"{_studentName} با واژه‌های تازه به راوی کمک می‌کند.", "سرنخ راه امن بعدی را باز می‌کند."),
        };
    }
    private void Next() { if (_stepIndex < _steps.Length - 1) _stepIndex++; else _screen = "Lessons"; ResetExercise(); }
    private void Back() { _screen = _screen switch { "Learning" => "Lessons", "Lessons" => "Grades", "Grades" => "Profile", "Profile" => "Login", _ => "Login" }; NotifyAll(); }

    private void CheckAnswer()
    {
        var expected = CurrentStep.Expected;
        var isCorrect = _answer.Trim().Contains(expected, StringComparison.OrdinalIgnoreCase);
        LastAnswerCorrect = isCorrect;
        _feedback = _grade == 7
            ? isCorrect
                ? $"آفرین {_studentName}! پاسخ درست است و راوی به تو افتخار می‌کند."
                : $"دوباره تلاش کن {_studentName}. راهنمای راوی: پاسخ شامل «{expected}» است."
            : isCorrect
                ? $"Correct, {_studentName}! Ravi is proud of you."
                : $"Almost, {_studentName}! Ravi’s hint: your answer includes “{expected}”.";
        NotifyAll();
    }

    private void ResetExercise()
    {
        _answer = string.Empty;
        _feedback = string.Empty;
        _showTranslation = false;
        VocabularyItems.Clear();
        LessonLines.Clear();
        if (IsVocabularyStep && _grade == 7 && _currentLessonNumber == "01")
        {
            foreach (var item in Grade7Lesson1Vocabulary)
                VocabularyItems.Add(item);
        }
        else if (IsVocabularyStep)
        {
            var english = CurrentStep.Content.Split('·', StringSplitOptions.TrimEntries);
            var persian = CurrentStep.Translation.Split('·', StringSplitOptions.TrimEntries);
            for (var index = 0; index < Math.Min(english.Length, persian.Length); index++)
                VocabularyItems.Add(new(index + 1, english[index], string.Empty, string.Empty, persian[index]));
        }
        else if (IsPhraseStep || IsStoryStep)
        {
            var separator = IsStoryStep ? "\n\n" : "\n";
            var english = CurrentStep.Content.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var persian = CurrentStep.Translation.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (var index = 0; index < english.Length; index++)
                LessonLines.Add(new(index + 1, english[index], index < persian.Length ? persian[index] : string.Empty));
        }
        NotifyAll();
    }
    private void NotifyAll() => OnPropertyChanged(string.Empty);
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));

    private sealed record LessonDefinition(string Number, string Title, string Topic, string Words, string WordsFarsi, string Grammar, string Story, string StoryFarsi, string Question, string Expected);
    private sealed record LessonStep(string Eyebrow, string Title, string Content, string Translation, string Expected, string AnswerPrompt)
    {
        public static LessonStep Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
    }
}

public sealed record LessonCard(string Number, string Title, string Meta, string Duration, bool IsAvailable, bool IsFinalExam)
{
    public string IconSource => IsFinalExam ? "icon_award.svg" : "icon_lesson.svg";
    public string ActionLabel => IsFinalExam ? "Final challenge  →" : "Start lesson  →";
}
public sealed record VocabularyItem(int Number, string English, string Phonetic, string Pronunciation, string Persian);
public sealed record LessonLine(int Number, string English, string Persian);
public sealed record GradeChoice(int Number, string PersianName, string Subtitle);
