using MASFoundation.Internal.Data;
using System;
using System.Linq;
using System.Text;
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
            string jsonText = null;

            try
            {
                var dataUri = new Uri("ms-appx:///" + fileName, UriKind.Absolute);
                var file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                jsonText = await FileIO.ReadTextAsync(file);
            }
            catch (Exception e)
            {
                ErrorFactory.ThrowError(ErrorCode.ConfigurationLoadingFailedFileNotFound, e);
            }

            try
            {
                jsonObject = JsonObject.Parse(jsonText);
            }
            catch (Exception e)
            {
                ErrorFactory.ThrowError(ErrorCode.ConfigurationLoadingFailedJsonSerialization, e);
            }

            Validate(jsonObject);

            try
            {
                Server = new ServerConfigData(jsonObject.GetNamedObject("server"));
                OAuth = new OAuthConfigData(jsonObject.GetNamedObject("oauth"));
                Mag = new MagConfigData(jsonObject.GetNamedObject("mag"));
                Custom = new CustomConfigData(jsonObject.GetNamedObject("custom"));
            }
            catch (Exception e)
            {
                ErrorFactory.ThrowError(ErrorCode.ConfigurationLoadingFailedJsonValidation, e);
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

        void Validate(JsonObject jsonObject)
        {
            var validator = new JsonValidator();

            //  Server config
            validator.AddRule(new JsonValidationRule("server.hostname", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("server.port", JsonValueType.Number));
            validator.AddRule(new JsonValidationRule("server.server_certs", JsonValueType.Array));

            //  OAuth config
            validator.AddRule(new JsonValidationRule("oauth.client.organization", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("oauth.client.client_ids[0].client_id", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("oauth.client.client_ids[0].scope", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("oauth.client.client_ids[0].redirect_uri", JsonValueType.String));

            //  OAuth system endpoint
            validator.AddRule(new JsonValidationRule("oauth.system_endpoints.authorization_endpoint_path", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("oauth.system_endpoints.token_endpoint_path", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("oauth.system_endpoints.token_endpoint_path", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("oauth.system_endpoints.usersession_logout_endpoint_path", JsonValueType.String));

            //  MAG system endpoint
            validator.AddRule(new JsonValidationRule("mag.system_endpoints.device_remove_endpoint_path", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("mag.system_endpoints.device_register_endpoint_path", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("mag.system_endpoints.device_client_register_endpoint_path", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("mag.system_endpoints.client_credential_init_endpoint_path", JsonValueType.String));

            //  MAG OAuth protected endpoint
            validator.AddRule(new JsonValidationRule("mag.oauth_protected_endpoints.enterprise_browser_endpoint_path", JsonValueType.String));

            //  MAG mobile SDK
            validator.AddRule(new JsonValidationRule("mag.mobile_sdk.sso_enabled", JsonValueType.Boolean));
            validator.AddRule(new JsonValidationRule("mag.mobile_sdk.location_enabled", JsonValueType.Boolean));
            validator.AddRule(new JsonValidationRule("mag.mobile_sdk.location_provider", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("mag.mobile_sdk.msisdn_enabled", JsonValueType.Boolean));
            validator.AddRule(new JsonValidationRule("mag.mobile_sdk.trusted_public_pki", JsonValueType.Boolean));
            validator.AddRule(new JsonValidationRule("mag.mobile_sdk.trusted_cert_pinned_public_key_hashes", JsonValueType.Array));
            validator.AddRule(new JsonValidationRule("mag.mobile_sdk.client_cert_rsa_keybits", JsonValueType.Number));

            //  MAG BLE
            validator.AddRule(new JsonValidationRule("mag.ble.msso_ble_service_uuid", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("mag.ble.msso_ble_characteristic_uuid", JsonValueType.String));
            validator.AddRule(new JsonValidationRule("mag.ble.msso_ble_rssi", JsonValueType.Number));

            var results = validator.Validate(jsonObject);

            if (results.HasErrors)
            {
                var configException = new MASConfigException();

                var sb = new StringBuilder("The configuration file failed to validate!");
                sb.AppendLine();

                foreach (var error in results.Errors)
                {
                    var errorText = error.ToString();

                    configException.Errors.Add(errorText);

                    sb.AppendLine(errorText);
                }

                Logger.LogError(sb.ToString());

                ErrorFactory.ThrowError(ErrorCode.ConfigurationLoadingFailedJsonValidation, configException);
            }
        }
    }
}
