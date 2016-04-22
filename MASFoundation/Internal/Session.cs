using MASFoundation.Internal.Http;
using System;
using System.Threading.Tasks;

namespace MASFoundation.Internal
{
    internal class Session
    {
        public Session()
        {
            Log = new Logger();
        }

        static Session _instance;
        public static Session Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Session();
                }

                return _instance;
            }
        }

        public bool IsRegistered
        {
            get
            {
                return User != null && Device != null && Device.IsRegistered;
            }
        }

        public event EventHandler<EventArgs> LoginRequested;

        public async Task StartAsync(string fileName, RegistrationKind regKind)
        {
            Log.Info("Framework starting...");

            _registrationKind = regKind;

            // Load configuration data
            _config = new Configuration();
            await _config.LoadAsync(fileName);

            // Load device and any previous registration info.
            Device = new Device(_config);
            await Device.InitializeAsync();

            // Authorization providers not supported in yet
            //var response = await MAGRequests.GetAuthorizationProviders(_config, Device);

            // load user and any previous access token or idtoken info
            User = new User(_config, Device);
            await User.InitializeAsync();

            if (!Device.IsRegistered)
            {
                switch (_registrationKind)
                {

                    case RegistrationKind.Client:
                        if (!_config.HasScope(ScopeNames.MssoClientRegister))
                        {
                            ErrorFactory.ThrowError(ErrorCode.DeviceRegistrationAttemptedWithUnregisteredScope);
                        }

                        await Device.RegisterWithClientAsync();
                        await User.LoginAsync();

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

            Log.Info("Framework started");
        }

        public async Task StopAsync()
        {
            Log.Info("Framework stopping...");

            HttpRequester.CancelAll();

            Log.Info("Framework stopped");
        }

        public async Task ResetAsync()
        {
            Log.Info("Framework reseting...");

            await SecureStorage.ResetAsync();

            await CertManager.UninstallAsync();

            HttpRequester.CancelAll();

            Log.Info("Framework reset");
        }

        public async Task LoginUserAsync(string username, string password)
        {
            Log.Info("Logging in user...");

            if (!Device.IsRegistered && _registrationKind == RegistrationKind.User)
            {
                Log.Info("User device registration starting...");
                await Device.RegisterWithUserAsync(username, password);
                Log.Info("User device registration complete");
            }

            await User.LoginAsync(username, password);

            Log.Info("User logged in");
        }

        public async Task LogoffUserAsync()
        {
            Log.Info("Logging out user...");

            if (!IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
                return;
            }

            await User.LogoffAsync();

            Log.Info("Logged out user.");
        }

        public async Task UnregisterDevice()
        {
            Log.Info("Deregistering device...");

            if (!IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
                return;
            }

            await Device.UnregisterAsync();

            // Remove all stored data since we unregistered the device
            await ResetAsync();

            Log.Info("Deregistered device");
        }

        public ILogger Log { get; set; }

        public Device Device { get; private set; }

        public User User { get; private set; }

        Configuration _config;
        RegistrationKind _registrationKind = RegistrationKind.Client;
    }
}
