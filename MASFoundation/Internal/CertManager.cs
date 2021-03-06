/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

#region Bouncy castle usings
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.Security.Cryptography;
//using System.IO;

namespace MASFoundation.Internal
{
    internal class CertManager
    {
        public CertManager(SharedSecureStorage storage)
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

        public async Task<Certificate> GetIfExistsAsync()
        {
            var Certificate = await GetAsync();
            try
            {
                if (Certificate == null)
                {
                    string pfx = await _storage.GetTextAsync(StorageKeyNames.PrivateKey);

                    if (pfx != null)
                    {
                        await CertificateEnrollmentManager.ImportPfxDataAsync(pfx, string.Empty, ExportOption.NotExportable, KeyProtectionLevel.NoConsent, InstallOptions.None, "MAG_CERT");
                    }
                    Certificate = await GetAsync();
                }
            }
            catch (Exception)
            {
            }
            return Certificate;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task InstallTrustedServerCert(string certText)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var cert = new Certificate(certText.ToBytes().AsBuffer());

            var trustedStore = CertificateStores.TrustedRootCertificationAuthorities;
            trustedStore.Add(cert);
        }

        public async Task<string> GenerateCSRAsync(Configuration config, MASDevice device, string username)
        {
            #region Bouncy castle CSR generation
            return await Task.Run(() =>
            {
                X509Name subject = new X509Name(string.Format("cn={0}, ou={1}, dc={2}, o={3}", username, device.Id, device.Name, config.OAuth.Client.Organization));
                var keyGenerationParameters = new KeyGenerationParameters(_random, config.Mag.MobileSdk.ClientCertRsaKeybits);

                var keyPairGenerator = new RsaKeyPairGenerator();
                keyPairGenerator.Init(keyGenerationParameters);
                var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

                Pkcs10CertificationRequest request = new Pkcs10CertificationRequest(new Asn1SignatureFactory("SHA256WithRSA", subjectKeyPair.Private),
                    subject, subjectKeyPair.Public, new DerSet(), subjectKeyPair.Private);

                var data = request.GetEncoded();

                _privateKey = subjectKeyPair.Private;
                return Convert.ToBase64String(data);
            });
            #endregion

            // MSFT way of generating the CSR
            //return await CertificateEnrollmentManager.CreateRequestAsync(new CertificateRequestProperties()
            //{
            //    KeySize = (uint)config.Mag.MobileSdk.ClientCertRsaKeybits,
            //    Subject = string.Format("cn={0}, ou={1}, dc={2}, o={3}", username, device.Id.GetHashCode(), device.Name, config.OAuth.Client.Organization),
            //    KeyUsages = EnrollKeyUsages.Decryption | EnrollKeyUsages.Signing
            //});
        }

        public async Task InstallAsync(string certResponse)
        {
            #region Bouncy castle PKCS #12 cert file generation
            var data = Convert.FromBase64String(certResponse);

            var parser = new X509CertificateParser();
            var cert = parser.ReadCertificate(data);

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            X509CertificateEntry certEntry = new X509CertificateEntry(cert);
            store.SetCertificateEntry(cert.SubjectDN.ToString(), certEntry); // use DN as the Alias.

            AsymmetricKeyEntry keyEntry = new AsymmetricKeyEntry(_privateKey);
            store.SetKeyEntry(cert.SubjectDN.ToString() + "_key", keyEntry, new X509CertificateEntry[] { certEntry }); // 

            string pfx = string.Empty;
            string password = "";
            using (MemoryStream ms = new MemoryStream())
            {
                store.Save(ms, password.ToCharArray(), _random);

                ms.Position = 0;
                await _storage.SetAsync("accessCert", ms.GetWindowsRuntimeBuffer());
                StreamReader streamReader = new StreamReader(ms);
                // Write to .PFX string
                byte[] arr = ms.ToArray();

                pfx = CryptographicBuffer.EncodeToBase64String(arr.AsBuffer());
            }
            #endregion
            await _storage.SetAsync(StorageKeyNames.PrivateKey, pfx);

            await CertificateEnrollmentManager.ImportPfxDataAsync(pfx, password, ExportOption.NotExportable, KeyProtectionLevel.NoConsent, InstallOptions.None, "MAG_CERT");

            // Store the registered cert subject
            if (cert.SubjectDN != null)
            {
                var valueList = cert.SubjectDN.GetValueList(X509Name.CN);
                if (valueList.Count > 0)
                {
                    await _storage.SetAsync(StorageKeyNames.RegisteredCertSubject, (string)valueList[0]);
                }
            }
        }

        public static async Task UninstallAsync()
        {
            // Windows 10 doesn't support an uninstall method at the moment.  Let's forget about the cert ourselves.
            await SharedSecureStorage.RemoveAsync(StorageKeyNames.RegisteredCertSubject);
        }

        #region Bouncy castle member variables
        static readonly X509V3CertificateGenerator _certificateGenerator = new X509V3CertificateGenerator();
        static readonly SecureRandom _random = new SecureRandom();
        static AsymmetricKeyParameter _privateKey;
        #endregion

        SharedSecureStorage _storage;
    }
}
