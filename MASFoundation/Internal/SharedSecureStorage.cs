/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MASFoundation.Internal
{
    internal class SharedSecureStorage
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
            return Task.Run<IBuffer>(async () =>
            {
                try
                {
                    StorageFolder storageFolder = await GetPublisherStorageFolder("MASFoundation");
                    StorageFile storageFile = await storageFolder.GetFileAsync(key);
                    string content = string.Empty;
                    if (storageFolder != null)
                    {
                        content = await FileIO.ReadTextAsync(storageFile);
                    }
                    return Convert.FromBase64String(content).AsBuffer();
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
            return Task.Run(async () =>
            {
                var bytes = buffer.ToArray();
                string content = Convert.ToBase64String(bytes);
                StorageFolder storageFolder = await GetPublisherStorageFolder("MASFoundation");
                StorageFile storageFile = await storageFolder.CreateFileAsync(key, CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(storageFile, content);
            });
        }

        public static Task RemoveAsync(string key)
        {
            return Task.Run(async () =>
            {
                try
                {
                    StorageFolder storageFolder = await GetPublisherStorageFolder("MASFoundation");
                    StorageFile storageFile = await storageFolder.GetFileAsync(key);
                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch
                {
                }
            });
        }

        public static Task ResetAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    StorageFolder storageFolder = await GetPublisherStorageFolder("MASFoundation");
                    await storageFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch
                {
                }
            });
        }

        public static async Task<StorageFolder> GetPublisherStorageFolder(string resourceFolderName)
        {
            StorageFolder subFolder = null;
            StorageFolder storageFolder = ApplicationData.Current.GetPublisherCacheFolder("keys");
            try
            {
                subFolder = await storageFolder.GetFolderAsync(resourceFolderName);
            }
            catch (Exception)
            {
            }
            if (subFolder == null)
            {
                subFolder = await storageFolder.CreateFolderAsync(resourceFolderName, CreationCollisionOption.OpenIfExists);
            }
            return subFolder;
        }
    }
}
