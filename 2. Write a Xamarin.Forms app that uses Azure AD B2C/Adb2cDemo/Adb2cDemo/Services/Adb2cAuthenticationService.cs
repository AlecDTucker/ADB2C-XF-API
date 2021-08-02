using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adb2cDemo.Services;
using Adb2cDemo.Services.Interfaces;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

[assembly: Dependency(typeof(Adb2cAuthenticationService))]
namespace Adb2cDemo.Services
{
    public class Adb2cAuthenticationService :IAuthenticationService
    {
        private IPublicClientApplication publicClientApplication;

        #region Constructors & initialisation
        public Adb2cAuthenticationService()
        {
            applicationSettingsService = DependencyService.Get<IApplicationSettingsService>();

            // default redirectURI; each platform specific project will have to override it with its own
            PublicClientApplicationBuilder builder = PublicClientApplicationBuilder.Create(applicationSettingsService.ADB2CClientId)
                .WithB2CAuthority(applicationSettingsService.ADB2CAuthoritySignUpSignIn)
                .WithIosKeychainSecurityGroup(applicationSettingsService.ADB2CiOSKeychainGroup)
                .WithRedirectUri(applicationSettingsService.ADB2CRedirectUri);

            // Android implementation is based on https://github.com/jamesmontemagno/CurrentActivityPlugin
            // iOS implementation would require to expose the current ViewControler - not currently implemented as it is not required
            // UWP does not require this
            IParentWindowLocatorService windowLocatorService = DependencyService.Get<IParentWindowLocatorService>();

            if (windowLocatorService != null)
            {
                builder = builder.WithParentActivityOrWindow(() => windowLocatorService?.GetCurrentParentWindow());
            }

            publicClientApplication = builder.Build();
        }
        #endregion

        #region Services
        private readonly IApplicationSettingsService applicationSettingsService;
        #endregion

        #region Properties
        public string ResetErrorCode => "AADB2C90118";
        #endregion

        #region Methods
        public async Task<UserContext> AcquireTokenSilent()
        {
            IEnumerable<IAccount> accounts = await publicClientApplication.GetAccountsAsync();

            AuthenticationResult authResult = await publicClientApplication.AcquireTokenSilent(
                applicationSettingsService.ADB2CScopes,
                GetAccountByPolicy(accounts, applicationSettingsService.ADB2CPolicySignUpSignIn))
               .WithB2CAuthority(applicationSettingsService.ADB2CAuthoritySignUpSignIn)
               .ExecuteAsync();

            UserContext newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        public async Task<UserContext> EditProfileAsync()
        {
            IEnumerable<IAccount> accounts = await publicClientApplication.GetAccountsAsync();

            AuthenticationResult authResult = await publicClientApplication.AcquireTokenInteractive(applicationSettingsService.ADB2CScopes)
                .WithAccount(GetAccountByPolicy(accounts, applicationSettingsService.ADB2CPolicyEditProfile))
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(applicationSettingsService.ADB2CAuthorityEditProfile)
                .ExecuteAsync();

            UserContext userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        public async Task<UserContext> ResetPasswordAsync()
        {
            AuthenticationResult authResult = await publicClientApplication.AcquireTokenInteractive(applicationSettingsService.ADB2CScopes)
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(applicationSettingsService.ADB2CAuthorityResetPassword)
                .ExecuteAsync();

            UserContext userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        public async Task<UserContext> SignInAsync()
        {
            UserContext newContext;
            try
            {
                // acquire token silent
                newContext = await AcquireTokenSilent();
            }
            catch (MsalUiRequiredException)
            {
                // acquire token interactive
                newContext = await SignInInteractively();
            }
            return newContext;
        }

        public async Task<UserContext> SignInInteractively()
        {
            AuthenticationResult authResult = await publicClientApplication.AcquireTokenInteractive(applicationSettingsService.ADB2CScopes)
                .ExecuteAsync();

            UserContext newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        public async Task<UserContext> SignOutAsync()
        {
            IEnumerable<IAccount> accounts = await publicClientApplication.GetAccountsAsync();
            while (accounts.Any())
            {
                await publicClientApplication.RemoveAsync(accounts.FirstOrDefault());
                accounts = await publicClientApplication.GetAccountsAsync();
            }
            UserContext signedOutContext = new UserContext();
            signedOutContext.IsLoggedOn = false;
            return signedOutContext;
        }

        public UserContext UpdateUserInfo(object authenticationResult)
        {
            UserContext newContext = new UserContext();

            if (authenticationResult is AuthenticationResult ar)
            {
                newContext.IsLoggedOn = false;
                JObject user = ParseIdToken(ar.IdToken);

                newContext.AccessToken = ar.AccessToken;
                newContext.Name = user["name"]?.ToString();
                newContext.UserIdentifier = user["oid"]?.ToString();

                newContext.GivenName = user["given_name"]?.ToString();
                newContext.FamilyName = user["family_name"]?.ToString();

                newContext.StreetAddress = user["streetAddress"]?.ToString();
                newContext.City = user["city"]?.ToString();
                newContext.Province = user["state"]?.ToString();
                newContext.PostalCode = user["postalCode"]?.ToString();
                newContext.Country = user["country"]?.ToString();

                newContext.JobTitle = user["jobTitle"]?.ToString();

                JArray emails = user["emails"] as JArray;
                if (emails != null)
                {
                    newContext.EmailAddress = emails[0].ToString();
                }
                newContext.IsLoggedOn = true;
            }

            return newContext;
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            return accounts.FirstOrDefault(a =>
                a.HomeAccountId.ObjectId.Split('.')[0]
                .EndsWith(policy.ToLower()));
        }

        private JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            byte[] byteArray = Convert.FromBase64String(s);
            string decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }
        #endregion
    }
}
