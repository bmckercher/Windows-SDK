using MASFoundation.Internal;
using MASFoundation.Internal.Data;
using System;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;

namespace MASFoundation
{
    public class MASUser
    {
        internal MASUser(Configuration config, MASDevice device)
        {

            _config = config;
            _device = device;
            _storage = new SecureStorage();
        }

        #region Public Properties

        public static MASUser Current { get; private set; }

        public bool IsLoggedIn
        {
            get
            {
                return _accessToken != null || _idToken != null;
            }
        }

        public IAsyncAction LogoffAsync()
        {
            return LogoffInternalAsync().AsAsyncAction();
        }

        #endregion

        #region Public Methods

        public static IAsyncOperation<MASUser> LoginAsync(string username, string password)
        {
            return LoginInternalAsync(username, password).AsAsyncOperation<MASUser>();
        }

        public IAsyncOperation<IUserInfo> GetInfoAsync()
        {
            return GetInfoInternalAsync().AsAsyncOperation<IUserInfo>();
        }

        #endregion

        #region Internal Methods

        internal static void Reset()
        {
            Current = null;
        }

        internal static async Task InitializeAsync(Configuration config, MASDevice device)
        {
            var previousUser = new MASUser(config, device);
            await previousUser.LoadAsync();
            if (previousUser.IsLoggedIn)
            {
                MASUser.Current = previousUser;
            }
        }

        internal static async Task LoginAnonymouslyAsync(Configuration config, MASDevice device)
        {
            var previousUser = new MASUser(config, device);
            await previousUser.LoginAsync();
            if (previousUser.IsLoggedIn)
            {
                MASUser.Current = previousUser;
            }
        }

        internal async Task LogoutDeviceAsync(bool clearLocal)
        {
            await MAGRequests.LogoutSessionAsync(_config, MASDevice.Current, this, clearLocal);

            if (clearLocal)
            {
                await RemoveCacheAsync();

                Current = null;
            }
        }
        
        internal async Task RequestAccessTokenAsync(string username, string password)
        {
            if (IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserAlreadyAuthenticated);
            }

            var data = await MAGRequests.RequestAccessTokenAsync(_config, _device, username, password);
            await UpdateTokens(data);
        }

        internal async Task LoginAsync()
        {
            if (IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserAlreadyAuthenticated);
            }

