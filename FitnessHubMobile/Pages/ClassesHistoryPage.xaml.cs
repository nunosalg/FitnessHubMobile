using FitnessHubMobile.Services;

namespace FitnessHubMobile.Pages;

public partial class ClassesHistoryPage : ContentPage
{
    private readonly IApiService _apiService;

    public ClassesHistoryPage(IApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var (response, errorMessage) = await _apiService.GetClientClassesHistory();

        if (response != null)
        {
            ClassesHistory.ItemsSource = response;
        }
        else
        {
            await DisplayAlert("Info", "No classes history found", "Ok");
        }
    }
}