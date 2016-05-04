using MASFoundation.Internal;
using MASFoundation.Internal.Data;
using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace MASFoundation
{
    public class Device
    {
        internal Device(Configuration config)
        {
            _config = config;
            _storage = new SecureStorage(config);
            _certManager = new CertManager(_storage);
        }

        public static Device Current { get; private set; }

        public string Id
        {
            get
            {
                return _deviceInfo.Id.ToString() + "1";
            }
        }

        public string Name
        {
            get
            {
                return _deviceInfo.FriendlyName;
            }
        }
    
        public bool IsRegistered
        {
            get
            {
                return MagId != null && Certificate != null;
            }
        }

        public bool IsApplicationRegistered
        {
            get
            {
                return ClientId != null && ClientSecret != null;
            }
        }

        public async Task RegisterWithClientAsync()
        {
            var username = "clientName";
            var csr = await _certManager.GenerateCSRAsync(_config, this, username);

            var response = await MAGRequests.RegisterDeviceAsync(_config, this, csr);

            await FinalizeRegistrationAsync(response, username);
        }

        public async Task RegisterWithUserAsync(string username, string password)
        {
            var csr = await _certManager.GenerateCSRAsync(_config, this, username);

            var response = await MAGRequests.RegisterDeviceForUserAsync(_config, this, username, password, csr);

            await FinalizeRegistrationAsync(response, username);
        }

        public async Task UnregisterAsync()
        {

            if (!IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            try
            {
                await MAGRequests.UnregisterDevice(_config, this);

                if (Certificate != null)
                {
                    await CertManager.UninstallAsync();
                    Certificate = null;
                    RegisteredUsername = null;
                }
            }
            catch (Exception e)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceCouldNotBeDeregistered, e);
            }
        }

        public async Task LogoutAsync(bool clearLocal)
        {
            if (!IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            var currentUser = User.Current;

            if (currentUser == null || !currentUser.IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserNotAuthenticated);
            }

            await currentUser.LogoutDeviceAsync(clearLocal);
        }

        internal string AuthHeaderValue
        {
            get
            {
                if (_clientId != null && _clientSecret != null)
                {
                    var clientUsernamePass = string.Format("{0}:{1}", _clientId, _clientSecret);
                    return "Basic " + clientUsernamePass.ToBase64();
                }

                return null;
            }
        }

        internal string MagId { get; private set; }

        internal Certificate Certificate { get; private set; }

        internal string RegisteredUsername { get; private set; }

        internal string ClientId { get { return _clientId; } }

        internal string ClientSecret { get { return _clientSecret; } }

        internal static async Task InitializeAsync(Configuration config)
        {
            Current = new Device(config);
            await Current.LoadAsync();
        }

        async Task FinalizeRegistrationAsync(RegisterResponseData data, string username)
        {
            MagId = data.DeviceIdentifier;

            await _certManager.InstallAsync(data.Certificate);

            Certificate = await _certManager.GetAsync();

            RegisteredUsername = Certificate.Subject;
            await _storage.SetAsync(StorageKeyNames.MagDeviceId, true, MagId);
        }

        async Task LoadAsync()
        {
            var serverCert = _config.Server.ServerCerts[0];
            await _certManager.InstallTrustedServerCert(serverCert);

            var clientId = await _storage.GetTextAsync(StorageKeyNames.ClientId);
            var clientSecret = await _storage.GetTextAsync(StorageKeyNames.ClientSecret);
            var expirationDate = await _storage.GetDateAsync(StorageKeyNames.ClientExpiration);

            if (clientId == null || clientSecret == null || expirationDate == null || DateTime.UtcNow >= expirationDate.Value)
            {
                var clientCredResponse = await MAGRequests.GetClientCredentialsAsync(_config, Id.ToBase64());
                _clientId = clientCredResponse.ClientId;
                _clientSecret = clientCredResponse.ClientSecret;

                if (clientCredResponse.Expiration == 0) // 0 means never expire
                {
                    _clientExpiration = DateTime.MaxValue;
                }
                else
                {
                    _clientExpiration = clientCredResponse.Expiration.FromUnixTime();
                }

                await _storage.SetAsync(StorageKeyNames.ClientId, false, _clientId);
                await _storage.SetAsync(StorageKeyNames.ClientSecret, false, _clientSecret);
                await _storage.SetAsync(StorageKeyNames.ClientExpiration, false, _clientExpiration);
            }
            else
            {
                _clientId = clientId;
                _clientSecret = clientSecret;
                _clientExpiration = expirationDate.Value;
            }

            // Check if we have magId
            MagId = await _storage.GetTextAsync(StorageKeyNames.MagDeviceId);

            // check if we have a certificate
            Certificate = await _certManager.GetAsync();

            if (Certificate == null || DateTime.Now > Certificate.ValidTo)
            {
                await SecureStorage.RemoveAsync(StorageKeyNames.MagDeviceId);
            }
            else
            {
                RegisteredUsername = Certificate.Subject;
            }
        }

        string _clientId;
        string _clientSecret;
        DateTime _clientExpiration = DateTime.MinValue;
        Configuration _config;
        SecureStorage _storage;
        CertManager _certManager;
        EasClientDeviceInformation _deviceInfo = new EasClientDeviceInformation();
    }
}
