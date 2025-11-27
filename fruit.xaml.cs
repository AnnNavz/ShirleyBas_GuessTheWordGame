using System.ComponentModel;

namespace ShirleyBas_GuessTheWordGame;

public partial class fruit : ContentPage, INotifyPropertyChanged
{

    public event PropertyChangedEventHandler PropertyChanged;

    #region UI Properties
    public string Spotlight
    {
        get => spotlight;
        set
        {
            spotlight = value;
            OnPropertyChanged(nameof(Spotlight));
        }
    }

    public List<char> Letters
    {
        get => letters;
        set
        {
            letters = value;
            OnPropertyChanged(nameof(Letters));
        }
    }

    public string Message
    {
        get => message;
        set
        {
            message = value;
            OnPropertyChanged(nameof(Message));
        }
    }

    public string Hint
    {
        get => hint;
        set
        {
            hint = value;
            OnPropertyChanged(nameof(Hint));
        }
    }

    public string GameStatus
    {
        get => gameStatus;
        set
        {
            gameStatus = value;
            OnPropertyChanged(nameof(GameStatus));
        }
    }

    public string CurrentImage
    {
        get => currentImage;
        set
        {
            currentImage = value;
            OnPropertyChanged(nameof(CurrentImage));
        }
    }
    #endregion

    #region Fields
    Dictionary<string, string> wordHints = new Dictionary<string, string>()
    {
        { "APPLE", "Crisp, round fruit with many varieties and colors."},
        { "BANANA", "Long, curved, yellow fruit; rich in potassium."},
        { "ORANGE", "Citrus fruit with a thick rind and segments; high in Vitamin C."},
        { "STRAWBERRY", "Small, red, sweet fruit with seeds on the outside."},
        { "GRAPE", "Small, round fruit that grows in clusters on vines."},
        { "WATERMELON", "Large, green-rinded fruit with sweet, red, watery flesh."},
        { "PINEAPPLE", "Tropical fruit with a spiky skin and sweet, yellow flesh."},
        { "MANGO", "Sweet, tropical stone fruit with a smooth skin and large pit."},
        { "AVOCADO", "Green, pear-shaped fruit with a large pit; creamy and fatty."},
        { "KIWI", "Small, brown, fuzzy fruit with bright green flesh and tiny black seeds."},
        { "LEMON", "Tart, yellow citrus fruit used mainly for flavoring."},
        { "PEACH", "Soft, round stone fruit with fuzzy skin and juicy flesh."},
        { "CHERRY", "Small, round, red fruit with a single small stone."}
    };
    #endregion

    string answer = "";
    private string spotlight;
    private List<char> guessed = new List<char>();
    private List<char> letters = new List<char>();
    private string message;
    private int mistakes = 0;
    private int maxWrong = 6;
    private string gameStatus;
    private string currentImage = "img0.png";
    private string hint;
    public fruit()
    {
        InitializeComponent();
        Letters.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        BindingContext = this;
        PickWord();
        CalculateWord(answer, guessed);
        UpdateStatus();
        EnableLetters();
    }


    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CalculateWord(string answer, List<char> guessed)
    {
        var temp = answer
            .Select(x => guessed.Contains(x) ? x : '_')
            .ToArray();
        Spotlight = string.Join(' ', temp);
    }

    #region Game Engine
    private void PickWord()
    {
        var random = new Random();

        List<string> keys = new List<string>(wordHints.Keys);
        int index = random.Next(keys.Count);
        string selectedWord = keys[index];

        answer = selectedWord;
        Hint = wordHints[selectedWord];

        int calculatedClues = answer.Length / 3;
        int minimumClues = 2;
        int cluesToAdd = Math.Max(minimumClues, calculatedClues);


        var wordLetters = answer.Distinct().ToList();
        cluesToAdd = Math.Min(cluesToAdd, wordLetters.Count);

        guessed.Clear();

        for (int i = 0; i < cluesToAdd; i++)
        {
            var availableClues = wordLetters.Except(guessed).ToList();

            if (availableClues.Any())
            {
                char clueLetter = availableClues[random.Next(availableClues.Count)];
                guessed.Add(clueLetter);
            }
            else
            {
                break;
            }
        }
    }


    #endregion

    private void Reset_Clicked(object sender, EventArgs e)
    {
        mistakes = 0;
        guessed.Clear();
        CurrentImage = "img0.png";
        PickWord();
        CalculateWord(answer, guessed);
        Message = "";
        UpdateStatus();
        EnableLetters();
    }

    private async void letter_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            var letter = btn.Text;
            await btn.FadeTo(0.5, 50, Easing.CubicOut);
            btn.IsEnabled = false;
            HandleGuess(letter[0]);
        }

    }

    private void HandleGuess(char letter)
    {
        if (!guessed.Contains(letter))
            guessed.Add(letter);

        if (answer.Contains(letter))
        {
            CalculateWord(answer, guessed);
            CheckIfGameWon();

        }
        else
        {
            mistakes++;
            UpdateStatus();
            CheckIfGameLost();
            CurrentImage = $"img{mistakes}.png";
            if (mistakes == 6)
            {
                CurrentImage = $"img6.gif";
            }
        }
    }

    private void CheckIfGameWon()
    {
        if (Spotlight.Replace(" ", "") == answer)
        {
            Message = "You win!";
            CurrentImage = "win.png";
            DisableLetters();
        }
    }

    private void UpdateStatus()
    {
        GameStatus = $"Errors: {mistakes} of {maxWrong}";
    }

    private void CheckIfGameLost()
    {
        if (mistakes >= maxWrong)
        {
            Message = $"You lost! The word was: {answer}";
            DisableLetters();
        }
    }

    private void DisableLetters()
    {
        foreach (var child in LettersContainer.Children)
        {
            if (child is Button btn)
            {
                btn.IsEnabled = false;
            }
        }
    }

    private void EnableLetters()
    {
        foreach (var child in LettersContainer.Children)
        {
            if (child is Button btn)
            {
                btn.Opacity = 1.0;
                if (btn.Text.Length == 1 && guessed.Contains(btn.Text[0]))
                {
                    btn.IsEnabled = false;
                    btn.Opacity = 0.5;
                }
                else
                {
                    btn.IsEnabled = true;
                }
            }
        }
    }
}