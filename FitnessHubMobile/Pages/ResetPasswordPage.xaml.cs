using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class ResetPasswordPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IValidator _validator;
    private readonly string _email;

    public ResetPasswordPage(IApiService apiService, IValidator validator, string email)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _email = email;
    }

    private async void TapBackToLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void TapReset_Tapped(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        FrameReset.IsEnabled = false;

        var errorMessage = string.Empty;
        var token = EntToken.Text;

        if (string.IsNullOrEmpty(token))
        {
            errorMessage = "Type your token";
        }

        var password = EntPassword.Text;

        if (string.IsNullOrEmpty(password))
        {
            errorMessage += errorMessage != string.Empty ? "\nType your password" : "Type your password";
        }

        var confirm = EntConfirmPassword.Text;

        if (string.IsNullOrEmpty(confirm))
        {
            errorMessage += errorMessage != string.Empty ? "\nConfirm your password" : "Confirm your password";
        }

        if (!string.IsNullOrEmpty(password) && password.Length < 6)
        {
            errorMessage += errorMessage != string.Empty ? "\nPassword must contain at least 6 characters" : "Password must contain at least 6 characters";
        }

        if (confirm != password)
        {
            errorMessage += errorMessage != string.Empty ? "\nPasswords are not the same" : "Passwords are not the same";
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            await DisplayAlert("Error", errorMessage, "Cancel");

            LoadingIndicator.IsVisible = false;
            FrameReset.IsEnabled = true;
            return;
        }

        var response = await _apiService.ResetPassword(_email, token, password, confirm);

        if (!response.HasError)
        {
            await DisplayAlert("Reset", "Password reset successfully!", "Ok");
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }

        LoadingIndicator.IsVisible = false;
        FrameReset.IsEnabled = true;

        return;
    }
}