namespace FitnessHubMobile.Validations
{
    public interface IValidator
    {
        string FirstNameError { get; set; }

        string LastNameError { get; set; }

        string EmailError { get; set; }

        string PhoneNumberError { get; set; }

        string BirthDateError { get; set; }

        string PasswordError { get; set; }

        string ConfirmPasswordError { get; set; }

        Task<bool> Validate(string firstName, string lastName, string email, string phoneNumber, DateTime birthDate, string password, string confirmPassword);
    }
}
