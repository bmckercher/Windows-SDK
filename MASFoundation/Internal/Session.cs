using MASFoundation.Internal.Http;
using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;

namespace MASFoundation.Internal
{
    internal class Session
    {
        public bool IsRegistered
        {
            get
            {
                return _user != null && _device != null && _device.IsRegistered;
            }
        }

        public string MagId
        {
            get
            {
                return _device?.MagId;
            }
        }

        public Certificate Certificate
        {
            get
            {
                return _device?.Certificate;
            }
        }

        public Task<string> GetAccessHeaderValueAsync()
        {
            return _user.GetAccessHeaderValueAsync();
        }

        public event EventHandler<EventArgs> LoginRequested;

        public async Task StartAsync(string fileName, RegistrationKind regKind)
        {
            Logger.LogInfo("Framework starting...");

            _registrationKind = regKind;

            // Load configuration data
            _config = new Configuration();
            await _config.LoadAsync(fileName);

            _storage = new SecureStorage(_config);

            _certManager = new CertManager(_storage);

            // Load device and any previous registration info.
            _device = new Device(_config, _storage);
            await _device.InitializeAsync();

            // Authorization providers not supported in yet
            //var response = await MAGRequests.GetAuthorizationProviders(_config, Device);

            // load user and any previous access token or idtoken info
            _user = new User(_config, _device, _storage);
            await _user.InitializeAsync();

            if (!_device.IsRegistered)
            {
                switch (_registrationKind)
                {

                    case RegistrationKind.Client:
                        if (!_config.HasScope(ScopeNames.MssoClientRegister))
                        {
                            ErrorFactory.ThrowError(ErrorCode.DeviceRegistrationAttemptedWithUnregisteredScope);
                        }

                        await _device.RegisterWithClientAsync();
                        await _user.LoginAsync();

                        break;
                    case RegistrationKind.User:
                        // Ask for login with user device registration
                        LoginRequested?.Invoke(null, EventArgs.Empty);
                        break;
                    default:
                        ErrorFactory.ThrowError(ErrorCode.DeviceRegistrationAttemptedWithUnregisteredScope);
                        break;

                }
            }

            Logger.LogInfo("Framework started");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task StopAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Logger.LogInfo("Framework stopping...");

            HttpRequestFactory.CancelAll();

            Logger.LogInfo("Framework stopped");
        }

        public async Task ResetAsync()
        {
            Logger.LogInfo("Framework reseting...");

            await SecureStorage.ResetAsync();

            await CertManager.UninstallAsync();

            HttpRequestFactory.CancelAll();

            Logger.LogInfo("Framework reset");
        }

        public async Task LoginUserAsync(string username, string password)
        {
            Logger.LogInfo("Logging in user...");

            if (!_device.IsRegistered && _registrationKind == RegistrationKind.User)
            {
                Logger.LogInfo("User device registration starting...");
                await _device.RegisterWithUserAsync(username, password);
                Logger.LogInfo("User device registration complete");
            }

            await _user.LoginAsync(username, password);

            Logger.LogInfo("User logged in");
        }

        public async Task LogoffUserAsync()
        {
            Logger.LogInfo("Logging out user...");

            if (!IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
                return;
            }

            await _user.LogoffAsync();

            Logger.LogInfo("Logged out user.");
        }

        public async Task LogoutDeviceAsync()
        {
            Logger.LogInfo("Logout device...");

            if (!IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
                return;
            }

            await _device.UnregisterAsync();

            // Remove all stored data since we unregistered the device
            await ResetAsync();

            Logger.LogInfo("Logout device");
        }

        Configuration _config;
        SecureStorage _storage;
        Device _device;
        User _user;
        RegistrationKind _registrationKind = RegistrationKind.Client;
        CertManager _certManager;
    }
}
