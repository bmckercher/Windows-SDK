
#region Bouncy castle usings
//using Org.BouncyCastle.Asn1;
//using Org.BouncyCastle.Asn1.X509;
//using Org.BouncyCastle.Crypto;
//using Org.BouncyCastle.Crypto.Generators;
//using Org.BouncyCastle.Crypto.Operators;
//using Org.BouncyCastle.Math;
//using Org.BouncyCastle.Pkcs;
//using Org.BouncyCastle.Security;
//using Org.BouncyCastle.Utilities;
//using Org.BouncyCastle.X509;
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using System.Runtime.InteropServices.WindowsRuntime;
//using System.IO;

namespace MASFoundation.Internal
{
    internal class CertManager
    {
        public CertManager(SecureStorage storage)
        {
            _storage = storage;
        }

        public async Task<Certificate> GetAsync()
        {
            var certUsername = await _storage.GetTextAsync(StorageKeyNames.RegisteredCertSubject);

            if (certUsername != null)
            {
                var certs = await CertificateStores.FindAllAsync(new CertificateQuery()
                {
                    IssuerName = "ca_msso",
                });

                var cert = certs.FirstOrDefault(c => c.Subject == certUsername);
                return cert;
            }

            return null;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task InstallTrustedServerCert(string certText)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var cert = new Certificate(certText.ToUTF8Bytes().AsBuffer());

            var trustedStore = CertificateStores.TrustedRootCertificationAuthorities;
            trustedStore.Add(cert);
        }

        public async Task<string> GenerateCSRAsync(Configuration config, MASDevice device, string username)
        {
            #region Bouncy castle CSR generation
            //X509Name subject = new X509Name(string.Format("cn={0}, ou={1}, dc={2}, o={3}", username, device.Id, device.Name, config.OAuth.Client.Organization));
            //var keyGenerationParameters = new KeyGenerationParameters(_random, config.Mag.MobileSdk.ClientCertRsaKeybits);

            //var keyPairGenerator = new RsaKeyPairGenerator();
            //keyPairGenerator.Init(keyGenerationParameters);
            //var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            //Pkcs10CertificationRequest request = new Pkcs10CertificationRequest(new Asn1SignatureFactory("SHA256WithRSA", subjectKeyPair.Private), 
            //    subject, subjectKeyPair.Public, new DerSet(), subjectKeyPair.Private);

            //var data = request.GetEncoded();

            //_privateKey = subjectKeyPair.Private;
            //return Convert.ToBase64String(data);
            #endregion

            // MSFT way of generating the CSR
            return await CertificateEnrollmentManager.CreateRequestAsync(new CertificateRequestProperties()
            {
                KeySize = (uint)config.Mag.MobileSdk.ClientCertRsaKeybits,
                Subject = string.Format("cn={0}, ou={1}, dc={2}, o={3}", username, device.Id, device.Name, config.OAuth.Client.Organization),
                KeyUsages = EnrollKeyUsages.Decryption | EnrollKeyUsages.Signing
            });
        }

        public async Task InstallAsync(string certResponse)
        {
            #region Bouncy castle PKCS #12 cert file generation
            //var data = Convert.FromBase64String(certResponse);

            //var parser = new X509CertificateParser();
            //var cert = parser.ReadCertificate(data);

            //Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            //X509CertificateEntry certEntry = new X509CertificateEntry(cert);
            //store.SetCertificateEntry(cert.SubjectDN.ToString(), certEntry); // use DN as the Alias.

            //AsymmetricKeyEntry keyEntry = new AsymmetricKeyEntry(_privateKey);
            //store.SetKeyEntry(cert.SubjectDN.ToString() + "_key", keyEntry, new X509CertificateEntry[] { certEntry }); // 

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    store.Save(ms, "".ToCharArray(), _random);

            //    ms.Position = 0;
            //    await SecureStorage.SetAsync("accessCert", true, ms.GetWindowsRuntimeBuffer());
            //}
            #endregion

            var cert = new Certificate(Convert.FromBase64String(certResponse).AsBuffer());

            await CertificateEnrollmentManager.InstallCertificateAsync(certResponse, InstallOptions.DeleteExpired);

            await _storage.SetAsync(StorageKeyNames.RegisteredCertSubject, cert.Subject);
        }

        public static async Task UninstallAsync()
        {
            // Windows 10 doesn't support an uninstall method at the moment.  Let's forget about the cert ourselves.
            await SecureStorage.RemoveAsync(StorageKeyNames.RegisteredCertSubject);
        }

        #region Bouncy castle member variables
        //static readonly X509V3CertificateGenerator _certificateGenerator = new X509V3CertificateGenerator();
        //static readonly SecureRandom _random = new SecureRandom();
        //static AsymmetricKeyParameter _privateKey;
        #endregion

        SecureStorage _storage;
    }
}
