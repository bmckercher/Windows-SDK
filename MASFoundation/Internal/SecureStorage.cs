using MASUtility;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MASFoundation.Internal
{
    internal static class SecureStorage
    {
        public static async Task<string> GetTextAsync(string key)
        {
            var data = await GetBytesAsync(key);

            if (data != null)
            {
                return Encoding.UTF8.GetString(data);
            }

            return null;
        }

        public static async Task<DateTime?> GetDateAsync(string key)
        {
            var data = await GetBytesAsync(key);
            if (data != null)
            {
                return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            }

            return null;
        }

        public static Task SetAsync(string key, bool isShared, string text)
        {
            return SetAsync(key, isShared, Encoding.UTF8.GetBytes(text));
        }

        public static Task SetAsync(string key, bool isShared, DateTime date)
        {
            return SetAsync(key, isShared, BitConverter.GetBytes(date.ToBinary()));
        }

        public static async Task<IBuffer> GetIBufferAsync(string key)
        {
            var file = await GetFileAsync(key);
            if (file != null)
            {
                var stream = await FileIO.ReadBufferAsync(file);
                var decrypted = await _encryptor.DecryptAsync(stream);
                return decrypted;
            }

            return null;
        }

        public static async Task<byte[]> GetBytesAsync(string key)
        {
            var buffer = await GetIBufferAsync(key);
            if (buffer != null)
            {
                return buffer.ToArray();
            }

            return null;
        }

        public static Task SetAsync(string key, bool isShared, byte[] data)
        {
            return SetAsync(key, isShared, data.AsBuffer());
        }

        public static async Task SetAsync(string key, bool isShared, IBuffer buffer)
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

            var encrypted = await _encryptor.EncryptAsync(buffer);
            await FileIO.WriteBufferAsync(file, encrypted);
        }

        public static async Task RemoveAsync(string key)
        {
            var file = await GetFileAsync(key);
            if (file != null)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
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

        async static Task<StorageFile> GetFileAsync(string key)
        {
            var folder = ApplicationData.Current.GetPublisherCacheFolder("keys");

            var item = await folder.TryGetItemAsync(key);

            if (item == null)
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("keys", CreationCollisionOption.OpenIfExists);
                item = await folder.TryGetItemAsync(key);
            }

            if (item != null)
            {
                return (StorageFile)item;
            }

            return null;
        }

        static Encryptor _encryptor = new Encryptor();
    }
}
