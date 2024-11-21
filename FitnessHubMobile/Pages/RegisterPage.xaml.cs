using FitnessHubMobile.Models;
using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly IValidator _validator;
    private List<CountryApi>? _countries = new List<CountryApi>();

    public RegisterPage(IApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        DateOfBirthPicker.MaximumDate = DateTime.Today;

        var response = await _apiService.GetCountriesAsync();
        if (response == null)
        {
            await DisplayAlert("Error", "Couldn't load countries", "Ok");
            return;
        }

        var (gyms, errorMessage) = await _apiService.GetGyms();
        if(gyms == null)
        {
            await DisplayAlert("Error", "Couldn't load gyms", "Ok");
            return;
        }

        CountriesPicker.ItemsSource = (List<CountryApi>?)response.Results;

        GymsPicker.ItemsSource = (List<Gym>?)gyms;
    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        FrameRegister.IsEnabled = false;
        LoadingIndicator.IsVisible = true;
        try
        {
            string errorMessage = "";

            var country = CountriesPicker.SelectedItem as CountryApi;
            if (country?.Callingcode == "" || country?.Callingcode == null)
            {
                errorMessage = "- No country selected";
            }

            var gym = GymsPicker.SelectedItem as Gym;
            if(gym?.Id == null)
            {
                errorMessage += "- No gym selected"; 
            }

            var phoneNumber = $"{country?.Callingcode}{EntPhoneNumber.Text}";

            if (await _validator.Validate(EntFirstName.Text, EntLastName.Text, EntEmail.Text, EntPhoneNumber.Text, DateOfBirthPicker.Date, EntPassword.Text, EntConfirmPassword.Text) && string.IsNullOrEmpty(errorMessage))
            {
                var response = await _apiService.Register(EntFirstName.Text, EntLastName.Text, phoneNumber, EntEmail.Text, DateOfBirthPicker.Date, EntPassword.Text, EntConfirmPassword.Text, gym.Id);

                if (!response.HasError)
                {
                    await DisplayAlert("Register", "Account created successfully! Check your email to confirm!", "OK");
                    await Navigation.PushAsync(new LoginPage(_apiService, _validator));
                }
                else
                {
                    await DisplayAlert("Error", response.ErrorMessage ?? "Something went wrong!!!", "Cancel");
                }
            }
            else
            {
                
                errorMessage += _validator.FirstNameError != null ? $"\n- {_validator.FirstNameError}" : "";
                errorMessage += _validator.LastNameError != null ? $"\n- {_validator.LastNameError}" : "";
                errorMessage += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
                errorMessage += _validator.BirthDateError != null ? $"\n- {_validator.BirthDateError}" : "";
                errorMessage += _validator.PhoneNumberError != null ? $"\n- {_validator.PhoneNumberError}" : "";
                errorMessage += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";
                errorMessage += _validator.ConfirmPasswordError != null ? $"\n- {_validator.ConfirmPasswordError}" : "";

                await DisplayAlert("Error", errorMessage, "OK");
            }
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            FrameRegister.IsEnabled = true;
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}