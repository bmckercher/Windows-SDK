/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Threading.Tasks;

namespace MASUnitTestApp
{
    [TestClass]
    public class MASFoundationTests
    {
        [TestMethod]
        public async Task TestClientRegistration()
        {
            await ResetTestAsync();
            
            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;

            await MAS.StartAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);
        }

        [TestMethod]
        public async Task TestMissingConfigFileStartup()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "bogus_filepath_msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;

            await ThrowsAsync(() =>
            {
                return MAS.StartAsync().AsTask();
            });
        }

        [TestMethod]
        public async Task TestConfigContentsStartup()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;

            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///msso_config.json"));
            var configText = await Windows.Storage.FileIO.ReadTextAsync(file);

            await MAS.StartWithConfigAsync(configText);
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);
        }

        [TestMethod]
        public async Task TestNullConfigContentsStartup()
        {
            await ResetTestAsync();

            await ThrowsAsync(() =>
            {
                return MAS.StartWithConfigAsync(null).AsTask();
            });
        }

        [TestMethod]
        public async Task TestEmptyConfigContentsStartup()
        {
            await ResetTestAsync();

            await ThrowsAsync(() =>
            {
                return MAS.StartWithConfigAsync(string.Empty).AsTask();
            });
        }

        [TestMethod]
        public async Task TestMissingPropertyConfigContentsStartup()
        {
            await ResetTestAsync();

            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///bogus_msso_config.json"));
            var configText = await Windows.Storage.FileIO.ReadTextAsync(file);

            await ThrowsAsync(() =>
            {
                return MAS.StartWithConfigAsync(configText).AsTask();
            });
        }


        [TestMethod]
        public async Task TestInvalidConfigContentsStartup()
        {
            await ResetTestAsync();

            await ThrowsAsync(() =>
            {
                return MAS.StartWithConfigAsync("{{{something something bad JSON").AsTask();
            });
        }

        [TestMethod]
        public async Task TestUserLogin()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;

            await MAS.StartAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);

            await MASUser.LoginAsync("zoljo01", "IdentityMine");
            Assert.IsTrue(MASUser.Current?.IsLoggedIn == true);
        }

        [TestMethod]
        public async Task TestUserInfoLogin()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;

            await MAS.StartAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);

            await MASUser.LoginAsync("zoljo01", "IdentityMine");
            Assert.IsTrue(MASUser.Current?.IsLoggedIn == true);

            var info = await MASUser.Current.GetInfoAsync();
            Assert.IsTrue(info != null);
        }

        [TestMethod]
        public async Task TestClientRegistrationUserLogout()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;

            await MAS.StartAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);

            await MASUser.LoginAsync("zoljo01", "IdentityMine");
            Assert.IsTrue(MASUser.Current?.IsLoggedIn == true);

            await MASUser.Current.LogoffAsync();
            Assert.IsTrue(MASUser.Current?.IsLoggedIn != true);
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);
        }

        [TestMethod]
        public async Task TestUserRegistrationLogout()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.User;

            await MAS.StartAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered != true);

            await MASUser.LoginAsync("zoljo01", "IdentityMine");
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);
            Assert.IsTrue(MASUser.Current?.IsLoggedIn == true);

            await MASUser.Current.LogoffAsync();
            Assert.IsTrue(MASUser.Current?.IsLoggedIn != true);
        }

        [TestMethod]
        public async Task TestClientDeregistration()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;

            await MAS.StartAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);

            await MASDevice.Current.UnregisterAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered != true);
        }

        [TestMethod]
        public async Task TestUserDeregistration()
        {
            await ResetTestAsync();

            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.User;

            await MAS.StartAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered != true);

            await MASUser.LoginAsync("zoljo01", "IdentityMine");
            Assert.IsTrue(MASDevice.Current?.IsRegistered == true);
            Assert.IsTrue(MASUser.Current?.IsLoggedIn == true);

            await MASDevice.Current.UnregisterAsync();
            Assert.IsTrue(MASDevice.Current?.IsRegistered != true);
        }

        async Task ThrowsAsync(Func<Task> taskFunc)
        {
            try
            {
                await taskFunc();
            }
            catch
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        // This function just tries to clean up any previous registartions for unit testing.
        async Task ResetTestAsync()
        {
            try
            {
                await MAS.StartAsync();
                if (MASDevice.Current?.IsRegistered == true)
                {
                    await MASDevice.Current?.UnregisterAsync();
                }
            }
            catch
            {
            }

            await MAS.ResetAsync();
        }
    }
}
