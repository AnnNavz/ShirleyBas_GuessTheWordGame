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

    private async void flower_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new flower());
    }

    private async void vegetable_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new vege());
    }

    private async void fruit_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new fruit());
    }
}