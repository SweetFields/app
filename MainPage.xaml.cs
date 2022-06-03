namespace SweetFields;

public partial class MainPage : ContentPage
{
    Generate.Helpers helpers = null;

    public MainPage()
    {
        Title = "Меню";
        InitializeComponent();
        PageLoad();
    }

    async void PageLoad()
    {
        Generate.Helpers helpers = new Generate.Helpers(new Generate.DB());

        if (!helpers.IsExists())
        {
            helpers.SetDefault();
            helpers.Export();
        }
        else
        {
            try
            {
                helpers.Import();
            }
            catch
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Внимание", "Стандартный файл (base.json) имеет некорректное содержимое, попробуйте его удалить и снова запустить приложение", "Хорошо"));
                Application.Current.Quit();
            }
        }

        if (!helpers.IsExistsDataFile())
            helpers.db.dataFileName = helpers.fileName;
    }

    public void StartGame_button_OnClick(object sender, EventArgs args) => App.Current.MainPage = new NavigationPage(new GamePage());

    public void Settings_button_OnClick(object sender, EventArgs args) => App.Current.MainPage = new NavigationPage(new SettingsPage());

    public void Toolkit_button_OnClick(object sender, EventArgs args) => App.Current.MainPage = new NavigationPage(new ToolkitPage());

    public void Exit_button_OnClick(object sender, EventArgs args) => Application.Current.Quit();
}
