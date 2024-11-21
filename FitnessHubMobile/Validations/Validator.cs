using System.Text.RegularExpressions;

namespace FitnessHubMobile.Validations
{
    public class Validator : IValidator
    {
        public string FirstNameError { get; set; } = "";

        public string LastNameError { get; set; } = "";

        public string EmailError { get; set; } = "";

        public string PhoneNumberError { get; set; } = "";

        public string BirthDateError { get; set; } = "";

        public string PasswordError { get; set; } = "";

        public string ConfirmPasswordError { get; set; } = "";

        private const string EmptyFirstNameErrorMsg = "Please, type your first name.";
        private const string InvalidFirstNameErrorMsg = "Please, type a valid first name.";
        private const string EmptyLastNameErrorMsg = "Please, type your last name.";
        private const string InvalidLastNameErrorMsg = "Please, type a valid last name.";
        private const string EmptyEmailErrorMsg = "Please, type your email.";
        private const string EmptyPhoneNumberErrorMsg = "Please, type your phone number.";
        private const string InvalidEmailErrorMsg = "Please, type a valid email.";
        private const string InvalidBirthDateErrorMsg = "User must be eighteen years old (minimum) to register.";
        private const string EmptyPasswordErrorMsg = "Please, type your password.";
        private const string InvalidPasswordErrorMsg = "Password must contain 6 characters.";
        private const string EmptyConfirmPasswordErrorMsg = "Please, confirm your password.";
        private const string PasswordsNotMatchingErrorMsg = "Confirm password must be the same as password.";

        public Task<bool> Validate(string firstName, string lastName, string email, string phoneNumber, DateTime birthDate, string password, string confirmPassword)
        {
            var isFirstNameValid = ValidateFirstName(firstName);
            var isLastNameValid = ValidateLastName(lastName);
            var isEmailValid = ValidateEmail(email);
            var isPhoneNumberValid = ValidatePhoneNumber(phoneNumber);
            var isBirthDateValid = ValidateBirthDate(birthDate);
            var isPasswordValid = ValidatePassword(password);
            var isConfirmPasswordValid = ValidateConfirmPassword(password, confirmPassword);

            return Task.FromResult(isFirstNameValid && isLastNameValid && isEmailValid && isBirthDateValid && isPasswordValid && isConfirmPasswordValid);
        }

        private bool ValidateConfirmPassword(string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(confirmPassword))
            {
                ConfirmPasswordError = EmptyConfirmPasswordErrorMsg;
                return false;
            }

            if (confirmPassword != password)
            {
                ConfirmPasswordError = PasswordsNotMatchingErrorMsg;
                return false;
            }

            ConfirmPasswordError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordErrorMsg;
                return false;
            }

            if (password.Length < 6)
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                PhoneNumberError = EmptyPhoneNumberErrorMsg;
                return false;
            }

            PhoneNumberError = "";
            return true;
        }

        private bool ValidateBirthDate(DateTime birthDate)
        {
            if (birthDate.AddYears(18) > DateTime.Now)
            {
                BirthDateError = InvalidBirthDateErrorMsg;
                return false;
            }

            BirthDateError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidateLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                LastNameError = EmptyLastNameErrorMsg;
                return false;
            }

            if (lastName.Length < 3)
            {
                LastNameError = InvalidLastNameErrorMsg;
                return false;
            }

            LastNameError = "";
            return true;
        }

        private bool ValidateFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                FirstNameError = EmptyFirstNameErrorMsg;
                return false;
            }

            if (firstName.Length < 3)
            {
                FirstNameError = InvalidFirstNameErrorMsg;
                return false;
            }

            FirstNameError = "";
            return true;
        }
    }
}
