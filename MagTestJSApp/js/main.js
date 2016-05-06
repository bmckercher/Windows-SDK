// Your code here!

(function () {

    var self = {

        start: function () {

            MASFoundation.MAS.registrationKind = MASFoundation.RegistrationKind.user;
            MASFoundation.MAS.logLevel = MASFoundation.LogLevel.full;

            window.onload = function (e) {

                self._addClickHandler("start-button", "start", function() {
                    MASFoundation.MAS.startAsync().done(function () {
                        console.log("Started!");
                    }, function (error) {
                        console.error("Start failed!");
                    });
                });

                self._addClickHandler("login-button", "login", function() {
                    MASFoundation.User.loginAsync("winsdktest2", "P@$$w0rd01").done(function () {
                        console.log("User logged in!");
                    }, function (error) {
                        console.log("User login failed!");
                    });
                });

                self._addClickHandler("unregister-button", "unregister", function () {
                    if (MASFoundation.Device.current) {
                        MASFoundation.Device.current.unregisterAsync().done(function () {
                            console.log("Device unregistered!");
                        }, function (error) {
                            console.log("Device unregister failed!");
                        });
                    }
                    else {
                        console.log("Device not registered!");
                    }
                });

                self._addClickHandler("device-logout-button", "logout", function () {

                    if (MASFoundation.Device.current) {
                        MASFoundation.Device.current.logoutAsync().done(function () {
                            console.log("Device logged out!");
                        }, function (error) {
                            console.log("Device logged out failed!");
                        });
                    }
                    else {
                        console.log("Device not registered!");
                    }
                });

                self._addClickHandler("user-logoff-button", "logoff", function () {

                    if (MASFoundation.User.current) {
                        MASFoundation.User.current.logoffAsync().done(function () {
                            console.log("User logged off!");
                        }, function (error) {
                            console.log("User logged off failed!");
                        });
                    }
                    else {
                        console.log("No current user!");
                    }
                });

                self._addClickHandler("user-info-button", "userinfo", function () {
                    if (MASFoundation.User.current) {
                        MASFoundation.User.current.getInfoAsync().done(function (info) {
                            console.log("User info returned!");
                        }, function (error) {
                            console.log("User info failed!");
                        });
                    }
                    else {
                        console.log("No current user!");
                    }
                });

                self._addClickHandler("reset-button", "reset", function () {
                    MASFoundation.MAS.resetAsync().done(function (info) {
                        console.log("MAS reset!");
                    }, function (error) {
                        console.log("MAS reset failed!");
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
            MASFoundation.User.loginAsync("winsdktest2", "P@$$w0rd01").done(function () {
                console.log("User logged in!");
            }, function (error) {
                console.log("User login failed!");
            });
        },

        _onLogMessage: function (message) {
            console.info(message.toString());
        }
    };

self.start();

})();