using FitnessHubMobile.Models;

namespace FitnessHubMobile.Services
{
    public interface IApiService
    {
        Task<ApiResponse<bool>> Register(string firstName, string lastName, string phoneNumber, string email, DateTime birthDate, string password, string confirmPassword);

        Task<ApiResponse<bool>> Login(string email, string password);

        Task<ApiResponse<bool>> RecoverPassword(string email);

        Task<ApiResponse<bool>> ResetPassword(string email, string token, string password, string confirm);

        Task<ApiResponse<bool>> UploadUserImage(byte[] imageArray);

        Task<ApiResponse<bool>> ChangePassword(string oldPassword, string newPassword, string confirm);

        Task<ApiResponse<bool>> RegisterClientInClass(int classId, bool isOnline);

        Task<ApiResponse<bool>> UpdateUserInfo(string firstName, string lastName, DateTime birthDate, string phoneNumber);

        Task<(ProfileImage? ProfileImage, string? ErrorMessage)> GetUserProfileImage();

        Task<(UserInfo? UserInfo, string? ErrorMessage)> GetUserInfo();

        Task<(IEnumerable<Gym>? Gyms, string? ErrorMessage)> GetGyms();

        Task<(IEnumerable<ClassModel>? GymClasses, string? ErrorMessage)> GetClassesByGym(int gymId);

        Task<(IEnumerable<ClassModel>? OnlineClasses, string? ErrorMessage)> GetOnlineClasses();

        Task<(IEnumerable<ClassModel>? ClientClasses, string? ErrorMessage)> GetClientClasses();

        Task<(IEnumerable<ClientClassHistory>? ClientClassesHistory, string? ErrorMessage)> GetClientClassesHistory();

        Task<Response> GetCountriesAsync();
    }
}
