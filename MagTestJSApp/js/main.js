﻿/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/


// Your code here!

(function () {

    var self = {

        start: function () {

            MASFoundation.MAS.registrationKind = MASFoundation.RegistrationKind.user;
            MASFoundation.MAS.logLevel = MASFoundation.LogLevel.full;

            window.onload = function (e) {

                self._debugText = document.querySelector(".debug-text-area");

                self._addClickHandler("start-button", "start", function () {

                    var storage = Windows.Storage;

                    var configFileLoadPromise = storage.StorageFile.getFileFromApplicationUriAsync(new Windows.Foundation.Uri("ms-appx:///my_msso_config.json")).
                        then(function (file) {
                            return storage.FileIO.readTextAsync(file);
                        });

                    configFileLoadPromise.done(function (configContent) {
                        MASFoundation.MAS.startWithConfigAsync(configContent).done(function () {
                            self._onLogMessage("Started!");
                        }, function (error) {
                            var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                            self._onLogMessage("Start failed! " + error.message);
                        });
                    });

                });

                self._addClickHandler("login-button", "login", function() {
                    MASFoundation.MASUser.loginAsync("zoljo01", "IdentityMine").done(function () {
                        self._onLogMessage("User logged in!");
                    }, function (error) {
                        var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                        self._onLogMessage("User login failed! " + error.message);
                    });
                });

                self._addClickHandler("unregister-button", "unregister", function () {
                    if (MASFoundation.MASDevice.current) {
                        MASFoundation.MASDevice.current.unregisterAsync().done(function () {
                            self._onLogMessage("Device unregistered!");
                        }, function (error) {
                            var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                            self._onLogMessage("Device unregister failed! " + error.message);
                        });
                    }
                    else {
                        self._onLogMessage("Device not registered!");
                    }
                });

                self._addClickHandler("user-logoff-button", "logoff", function () {

                    if (MASFoundation.MASUser.current) {
                        MASFoundation.MASUser.current.logoffAsync().done(function () {
                            self._onLogMessage("User logged off!");
                        }, function (error) {
                            var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                            self._onLogMessage("User logged off failed! " + error.message);
                        });
                    }
                    else {
                        self._onLogMessage("No current user!");
                    }
                });

                self._addClickHandler("user-info-button", "userinfo", function () {
                    if (MASFoundation.MASUser.current) {
                        MASFoundation.MASUser.current.getInfoAsync().done(function (info) {
                            self._onLogMessage("User info returned!");
                        }, function (error) {
                            var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                            self._onLogMessage("User info failed! " + error.message);
                        });
                    }
                    else {
                        self._onLogMessage("No current user!");
                    }
                });

                self._addClickHandler("reset-button", "reset", function () {
                    MASFoundation.MAS.resetAsync().done(function (info) {
                        self._onLogMessage("MAS reset!")
                    }, function (error) {
                        var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                        console.error("MAS reset failed! " + error.message);
                    });
                });

                self._addClickHandler("postcode-1-button", "postcode1", function () {
                    var code = "DM001PL";

                    var params = new MASFoundation.PropertyCollection();
                    params.add("zzz", "woohoo");
                    params.add("dummy", "3423");
                    params.add("dummy", "1234");

                    var headers = new MASFoundation.PropertyCollection();
                    headers.add("zzz", "woohoo");
                    headers.add("dummy", "2345");

                    MASFoundation.MAS.getFromAsync("https://test.pulsenow.co.uk/mobile/customerinfo/v2/postcode/" + code,
                        params, headers, MASFoundation.ResponseType.json).then(function (result) {
                            self._onLogMessage("Got postcode")
                        }, function (error) {
                            var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                            self._onLogMessage("Got postcode failed! " + error.message)
                        });
                });

                self._addClickHandler("postcode-2-button", "postcode2", function () {
                    var code = "DM002PL";
                    MASFoundation.MAS.getFromAsync("https://test.pulsenow.co.uk/mobile/customerinfo/v2/postcode/" + code,
                        null, null, MASFoundation.ResponseType.json).then(function (result) {
                            self._onLogMessage("Got postcode")
                        }, function (error) {
                            var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                            self._onLogMessage("Got postcode failed! " + error.message)
                        });
                });

                self._addClickHandler("postcode-3-button", "postcode3", function () {
                    var code = "DM003PL";
                    MASFoundation.MAS.getFromAsync("https://test.pulsenow.co.uk/mobile/customerinfo/v2/postcode/" + code,
                        null, null, MASFoundation.ResponseType.json).then(function (result) {
                            self._onLogMessage("Got postcode")
                        }, function (error) {
                            var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                            self._onLogMessage("Got postcode failed! " + error.message)
                        });
                });

                self._onLoginRequestedBind = self._onLoginRequested.bind(self);
                MASFoundation.MAS.addEventListener("loginrequested", self._onLoginRequestedBind);

                self._onLogMessageBind = self._onLogMessage.bind(self);
                MASFoundation.MAS.addEventListener("logmessage", self._onLogMessageBind);
            };
        },

        _addClickHandler: function(classname, name, handler) {
            var btn = document.querySelector("." + classname);
            if (btn) {
                var handlerBind = handler.bind(this);
                self["_on" + name + "bind"] = handlerBind;
                btn.addEventListener("click", handlerBind);
            }
        },

        _onLoginRequested: function () {
            MASFoundation.MASUser.loginAsync("zoljo01", "IdentityMine").done(function () {
                self._onLogMessage("User logged in!");
            }, function (error) {
                self._onLogMessage("User login failed!");
            });
        },

        _onLogMessage: function (message) {
            this._debugText.textContent += message + "\n";
            this._debugText.scrollTop = this._debugText.scrollHeight;
        }
    };

self.start();

})();