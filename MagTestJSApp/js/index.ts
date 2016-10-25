window.onload = () => {
    MASFoundation.MAS.configFileName = "msso_config.json";
    MASFoundation.MAS.registrationKind = MASFoundation.RegistrationKind.client;
    MASFoundation.MAS.logLevel = MASFoundation.LogLevel.full;
    var indexController: IndexController = new IndexController();
};

class IndexController {

    userName: string = "zoljo01";
    password: string = "IdentityMine";
    debugText: HTMLElement;

    constructor() {

        this.debugText = document.getElementById("debugText");

        this.init();

    }

    init(): void {

        var self: IndexController = this;

        var registerButton: HTMLElement = document.getElementById("registerButton");
        var loginButton: HTMLElement = document.getElementById("loginButton");
        var unregisterButton: HTMLElement = document.getElementById("unregisterButton");
        var logoffButton: HTMLElement = document.getElementById("logoffButton");
        var userInfoButton: HTMLElement = document.getElementById("userInfoButton");
        var resetButton: HTMLElement = document.getElementById("resetButton");

        registerButton.onclick = () => {
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
        }

        loginButton.onclick = () => {
            MASFoundation.MASUser.loginAsync(self.userName, self.password).done(function () {
                self._onLogMessage("User logged in!");
            }, function (error) {
                var errorInfo: MASFoundation.ErrorInfo = MASFoundation.MAS.errorLookup(error.number);
                self._onLogMessage("User login failed! " + errorInfo.text);
            });
        }

        unregisterButton.onclick = () => {
            if (MASFoundation.MASDevice.current) {
                MASFoundation.MASDevice.current.unregisterAsync().done(function () {
                    self._onLogMessage("Device unregistered!");
                }, function (error) {
                    var errorInfo: MASFoundation.ErrorInfo = MASFoundation.MAS.errorLookup(error.number);
                    self._onLogMessage("Device unregister failed! " + errorInfo.text);
                });
            }
            else {
                self._onLogMessage("Device not registered!");
            }
        };

        logoffButton.onclick = () => {
            if (MASFoundation.MASUser.current) {
                MASFoundation.MASUser.current.logoffAsync().done(function () {
                    self._onLogMessage("User logged off!");
                }, function (error) {
                    var errorInfo: MASFoundation.ErrorInfo = MASFoundation.MAS.errorLookup(error.number);
                    self._onLogMessage("User logged off failed! " + errorInfo.text);
                });
            }
            else {
                self._onLogMessage("No current user!");
            }
        }

        userInfoButton.onclick = () => {
            if (MASFoundation.MASUser.current) {
                MASFoundation.MASUser.current.getInfoAsync().done(function (info) {
                    self._onLogMessage("User info returned!");
                }, function (error) {
                    var errorInfo: MASFoundation.ErrorInfo = MASFoundation.MAS.errorLookup(error.number);
                    self._onLogMessage("User info failed! " + errorInfo.text);
                });
            }
            else {
                self._onLogMessage("No current user!");
            }
        }

        resetButton.onclick = () => {
            MASFoundation.MAS.resetAsync().done(function () {
                self._onLogMessage("MAS reset!")
            }, function (error) {
                var errorInfo: MASFoundation.ErrorInfo = MASFoundation.MAS.errorLookup(error.number);
                console.error("MAS reset failed! " + errorInfo.text);
            });
        }


        self._onLogMessageBind = self._onLogMessage.bind(self);
        MASFoundation.MAS.addEventListener("logmessage", self._onLogMessageBind);


    }

    _onLogMessageBind: any;
    _onLogMessage(message: string): void {
        this.debugText.textContent += message + "\n";
        this.debugText.scrollTop = this.debugText.scrollHeight;
    }
}