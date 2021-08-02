namespace Adb2cDemo.Services.Interfaces
{
    public interface IApplicationSettingsService
    {
        string ApiBaseUrl { get; }

        string ADB2CTenantName { get; }
        string ADB2CHostName { get; }
        string ADB2CClientId { get; }
        string ADB2CPolicySignUpSignIn { get; }
        string ADB2CPolicyEditProfile { get; }
        string ADB2CPolicyResetPassword { get; }
        string ADB2CRedirectUri { get; }
        string[] ADB2CScopes { get; }
        string ADB2CAuthorityBase { get; }
        string ADB2CAuthoritySignUpSignIn { get; }
        string ADB2CAuthorityEditProfile { get; }
        string ADB2CAuthorityResetPassword { get; }
        string ADB2CiOSKeychainGroup { get; }
    }
}