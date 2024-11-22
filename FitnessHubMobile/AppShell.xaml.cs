using FitnessHubMobile.Pages;
using FitnessHubMobile.Services;
using FitnessHubMobile.Validations;

namespace FitnessHubMobile
{
    public partial class AppShell : Shell
    {
        private readonly IApiService _apiService;
        private readonly IValidator _validator;

        public AppShell(IApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;

            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService);
            var myClassesPage = new MyClassesPage(_apiService);
            var myWorkoutsPage = new MyWorkoutsPage(_apiService);
            var classesHistoryPage = new ClassesHistoryPage(_apiService);
            var accountPage = new AccountPage(_apiService, _validator);
            //var aboutPage = new AboutPage();

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent { Title = "Home",Icon = "home",Content = homePage  },
                    new ShellContent { Title = "Classes",Icon = "classes",Content = myClassesPage },
                    new ShellContent { Title = "Workouts",Icon = "workout",Content = myWorkoutsPage },
                    new ShellContent { Title = "History",Icon = "history",Content = classesHistoryPage },
                    new ShellContent { Title = "Account",Icon = "account",Content = accountPage },
                    //new ShellContent { Title = "About",Icon = "about",Content = aboutPage }
                }
            });
        }
    }
}
