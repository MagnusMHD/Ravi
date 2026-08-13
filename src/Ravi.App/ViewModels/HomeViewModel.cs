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
        Grades = new(Enumerable.Range(7, 6));
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
    public ObservableCollection<int> Grades { get; }
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
    public string StudentBadge => string.IsNullOrWhiteSpace(_studentName) ? "🔥 6" : $"🦊 {_studentName}";
    public string CourseProgressLabel => $"0 of {LessonCount} lessons completed";
    public int LessonCount => _grade == 8 ? Grade8.Length : Grade7.Length;
    public string StepEyebrow => CurrentStep.Eyebrow;
    public string StepTitle => CurrentStep.Title;
    public string StepContent => CurrentStep.Content;
    public string StepTranslation => _showTranslation ? CurrentStep.Translation : "Show Persian translation  فارسی";
    public string AnswerPrompt => CurrentStep.AnswerPrompt;
    public string ProgressLabel => $"Step {_stepIndex + 1} of {_steps.Length}";
    public double LessonProgress => _steps.Length == 0 ? 0 : (_stepIndex + 1d) / _steps.Length;
    public string NextLabel => _stepIndex == _steps.Length - 1 ? "Complete lesson  ✦" : "Continue  →";
    public string Feedback => _feedback;
    public bool HasFeedback => !string.IsNullOrWhiteSpace(_feedback);
    public string Answer { get => _answer; set { _answer = value; OnPropertyChanged(); } }
    private LessonStep CurrentStep => _steps.Length == 0 ? LessonStep.Empty : _steps[_stepIndex];
    public bool IsVocabularyStep => CurrentStep.Eyebrow.EndsWith("VOCABULARY", StringComparison.Ordinal);
    public bool IsPhraseStep => _grade == 7 && _currentLessonNumber == "01" && CurrentStep.Eyebrow.StartsWith("02 · PRONUNCIATION", StringComparison.Ordinal);
    public bool IsStoryStep => CurrentStep.Eyebrow.Contains("STORY", StringComparison.Ordinal);
    public bool IsRegularStep => !IsVocabularyStep && !IsPhraseStep && !IsStoryStep;
    public bool ShowExercise => !IsVocabularyStep && !IsPhraseStep && !IsStoryStep;
    public bool HasTranslation => !string.IsNullOrWhiteSpace(CurrentStep.Translation);

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
            new("01 · WELCOME", $"{lesson.Title}, {_studentName}!", $"Hello, {_studentName}! Today’s mission is {lesson.Topic}.", $"سلام {_studentName}! مأموریت امروز درباره {lesson.Topic} است.", "hello", "Write an English greeting to Ravi."),
            new("02 · VOCABULARY", "New vocabulary", lesson.Words, lesson.WordsFarsi, lesson.Words.Split('·')[0].Trim(), "Tap each speaker, listen, and learn every word."),
            new("03 · ENGLISH → FARSI", "English to Persian", lesson.Words, lesson.WordsFarsi, lesson.WordsFarsi.Split('·')[0].Trim(), "Translate the first English word into Persian."),
            new("04 · FARSI → ENGLISH", "Persian to English", lesson.WordsFarsi, lesson.Words, lesson.Words.Split('·')[0].Trim(), "Translate the first Persian word into English."),
            new("05 · STORY", "Ravi’s continuing adventure", story, storyFarsi, lesson.Expected, lesson.Question),
            new("06 · GRAMMAR", lesson.Grammar, $"Example: {_studentName} is {_ageText} years old. {subjectTitle} is Ravi’s friend. Read the rule, then complete the sentence below.", $"مثال: {_studentName} {_ageText} سال دارد. او دوست راوی است.", subject, $"Complete with he or she: {_studentName} is my friend. ___ is {_ageText}."),
            new("07 · LISTENING", "Listen without reading first", story, storyFarsi, lesson.Expected, lesson.Question),
            new("08 · DICTATION", "Listen and write", $"{_studentName} is Ravi’s friend.", $"{_studentName} دوست راوی است.", _studentName, "Listen without reading, then write the English sentence."),
            new("09 · WRITING", "Your turn", $"Write two sentences about {lesson.Topic.ToLowerInvariant()}. Ravi gives {obj} a golden leaf for a complete answer.", $"دو جمله درباره موضوع درس بنویس. راوی برای پاسخ کامل یک برگ طلایی می‌دهد.", _studentName, $"Begin with: My name is {_studentName}."),
            new("10 · LESSON EXAM", "Ravi’s Challenge", $"Exam: vocabulary, English↔Farsi, grammar, listening, dictation, reading and writing. Pass mark: 60%. Story question: {lesson.Question}", $"آزمون: واژگان، ترجمه دوطرفه، دستور زبان، شنیداری، املا، خواندن و نوشتن. حد قبولی: ۶۰٪", lesson.Expected, lesson.Question),
            new("11 · REWARD", $"Well done, {_studentName}!", $"You completed {lesson.Title}. Reward: up to 3 stars, 50 golden leaves and a Ravi badge.", $"آفرین {_studentName}! این درس را تمام کردی. جایزه: تا سه ستاره، ۵۰ برگ طلایی و نشان راوی.", "ravi", "Write: Thank you, Ravi!"),
        ];
    }

    private LessonStep[] BuildGrade7Lesson1()
    {
        const string story = "It is morning. A young fox is standing near a school. His name is Ravi.\n\nRavi sees a blue door under a tree. The door opens slowly. A girl comes out.\n\n“Hello,” says the girl. “My name is Nika. What’s your name?”\n\n“Hi! I’m Ravi,” says the fox.\n\n“How are you today?” asks Nika.\n\n“I’m great, thank you. How are you?”\n\n“I’m fine.”\n\nNika looks at the blue door. A small golden light is behind it.\n\n“Nice to meet you, Ravi. I need your help.”\n\nRavi smiles. “Nice to meet you, too. Let’s go!”\n\nTogether, they walk through the secret door.";
        const string storyFarsi = "صبح است. یک روباه جوان نزدیک یک مدرسه ایستاده است. نام او راوی است.\n\nراوی زیر یک درخت، یک درِ آبی می‌بیند. در به‌آرامی باز می‌شود. دختری بیرون می‌آید.\n\nدختر می‌گوید: «سلام. نام من نیکا است. نام تو چیست؟»\n\nروباه می‌گوید: «سلام! من راوی هستم.»\n\nنیکا می‌پرسد: «امروز حالت چطور است؟»\n\nراوی می‌گوید: «عالی‌ام، متشکرم. تو چطوری؟»\n\nنیکا می‌گوید: «خوبم.»\n\nنیکا به درِ آبی نگاه می‌کند. پشت آن نور طلایی کوچکی دیده می‌شود.\n\nنیکا می‌گوید: «از آشنایی با تو خوشحالم، راوی. به کمک تو نیاز دارم.»\n\nراوی لبخند می‌زند: «من هم از آشنایی با تو خوشحالم. بیا برویم!»\n\nآن‌ها با هم از درِ مخفی عبور می‌کنند.";

        return
        [
            new("01 · NEW VOCABULARY", "Words and useful expressions", "hello · hi · good morning · good afternoon · good evening · goodbye · see you · please · thank you · fine · great · tired · today · name · friend", "سلام · سلام · صبح بخیر · بعدازظهر بخیر · عصر بخیر · خداحافظ · به امید دیدار · لطفاً · متشکرم · خوب · عالی · خسته · امروز · نام · دوست", "hello", "Listen to every word before you continue."),
            new("02 · PRONUNCIATION", "Ravi’s Echo", "1. Hello!\n2. Good morning.\n3. How are you today?\n4. I’m fine, thank you.\n5. Nice to meet you.\n6. Goodbye. See you!", "۱. سلام!\n۲. صبح بخیر.\n۳. امروز حالت چطور است؟\n۴. خوبم، متشکرم.\n۵. از آشنایی با شما خوشحالم.\n۶. خداحافظ. به امید دیدار!", "hello", "Listen and repeat each line aloud."),
            new("03 · VOCABULARY PRACTICE", "English → Persian", "Write the Persian meaning of the English word: hello", "", "سلام", "What does “hello” mean in Persian? Write one answer."),
            new("04 · VOCABULARY PRACTICE", "Persian → English", "Write the English word for the Persian meaning: متشکرم", "", "thank you", "What is “متشکرم” in English? Write one answer."),
            new("05 · STORY", "The Secret Door", story, storyFarsi, "blue", "What colour is the secret door?"),
            new("06 · READING", "Story comprehension", "TRUE OR FALSE\n1. Ravi is a fox.\n2. It is evening.\n3. The door is blue.\n4. The girl’s name is Sara.\n5. Nika needs Ravi’s help.\n\nQUESTIONS\n• Where is Ravi?\n• What is behind the door?\n• How does Ravi feel?", "درست یا نادرست و پرسش‌های درک مطلب درباره داستان", "school", "Where is Ravi? Write: Near a school."),
            new("07 · TRANSLATION", "English → Persian", "1. Hello.\n2. My name is Ravi.\n3. How are you today?\n4. I’m fine, thank you.\n5. Nice to meet you.\n6. Goodbye. See you!", "۱. سلام.\n۲. نام من راوی است.\n۳. امروز حالت چطور است؟\n۴. خوبم، متشکرم.\n۵. از آشنایی با شما خوشحالم.\n۶. خداحافظ. به امید دیدار!", "سلام", "Translate “Hello.” into Persian."),
            new("08 · TRANSLATION", "Persian → English", "1. سلام.\n2. نام من نیکا است.\n3. حالت چطور است؟\n4. من عالی‌ام.\n5. متشکرم.\n6. از آشنایی با شما خوشحالم.", "1. Hello. / Hi.\n2. My name is Nika.\n3. How are you?\n4. I’m great.\n5. Thank you.\n6. Nice to meet you.", "hello", "Translate “سلام” into English."),
            new("09 · GRAMMAR", "The verb “to be”", "I am → I’m\nyou are → you’re\nhe is → he’s\nshe is → she’s\nit is → it’s\n\nExamples:\nI am Ravi. → I’m Ravi.\nI am fine. → I’m fine.\nI am tired. → I’m tired.", "من هستم · تو هستی · او هست · آن هست", "am", "Complete: I ___ Ravi."),
            new("10 · LISTENING", "Listen carefully", "Good morning. My name is Amir. I’m fine today.\n\nListen twice. On the first listen, try not to read. Who is speaking? Is it morning? How is Amir?", "صبح بخیر. نام من امیر است. امروز خوبم.", "amir", "What is his name?"),
            new("11 · DICTATION", "Listen and write", "Hello.\nMy name is Ravi.\nHow are you today?\nI’m fine, thank you.\nNice to meet you.\nGoodbye. See you!", "هر جمله را گوش کن و به انگلیسی بنویس.", "hello", "Listen, then write the first sentence."),
            new("12 · WRITING", "Introduce yourself", $"Hello. My name is {_studentName}.\nI’m great today.\nNice to meet you.\n\nNow write three sentences: a greeting, your name, and how you feel.", $"سلام. نام من {_studentName} است. امروز عالی‌ام. از آشنایی با شما خوشحالم.", _studentName, "Write three English sentences to introduce yourself."),
            new("13 · SPEAKING", "Speak with Ravi", $"Hello. My name is {_studentName}. I’m fine today. Nice to meet you.", $"سلام. نام من {_studentName} است. امروز خوبم. از آشنایی با شما خوشحالم.", _studentName, "Read the introduction aloud, then type it once."),
            new("14 · LESSON EXAM", "Ravi’s First Challenge", "20 points · Pass mark: 12\n\nA. Vocabulary (4)\nB. Sentences (4)\nC. Grammar (3)\nD. Listening (3)\nE. Dictation (2)\nF. Story (2)\nG. Writing (2)\n\nStory question: Who needs Ravi’s help?", "۲۰ امتیاز · حد قبولی: ۱۲\nآزمون واژگان، جمله‌ها، دستور زبان، شنیداری، املا، داستان و نوشتن", "nika", "Who needs Ravi’s help?"),
            new("15 · REWARD", "Ravi’s New Friend", "You earned up to three stars, 50 golden leaves, the first picture of the secret door, and access to Lesson 2: The New Student.", "تو تا سه ستاره، ۵۰ برگ طلایی، اولین تصویر در مخفی و دسترسی به درس دوم را به دست آوردی.", "thank", "Write: Thank you, Ravi!"),
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

        var words = lesson.Words.Split('·', StringSplitOptions.TrimEntries);
        return $"{Personalize(lesson.Story)} Ravi and {_studentName} stop and look carefully. They notice the words {string.Join(", ", words.Take(3))} on the next clue. Ravi reads the clue aloud, and {_studentName} chooses the path forward. Their English adventure continues.";
    }

    private string BuildStoryFarsi(LessonDefinition lesson)
    {
        if (lesson.Number == "01" && _grade == 7)
            return $"راوی نزدیک مدرسه ایستاده است که {_studentName} را می‌بیند. راوی می‌گوید: «سلام {_studentName}!» {_studentName} لبخند می‌زند و می‌گوید: «صبح بخیر!» ناگهان زیر یک درخت قدیمی دری آبی ظاهر می‌شود. راوی کلیدی طلایی در چمن پیدا می‌کند. او می‌پرسد: «برای یک ماجراجویی آماده‌ای؟» آن‌ها با هم به سوی در اسرارآمیز می‌روند.";

        return $"{Personalize(lesson.StoryFarsi)} راوی و {_studentName} با دقت به سرنخ بعدی نگاه می‌کنند. راوی سرنخ را با صدای بلند می‌خواند و {_studentName} مسیر بعدی را انتخاب می‌کند. ماجراجویی انگلیسی آن‌ها ادامه پیدا می‌کند.";
    }
    private void Next() { if (_stepIndex < _steps.Length - 1) _stepIndex++; else _screen = "Lessons"; ResetExercise(); }
    private void Back() { _screen = _screen switch { "Learning" => "Lessons", "Lessons" => "Grades", "Grades" => "Profile", "Profile" => "Login", _ => "Login" }; NotifyAll(); }

    private void CheckAnswer()
    {
        var expected = CurrentStep.Expected;
        _feedback = _answer.Trim().Contains(expected, StringComparison.OrdinalIgnoreCase)
            ? $"Correct, {_studentName}! Ravi is proud of you ✨"
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

public sealed record LessonCard(string Number, string Title, string Meta, string Duration, bool IsAvailable, bool IsFinalExam);
public sealed record VocabularyItem(int Number, string English, string Phonetic, string Pronunciation, string Persian);
public sealed record LessonLine(int Number, string English, string Persian);
