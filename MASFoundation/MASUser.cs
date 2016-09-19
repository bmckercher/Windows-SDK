/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation.Internal;
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
            _sharedStorage = new SharedSecureStorage();
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

        internal static async Task<MASUser> InitializeAsync(Configuration config, MASDevice device, bool isAnonymous)
        {
            var user = new MASUser(config, device);
            user._isAnonymous = isAnonymous;

            await user.LoadAsync();

            if (!isAnonymous && user.IsLoggedIn)
            {
                MASUser.Current = user;
            }

            return user;
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

            _isAnonymous = true;

            var data = await MAGRequests.RequestAccessTokenAnonymouslyAsync(_config, _device);
            await UpdateTokens(data);

            _isAnonymous = true;
        }

        internal async Task RefreshAccessTokenAsync()
        {
            // We have a refresh token, lets use that.
            if (_refreshToken != null)
            {
                RequestTokenResponseData data = null;
                try
                {
                    data = await MAGRequests.RefreshAccessTokenAsync(_config, _device, _refreshToken);
                }
                catch
                {
                    // TODO If this fails try to use idToken to get access token
                    if (_idToken != null && _idTokenType != null)
                    {
                        data = await MAGRequests.RequestAccessTokenFromIdTokenAsync(_config, _device, _idToken, _idTokenType);
                    }
                }

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
                    // TODO Double check the error here for client registration flow
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

        #region Internal Properties

        internal string IdToken
        {
            get { return _idToken; }
        }

        internal string IdTokenType
        {
            get { return _idTokenType; }
        }

        #endregion

        #region Private Methods

        void ClearAllTokens()
        {
            ClearAccessTokens();
            ClearIdTokens();
        }

        void ClearAccessTokens()
        {
            _accessToken = _refreshToken = null;
            _expireTimeUtc = DateTime.MinValue;
        }

        void ClearIdTokens()
        {
            _idToken = _idTokenType = null;
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
            catch (Exception exp)
            {
                var masException = exp.InnerException as MASException;
                if (masException?.MASErrorCode != ErrorCode.TokenAccessExpired)
                {
                    throw exp;
                }
            }

            // Our access token is expired
            _accessToken = null;
            await SaveTokensAsync();

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

            if (_idToken != null && _config.Mag.MobileSdk.IsSSOEnabled)
            {
                await MAGRequests.LogoutSessionAsync(_config, MASDevice.Current, this);
            }
            else
            {
                await MAGRequests.RevokeAccessTokenAsync(_config, _device, this);
            }

            await RemoveAccessTokensAsync();

            Current = null;
        }

        async Task LoadAsync()
        {
            await LoadIdTokenAsync();
            var accessInfo = await _storage.GetTextAsync(_isAnonymous ? StorageKeyNames.ClientAccessInfo : StorageKeyNames.UserAccessInfo);
            if (accessInfo != null)
            {
                try
                {
                    var jsonObj = JsonObject.Parse(accessInfo);

                    _accessToken = jsonObj.GetNamedString("accessToken");
                    _refreshToken = jsonObj.GetStringOrNull("refreshToken");
                    _expireTimeUtc = DateTime.FromBinary((long)jsonObj.GetNamedNumber("accessTokenExpiration"));
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

        async Task LoadIdTokenAsync()
        {
            var accessInfo = await _sharedStorage.GetTextAsync(_isAnonymous ? StorageKeyNames.ClientAccessInfo : StorageKeyNames.UserAccessInfo);
            if (accessInfo != null)
            {
                try
                {
                    var jsonObj = JsonObject.Parse(accessInfo);

                    _idToken = jsonObj.GetStringOrNull("idToken");
                    _idTokenType = jsonObj.GetStringOrNull("idTokenType");
                }
                catch
                {
                    ClearAllTokens();
                }
            }
            else
            {
                ClearAllTokens();
            }
        }

        async Task SaveTokensAsync()
        {
            await SaveIdTokenAsync();

            JsonObject obj = new JsonObject();
            obj.SetNamedValue("accessToken", _accessToken.ToJsonValue());
            obj.SetNamedValue("refreshToken", _refreshToken.ToJsonValue());
            obj.SetNamedValue("accessTokenExpiration", JsonValue.CreateNumberValue(_expireTimeUtc.ToBinary()));

            await _storage.SetAsync(_isAnonymous ? StorageKeyNames.ClientAccessInfo : StorageKeyNames.UserAccessInfo, obj.Stringify());
        }

        async Task SaveIdTokenAsync()
        {
            JsonObject obj = new JsonObject();
            obj.SetNamedValue("idToken", _idToken.ToJsonValue());
            obj.SetNamedValue("idTokenType", _idTokenType.ToJsonValue());

            await _sharedStorage.SetAsync(_isAnonymous ? StorageKeyNames.ClientAccessInfo : StorageKeyNames.UserAccessInfo, obj.Stringify());
        }

        async Task RemoveAccessTokensAsync()
        {
            ClearAllTokens();

            await SharedSecureStorage.RemoveAsync(_isAnonymous ? StorageKeyNames.ClientAccessInfo : StorageKeyNames.UserAccessInfo);
            await SecureStorage.RemoveAsync(_isAnonymous ? StorageKeyNames.ClientAccessInfo : StorageKeyNames.UserAccessInfo);
        }

        async Task UpdateTokens(RequestTokenResponseData data)
        {
            _accessToken = data.AccessToken;
            _refreshToken = data.RefreshToken;
            _expireTimeUtc = DateTime.UtcNow.AddSeconds(data.ExpiresIn);

            if (data.IdToken != null && data.IdTokenType != null)
            {
                _idToken = data.IdToken;
                _idTokenType = data.IdTokenType;
            }

            await SaveTokensAsync();
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
        SharedSecureStorage _sharedStorage;
        SecureStorage _storage;
        bool _isAnonymous;

        #endregion
    }
}
