using MASFoundation.Internal.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace MASFoundation.Internal
{
    internal class Configuration
    {
        public Configuration()
        {
        }

        public async Task LoadAsync(string fileName)
        {
            JsonObject jsonObject = null;

            try
            {
                var dataUri = new Uri("ms-appx:///" + fileName, UriKind.Absolute);
                var file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                var jsonText = await FileIO.ReadTextAsync(file);

                jsonObject = JsonObject.Parse(jsonText);
            }
            catch (Exception e)
            {
                ErrorFactory.throwError(ErrorKind.UnableToLoadConfigurationFile, e);
            }

            try
            {
                Server = new ServerConfigData(jsonObject.GetNamedObject("server"));
                OAuth = new OAuthConfigData(jsonObject.GetNamedObject("oauth"));
                Mag = new MagConfigData(jsonObject.GetNamedObject("mag"));
                Custom = new CustomConfigData(jsonObject.GetNamedObject("custom"));
            }
            catch (Exception e)
            {
                ErrorFactory.throwError(ErrorKind.InvalidConfigurationFile, e);
            }
        }

        public string GetEndpointPath(string partialPath)
        {
            UriBuilder builder = new UriBuilder("https", Server.Hostname, Server.Port, partialPath);

            return builder.Uri.AbsoluteUri;
        }

        public OAuthClientIdConfigData DefaultClientId
        {
            get
            {
                if (OAuth != null && OAuth.Client != null && OAuth.Client.ClientIds != null && OAuth.Client.ClientIds.Count > 0)
                {
                    return OAuth.Client.ClientIds[0];
                }

                return null;
            }
        }

        public bool HasScope(string scopeName)
        {
            var clientId = DefaultClientId;
            if (clientId != null)
            {
                var scopes = clientId.Scope.Split(' ');
                return scopes.Any(s => s == scopeName);
            }

            return false;
        }

        public ServerConfigData Server
        {
            get;
            internal set;
        }

        public OAuthConfigData OAuth
        {
            get;
            internal set;
        }

        public MagConfigData Mag
        {
            get;
            internal set;
        }

        public CustomConfigData Custom
        {
            get;
            internal set;
        }
    }
}
