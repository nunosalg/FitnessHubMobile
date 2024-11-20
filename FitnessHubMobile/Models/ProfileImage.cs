namespace FitnessHubMobile.Models
{
    public class ProfileImage
    {
        public string? ImagePath { get; set; }

        public string? ImageFullPath => AppConfig.BaseUrl + ImagePath;
    }
}
