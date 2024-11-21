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
}