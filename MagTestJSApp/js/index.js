window.onload = function () {
    MASFoundation.MAS.configFileName = "msso_config.json";
    MASFoundation.MAS.registrationKind = MASFoundation.RegistrationKind.client;
    MASFoundation.MAS.logLevel = MASFoundation.LogLevel.full;
    var indexController = new IndexController();
};
var IndexController = (function () {
    function IndexController() {
        this.userName = "zoljo01";
        this.password = "IdentityMine";
        this.debugText = document.getElementById("debugText");
        this.init();
    }
    IndexController.prototype.init = function () {
        var self = this;
        var registerButton = document.getElementById("registerButton");
        var loginButton = document.getElementById("loginButton");
        var unregisterButton = document.getElementById("unregisterButton");
        var logoffButton = document.getElementById("logoffButton");
        var userInfoButton = document.getElementById("userInfoButton");
        var resetButton = document.getElementById("resetButton");
        registerButton.onclick = function () {
            //MASFoundation.MAS.startAsync().done(function () {
            //    self._onLogMessage("Started!")
            //}, function (error) {
            //    var errorInfo: MASFoundation.ErrorInfo = MASFoundation.MAS.errorLookup(error.number);
            //    self._onLogMessage("Start failed! " + errorInfo.text);
            //});
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
        };
        loginButton.onclick = function () {
            MASFoundation.MASUser.loginAsync(self.userName, self.password).done(function () {
                self._onLogMessage("User logged in!");
            }, function (error) {
                var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                self._onLogMessage("User login failed! " + errorInfo.text);
            });
        };
        unregisterButton.onclick = function () {
            if (MASFoundation.MASDevice.current) {
                MASFoundation.MASDevice.current.unregisterAsync().done(function () {
                    self._onLogMessage("Device unregistered!");
                }, function (error) {
                    var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                    self._onLogMessage("Device unregister failed! " + errorInfo.text);
                });
            }
            else {
                self._onLogMessage("Device not registered!");
            }
        };
        logoffButton.onclick = function () {
            if (MASFoundation.MASUser.current) {
                MASFoundation.MASUser.current.logoffAsync().done(function () {
                    self._onLogMessage("User logged off!");
                }, function (error) {
                    var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                    self._onLogMessage("User logged off failed! " + errorInfo.text);
                });
            }
            else {
                self._onLogMessage("No current user!");
            }
        };
        userInfoButton.onclick = function () {
            if (MASFoundation.MASUser.current) {
                MASFoundation.MASUser.current.getInfoAsync().done(function (info) {
                    self._onLogMessage("User info returned!");
                }, function (error) {
                    var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                    self._onLogMessage("User info failed! " + errorInfo.text);
                });
            }
            else {
                self._onLogMessage("No current user!");
            }
        };
        resetButton.onclick = function () {
            MASFoundation.MAS.resetAsync().done(function () {
                self._onLogMessage("MAS reset!");
            }, function (error) {
                var errorInfo = MASFoundation.MAS.errorLookup(error.number);
                console.error("MAS reset failed! " + errorInfo.text);
            });
        };
        self._onLogMessageBind = self._onLogMessage.bind(self);
        MASFoundation.MAS.addEventListener("logmessage", self._onLogMessageBind);
    };
    IndexController.prototype._onLogMessage = function (message) {
        this.debugText.textContent += message + "\n";
        this.debugText.scrollTop = this.debugText.scrollHeight;
    };
    return IndexController;
}());
