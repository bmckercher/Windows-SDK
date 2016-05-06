using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage.Streams;

namespace MASFoundation.Internal
{
    internal class SecureStorage
    {
        public async Task<string> GetTextAsync(string key)
        {
            var data = await GetBytesAsync(key);

            if (data != null)
            {
                return Encoding.UTF8.GetString(data);
            }

            return null;
        }

        public async Task<DateTime?> GetDateAsync(string key)
        {
            var data = await GetBytesAsync(key);
            if (data != null)
            {
                return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            }

            return null;
        }

        public Task SetAsync(string key, string text)
        {
            return SetAsync(key, Encoding.UTF8.GetBytes(text));
        }

        public Task SetAsync(string key, DateTime date)
        {
            return SetAsync(key, BitConverter.GetBytes(date.ToBinary()));
        }

        public Task<IBuffer> GetIBufferAsync(string key)
        {
            return Task.Run<IBuffer>(() =>
            {
                try
                {
                    PasswordVault vault = new PasswordVault();
                    var value = vault.Retrieve("MASFoundation", key);

                    return Convert.FromBase64String(value.Password).AsBuffer();
                }
                catch
                {
                    return null;
                }
            });
        }

        public async Task<byte[]> GetBytesAsync(string key)
        {
            var buffer = await GetIBufferAsync(key);
            if (buffer != null)
            {
                return buffer.ToArray();
            }

            return null;
        }

        public Task SetAsync(string key, byte[] data)
        {
            return SetAsync(key, data.AsBuffer());
        }

        public Task SetAsync(string key, IBuffer buffer)
        {
            return Task.Run(() =>
            {
                PasswordVault vault = new PasswordVault();
                var bytes = buffer.ToArray();

                var cred = new PasswordCredential()
                {
                    Password = Convert.ToBase64String(bytes),
                    UserName = key,
                    Resource = "MASFoundation",
                };

                try
                {
                    vault.Remove(cred);
                }
                catch
                {
                }

                vault.Add(cred);
            });
        }

        public static Task RemoveAsync(string key)
        {
            return Task.Run(() =>
            {
                try
                {
                    PasswordVault vault = new PasswordVault();
                    vault.Remove(new PasswordCredential()
                    {
                        UserName = key,
                        Resource = "MASFoundation",
                    });
                }
                catch
                {
                }
            });
        }

        public static Task ResetAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    PasswordVault vault = new PasswordVault();
                    var items = vault.FindAllByResource("MASFoundation");
                    foreach (var item in items)
                    {
                        vault.Remove(item);
                    }
                }
                catch
                {
                }
            });
        }
    }
}
