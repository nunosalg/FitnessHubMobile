using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class AboutPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IValidator _validator;

    public AboutPage(IApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void ImgBtnInstagram_Clicked(object sender, EventArgs e)
    {
        var instagramUrl = "https://www.instagram.com/fitnesshub.network/";

        if (Uri.TryCreate(instagramUrl, UriKind.Absolute, out var uri))
        {
            await Launcher.OpenAsync(uri);
        }
        else
        {
            await DisplayAlert("Error", "Unable to open Instagram page", "Ok");
        }
    }

    private async void ImgBtnFacebook_Clicked(object sender, EventArgs e)
    {
        var linkedinUrl = "https://www.facebook.com/fitnesshub.network/";

        if (Uri.TryCreate(linkedinUrl, UriKind.Absolute, out var uri))
        {
            await Launcher.OpenAsync(uri);
        }
        else
        {
            await DisplayAlert("Error", "Unable to open Facebook page", "Ok");
        }
    }

    private async void ImgBtnX_Clicked(object sender, EventArgs e)
    {
        var githubUrl = "https://x.com/FitnessHub_87";

        if (Uri.TryCreate(githubUrl, UriKind.Absolute, out var uri))
        {
            await Launcher.OpenAsync(uri);
        }
        else
        {
            await DisplayAlert("Error", "Unable to open X page", "Ok");
        }
    }

    private async void TapBackAccount_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AccountPage(_apiService, _validator));
    }
}