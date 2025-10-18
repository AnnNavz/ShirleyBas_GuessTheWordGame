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
		{ "CSHARP","The language used to write this .NET MAUI app."},
		{ "JAVASCRIPT","The language used for web development."},
		{ "JOINS","Used in SQL to combine rows from two or more tables."},
		{ "HASHING","The process of converting input into a fixed-size string of characters, often used for passwords."},
		{ "SYSTEM","A set of components that work together to perform a specific goal."},
		{ "DATA","Raw facts and figures, often stored and processed by a computer."},
		{ "INFORMATION", "These are alternatively called the processed data."},
		{ "SOFTWARE", "The programs used by a computer."},
		{ "HARDWARE","The physical components of a computer."},
		{ "NETWORK","A group of interconnected computers and devices."},
		{ "INTERFACE","This is what users see first in the screen."},
		{ "VISUALSTUDIO","An app that mostly students used for desktop development."},
		{ "GITHUB", "A platform for storing code projects and collaborative software development."}
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
	private string hint;

	public Game()
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
		int cluesToAdd = Math.Max(1, calculatedClues);
	

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