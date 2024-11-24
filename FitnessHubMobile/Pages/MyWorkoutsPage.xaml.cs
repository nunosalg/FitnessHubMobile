using FitnessHubMobile.Services;

namespace FitnessHubMobile.Pages;

public partial class MyWorkoutsPage : ContentPage
{
    private readonly IApiService _apiService;

    public MyWorkoutsPage(IApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var (response, errorMessage) = await _apiService.GetClientWorkouts();

        if (response != null && response.Any())
        {
            ClientWorkouts.ItemsSource = response;
        }
        else
        {
            await DisplayAlert("Info", "You have no workouts", "Ok");
        }
    }
}