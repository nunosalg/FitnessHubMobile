using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IValidator _validator;

    public LoginPage(IApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void TapForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RecoverPasswordPage(_apiService, _validator));
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        FrameLogin.IsEnabled = false;

        var errorMessage = string.Empty;

        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            errorMessage = "Type your email";
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            errorMessage += errorMessage != string.Empty ? "\nType your password" : "Type your password";
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            await DisplayAlert("Error", errorMessage, "Cancel");

            LoadingIndicator.IsVisible = false;
            FrameLogin.IsEnabled = true;
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            var result = await _apiService.GetUserInfo();

            Preferences.Set("gymid", result.UserInfo!.GymId);

            Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }

        LoadingIndicator.IsVisible = false;
        FrameLogin.IsEnabled = true;
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_apiService, _validator));
    }
}