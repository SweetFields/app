namespace SweetFields;

public partial class ToolkitPage : ContentPage
{
    Generate.Helpers helpers = null;

    public ToolkitPage()
    {
        InitializeComponent();

        helpers = new Generate.Helpers(new Generate.DB());

        if (!helpers.IsExists())
        {
            helpers.SetDefault();
            helpers.Export();
        }
        else
            helpers.Import();

        if (!helpers.IsExistsDataFile())
            helpers.db.dataFileName = helpers.fileName;

        Title = $"Целевой файл ({helpers.db.dataFileName})";

        foreach (var cleverIdea in helpers.db.listCleverIdeas)
            List_clever_phrases_editor.Text += cleverIdea + "\n";

        foreach (var word in helpers.db.listWords) {
            List_questions_editor.Text += word.Question + "\n";
            List_answers_editor.Text += word.Answer + "\n";
            List_level_challenges_editor.Text += word.Level + "\n";
        }
    }

    public async void SaveParams_button_OnClick(object sender, EventArgs args)
    {
        helpers.db.listCleverIdeas = new List<string>();
        helpers.db.listWords = new List<Generate.Word>();

        string[] questions_temp = List_questions_editor.Text.Split('\n');
        string[] answers_temp = List_answers_editor.Text.Split('\n');
        string[] level_challenges_temp = List_level_challenges_editor.Text.Split('\n');
        string[] clever_phrases_temp = List_clever_phrases_editor.Text.Split('\n');

        for (int i = 0; i < questions_temp.Length; i++)
            if(answers_temp[i] != "" && questions_temp[i] != "" && level_challenges_temp[i] != "")
                helpers.db.listWords.Add(new Generate.Word() { ID = helpers.db.listWords.Count(), Answer = answers_temp[i], Question = questions_temp[i], Level = Convert.ToInt16(level_challenges_temp[i]) });

        for (int i = 0; i < clever_phrases_temp.Length; i++)
            if (clever_phrases_temp[i] != "")
                helpers.db.listCleverIdeas.Add(clever_phrases_temp[i]);

        helpers.ExportTarget();

        await DisplayAlert("Внимание", "В целевой файл были сохранены введеные в поля данные", "Хорошо");
    }

    public void Exit_button_OnClick(object sender, EventArgs args) => App.Current.MainPage = new NavigationPage(new MainPage());
}

