namespace SweetFields;

public partial class GamePage : ContentPage
{
    Random random = null;

    Generate.Helpers helpers = null;
    Generate.Matrix matrix = null;

    bool isTimerWork = false;

    int boxRows = 10;
    int boxColumns = 10;

    Grid grid_board = null;
    BoxView[,] grid_board_background = null;
    Entry[,] grid_board_foreground = null;
    int boxBoardRows = 0;
    int boxBoardColumns = 0;

    Grid grid = null;
    Frame frameBoardView = null;
    Frame frameCleverIdeas = null;

    TimeOnly timeOnTimer = default(TimeOnly);
    Thread timerThread = null;

    public GamePage()
    {
        Title = "Игровой процесс";
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

        random = new Random();
        grid = new Grid();
        timeOnTimer = new TimeOnly();
        timerThread = new Thread(Timer);
        boxBoardColumns = Convert.ToInt16(Math.Round(helpers.db.board.X));
        boxBoardRows = Convert.ToInt16(Math.Round(helpers.db.board.Y));

        CreateBox();
        CreateBoardBox();
        MenuView();
        CleverIdeas();

        if (!GenerateMatrixWords())
            Device.InvokeOnMainThreadAsync(async () => {
                await DisplayAlert("Внимание", "Кроссворд не может быть построен!", "Хорошо");

                if (await DisplayActionSheet("Что вы хотите выбрать?", "Отмена", null, "Хочу на главную страницу", "Хочу изменить вопросы") == "Хочу на главную страницу")
                    App.Current.MainPage = new NavigationPage(new MainPage());
                else
                    App.Current.MainPage = new NavigationPage(new SettingsPage());
            });

        App.Current.MainPage = new NavigationPage(new MainPage());

        Content = grid;

        timerThread.Start();

        isTimerWork = true;
    }

    private bool GenerateMatrixWords()
    {
        grid_board_background = new BoxView[boxBoardColumns, boxBoardRows];
        grid_board_foreground = new Entry[boxBoardColumns, boxBoardRows];

        matrix = new Generate.Matrix(helpers.db.listWords, helpers.db.levelChallenges, boxBoardColumns, boxBoardRows);

        if (matrix.Generate()) {
            System.Console.WriteLine("\n" + matrix.View());

            List<Generate.Word> words_used = matrix.Words.Where(x => (x.Level <= matrix.Level && x.Answer.Length <= Math.Min(matrix.Rows, matrix.Columns) && x.IsUsed)).ToList<Generate.Word>();

            for (int i = 0; i < matrix.Columns; i++)
            {
                for (int j = 0; j < matrix.Rows; j++)
                {
                    if (matrix.Answer[i, j]?.Item2.ID != null)
                    {
                        grid_board_background[i, j] = new BoxView
                        {
                            CornerRadius = 1,
                            Color = ((AppTheme)Application.Current.RequestedTheme == AppTheme.Light) ? Color.FromHex("#dddddd") : Color.FromHex("#686868"),
                            ZIndex = 0
                        };
                        grid_board.Add(grid_board_background[i, j], i, j);


                        grid_board_foreground[i, j] = new Entry
                        {
                            Text = "",
                            Placeholder = (matrix.Answer[i, j]?.Item2.Number).ToString(),
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            MaxLength = 1,
                            HorizontalTextAlignment = TextAlignment.Center,
                            IsEnabled = true,
                            IsVisible = true,
                            ZIndex = 1
                        };
                        grid_board.Add(grid_board_foreground[i, j], i, j);
                    }
                }
            }

            QuestionsView(matrix, words_used);

            return true;
        }

        return false;
    }

    private void CreateBox()
    {
        for (int i = 0; i < boxColumns; i++)
            grid.AddRowDefinition(new RowDefinition());

        for (int j = 0; j < boxRows; j++)
            grid.AddColumnDefinition(new ColumnDefinition());
    }

    private void CreateBoardBox()
    {
        grid_board = new Grid();

        for (int i = 0; i < boxBoardColumns; i++)
            grid_board.AddRowDefinition(new RowDefinition());

        for (int j = 0; j < boxBoardRows; j++)
            grid_board.AddColumnDefinition(new ColumnDefinition());

        frameBoardView = new Frame
        {
            Padding = new Thickness(5),
            Margin = new Thickness(5),
            CornerRadius = 5,
            Content = grid_board
        };

        Grid.SetRowSpan(frameBoardView, boxRows - 2);
        Grid.SetColumnSpan(frameBoardView, boxColumns);

        grid.Add(frameBoardView, 0, 0);
    }

