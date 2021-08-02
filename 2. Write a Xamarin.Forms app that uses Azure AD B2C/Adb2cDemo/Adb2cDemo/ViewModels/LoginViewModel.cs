using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Adb2cDemo.Services.Interfaces;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace Adb2cDemo.ViewModels
{
    public class LoginViewModel
    {
        #region Constructors & initialisation
        public LoginViewModel()
        {
            authenticationService = DependencyService.Get<IAuthenticationService>();
        }
        #endregion

        #region Services
        private readonly IAuthenticationService authenticationService;
        #endregion

        #region Commands
        private ICommand _loginCommand = null;
        public ICommand LoginCommand => _loginCommand = _loginCommand ?? new Command(async () => await DoLoginCommand());

        private ICommand _logoutCommand = null;
        public ICommand LogoutCommand => _logoutCommand = _logoutCommand ?? new Command(async () => await DoLogoutCommand());
        #endregion

        #region Methods
        private async Task DoLoginCommand()
        {
            try
            {
                UserContext userContext = await authenticationService.SignInAsync();
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // Should ONLY be done for B2C reset and not any other error.
                if (ex.Message.Contains(authenticationService.ResetErrorCode))
                {
                    OnPasswordReset();
                }
                // Alert if any exception excluding user canceling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                {
                    await Application.Current.MainPage.DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }
        }

        private async Task DoLogoutCommand()
        {
            try
            {
                UserContext userContext = await authenticationService.SignOutAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception, for example:
                await Application.Current.MainPage.DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        private async void OnPasswordReset()
        {
            try
            {
                UserContext userContext = await authenticationService.ResetPasswordAsync();
                //TODO: update local information in secure storage
            }
            catch (Exception ex)
            {
                // Alert if any exception excluding user canceling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                {
                    // Handle the exception, for example:
                    await Application.Current.MainPage.DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }
        }
        #endregion
    }
}
