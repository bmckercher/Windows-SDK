using MASFoundation.Internal;
using MASFoundation.Internal.Data;
using System;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;

namespace MASFoundation
{
    /// <summary>
    /// Local representation of user data.
    /// </summary>
    public class MASUser
    {
        internal MASUser(Configuration config, MASDevice device)
        {

            _config = config;
            _device = device;
            _storage = new SecureStorage();
        }

        #region Public Properties

        /// <summary>
        ///  The authenticated user for the application, if any. Null returned if none.  This is a singleton object.
        /// </summary>
        public static MASUser Current { get; private set; }

        /// <summary>
        /// Is this user logged in.
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return _accessToken != null || _idToken != null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Logoff an already authenticated user via asynchronous request.
        /// </summary>
        public IAsyncAction LogoffAsync()
        {
            return LogoffInternalAsync().AsAsyncAction();
        }

        /// <summary>
        /// Authenticate a user via asynchronous request with basic credentials.
        /// 
        /// This will set MASUser.Current upon a successful result.  
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        public static IAsyncOperation<MASUser> LoginAsync(string username, string password)
        {
            return LoginInternalAsync(username, password).AsAsyncOperation<MASUser>();
        }

        /// <summary>
        /// Requesting user information for the MASUser object.
        /// This method will retrieve additional information on the MASUser object.
        /// </summary>
        /// <returns>User information</returns>
        public IAsyncOperation<IUserInfo> GetInfoAsync()
        {
            return GetInfoInternalAsync().AsAsyncOperation<IUserInfo>();
        }

        /// <summary>
        /// Checks if user has a valid access token
        /// </summary>
        /// <returns>Return true if user has an access token</returns>
        public IAsyncOperation<bool> CheckAccessAsync()
        {
            return CheckAccessInternalAsync().AsAsyncOperation<bool>();
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

        internal async Task LogoutDeviceAsync(bool clearLocal)
        {
            if (_idToken == null)
            {
                ErrorFactory.ThrowError(ErrorCode.DeviceNotLoggedIn);
            }

            await MAGRequests.LogoutSessionAsync(_config, MASDevice.Current, this, clearLocal);

            if (clearLocal)
            {
                await RemoveAccessTokensAsync();

                Current = null;
            }
            else
            {
                _idToken = null;
                _idTokenType = null;

                JsonObject obj = new JsonObject();
                obj.SetNamedValue("accessToken", JsonValue.CreateStringValue(_accessToken));
                obj.SetNamedValue("refreshToken", _refreshToken != null ? JsonValue.CreateStringValue(_refreshToken) : JsonValue.CreateNullValue());
                obj.SetNamedValue("accessTokenExpiration", JsonValue.CreateNumberValue(_expireTimeUtc.ToBinary()));
                obj.SetNamedValue("idToken", JsonValue.CreateNullValue());
                obj.SetNamedValue("idTokenType", JsonValue.CreateNullValue());

                await _storage.SetAsync(StorageKeyNames.AccessInfo, obj.Stringify());
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

        internal async Task LoginAnonymouslyAsync()
        {
            if (IsLoggedIn)
            {
                ErrorFactory.ThrowError(ErrorCode.UserAlreadyAuthenticated);
            }

            var data = await MAGRequests.RequestAccessTokenAnonymouslyAsync(_config, _device);
            await UpdateTokens(data);

            _isAnonymous = true;
        }

        internal async Task RefreshAccessTokenAsync()
        {
            // We have a refresh token, lets use that.
            if (_refreshToken != null)
            {
                var data = await MAGRequests.RefreshAccessTokenAsync(_config, _device, _refreshToken);
                await UpdateTokens(data);
            }
            // We have an id token, lets use that to request an access token.
            else if (_idToken != null && _idTokenType != null)
            {
                var data = await MAGRequests.RequestAccessTokenFromIdTokenAsync(_config, _device, _idToken, _idTokenType);
                await UpdateTokens(data);
            }
            // Lets check if we are anonymous, we can simply just login in again to get an access token.
            else if (_isAnonymous)
            {
                var data = await MAGRequests.RequestAccessTokenAnonymouslyAsync(_config, _device);
                await UpdateTokens(data);
            }
        }

        internal async Task<string> GetAccessTokenAsync()
        {
            bool needsRefresh = false;
            // If we have an id token but no access token, refresh access token.
            if (_idToken != null && _idTokenType != null && _accessToken == null)
            {
                needsRefresh = true;
            }
            // If we have an access token but it has expired, attempt refresh.
            else if (_accessToken != null && DateTime.UtcNow > _expireTimeUtc)
            {
                needsRefresh = true;
            }

            if (needsRefresh)
            {
                try
                {
                    await RefreshAccessTokenAsync();
                }
                catch (MASException exp)
                {
                    if (exp.MASErrorCode != ErrorCode.NetworkNotReachable)
                    {
                        // We failed to get our access token and it was not a network error, make sure we clear our access token 
                        // from memory and storage
                        await RemoveAccessTokensAsync();
                    }
                    else
                    {
                        // Rethrow error here
                        throw exp;
                    }
                }
            }

            if (_accessToken == null)
            {
                // Throw user not authenticated error
                if (_isAnonymous)
                {
                    ErrorFactory.ThrowError(ErrorCode.DeviceNotLoggedIn);
                }
                else
                {
                    ErrorFactory.ThrowError(ErrorCode.UserNotAuthenticated);
                }
            }

            return _accessToken;
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

        void ClearAccessTokens()
        {
            _accessToken = _refreshToken = _idToken = _idTokenType = null;
            _expireTimeUtc = DateTime.MinValue;
        }

        async Task RemoveAccessTokensAsync()
        {
            ClearAccessTokens();

            await SecureStorage.RemoveAsync(StorageKeyNames.AccessInfo);
        }

        async Task<bool> CheckAccessInternalAsync()
        {
            if (_accessToken != null)
            {
                if (DateTime.UtcNow > _expireTimeUtc)
                {
                    try
                    {
                        return await GetAccessTokenAsync() != null;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (_idToken != null && _idTokenType != null)
            {
                try
                {
                    return await GetAccessTokenAsync() != null;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        static async Task<MASUser> LoginInternalAsync(string username, string password)
        {
            if (!MASApplication.IsRegistered)
            {
                ErrorFactory.ThrowError(ErrorCode.ApplicationNotRegistered);
            }

            if (!MASDevice.Current.IsRegistered && MAS.RegistrationKind == RegistrationKind.User)
            {
                if (Current != null)
                {
                    await Current.RemoveAccessTokensAsync();
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
            if (!MASApplication.IsRegistered)
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

            try
            {
                return (IUserInfo)await MAGRequests.GetUserInfoAsync(_config, _device, this);
            }
            catch (MASException exp)
            {
                if (exp.MASErrorCode != ErrorCode.TokenAccessExpired)
                {
                    throw exp;
                }
            }

            await RefreshAccessTokenAsync();

            return (IUserInfo)await MAGRequests.GetUserInfoAsync(_config, _device, this);
        }

        async Task LogoffInternalAsync()
        {
            if (!MASApplication.IsRegistered)
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

            await RemoveAccessTokensAsync();
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
                    ClearAccessTokens();
                }
            }
            else
            {
                ClearAccessTokens();
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
        bool _isAnonymous;

        #endregion
    }
}
