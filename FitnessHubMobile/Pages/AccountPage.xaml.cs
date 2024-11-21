using FitnessHubMobile.Models;
using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class AccountPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public AccountPage(IApiService apiService, IValidator validator)
	{
		InitializeComponent();
        LblEmail.Text = Preferences.Get("username", string.Empty);

        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnProfile.Source = await GetProfileImage();

        var userInfo = await GetUserInfo();
        EntFirstName.Text = userInfo.FirstName;
        EntLastName.Text = userInfo.LastName;
        DateOfBirthPicker.Date = userInfo.BirthDate;
        EntPhoneNumber.Text = userInfo.PhoneNumber;

        DateOfBirthPicker.MaximumDate = DateTime.Today;
    }

    private async Task<string?> GetProfileImage()
    {
        string defaultImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Couldn't retrieve image.", "Ok");
                    return defaultImage;
            }
        }

        if (response?.ImagePath is not null)
        {
            var adjustedImagePath = response.ImageFullPath.Replace("~/", "");

            return adjustedImagePath;
        }

        return defaultImage;
    }

    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var archive = await MediaPicker.PickPhotoAsync();

            if (archive is null) return null;

            using (var stream = await archive.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "The functionality is not supported on the device.", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permissions not granted to access the camera or gallery.", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error selecting image: {ex.Message}", "Ok");
        }
        return null;
    }

    private async Task<UserInfo?> GetUserInfo()
    {
        var (response, errorMessage) = await _apiService.GetUserInfo();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Couldn't retrieve user info.", "Ok");
                    return null;
            }
        }

        if (response != null)
        {
            return response;
        }

        await DisplayAlert("Error", errorMessage ?? "Couldn't retrieve user info.", "Ok");
        return null;
    }

    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();
            if (imageArray is null)
            {
                await DisplayAlert("Error", "Unable to load the image.", "Ok");
                return;
            }
            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);
            if (response.Data)
            {
                await DisplayAlert("", "Image loaded successfully", "Ok");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "Unknown error ocurred", "Cancel");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error ocurred: {ex.Message}", "Ok");
        }
    }

    private async void TapChangeInfo_Tapped(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;

        var firstName = EntFirstName.Text;
        var lastName = EntLastName.Text;
        var birthDate = DateOfBirthPicker.Date;
        var phoneNumber = EntPhoneNumber.Text;

        if (string.IsNullOrEmpty(firstName))
        {
            await DisplayAlert("Error", "Type your first name", "Cancel");
            return;
        }

        if (string.IsNullOrEmpty(lastName))
        {
            await DisplayAlert("Error", "Type your last name", "Cancel");
            return;
        }

        if (birthDate.AddYears(18) > DateTime.Now)
        {
            await DisplayAlert("Error", "User must be at least 18 years old", "Cancel");
            return;
        }

        if (string.IsNullOrEmpty(phoneNumber))
        {
            await DisplayAlert("Error", "Type your phone number", "Cancel");
            return;
        }

        var response = await _apiService.UpdateUserInfo(firstName, lastName, birthDate, phoneNumber);
        if (!response.HasError)
        {
            await DisplayAlert("Update", "User info updated successfully!", "Ok");
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }

        LoadingIndicator.IsVisible = false;
    }

    private async void TapChangePassword_Tapped(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        var currentPassword = EntCurrentPassword.Text;

        if (string.IsNullOrEmpty(currentPassword))
        {
            await DisplayAlert("Error", "Type your current password", "Cancel");
            return;
        }

        var newPassword = EntNewPassword.Text;

        if (string.IsNullOrEmpty(newPassword))
        {
            await DisplayAlert("Error", "Type your new password", "Cancel");
            return;
        }

        var confirm = EntConfirmPassword.Text;

        if (string.IsNullOrEmpty(confirm))
        {
            await DisplayAlert("Error", "Confirm your password", "Cancel");
            return;
        }

        if (newPassword.Length < 8)
        {
            await DisplayAlert("Error", "New password must contain at least 8 characters", "Cancel");
            return;
        }

        if (confirm != newPassword)
        {
            await DisplayAlert("Error", "Confirm doesn't match with new password", "Cancel");
            return;
        }

        var response = await _apiService.ChangePassword(currentPassword, newPassword, confirm);

        if (!response.HasError)
        {
            await DisplayAlert("Reset", "Password changed successfully!", "Ok");
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong", "Cancel");
        }

        EntNewPassword.Text = string.Empty;
        EntConfirmPassword.Text = string.Empty;
        LoadingIndicator.IsVisible = false;
    }

    private void TapLogout_Tapped(object sender, TappedEventArgs e)
    {
        Preferences.Set("accesstoken", string.Empty);
        Application.Current!.MainPage = new NavigationPage(new LoginPage(_apiService, _validator));
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}