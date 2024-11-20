namespace FitnessHubMobile.Models
{
    public class Gym
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Country { get; set; }

        public string? FlagUrl { get; set; }

        public string? City { get; set; }

        public string? Address { get; set; }

        public string? ImagePath { get; set; }

        public string Data => $"{Name} - {Address}, {City}, {Country}";

        public string ImageDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return $"/images/noimage.png";
                }
                else
                {
                    return $"{ImagePath.Substring(1)}";
                }
            }
        }
    }
}
