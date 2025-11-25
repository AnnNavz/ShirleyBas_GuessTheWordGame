namespace ShirleyBas_GuessTheWordGame
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

		private async void Play_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new CategoryPage());
		}
    }
}
