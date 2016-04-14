using MASFoundation.Internal.Data;
using System;
using System.Threading.Tasks;

namespace MASFoundation.Internal
{
    internal class User
    {
        public User(Configuration config, Device device)
        {
            _config = config;
            _device = device;
        }

        public async Task InitializeAsync()
        {
            var accessToken = await SecureStorage.GetTextAsync("accessToken");
            var refreshToken = await SecureStorage.GetTextAsync("refreshToken");
            var expireTime = await SecureStorage.GetDateAsync("expireTime");

            var idToken = await SecureStorage.GetTextAsync("idToken");
            var idTokenType = await SecureStorage.GetTextAsync("idTokenType");

            if (accessToken != null && refreshToken != null && expireTime != null)
            {
                _accessToken = accessToken;
                _refreshToken = refreshToken;
                _expireTimeUtc = expireTime.Value;
            }

            if (idToken != null && idTokenType != null)
            {
                _idToken = idToken;
                _idTokenType = idTokenType;
            }

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

        async Task UpdateTokens(RequestTokenResponseData data)
        {
            _accessToken = data.AccessToken;
            _refreshToken = data.RefreshToken;
            _expireTimeUtc = DateTime.UtcNow.AddSeconds(data.ExpiresIn);

            await SecureStorage.SetAsync("accessToken", false, _accessToken);
            await SecureStorage.SetAsync("refreshToken", false, _refreshToken);
            await SecureStorage.SetAsync("expireTime", false, _expireTimeUtc);

            if (data.IdToken != null && data.IdTokenType != null)
            {
                _idToken = data.IdToken;
                _idTokenType = data.IdTokenType;

                await SecureStorage.SetAsync("idToken", true, _idToken);
                await SecureStorage.SetAsync("idTokenType", true, _idTokenType);
            }
        }

        string _accessToken;
        string _refreshToken;
        DateTime _expireTimeUtc;
        string _idToken;
        string _idTokenType;
        Configuration _config;
        Device _device;
    }
}
