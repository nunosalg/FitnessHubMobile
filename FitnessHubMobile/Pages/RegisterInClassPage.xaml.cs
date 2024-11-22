using FitnessHubMobile.Models;
using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class RegisterInClassPage : ContentPage
{
    private readonly IApiService _apiService;
    private ClassModel _classModel;

    public RegisterInClassPage(IApiService apiService, ClassModel classModel)
    {
        InitializeComponent();
        _apiService = apiService;
        _classModel = classModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LblClassType.Text = _classModel.ClassType;
        EntCategory.Text = _classModel.Category;
        EntInstructor.Text = _classModel.Instructor;
        EntDateStart.Text = _classModel.DateStart.ToString();
        EntDateEnd.Text = _classModel.DateEnd.ToString();
        EntLocation.Text = _classModel.Location;
    }

    private async void TapRegisterInClass_Tapped(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        FrameRegisterInClass.IsEnabled = false;

        var response = await _apiService.RegisterClientInClass(_classModel.Id, _classModel.IsOnline);
        if (!response.HasError)
        {
            await DisplayAlert("Register", "Registered successfully! Check your email!", "OK");
            await Navigation.PushAsync(new HomePage(_apiService));
        }
        else
        {
            await DisplayAlert("Error", response.ErrorMessage ?? "Something went wrong!!!", "Cancel");
        }

        LoadingIndicator.IsVisible = false;
        FrameRegisterInClass.IsEnabled = true;
    }

    private async void TapBackHomepage_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new HomePage(_apiService));
    }
}