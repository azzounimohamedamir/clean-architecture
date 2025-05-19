using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId)> CreateUserAsync(string userName, string password);
    Task<bool> ValidateUserAsync(string userName, string password);
    Task<string> GenerateJwtTokenAsync(string userName);
    Task<(Result Result, string UserId)> DeleteUserAsync(string userId);
}
