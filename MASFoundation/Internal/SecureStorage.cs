using MASUtility;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MASFoundation.Internal
{
    internal class SecureStorage
    {
        public SecureStorage(Configuration config)
        {
            _config = config;
        }

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

        public Task SetAsync(string key, bool isShared, string text)
        {
            return SetAsync(key, isShared, Encoding.UTF8.GetBytes(text));
        }

        public Task SetAsync(string key, bool isShared, DateTime date)
        {
            return SetAsync(key, isShared, BitConverter.GetBytes(date.ToBinary()));
        }

        public async Task<IBuffer> GetIBufferAsync(string key)
        {
            var info = await GetFileAsync(key);
            if (info != null)
            {
                try
                {
                    var stream = await FileIO.ReadBufferAsync(info.File);
                    var decrypted = await _encryptor.DecryptAsync(stream, GetEntropy(info.IsShared));
                    return decrypted;
                }
                catch
                {
                }

                return null;
            }

            return null;
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

        public Task SetAsync(string key, bool isShared, byte[] data)
        {
            return SetAsync(key, isShared, data.AsBuffer());
        }

        public async Task SetAsync(string key, bool isShared, IBuffer buffer)
        {
            StorageFile file = null;
            if (isShared)
            {
                var folder = ApplicationData.Current.GetPublisherCacheFolder("keys");

                try
                {
                    file = await folder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
                }
                catch
                {
                }
            }

            if (file == null)
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("keys", CreationCollisionOption.OpenIfExists);
                file = await folder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
            }

            var encrypted = await _encryptor.EncryptAsync(buffer, GetEntropy(isShared));
            await FileIO.WriteBufferAsync(file, encrypted);
        }

        public static async Task RemoveAsync(string key)
        {
            var info = await GetFileAsync(key);
            if (info != null)
            {
                await info.File.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public static async Task ResetAsync()
        {
            try
            {
                await ApplicationData.Current.ClearPublisherCacheFolderAsync("keys");
            }
            catch
            {
            }

            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("keys");
            if (item != null)
            {
                await item.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        async static Task<LoadedFileInfo> GetFileAsync(string key)
        {
            bool isShared = true;
            var folder = ApplicationData.Current.GetPublisherCacheFolder("keys");

            var item = await folder.TryGetItemAsync(key);

            if (item == null)
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("keys", CreationCollisionOption.OpenIfExists);
                item = await folder.TryGetItemAsync(key);
                isShared = false;
            }

            if (item != null)
            {
                return new LoadedFileInfo()
                {
                    File = (StorageFile)item,
                    IsShared = isShared
                };
            }

            return null;
        }

        string GetEntropy(bool isShared)
        {
            if (!isShared)
            {
                return _config.DefaultClientId.Id;
            }
            else
            {
                return _config.OAuth.Client.Organization;
            }
        }

        Encryptor _encryptor = new Encryptor();

        class LoadedFileInfo
        {
            public StorageFile File { get; set; }
            public bool IsShared { get; set;}
        }

        Configuration _config;
    }
}
