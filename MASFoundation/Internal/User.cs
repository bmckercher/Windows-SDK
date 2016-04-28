using MASFoundation.Internal.Data;
using System;
using System.Threading.Tasks;

namespace MASFoundation.Internal
{
    internal class User
    {
        public User(Configuration config, Device device, SecureStorage storage)
        {
            _config = config;
            _device = device;
            _storage = storage;
        }

        public bool IsLoggedIn
        {
            get { return _accessToken != null; }
        }

        public async Task InitializeAsync()
        {
            _accessToken = await _storage.GetTextAsync("accessToken");
            _refreshToken = await _storage.GetTextAsync("refreshToken");

            var expireTime = await _storage.GetDateAsync("expireTime");
            if (expireTime != null)
            {
                _expireTimeUtc = expireTime.Value;
            }
            else
            {
                _expireTimeUtc = DateTime.MinValue;
            }

            _idToken = await _storage.GetTextAsync("idToken");
            _idTokenType = await _storage.GetTextAsync("idTokenType");

            // We have an id token but not an access token, try to get an access token.
            if (_idToken != null && _idTokenType != null && _accessToken == null)
            {
                var data = await MAGRequests.RequestAccessTokenFromIdTokenAsync(_config, _device, _idToken, _idTokenType);
                await UpdateTokens(data);
            }
        }

        public async Task LoginAsync(string username, string password)
        {
            var data = await MAGRequests.RequestAccessTokenAsync(_config, _device, username, password);
            await UpdateTokens(data);
        }

        public async Task LogoffAsync()
        {
            await MAGRequests.RevokeAccessTokenAsync(_config, _device, this);

            await SecureStorage.RemoveAsync("accessToken");
            await SecureStorage.RemoveAsync("refreshToken");
            await SecureStorage.RemoveAsync("expireTime");
        }

        public async Task<string> GetAccessTokenAsync()
        {
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

        public async Task<string> GetAccessHeaderValueAsync()
        {
            var token = await GetAccessTokenAsync();

            if (token != null)
            {
                return string.Format("Bearer {0}", token);
            }

            return null;
        }

        public Task<UserInfoResponseData> GetInfoAsync()
        {
            return MAGRequests.GetUserInfoAsync(_config, _device, this);
        }

        public async Task LoginAsync()
        {
            var data = await MAGRequests.RequestAccessTokenAnonymouslyAsync(_config, _device);
            await UpdateTokens(data);
        }

        async Task UpdateTokens(RequestTokenResponseData data)
        {
            _accessToken = data.AccessToken;
            _refreshToken = data.RefreshToken;
            _expireTimeUtc = DateTime.UtcNow.AddSeconds(data.ExpiresIn);

            await _storage.SetAsync("accessToken", false, _accessToken);
            await _storage.SetAsync("expireTime", false, _expireTimeUtc);

            if (_refreshToken != null)
            {
                await _storage.SetAsync("refreshToken", false, _refreshToken);
            }

            if (data.IdToken != null && data.IdTokenType != null)
            {
                _idToken = data.IdToken;
                _idTokenType = data.IdTokenType;

                await _storage.SetAsync("idToken", true, _idToken);
                await _storage.SetAsync("idTokenType", true, _idTokenType);
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
