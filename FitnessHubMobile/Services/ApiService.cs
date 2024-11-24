using FitnessHubMobile.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FitnessHubMobile.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly string _baseUrl = AppConfig.BaseUrl;

        JsonSerializerOptions _serializerOptions;
        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<ApiResponse<bool>> Register(string firstName, string lastName, string phoneNumber, string email, DateTime birthDate, string password, string confirmPassword, int gymId)
        {
            try
            {
                var register = new Register()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    BirthDate = birthDate,
                    Password = password,
                    Confirm = confirmPassword,
                    GymId = gymId
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Clients/Register", content);

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Detailed error: {errorContent}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP requisition: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Clients/Login", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP requisition: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response content: {jsonResult}");
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("userid", result.UserId!);
                Preferences.Set("username", result.UserName);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> RecoverPassword(string email)
        {
            try
            {
                var recover = new RecoverPassword()
                {
                    Email = email,
                };

                var json = JsonSerializer.Serialize(recover, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Clients/RecoverPassword", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP requisition: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RecoverPassword>(jsonResult, _serializerOptions);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error recovering password : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ResetPassword(string email, string token, string password, string confirm)
        {
            try
            {
                var reset = new ResetPassword()
                {
                    Email = email,
                    Token = token,
                    NewPassword = password,
                    Confirm = confirm
                };

                var json = JsonSerializer.Serialize(reset, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Clients/ResetPassword", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP requisition: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ResetPassword>(jsonResult, _serializerOptions);

                Preferences.Set("token", result!.Token);
                Preferences.Set("email", result!.Email);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error resetting password : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UploadUserImage(byte[] imageArray)
        {
            try
            {
                AddAuthorizationHeader();

                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "image", "image.jpg");
                var response = await PostRequest("api/Clients/UploadUserImage", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                      ? "Unauthorized"
                      : $"Error sending HTTP requisition: {response.StatusCode}";

                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading user image: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ChangePassword(string oldPassword, string newPassword, string confirm)
        {
            try
            {
                AddAuthorizationHeader();

                var change = new ChangePassword()
                {
                    OldPassword = oldPassword,
                    NewPassword = newPassword,
                    Confirm = confirm
                };

                var json = JsonSerializer.Serialize(change, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Clients/ChangePassword", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP requisition: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ChangePassword>(jsonResult, _serializerOptions);

                Preferences.Set("password", result!.NewPassword);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error changing password : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> RegisterClientInClass(int classId, bool isOnline)
        {
            try
            {
                AddAuthorizationHeader();

                var register = new RegisterInClass()
                {
                    ClassId = classId,
                    IsOnline = isOnline
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/ClientClassesApi/RegisterInClass", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP requisition: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RegisterInClass>(jsonResult, _serializerOptions);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering in class : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var urlAddress = _baseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending POST requisition to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<ApiResponse<bool>> UpdateUserInfo(string firstName, string lastName, DateTime birthDate, string phoneNumber)
        {
            try
            {
                var userInfo = new UserInfo()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = birthDate,
                    PhoneNumber = phoneNumber
                };

                var json = JsonSerializer.Serialize(userInfo, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PutRequest("api/Clients/UpdateUserInfo", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP requisition: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP requisition: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UserInfo>(jsonResult, _serializerOptions);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user info : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var urlAddress = AppConfig.BaseUrl + uri;
            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending PUT requisition for {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<(ProfileImage? ProfileImage, string? ErrorMessage)> GetUserProfileImage()
        {
            string endpoint = "api/Clients/UserImage";
            return await GetAsync<ProfileImage>(endpoint);
        }

        public async Task<(UserInfo? UserInfo, string? ErrorMessage)> GetUserInfo()
        {
            string endpoint = "api/Clients/UserInfo";
            return await GetAsync<UserInfo>(endpoint);
        }

        public async Task<(IEnumerable<Gym>? Gyms, string? ErrorMessage)> GetGyms()
        {
            string endpoint = "api/GymsApi/GetAllGyms";
            return await GetAsync<IEnumerable<Gym>>(endpoint);
        }

        public async Task<(IEnumerable<ClassModel>? GymClasses, string? ErrorMessage)> GetClassesByGym(int gymId)
        {
            string endpoint = "api/ClassesApi/GetClassesByGym";
            endpoint += $"?gymId={gymId}";
            return await GetAsync<IEnumerable<ClassModel>>(endpoint);
        }

        public async Task<(IEnumerable<ClassModel>? OnlineClasses, string? ErrorMessage)> GetOnlineClasses()
        {
            string endpoint = "api/ClassesApi/GetOnlineClasses";
            return await GetAsync<IEnumerable<ClassModel>>(endpoint);
        }

        public async Task<(IQueryable<ClassModel>? ClientClasses, string? ErrorMessage)> GetClientClasses()
        {
            string endpoint = "api/ClientClassesApi/MyClasses";
            return await GetAsync<IQueryable<ClassModel>>(endpoint);
        }

        public async Task<(IEnumerable<ClientClassHistory>? ClientClassesHistory, string? ErrorMessage)> GetClientClassesHistory()
        {
            string endpoint = "api/ClientClassesApi/MyClassesHistory";
            return await GetAsync<IEnumerable<ClientClassHistory>>(endpoint);
        }

        public async Task<(IEnumerable<Workout>? ClientWorkouts, string? ErrorMessage)> GetClientWorkouts()
        {
            string endpoint = "api/Clients/GetWorkouts";
            return await GetAsync<IEnumerable<Workout>>(endpoint);
        }

        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var url = AppConfig.BaseUrl + endpoint;
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Requisition error: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"HTTP requisition error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"JSON deserialization error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Unexpected error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<Response> GetCountriesAsync()
        {
            var response = await _httpClient.GetAsync("https://countryinfoapi.com/api/countries");
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var countries = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CountryApi>>(result);

                List<CountryApi> countryList = new List<CountryApi>();

                foreach (var country in countries)
                {
                    if (!string.IsNullOrEmpty(country.Callingcode) && country.Callingcode.ToLower() != "undefined")
                    {
                        countryList.Add(country);
                    }
                }

                return new Response
                {
                    IsSuccess = true,
                    Results = countryList.OrderBy(c => c.Name).ToList()
                };
            }

            return new Response
            {
                IsSuccess = false,
                Message = result
            };
        }
    }
}
