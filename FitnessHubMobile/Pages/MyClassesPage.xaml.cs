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

        ClientClasses.ItemsSource = response;

        if (response == null)
        {
            await DisplayAlert("Info", "Client not registered in any classes", "Ok");
        }
    }
}