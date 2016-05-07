﻿using MASFoundation.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MASFoundation
{
    public sealed class MASApplication
    {
        static MASApplication _current;
        public static MASApplication Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new MASApplication();
                }

                return _current;
            }
        }

        /// <summary>
        /// The MASApplication organization.
        /// </summary>
        public string Organization { get; private set; }

        /// <summary>
        /// The MASApplication name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The MASApplication identifier.
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// The MASApplication description.
        /// </summary>
        public string DetailedDescription { get; private set; }

        /// <summary>
        /// The MASApplication environment.
        /// </summary>
        public string Environment { get; private set; }

        /// <summary>
        /// The MASApplication redirect URL.
        /// </summary>
        public string RedirectUri { get; private set; }

        /// <summary>
        /// The MASApplication registeredBy identifier.
        /// </summary>
        public string RegisteredBy { get; private set; }

        /// <summary>
        /// The MASApplication scope array.
        /// </summary>
        public IReadOnlyList<string> Scope { get; private set; }

        /// <summary>
        /// The MASApplication scope array as a space seperated string.
        /// </summary>
        public string ScopeAsString { get; private set; }

        /// <summary>
        /// The MASApplication status.
        /// </summary>
        public string Status { get; private set; }

        internal event EventHandler<EventArgs> LoginRequested;

        #region Internal Methods

        internal async Task StartAsync(string configContent, RegistrationKind regKind)
        {
            Logger.LogInfo("Framework starting...");

            // Load and validate configuration data
            var config = new Configuration();
            config.Load(configContent);
            _config = config;

            var clientId = config.DefaultClientId;

            Organization = config.OAuth.Client.Organization;
            Name = config.OAuth.Client.ClientName;
            DetailedDescription = config.OAuth.Client.Description;

            Identifier = config.DefaultClientId.Id;
            Environment = config.DefaultClientId.Environment;
            RegisteredBy = config.DefaultClientId.RegisteredBy;
            Scope = new List<string>(config.DefaultClientId.Scope.Split(' ')).AsReadOnly();
            ScopeAsString = config.DefaultClientId.Scope;
            Status = config.DefaultClientId.Status;

            RedirectUri = config.DefaultClientId.RedirectUri;

            // Load device and any previous registration info.
            await MASDevice.InitializeAsync(config);

            // Authorization providers not supported yet
            //var response = await MAGRequests.GetAuthorizationProviders(_config, Device);

            // Load user and any previous access token or idtoken info
            await MASUser.InitializeAsync(config, MASDevice.Current);

            if (!MASDevice.Current.IsRegistered)
            {
                switch (regKind)
                {
                    case RegistrationKind.Client:
                        if (!config.HasScope(ScopeNames.MssoClientRegister))
                        {
                            ErrorFactory.ThrowError(ErrorCode.DeviceRegistrationAttemptedWithUnregisteredScope);
                        }

                        await MASDevice.Current.RegisterWithClientAsync();

                        // Create a device anonymous user
                        await MASUser.LoginAnonymouslyAsync(config, MASDevice.Current);

                        break;
                    case RegistrationKind.User:
                        // Ask for login with user device registration
                        LoginRequested?.Invoke(null, EventArgs.Empty);
                        break;
                    default:
                        ErrorFactory.ThrowError(ErrorCode.DeviceRegistrationAttemptedWithUnregisteredScope);
                        break;

                }
            }

            Logger.LogInfo("Framework started");
        }

        internal async Task ResetAsync()
        {
            Logger.LogInfo("Framework reseting...");

            MASDevice.Reset();

            MASUser.Reset();

            await SecureStorage.ResetAsync();

            await CertManager.UninstallAsync();

            HttpRequestFactory.CancelAll();

            Logger.LogInfo("Framework reset");
        }

        #endregion

        internal Configuration Config
        {
            get
            {
                return _config;
            }
        }

        Configuration _config;
    }
}

