using FitnessHubMobile.Models;
using FitnessHubMobile.Services;

namespace FitnessHubMobile.Pages;

public partial class HomePage : ContentPage
{
    private readonly IApiService _apiService;
    private List<ClassModel> _classes = new List<ClassModel>();
    private List<ClassModel> _clientClasses = new List<ClassModel>();
    private int _classId = 0;

    public HomePage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        datePicker.MinimumDate = DateTime.Today;

        var classes = new List<ClassModel>();

        var (onlineClasses, errorMessage2) = await _apiService.GetOnlineClasses();
        if (onlineClasses != null)
        {
            classes.AddRange(onlineClasses);
        }

        var gymId = Preferences.Get("gymid", 1);

        var (gymClasses, errorMessage3) = await _apiService.GetClassesByGym(gymId);
        if (gymClasses != null)
        {
            classes.AddRange(gymClasses);
        }

        if (classes != null)
        {
            _classes = classes;

            var distinctCategories = classes
                                .Select(c => c.Category)
                                .Distinct()
                                .OrderBy(c => c)
                                .ToList();

            CategoryPicker.ItemsSource = distinctCategories;
        }

        var response = await _apiService.GetClientClasses();
        if(response.ClientClasses != null)
        {
            _clientClasses = response.ClientClasses.ToList();
        }

        if (_clientClasses != null)
        {
            var unregisteredGymClasses = gymClasses
                                    .Where(searchedClass => !_clientClasses.Any(clientClass => clientClass.Id == searchedClass.Id))
                                    .ToList();

            ClassesSearchResult.ItemsSource = unregisteredGymClasses;
        }
    }

    private async void TapSearch_Tapped(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        FrameSearch.IsEnabled = false;

        var selectedCategory = CategoryPicker.SelectedItem;
        
        DateTime? selectedDate = null;
        if (dateCheckbox.IsChecked)
        {
            selectedDate = datePicker.Date;
        }

        if (selectedCategory == null && selectedDate == null)
        {
            await DisplayAlert("Error", "Choose a category or a date.", "Ok");
            return;
        }

        var searchedClasses = new List<ClassModel>();
        if (selectedCategory != null && selectedDate == null)
        {
            searchedClasses = _classes.Where(c => string.Equals(c.Category, selectedCategory)).ToList();
        }

        if (selectedCategory == null && selectedDate != null)
        {
            searchedClasses = _classes.Where(c => c.DateStart.Date == selectedDate).ToList();
        }

        if (selectedCategory != null && selectedDate != null)
        {
            searchedClasses = _classes.Where(c => c.DateStart == selectedDate && string.Equals(c.Category, selectedCategory)).ToList();
        }

        var myClasses = _clientClasses;

        var unregisteredSearchedClasses = searchedClasses
                                    .Where(searchedClass => !myClasses.Any(clientClass => clientClass.Id == searchedClass.Id))
                                    .ToList();

        ClassesSearchResult.ItemsSource = unregisteredSearchedClasses;

        LoadingIndicator.IsVisible = false;
        FrameSearch.IsEnabled = true;
    }

    private async void TapRegisterInClass_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not int classId)
            return;

        var classModel = _classes.Where(c => c.Id == classId).FirstOrDefault();
        if(classModel == null)
        {
            await DisplayAlert("Error", "No class found", "Ok");
            return;
        }

        await Navigation.PushAsync(new RegisterInClassPage(_apiService, classModel));
    }
}