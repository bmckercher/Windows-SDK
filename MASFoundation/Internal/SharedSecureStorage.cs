/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MASFoundation.Internal
{
    internal class SharedSecureStorage
    {
        string folderName;

        internal SharedSecureStorage() { folderName = "MASFoundation"; }

        internal SharedSecureStorage(string folderName) { this.folderName = folderName; }

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

        public async Task<IBuffer> GetIBufferAsync(string key)
        {
            try
            {
                StorageFolder storageFolder = await GetPublisherStorageFolder(folderName);
                StorageFile storageFile = await storageFolder.GetFileAsync(key);
                string content = string.Empty;
                if (storageFolder != null)
                {
                    var protectedContent = await FileIO.ReadBufferAsync(storageFile);
                    content = await UnprotectAsync(protectedContent);
                }
                return Convert.FromBase64String(content).AsBuffer();
            }
            catch
            {
                return null;
            }
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
                StorageFolder storageFolder = await GetPublisherStorageFolder(folderName);
                StorageFile storageFile = await storageFolder.CreateFileAsync(key, CreationCollisionOption.OpenIfExists);
                await FileIO.WriteBufferAsync(storageFile, await ProtectAsync(content));
            });
        }

        public static Task RemoveAsync(string key, string subFolderName = "MASFoundation")
        {
            return Task.Run(async () =>
            {
                try
                {
                    StorageFolder storageFolder = await GetPublisherStorageFolder(subFolderName);
                    StorageFile storageFile = await storageFolder.GetFileAsync(key);
                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch
                {
                }
            });
        }

        public static Task ResetAsync(string subFolderName = "MASFoundation")
        {
            return Task.Run(async () =>
            {
                try
                {
                    StorageFolder storageFolder = await GetPublisherStorageFolder(subFolderName);
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

        async Task<IBuffer> ProtectAsync(string content)
        {
            // Create a DataProtectionProvider object for the specified descriptor.
            DataProtectionProvider Provider = new DataProtectionProvider("LOCAL=user");

            // Encode the plaintext input message to a buffer.
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);

            // Encrypt the message.
            IBuffer buffProtected = await Provider.ProtectAsync(buffMsg);

            // Execution of the SampleProtectAsync function resumes here
            // after the awaited task (Provider.ProtectAsync) completes.
            return buffProtected;
        }

        async Task<string> UnprotectAsync(IBuffer buffProtected)
        {
            // Create a DataProtectionProvider object.
            DataProtectionProvider Provider = new DataProtectionProvider();

            // Decrypt the protected message specified on input.
            IBuffer buffUnprotected = await Provider.UnprotectAsync(buffProtected);

            // Execution of the SampleUnprotectData method resumes here
            // after the awaited task (Provider.UnprotectAsync) completes
            // Convert the unprotected message from an IBuffer object to a string.
            string content = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffUnprotected);

            // Return the plaintext string.
            return content;
        }
    }
}
