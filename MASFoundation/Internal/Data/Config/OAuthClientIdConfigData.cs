using System;
using System.Text;
using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class OAuthClientIdConfigData
    {
        public OAuthClientIdConfigData(JsonObject jsonObject)
        {
            Id = jsonObject.GetNamedString("client_id");
            Secret = jsonObject.GetNamedString("client_secret");
            Scope = jsonObject.GetNamedString("scope");
            RedirectUri = jsonObject.GetNamedString("redirect_uri");
            Environment = jsonObject.GetNamedString("environment");
            Status = jsonObject.GetNamedString("status");
            RegisteredBy = jsonObject.GetNamedString("registered_by");
        }

        public string Id { get; private set; }
        public string Secret { get; private set; }
        public string Scope { get; private set; }
        public string RedirectUri { get; private set; }
        public string Environment { get; private set; }
        public string Status { get; private set; }
        public string RegisteredBy { get; private set; }
        public string BasicAuthValue
        {
            get
            {
                var clientUsernamePass = string.Format("{0}:{1}", Id, Secret);
                return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientUsernamePass));
            }
        }
    }
}
