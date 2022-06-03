using System.Text.Json;

namespace SweetFields.Generate;

class Helpers
{
    public DB db { get; set; }
    public readonly string folderName = "../Data/";
    public readonly string fileName = "base.json";

    public Helpers(DB db) => this.db = db;

    public void SetDefault()
    {
        DefaultCleverIdeas();
        DefaultWords();
        DefaultBoardSize();
        DefaultLevelChallenges();
        DefaultDataFileName();
    }

    public bool IsExists()
    {
        if (!System.IO.Directory.Exists(folderName))
            System.IO.Directory.CreateDirectory(folderName);

        return System.IO.File.Exists(folderName + fileName);
    }

    public bool IsExistsDataFile()
    {
        if (!System.IO.Directory.Exists(folderName))
            System.IO.Directory.CreateDirectory(folderName);

        if (System.IO.File.Exists(folderName + fileName))
            Export();

        return System.IO.File.Exists(folderName + db.dataFileName);
    }

    public void Import()
    {
        db = JsonSerializer.Deserialize<Generate.DB>(System.IO.File.ReadAllText(folderName + fileName));

        if (!IsExistsDataFile())
            db.dataFileName = fileName;

        DB db_target = JsonSerializer.Deserialize<Generate.DB>(System.IO.File.ReadAllText(folderName + db.dataFileName));
        db.listCleverIdeas = db_target.listCleverIdeas;
        db.listWords = db_target.listWords;
    }

    public void Export()
    {
        if (!System.IO.Directory.Exists(folderName))
            System.IO.Directory.CreateDirectory(folderName);
        else if (System.IO.File.Exists(folderName + fileName))
            System.IO.File.Delete(folderName + fileName);

        System.IO.File.AppendAllText(folderName + fileName, JsonSerializer.Serialize<Generate.DB>(db));
    }

    public void ExportTarget()
    {
        if (!System.IO.Directory.Exists(folderName))
            System.IO.Directory.CreateDirectory(folderName);
        else if (System.IO.File.Exists(folderName + db.dataFileName))
            System.IO.File.Delete(folderName + db.dataFileName);

        System.IO.File.AppendAllText(folderName + db.dataFileName, JsonSerializer.Serialize<Generate.DB>(db));
    }

    public string[] GetDataFiles() => System.IO.Directory.GetFiles(folderName).Select(x => x.Substring(8)).ToArray();

    private void DefaultCleverIdeas()
    {
        db.listCleverIdeas = new List<string>();
        db.listCleverIdeas.Add("возможность чему-то научиться нельзя упускать");
        db.listCleverIdeas.Add("решая кроссворд, чувствуешь себя многоклеточным");
        db.listCleverIdeas.Add("кроссворды — последний этап человечества в многовековых поисках истины");
        db.listCleverIdeas.Add("на смену энциклопедическому образованию пришло кроссвордное");
        db.listCleverIdeas.Add("кроссворд хорош уже тем, что тут решение всегда существует");
        db.listCleverIdeas.Add("что означает действительно владеть иностранным языком? Решать на нем кроссворды");
        db.listCleverIdeas.Add("это ложь, что газеты отучили нас думать. Разве они не публикуют кроссворды?");
        db.listCleverIdeas.Add("в неразвитых обществах наибольшие страсти вызывают власть, деньги и женщины; в развитых – деньги, власть и кроссворды");
        db.listCleverIdeas.Add("оптимист: человек, который заполняет кроссворд сразу чернилами");
    }

    private void DefaultWords()
    {
        db.listWords = new List<Word>();
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "тигр", Question = "какая кошка самая большая на планете?", Level = 1 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "бегемот", Question = "какое сухопутное животное может открыть рот максимально широко?", Level = 2 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "понюхать", Question = "почему змеи высовывают язык?", Level = 3 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "вымирание", Question = "как называется явление, обозначающее, что на земле не осталось ни одного животного конкретного вида?", Level = 1 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "юпитер", Question = "какая планета самая большая в солнечной системе?", Level = 2 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "юпитер", Question = "на какой планете самый короткий день?", Level = 3 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "солнце", Question = "какая звезда ближе всего к земле?", Level = 1 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "челябинский", Question = "как называется метеорит, который упал на землю 15 февраля 2013 года?", Level = 2 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "восток", Question = "в каком направлении восходит солнце?", Level = 3 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "плавает", Question = "как ведет себя лед в воде?", Level = 1 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "гравитация", Question = "под воздействием какой силы предметы падают на землю?", Level = 2 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "дуб", Question = "какие деревья растут из желудей?", Level = 3 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "семь", Question = "сколько цветов в радуге?", Level = 1 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "зубы", Question = "какое самое твердое вещество в нашем теле?", Level = 2 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "глаз", Question = "где самая быстрая мышца в теле?", Level = 3 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "ватикан", Question = "какая страна самая маленькая на земле?", Level = 1 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "антарктида", Question = "какое место самое холодное на земле?", Level = 2 });
        db.listWords.Add(new Generate.Word() { ID = db.listWords.Count(), Number = 0, Answer = "шесть", Question = "сколько всего континентов?", Level = 3 });
    }

    private void DefaultBoardSize() => db.board = new Point(6, 6);

    private void DefaultLevelChallenges() => db.levelChallenges = 3;

    private void DefaultDataFileName() => db.dataFileName = "base.json";
}
