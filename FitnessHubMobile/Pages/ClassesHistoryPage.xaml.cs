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
}