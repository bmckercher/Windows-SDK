using System.Collections.Generic;
using System.Text;
using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    internal class ServerConfigData
    {
        internal ServerConfigData(JsonObject jsonObject)
        {
            Hostname = jsonObject.GetNamedString("hostname");
            Port = (int)jsonObject.GetNamedNumber("port");
            Prefix = jsonObject.GetNamedString("prefix");

            var certs = new List<string>();
            var jsonCerts = jsonObject.GetNamedArray("server_certs");
            foreach (var jsonCert in jsonCerts)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var jsonCertLines in jsonCert.GetArray())
                {
                    sb.AppendLine(jsonCertLines.GetString());
                }

                certs.Add(sb.ToString());
            }
            ServerCerts = certs.AsReadOnly();
        }

        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string Prefix { get; private set; }
        public IReadOnlyList<string> ServerCerts { get; private set; }
    }
}
