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

        ClassesHistory.ItemsSource = response;

        if (response == null)
        {
            await DisplayAlert("Info", "No history found", "Ok");
        }
    }
}