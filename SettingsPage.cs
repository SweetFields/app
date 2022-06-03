namespace SweetFields;

public partial class SettingsPage : ContentPage
{
    Generate.Helpers helpers = null;

    public SettingsPage()
    {
        Title = "Настройки";
        InitializeComponent();

        helpers = new Generate.Helpers(new Generate.DB());

        if (!helpers.IsExists())
        {
            helpers.SetDefault();
            helpers.Export();
        }
        else
            helpers.Import();

        RowsAndCells_slider.Value = Math.Round(helpers.db.board.X);
        RowsAndCells_label.Text = String.Format("Количество строк и столбцов ({0}, {1})", Math.Round(helpers.db.board.X).ToString(), Math.Round(helpers.db.board.Y).ToString());

        Level_challenges_slider.Value = Convert.ToDouble(helpers.db.levelChallenges);
        Level_challenges_label.Text = String.Format("Уровень сложности ({0})", helpers.db.levelChallenges);

        if (!helpers.IsExistsDataFile())
            helpers.db.dataFileName = helpers.fileName;

        if (helpers.GetDataFiles().Length == 1)
        {
            Data_files_slider.Minimum = 0;
            Data_files_slider.Maximum = helpers.GetDataFiles().Length;
        }
        else
        {
            Data_files_slider.Minimum = 1;
            Data_files_slider.Maximum = helpers.GetDataFiles().Length;
        }

        Data_files_slider.Value = Convert.ToDouble(Array.IndexOf(helpers.GetDataFiles(), helpers.db.dataFileName) + 1);
        Data_files_label.Text = String.Format("Файл с данными ({0})", helpers.db.dataFileName);
    }

    public void RowsAndCells_slider_OnValueChanged(object sender, ValueChangedEventArgs args)
    {
        double value = Math.Round(args.NewValue);
        RowsAndCells_label.Text = String.Format("Количество строк и столбцов ({0}, {1})", value.ToString(), value.ToString());
    }

    public void Level_challenges_slider_OnValueChanged(object sender, ValueChangedEventArgs args)
    {
        double value = Math.Round(args.NewValue);
        Level_challenges_label.Text = String.Format("Уровень сложности ({0})", value);
    }

    public void Data_files_slider_OnValueChanged(object sender, ValueChangedEventArgs args)
    {
        int value = Convert.ToInt16(Math.Round(args.NewValue));
        string dataFile = helpers.GetDataFiles()[--value];

        try
        {
            System.Text.Json.JsonSerializer.Deserialize<Generate.DB>(System.IO.File.ReadAllText(helpers.folderName + dataFile));
            Data_files_slider.Maximum = helpers.GetDataFiles().Length;
            Data_files_label.Text = String.Format("Файл с данными ({0})", dataFile);
        }
        catch
        {
            DisplayAlert("Внимание", $"Выбранный файл ({dataFile}) имеет некорректное содержимое", "Хорошо");
        }
    }

    public async void SaveParams_button_OnClick(object sender, EventArgs args)
    {
        helpers.db.board = new Point(Math.Round(RowsAndCells_slider.Value), Math.Round(RowsAndCells_slider.Value));
        helpers.db.levelChallenges = Convert.ToInt16(Math.Round(Level_challenges_slider.Value));
        helpers.db.dataFileName = helpers.GetDataFiles()[Convert.ToInt16(Math.Round(Data_files_slider.Value)) - 1];
        helpers.Export();

        await DisplayAlert("Внимание", "Параметры кроссворда были изменены", "Хорошо");
    }

    public void Exit_button_OnClick(object sender, EventArgs args) => App.Current.MainPage = new NavigationPage(new MainPage());
}

