namespace FitnessHubMobile.Services
{
    public class CountryApi
    {
        public string? Name { get; set; }

        public string? Cca2 { get; set; }

        public string? Flag { get; set; }

        public string? Callingcode { get; set; }

        public string? Data => $"({Callingcode}) {Name}";
    }
}
