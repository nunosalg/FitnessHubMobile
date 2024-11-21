namespace FitnessHubMobile.Models
{
    public class Register
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

        public int GymId { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Confirm { get; set; }
    }
}