            var data = await MAGRequests.RequestAccessTokenAnonymouslyAsync(_config, _device);
            await UpdateTokens(data);
        }

        internal async Task RemoveCacheAsync()
        {
            _accessToken = _refreshToken = _idToken = _idTokenType = null;
            _expireTimeUtc = DateTime.MinValue;

            await SecureStorage.RemoveAsync(StorageKeyNames.AccessInfo);
        }

        internal async Task<string> GetAccessTokenAsync()
        {
            // We have an id token but not an access token, try to get an access token.
            if (_idToken != null && _idTokenType != null && _accessToken == null)
            {
                var data = await MAGRequests.RequestAccessTokenFromIdTokenAsync(_config, _device, _idToken, _idTokenType);
                await UpdateTokens(data);
            }

            if (_accessToken != null)
            {
                if (_refreshToken != null && DateTime.UtcNow > _expireTimeUtc)
                {
                    var data = await MAGRequests.RefreshAccessTokenAsync(_config, _device, _refreshToken);
                    await UpdateTokens(data);
                }

                return _accessToken;
            }

            return null;
        }

        internal async Task<string> GetAccessHeaderValueAsync()
        {
            var token = await GetAccessTokenAsync();

            if (token != null)
            {
                return string.Format("Bearer {0}", token);
            }

            return null;
        }

        #endregion

        #region Private Methods

        static async Task<MASUser> LoginInternalAsync(string username, string password)
        {
            if (!MASDevice.Current.IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!MASDevice.Current.IsRegistered && MAS.RegistrationKind == RegistrationKind.User)
            {
                if (Current != null)
                {
                    await Current.RemoveCacheAsync();
                }

                await MASDevice.Current.RegisterWithUserAsync(username, password);
            }

            if (!MASDevice.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
            }

            if (Current != null && Current.IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserAlreadyAuthenticated);
            }

            if (!MASDevice.Current.IsRegistered)
            {
                if (MAS.RegistrationKind == RegistrationKind.User)
                {
                    Logger.LogInfo("User device registration starting...");
                    await MASDevice.Current.RegisterWithUserAsync(username, password);
                    Logger.LogInfo("User device registration complete");
                }
                else
                {
                    ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
                }
            }

            var user = new MASUser(MASApplication.Current.Config, MASDevice.Current);
            await user.RequestAccessTokenAsync(username, password);

            Current = user;

            return user;
        }

        async Task<IUserInfo> GetInfoInternalAsync()
        {
            if (!MASDevice.Current.IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!MASDevice.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
            }

            if (!IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserNotAuthenticated);
            }

            return (IUserInfo)await MAGRequests.GetUserInfoAsync(_config, _device, this);
        }

        async Task LogoffInternalAsync()
        {
            if (!MASDevice.Current.IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!MASDevice.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
            }

            if (Current == null || !Current.IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserNotAuthenticated);
            }

            await MAGRequests.RevokeAccessTokenAsync(_config, _device, this);

            await RemoveCacheAsync();
        }

        async Task LoadAsync()
        {
            var accessInfo = await _storage.GetTextAsync(StorageKeyNames.AccessInfo);
            if (accessInfo != null)
            {
                try
                {
                    var jsonObj = JsonObject.Parse(accessInfo);

                    _accessToken = jsonObj.GetNamedString("accessToken");
                    _refreshToken = jsonObj.GetStringOrNull("refreshToken");
                    _expireTimeUtc = DateTime.FromBinary((long)jsonObj.GetNamedNumber("accessTokenExpiration"));

                    _idToken = jsonObj.GetStringOrNull("idToken");
                    _idTokenType = jsonObj.GetStringOrNull("idTokenType");
                }
                catch
                {
                    _accessToken = null;
                    _refreshToken = null;
                    _expireTimeUtc = DateTime.MinValue;

                    _idToken = null;
                    _idTokenType = null;
                }
            }
            else
            {
                _accessToken = null;
                _refreshToken = null;
                _expireTimeUtc = DateTime.MinValue;

                _idToken = null;
                _idTokenType = null;
            }
        }

        async Task UpdateTokens(RequestTokenResponseData data)
        {
            _accessToken = data.AccessToken;
            _refreshToken = data.RefreshToken;
            _expireTimeUtc = DateTime.UtcNow.AddSeconds(data.ExpiresIn);
            
            _idToken = data.IdToken;
            _idTokenType = data.IdTokenType;

            JsonObject obj = new JsonObject();
            obj.SetNamedValue("accessToken", JsonValue.CreateStringValue(_accessToken));
            obj.SetNamedValue("refreshToken", _refreshToken != null ? JsonValue.CreateStringValue(_refreshToken) : JsonValue.CreateNullValue());
            obj.SetNamedValue("accessTokenExpiration", JsonValue.CreateNumberValue(_expireTimeUtc.ToBinary()));
            obj.SetNamedValue("idToken", _idToken != null ? JsonValue.CreateStringValue(_idToken) : JsonValue.CreateNullValue());
            obj.SetNamedValue("idTokenType", _idTokenType != null ? JsonValue.CreateStringValue(_idTokenType) : JsonValue.CreateNullValue());

            await _storage.SetAsync(StorageKeyNames.AccessInfo, obj.Stringify());
        }

        #endregion

        #region Fields

        string _accessToken;
        string _refreshToken;
        DateTime _expireTimeUtc;
        string _idToken;
        string _idTokenType;
        Configuration _config;
        MASDevice _device;
        SecureStorage _storage;

        #endregion
    }
}
