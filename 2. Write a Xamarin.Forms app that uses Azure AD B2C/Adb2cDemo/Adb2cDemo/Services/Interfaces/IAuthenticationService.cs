using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Adb2cDemo.Services.Interfaces
{
    public interface IAuthenticationService
    {
        string ResetErrorCode { get; }
        Task<UserContext> SignInAsync();
        Task<UserContext> AcquireTokenSilent();
        Task<UserContext> ResetPasswordAsync();
        Task<UserContext> EditProfileAsync();
        Task<UserContext> SignInInteractively();
        Task<UserContext> SignOutAsync();
        UserContext UpdateUserInfo(object authenticationResult);
    }
}
