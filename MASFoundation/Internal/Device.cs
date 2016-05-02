using MASFoundation.Internal.Data;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace MASFoundation.Internal
{
    internal class Device
    {
        public Device(Configuration config, SecureStorage storage)
        {
            _config = config;
            _storage = storage;
            _certManager = new CertManager(storage);
        }

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
    
        public string AuthHeaderValue
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

        public string MagId { get; private set; }

        public Certificate Certificate { get; private set; }

        public bool IsRegistered { get { return MagId != null && Certificate != null; } }

        public string RegisteredUsername { get; private set; }

        public string ClientId { get { return _clientId; } }

        public string ClientSecret { get { return _clientSecret; } }

        public async Task InitializeAsync()
        {
            var serverCert = _config.Server.ServerCerts[0];
            await _certManager.InstallTrustedServerCert(serverCert);           

            var clientId = await _storage.GetTextAsync("clientId");
            var clientSecret = await _storage.GetTextAsync("clientSecret");
            var expirationDate = await _storage.GetDateAsync("clientExpiration");

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

                await _storage.SetAsync("clientId", false, _clientId);
                await _storage.SetAsync("clientSecret", false, _clientSecret);
                await _storage.SetAsync("clientExpiration", false, _clientExpiration);
            }
            else
            {
                _clientId = clientId;
                _clientSecret = clientSecret;
                _clientExpiration = expirationDate.Value;
            }

            // Check if we have magId
            MagId = await _storage.GetTextAsync("magDeviceId");

            // check if we have a certificate
            Certificate = await _certManager.GetAsync();

            if (Certificate == null || DateTime.Now > Certificate.ValidTo)
            {
                await SecureStorage.RemoveAsync("magDeviceId");
            }
            else
            {
                RegisteredUsername = Certificate.Subject;
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

        async Task FinalizeRegistrationAsync(RegisterResponseData data, string username)
        {
            MagId = data.DeviceIdentifier;

            await _certManager.InstallAsync(data.Certificate);

            Certificate = await _certManager.GetAsync();

            RegisteredUsername = Certificate.Subject;
            await _storage.SetAsync("magDeviceId", true, MagId);
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
