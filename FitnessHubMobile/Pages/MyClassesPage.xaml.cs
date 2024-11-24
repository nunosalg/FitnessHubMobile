using FitnessHubMobile.Services;

namespace FitnessHubMobile.Pages;

public partial class MyClassesPage : ContentPage
{
    private readonly IApiService _apiService;

    public MyClassesPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var (response, errorMessage) = await _apiService.GetClientClasses();

        if (response != null)
        {
            ClientClasses.ItemsSource = response;
        }
        else
        {
            await DisplayAlert("Info", "Not registered in any classes", "Ok");
        }
    }
}