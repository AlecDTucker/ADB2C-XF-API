using System;
using Adb2cDemo.Services;
using Adb2cDemo.Services.Interfaces;
using Xamarin.Forms;

[assembly:Dependency(typeof(ApplicationSettingsService))]
namespace Adb2cDemo.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        public ApplicationSettingsService()
        {
        }

        #region Properties
        public string ApiBaseUrl => "ApiBaseUrl";

        public string ADB2CTenantName => "blogdemoorg.onmicrosoft.com";
        public string ADB2CHostName => "blogdemoorg.b2clogin.com";
        public string ADB2CClientId => "85xxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxx76";
        public string ADB2CPolicySignUpSignIn => "B2C_1_BlogDemoSusi";
        public string ADB2CPolicyEditProfile => "ADB2C.PolicyEditProfile";
        public string ADB2CPolicyResetPassword => "ADB2C.PolicyResetPassword";
        public string ADB2CRedirectUri => "blogdemo://auth";
        public string[] ADB2CScopes => new string[] { "openid", "offline_access" };
        public string ADB2CAuthorityBase => $"https://{ADB2CHostName}/tfp/{ADB2CTenantName}/";
        public string ADB2CAuthoritySignUpSignIn => $"{ADB2CAuthorityBase}{ADB2CPolicySignUpSignIn}";
        public string ADB2CAuthorityEditProfile => $"{ADB2CAuthorityBase}{ADB2CPolicyEditProfile}";
        public string ADB2CAuthorityResetPassword => $"{ADB2CAuthorityBase}{ADB2CPolicyResetPassword}";
        public string ADB2CiOSKeychainGroup => "com.obq.adb2cdemo";
        #endregion
    }
}
