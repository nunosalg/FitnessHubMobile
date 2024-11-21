using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class AccountPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IValidator _validator;

    public AccountPage(IApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
}