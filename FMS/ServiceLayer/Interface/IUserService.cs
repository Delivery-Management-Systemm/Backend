using FMS.Models;
using FMS.ServiceLayer.DTO.UserDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> RegisterAsync(User user, string password);
        Task<(User user, string token)> LoginAsync(string email, string password);
        Task<bool> UpdateProfileAsync(int userId, UpdateUserProfileDto dto);
        Task<bool> DeleteAccountAsync(int userId);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);
        Task<string> SendRegistrationOtpAsync(string email);
        Task<bool> VerifyRegistrationOtpAsync(string email, string otp);
    }
}
