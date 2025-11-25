namespace ShirleyBas_GuessTheWordGame;

public partial class CategoryPage : ContentPage
{
	public CategoryPage()
	{
		InitializeComponent();
	}

	private async void program_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new Game());
	}

	private async void animal_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new AnimalGame());
	}
}