    private void MenuView()
    {
        Button checkButton = new Button
        {
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            Text = "Проверить"
        };
        checkButton.Clicked += GamePage_checkButton_OnClick;

        Frame frameCheckButton = new Frame
        {
            Padding = new Thickness(0),
            Margin = new Thickness(5),
            CornerRadius = 5,
            Content = checkButton
        };

        Grid.SetColumnSpan(frameCheckButton, 2);
        grid.Add(frameCheckButton, boxColumns - 2, boxRows - 2);


        Button exitButton = new Button
        {
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            Text = "Выйти"
        };
        exitButton.Clicked += GamePage_exitButton_OnClick;

        Frame frameExitButton = new Frame
        {
            Padding = new Thickness(0),
            Margin = new Thickness(5),
            CornerRadius = 5,
            Content = exitButton
        };

        Grid.SetColumnSpan(frameExitButton, 2);
        grid.Add(frameExitButton, boxColumns - 2, boxRows - 1);
    }

    public void GamePage_exitButton_OnClick(object sender, EventArgs args)
    {
        Vibration.Vibrate(TimeSpan.FromSeconds(1));
        App.Current.MainPage = new NavigationPage(new MainPage());
    }

    public async void GamePage_checkButton_OnClick(object sender, EventArgs args)
    {
        bool isHasErrors = false;

        for (int i = 0; i < matrix.Columns; i++)
        {
            for (int j = 0; j < matrix.Rows; j++)
            {
                if (matrix.Answer[i, j]?.Item2.ID != null)
                {
                    if (grid_board_foreground[i, j].Text.ToLower() == matrix.Answer[i, j].Item1.ToString())
                    {
                        grid_board.Remove(grid_board_background[i, j]);
                        grid_board_background[i, j] = new BoxView
                        {
                            CornerRadius = 1,
                            Color = ((AppTheme)Application.Current.RequestedTheme == AppTheme.Light) ? Color.FromHex("#B4E197") : Color.FromHex("#5a704b"),
                            ZIndex = 0
                        };
                        grid_board.Add(grid_board_background[i, j], i, j);
                    }
                    else
                    {
                        grid_board.Remove(grid_board_background[i, j]);
                        grid_board_background[i, j] = new BoxView
                        {
                            CornerRadius = 1,
                            Color = ((AppTheme)Application.Current.RequestedTheme == AppTheme.Light) ? Color.FromHex("#FF5D5D") : Color.FromHex("#7f2e2e"),
                            ZIndex = 0
                        };
                        grid_board.Add(grid_board_background[i, j], i, j);

                        isHasErrors = true;
                    }
                }
            }
        }

        if (!isHasErrors)
        {
            await DisplayAlert("Внимание", "Кроссворд был успешно решен!", "Хорошо");
            if (await DisplayActionSheet("Что вы хотите выбрать?", "Отмена", null, "Хочу на главную страницу", "Хочу попробовать еще раз") == "Хочу на главную страницу")
                App.Current.MainPage = new NavigationPage(new MainPage());
            else
                App.Current.MainPage = new NavigationPage(new GamePage());
        }
        else
            DisplayAlert("Внимание", "В кроссворде допущены ошибки, попробуйте еще раз", "Хорошо");
    }

    private void QuestionsView(Generate.Matrix matrix, List<Generate.Word> words_used)
    {
        string questions = String.Join("\n", words_used.OrderBy(x => x.Number).Select(x => $"{x.Number}. {Char.ToUpper(x.Question[0]) + x.Question.Substring(1)}").ToArray());

        System.Console.WriteLine(String.Join("\n", words_used));

        Frame frameQuestionsView = new Frame
        {
            Padding = new Thickness(1),
            Margin = new Thickness(5),
            CornerRadius = 5,
            Content = new ScrollView
            {
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                Content = new Label
                {
                    Text = questions,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };

        Grid.SetRowSpan(frameQuestionsView, 2);
        Grid.SetColumnSpan(frameQuestionsView, (boxColumns - 2) / 2);
        grid.Add(frameQuestionsView, 0, boxRows - 2);
    }

    private void Timer()
    {
        while (isTimerWork)
        {
            timeOnTimer = timeOnTimer.Add(TimeSpan.FromSeconds(1));

            MomentProcessingOnTimer();

            Thread.Sleep(1000);
        }
    }

    private void MomentProcessingOnTimer()
    {
        if (timeOnTimer.Second == 59)
            CleverIdeas();
    }

    [Obsolete]
    private async void CleverIdeas()
    {
        await Device.InvokeOnMainThreadAsync(async () =>
        {
            grid.Remove(frameCleverIdeas);
        });

        string ideaText = helpers.db.listCleverIdeas[random.Next(0, helpers.db.listCleverIdeas.Count())];

        frameCleverIdeas = new Frame
        {
            Padding = new Thickness(1),
            Margin = new Thickness(5),
            CornerRadius = 5,
            Content = new ScrollView
            {
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                Content = new Label
                {
                    Text = Char.ToUpper(ideaText[0]) + ideaText.Substring(1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };

        Grid.SetRowSpan(frameCleverIdeas, 2);
        Grid.SetColumnSpan(frameCleverIdeas, (boxColumns - 2) / 2);

        await Device.InvokeOnMainThreadAsync(async () =>
        {
            grid.Add(frameCleverIdeas, (boxColumns - 2) / 2, boxRows - 2);
        });
    }
}
