using MASFoundation.Internal;
using MASFoundation.Internal.Data;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MASFoundation
{
    public class User
    {
        internal User(Configuration config, Device device)
        {

            _config = config;
            _device = device;
            _storage = new SecureStorage();
        }

        public static User Current { get; private set; }

        public static IAsyncOperation<User> LoginAsync(string username, string password)
        {
            return LoginInternalAsync(username, password).AsAsyncOperation<User>();
        }

        static async Task<User> LoginInternalAsync(string username, string password)
        {
            if (!Device.Current.IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!Device.Current.IsRegistered && MAS.RegistrationKind == RegistrationKind.User)
            {
                if (Current != null)
                {
                    await Current.RemoveCacheAsync();
                }

                await Device.Current.RegisterWithUserAsync(username, password);
            }

            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
            }

            if (Current != null && Current.IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserAlreadyAuthenticated);
            }

            if (!Device.Current.IsRegistered)
            {
                if (MAS.RegistrationKind == RegistrationKind.User)
                {
                    Logger.LogInfo("User device registration starting...");
                    await Device.Current.RegisterWithUserAsync(username, password);
                    Logger.LogInfo("User device registration complete");
                }
                else
                {
                    ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
                }
            }

            var user = new User(Application.Current.Config, Device.Current);
            await user.RequestAccessTokenAsync(username, password);

            Current = user;

            return user;
        }

        public IAsyncOperation<IUserInfo> GetInfoAsync()
        {
            return GetInfoInternalAsync().AsAsyncOperation<IUserInfo>();
        }

        async Task<IUserInfo> GetInfoInternalAsync()
        {
            if (!Device.Current.IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!Device.Current.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceNotRegistered);
            }

            if (!IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserNotAuthenticated);
            }

            return (IUserInfo)await MAGRequests.GetUserInfoAsync(_config, _device, this);
        }

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

        async Task LogoffInternalAsync()
        {
            if (!Device.Current.IsApplicationRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!Device.Current.IsRegistered)
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

        #region Internal Methods

        internal static void Reset()
        {
            Current = null;
        }

        internal static async Task InitializeAsync(Configuration config, Device device)
        {
            var previousUser = new User(config, device);
            await previousUser.LoadAsync();
            if (previousUser.IsLoggedIn)
            {
                User.Current = previousUser;
            }
        }

        internal static async Task LoginAnonymouslyAsync(Configuration config, Device device)
        {
            var previousUser = new User(config, device);
            await previousUser.LoginAsync();
            if (previousUser.IsLoggedIn)
            {
                User.Current = previousUser;
            }
        }

        internal async Task LogoutDeviceAsync(bool clearLocal)
        {
            await MAGRequests.LogoutSessionAsync(_config, Device.Current, this, clearLocal);

            if (clearLocal)
            {
                await RemoveCacheAsync();

                Current = null;
            }
        }

        private async Task LoadAsync()
        {
            _accessToken = await _storage.GetTextAsync(StorageKeyNames.AccessToken);
            _refreshToken = await _storage.GetTextAsync(StorageKeyNames.RefreshToken);

            var expireTime = await _storage.GetDateAsync(StorageKeyNames.AccessTokenExpiration);
            if (expireTime != null)
            {
                _expireTimeUtc = expireTime.Value;
            }
            else
            {
                _expireTimeUtc = DateTime.MinValue;
            }

            _idToken = await _storage.GetTextAsync(StorageKeyNames.IdToken);
            _idTokenType = await _storage.GetTextAsync(StorageKeyNames.IdTokenType);
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

            await SecureStorage.RemoveAsync(StorageKeyNames.AccessToken);
            await SecureStorage.RemoveAsync(StorageKeyNames.RefreshToken);
            await SecureStorage.RemoveAsync(StorageKeyNames.AccessTokenExpiration);

            await SecureStorage.RemoveAsync(StorageKeyNames.IdToken);
            await SecureStorage.RemoveAsync(StorageKeyNames.IdTokenType);
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

        async Task UpdateTokens(RequestTokenResponseData data)
        {
            _accessToken = data.AccessToken;
            _refreshToken = data.RefreshToken;
            _expireTimeUtc = DateTime.UtcNow.AddSeconds(data.ExpiresIn);

            await _storage.SetAsync(StorageKeyNames.AccessToken, _accessToken);
            await _storage.SetAsync(StorageKeyNames.AccessTokenExpiration, _expireTimeUtc);

            if (_refreshToken != null)
            {
                await _storage.SetAsync(StorageKeyNames.RefreshToken, _refreshToken);
            }

            if (data.IdToken != null && data.IdTokenType != null)
            {
                _idToken = data.IdToken;
                _idTokenType = data.IdTokenType;

                await _storage.SetAsync(StorageKeyNames.IdToken, _idToken);
                await _storage.SetAsync(StorageKeyNames.IdTokenType, _idTokenType);
            }
        }

        string _accessToken;
        string _refreshToken;
        DateTime _expireTimeUtc;
        string _idToken;
        string _idTokenType;
        Configuration _config;
        Device _device;
        SecureStorage _storage;
    }
}
