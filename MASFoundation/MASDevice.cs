﻿using MASFoundation.Internal;
using MASFoundation.Internal.Data;
using System;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Security.Cryptography.Certificates;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace MASFoundation
{
    /// <summary>
    /// Local representation of device data.
    /// </summary>
    public class MASDevice
    {
        internal MASDevice(Configuration config)
        {
            _config = config;
            _storage = new SecureStorage();
            _certManager = new CertManager(_storage);
        }

        #region Public Properties

        /// <summary>
        /// The device the application is running on. This is a singleton object.
        /// </summary>
        public static MASDevice Current { get; private set; }

        /// <summary>
        /// Device identifier
        /// </summary>
        public string Id
        {
            get
            {
                return _deviceInfo.Id.ToString() + "1";
            }
        }

        /// <summary>
        /// Device name
        /// </summary>
        public string Name
        {
            get
            {
                return _deviceInfo.FriendlyName;
            }
        }

        /// <summary>
        /// Is the MASDevice registered.
        /// </summary>
        public bool IsRegistered
        {
            get
            {
                return MagId != null && Certificate != null;
            }
        }

        /// <summary>
        /// The MASDevice status.
        /// </summary>
        public string Status { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deregister the application resources on this device. This is a two step operation.
        /// 
        /// It will first attempt to remove the device's registered record in the cloud.  If it fails,
        /// an error is returned and the appropriate notification is sent and it will stop there.
        /// 
        /// Upon success of the first operation, deregistration in the cloud, it will then attempt to
        /// wipe the device of all credential settings.  If it fails, an error is returned and the appropriate
        /// notification is sent.It will stop here.
        /// </summary>
        public IAsyncAction UnregisterAsync()
        {
            return UnregisterInternalAsync().AsAsyncAction();
        }

        /// <summary>
        /// Logout the device from the server.This will revoke the id_token from the server and local by calling log out endpoint
        /// If clearLocal is defined as true, as part of log out process (revoking id_token),
        /// this method will also clear access_token, and refresh_token that are stored in local.
        /// </summary>
        /// <param name="clearLocal">Boolean to indicate to clear local access_token and refresh_token or not.</param>
        public IAsyncAction LogoutAsync(bool clearLocal)
        {
            return LogoutInternalAsync(clearLocal).AsAsyncAction();
        }

        #endregion

        #region Internal Methods

        internal static void Reset()
        {
            Current = null;
        }

        internal static async Task InitializeAsync(Configuration config)
        {
            Current = new MASDevice(config);
            await Current.LoadAsync();
        }

        #endregion

        #region Internal Properties

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

        #endregion

        #region Private Methods

        internal async Task RegisterWithClientAsync()
        {
            var username = "clientName";
            var csr = await _certManager.GenerateCSRAsync(_config, this, username);

            var response = await MAGRequests.RegisterDeviceAsync(_config, this, csr);

            await FinalizeRegistrationAsync(response, username);
        }

        internal async Task RegisterWithUserAsync(string username, string password)
        {
            var csr = await _certManager.GenerateCSRAsync(_config, this, username);

            var response = await MAGRequests.RegisterDeviceForUserAsync(_config, this, username, password, csr);

            await FinalizeRegistrationAsync(response, username);
        }

        async Task UnregisterInternalAsync()
        {
            if (!MASApplication.IsRegistered)
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

                await MASApplication.Current.ResetAsync();
            }
            catch (Exception e)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceCouldNotBeDeregistered, e);
            }
        }

        async Task LogoutInternalAsync(bool clearLocal)
        {
            if (!MASApplication.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            var currentUser = MASUser.Current;

            if (currentUser == null || !currentUser.IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserNotAuthenticated);
            }

            await currentUser.LogoutDeviceAsync(clearLocal);
        }

        async Task FinalizeRegistrationAsync(RegisterResponseData data, string username)
        {
            MagId = data.DeviceIdentifier;
            Status = data.DeviceStatus;

            if (data.Certificate != null)
            {
                await _certManager.InstallAsync(data.Certificate);
                Certificate = await _certManager.GetAsync();
                RegisteredUsername = Certificate.Subject;
            }

            JsonObject obj = new JsonObject();
            obj.SetNamedValue("magId", JsonValue.CreateStringValue(MagId));
            obj.SetNamedValue("status", JsonValue.CreateStringValue(Status));

            await _storage.SetAsync(StorageKeyNames.DeviceInfo, obj.Stringify());
        }

        async Task LoadAsync()
        {
            //var serverCert = _config.Server.ServerCerts[0];
            //await _certManager.InstallTrustedServerCert(serverCert);

            var clientInfo = await _storage.GetTextAsync(StorageKeyNames.ClientInfo);
            string clientId = null;
            string clientSecret = null;
            DateTime? expirationDate = null;
            if (clientInfo != null)
            {
                try
                {
                    var jsonObj = JsonObject.Parse(clientInfo);

                    clientId = jsonObj.GetNamedString("clientId");
                    clientSecret = jsonObj.GetNamedString("clientSecret");
                    expirationDate = DateTime.FromBinary((long)jsonObj.GetNamedNumber("clientExpiration"));
                }
                catch
                {
                    clientId = null;
                    clientSecret = null;
                    expirationDate = null;
                }
            }

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

                JsonObject obj = new JsonObject();
                obj.SetNamedValue("clientId", JsonValue.CreateStringValue(_clientId));
                obj.SetNamedValue("clientSecret", JsonValue.CreateStringValue(_clientSecret));
                obj.SetNamedValue("clientExpiration", JsonValue.CreateNumberValue(_clientExpiration.ToBinary()));

                await _storage.SetAsync(StorageKeyNames.ClientInfo, obj.Stringify());
            }
            else
            {
                _clientId = clientId;
                _clientSecret = clientSecret;
                _clientExpiration = expirationDate.Value;
            }

            var deviceInfo = await _storage.GetTextAsync(StorageKeyNames.DeviceInfo);
            if (deviceInfo != null)
            {
                try
                {
                    var jsonObj = JsonObject.Parse(deviceInfo);

                    MagId = jsonObj.GetNamedString("magId");
                    Status = jsonObj.GetNamedString("status");
                }
                catch
                {
                    MagId = null;
                    Status = null;
                }
            }
            else
            {
                MagId = null;
                Status = null;
            }

            // check if we have a certificate
            Certificate = await _certManager.GetAsync();

            if (Certificate == null || DateTime.Now > Certificate.ValidTo)
            {
                RegisteredUsername = null;
                Certificate = null;
                await SecureStorage.RemoveAsync(StorageKeyNames.DeviceInfo);
            }
            else
            {
                RegisteredUsername = Certificate.Subject;
            }
        }

        #endregion

        #region Fields

        string _clientId;
        string _clientSecret;
        DateTime _clientExpiration = DateTime.MinValue;
        Configuration _config;
        SecureStorage _storage;
        CertManager _certManager;
        EasClientDeviceInformation _deviceInfo = new EasClientDeviceInformation();

        #endregion
    }
}
