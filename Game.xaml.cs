using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace ShirleyBas_GuessTheWordGame;

public partial class Game : ContentPage, INotifyPropertyChanged
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
	List<string> words = new List<string>()
	{
	"python",
	"javascript",
	"csharp",
	"mongodb",
	"sql",
	"xaml",
	"powerpoint",
	"code"
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
	private string currentImage = "img0.jpg";

	public Game()
	{
		InitializeComponent();
		Letters.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
		BindingContext = this;
		PickWord();
		CalculateWord(answer, guessed);
		UpdateStatus();

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
		answer = words[new Random().Next(words.Count)];
	}


	#endregion

	private void Reset_Clicked(object sender, EventArgs e)
	{
		mistakes = 0;
		guessed.Clear();
		CurrentImage = "img0.jpg";
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
			CurrentImage = $"img{mistakes}.jpg";
		}
	}

	private void CheckIfGameWon()
	{
		if (Spotlight.Replace(" ", "") == answer)
		{
			Message = "You win!";
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
				btn.IsEnabled = true;
				btn.Opacity = 1.0;
			}
		}

	}

}