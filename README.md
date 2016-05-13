### Starting SDK ###

We should always start the SDK with [MAS.StartAsync](@ref MASFoundation::MAS::StartAsync) or [MAS.StartWithConfigAsync](@ref MASFoundation::MAS::StartWithConfigAsync).  This will load the configuration file and attempt to do device registration if it hasn't been done already.

The `StartAsync` must be used with a msso_config.json file with the file name specified in property [MAS.ConfigFileName](@ref MASFoundation::MAS::ConfigFileName).  `StartWithConfigAsync` is just like `StartAsync` but allows the caller to pass in the contents of the configuration file instead.


~~~~~~~~~~~~~~~{.cs}
try
{
	await MAS.StartAsync();
}
catch (Exception exp)
{
	Debug.WriteLine("Start failed " + exp.ToString());
}
~~~~~~~~~~~~~~~


`MAS.StartAsync` call can start two different registration methods: user and client.  User is the default registration method.  Client is used for device and user anonymous registration.  This option is set with property [MAS.RegistrationKind](@ref MASFoundation::MAS::RegistrationKind).  It must be set before `StartAsync` is called.

### User registration flow ###

1. If the configuration file does not include a client secret, the start function will attempt to retrieve client credentials.  If no credentials are found locally a request to the server will be made.
2. Next the device registration is attempted if this has not been done previously.
	+ The SDK will fire the event `LoginRequested`.  The application should listen to this event to prompt for user credentials.
	+ Once the application has user credentials, it should call `MASUser.LoginAsync` to complete the user registration.
	+ The SDK will then store device registration information and user access token. It will automatically refresh the access token as needed.
3. If we have previously registered, registration and user access tokens will be restored.

### Client registration flow ###

1. If the configuration file does not include a client secret, the start function will attempt to retrieve client credentials.  If no credentials are found locally a request to the server will be made.
2. Next the device registration is attempted if this has not been done previously.
	+ After client registration, an anonymous login is attempted.
	+ The SDK will then store device registration information and access token.
3. If we have previously registered, registration and access token will be restored.


### After succesful registration ###

After succesful registration, applications can:
+ Access the current user and device.  [MASUser.Current](@ref MASFoundation::MASUser::Current) and [MASDevice.Current](@ref MASFoundation::MASDevice::Current).
+ Make authenticated network requests with [MAS.GetFromAsync](@ref MASFoundation::MAS::GetFromAsync), [MAS.DeleteFromAsync](@ref MASFoundation::MAS::DeleteFromAsync), [MAS.PostToAsync](@ref MASFoundation::MAS::PostToAsync), [MAS.PutToAsync](@ref MASFoundation::MAS::PutToAsync), and [MAS.PatchToAsync](@ref MASFoundation::MAS::PatchToAsync)
+ Logout and unregister the device.  [MASDevice.Current.LogoutAsync](@ref MASFoundation::MASDevice::LogoutAsync) and [MASDevice.Current.LogoutAsync](@ref MASFoundation::MASDevice::UnregisterAsync)
+ Get user information and logoff the user.  [MASUser.Current.GetInfoAsync](@ref MASFoundation::MASUser::GetInfoAsync) and [MASUser.Current.LogoffAsync](@ref MASFoundation::MASUser::LogoffAsync)
	
### Debugging ###

Applications can enable debug logging by setting the [MAS.LogLevel](@ref MASFoundation::MAS::LogLevel) and subscribing to event [MAS.LogMessage](@ref MASFoundation::MAS::LogMessage).

~~~~~~~~~~~~~~~{.cs}
MAS.LogLevel = LogLevel.Full;
MAS.LogMessage += MAS_LogMessage;

private void MAS_LogMessage(object sender, string e)
{
	// e contains the log message sent from MASFoundation SDK

}
~~~~~~~~~~~~~~~

Messages sent with `LogMessage` event show detailed SDK logging including HTTP request and response messages.  These messages can aid in debugging API and service request errors.  Logging is turned off by default with `MAS.LogLevel` set to `LogLevel.None`.

### Error handling ###

Most Async functions can throw errors, it is a good idea to use try / catch for C#.  And for JavaScript pass an error function to then or done.  See below for examples.

In Javascript, you can also get the full error description, [ErrorInfo](@ref MASFoundation::ErrorInfo), by passing the exception number to [MAS.ErrorLookup](@ref MASFoundation::MAS::ErrorLookup)
~~~~~~~~~~~~~~~{.js}
MASFoundation.MAS.startWithConfigAsync(configContent).done(function () {
	self._onLogMessage("Started!");
}, function (error) {
	var errorInfo = MASFoundation.MAS.errorLookup(error.number);
	
	...
});
~~~~~~~~~~~~~~~

C# has similar method except the exception property is named HResult instead.
~~~~~~~~~~~~~~~{.cs}
try
{
    await MAS.StartAsync();
}
catch (Exception exp)
{
    var errorInfo = MAS.ErrorLookup(exp.HResult);
    
	...
}
~~~~~~~~~~~~~~~

