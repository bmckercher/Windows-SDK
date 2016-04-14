using MASFoundation.Internal.Http;
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

        public async Task StartAsync(string fileName)
        {
            // Load configuration data
            _config = new Configuration();
            await _config.LoadAsync(fileName);

            Device = new Device(_config);
            await Device.InitializeAsync();

            // Attempt client device registration if scope allows
            if (!Device.IsRegistered && _config.HasScope(ScopeNames.MssoClientRegister))
            {
                await Device.RegisterWithClientAsync();
            }

            User = new User(_config, Device);
            await User.InitializeAsync();
        }

        public async Task StopAsync()
        {
            HttpRequester.CancelAll();
        }

        public async Task ResetAsync()
        {
            await SecureStorage.ResetAsync();

            await CertManager.UninstallAsync();

            HttpRequester.CancelAll();
        }

        public async Task LoginUserAsync(string username, string password)
        {
            if (User == null || Device == null)
            {
                // TODO throw session not started!
                return;
            }

            // We we do not have client registration flow set, do a device with user registration flow if needed
            if (!_config.HasScope(ScopeNames.MssoClientRegister) && (!Device.IsRegistered || Device.RegisteredUsername != username))
            {
                //Registering the Device with User flow
                await Device.RegisterWithUserAsync(username, password);
            }

            await User.LoginAsync(username, password);
        }

        public async Task LogoffUserAsync()
        {
            if (User == null || Device == null)
            {
                // TODO throw session not started!
                return;
            }

            await User.LogoffAsync();
        }

        public async Task UnregisterDevice()
        {
            if (Device == null)
            {
                // TODO throw session not started!
                return;
            }

            await Device.UnregisterAsync();

            // Remove all stored data since we unregistered the device
            await ResetAsync();
        }

        public Logger Log { get; private set; }

        public Device Device { get; private set; }

        public User User { get; private set; }

        Configuration _config;
    }
}
