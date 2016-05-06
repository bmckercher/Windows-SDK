using MASFoundation.Internal;
using System;
using System.Threading.Tasks;

namespace MASFoundation
{
    public sealed class Application
    {
        static Application _current;
        public static Application Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new Application();
                }

                return _current;
            }
        }

        // TODO expose application properties as in iOS MASApplication code.

        internal event EventHandler<EventArgs> LoginRequested;

        internal async Task StartAsync(string configContent, RegistrationKind regKind)
        {
            Logger.LogInfo("Framework starting...");

            // Load and validate configuration data
            var config = new Configuration();
            config.Load(configContent);
            _config = config;

            // Load device and any previous registration info.
            await Device.InitializeAsync(config);

            // Authorization providers not supported yet
            //var response = await MAGRequests.GetAuthorizationProviders(_config, Device);

            // Load user and any previous access token or idtoken info
            await User.InitializeAsync(config, Device.Current);

            if (!Device.Current.IsRegistered)
            {
                switch (regKind)
                {
                    case RegistrationKind.Client:
                        if (!config.HasScope(ScopeNames.MssoClientRegister))
                        {
                            ErrorFactory.ThrowError(ErrorCode.DeviceRegistrationAttemptedWithUnregisteredScope);
                        }

                        await Device.Current.RegisterWithClientAsync();

                        // Create a device anonymous user
                        await User.LoginAnonymouslyAsync(config, Device.Current);

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

        internal async Task ResetAsync()
        {
            Logger.LogInfo("Framework reseting...");

            Device.Reset();

            User.Reset();

            await SecureStorage.ResetAsync();

            await CertManager.UninstallAsync();

            HttpRequestFactory.CancelAll();

            Logger.LogInfo("Framework reset");
        }

        internal Configuration Config
        {
            get
            {
                return _config;
            }
        }

        Configuration _config;
    }
}
