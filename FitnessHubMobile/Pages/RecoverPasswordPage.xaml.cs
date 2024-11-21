using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class RecoverPasswordPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IValidator _validator;

    public RecoverPasswordPage(IApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void TapRecover_Tapped(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        FrameRecover.IsEnabled = false;

        var email = EntEmail.Text;

        if (string.IsNullOrEmpty(email))
        {
            LoadingIndicator.IsVisible = false;
            FrameRecover.IsEnabled = true;

            await DisplayAlert("Error", "Type your email", "Cancel");
            return;
        }

        var response = await _apiService.RecoverPassword(email);

        if (!response.HasError)
        {
            await DisplayAlert("Recover", "Check your email to find the token", "Ok");
            await Navigation.PushAsync(new ResetPasswordPage(_apiService, _validator, email));
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }

        LoadingIndicator.IsVisible = false;
        FrameRecover.IsEnabled = true;
    }

    private async void TapBackToLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}