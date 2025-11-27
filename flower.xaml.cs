using System.ComponentModel;

namespace ShirleyBas_GuessTheWordGame;

public partial class flower : ContentPage, INotifyPropertyChanged
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
        { "ROSE", "Symbol of love and beauty; comes in many colors."},
        { "TULIP", "Spring-blooming flower; cup-shaped and bright."},
        { "SUNFLOWER", "Tall flower with a large head; follows the sun."},
        { "DAISY", "Small flower with white petals and a yellow center; simple and cheerful."},
        { "LILY", "Large, trumpet-shaped, and often fragrant flower."},
        { "ORCHID", "Exotic and delicate flower with complex, vibrant shapes."},
        { "CARNATION", "Fringed petals; often used for boutonnieres and corsages."},
        { "HYDRANGEA", "Shrub with large, globe-shaped clusters of small flowers."},
        { "LAVENDER", "Fragrant purple flower known for its calming properties."},
        { "POPPY", "Brightly colored flower with delicate, papery petals."},
        { "PETUNIA", "Common garden flower with trumpet-shaped blossoms."},
        { "IRIS", "Distinctive flower with three upright and three drooping petals."},
        { "PEONY", "Large, fragrant flower with many layers of soft petals."}
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

    public flower()
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