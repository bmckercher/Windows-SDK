# Windows Mobile SDK
## for CA Mobile API Gateway

## Overview
The Windows Mobile SDK gives developers simple and secure access to the services of CA Mobile API Gateway. 
For more information about our mobile products see [the website][mas.ca.com].

***UNSUPPORTED VERSION***

This SDK is NOT been supported by CA at this time.

## Get Started
Follow our step-by-step guide to [get started][get-started].

To build the SDK follow these steps

1. Install Visual Studio 2010. Make sure that 'Universial Windows App Development Tools' is included in your install.
2. Download the latest version of doxygen binaries from [here][doxygen]. Doxygen utility is used for generating documentation from source code. Make sure that there are four binaries in the archieve viz. doxygen.exe, doxyindexer.exe, doxysearch.cgi.exe, libclang.dll.
3. Create a folder "doxygen" in project home folder. Extract all the four binaries to the empty folder "doxygen" in the project.
4. Open MagTestApp solution and build.
5. Depending on your configuration (Debug or Release) and CPU.  The build output folder will be different.  
	+ MASFoundation SDK output folder has this relative location: "MASFoundation\bin\'CPU'\'Debug or Release'".
	+ This folder will include .winmd and generated documentation in "Docs\html".  The .winmd file contains both metadata and implementation.  This can be used as a reference for other projects.
	+ MASTestApp is a sample application showing usage of the MASFoundation SDK.

	
### Unit testing ###

MASUnitTestApp contains MASFoundation unit tests in file MASFoundationTests.cs.  These tests can be run via visual studio or by command line.

To run with Visual Studio:
1. Click on the Test menu
2. Then select Windows, Test Explorer
3. Test Explorer will open and search for tests in the MASUnitTestApp project.
4. Once all tests are found, select Run All to run all the found tests.
5. Progress and results are reported as tests are run.

To run via command line
1. Start Visual studio 2015 command prompt
2. First build MASUnitTestApp with msbuild: msbuild MASUnitTestApp.csproj.  You can pass in additional arguments to specify Debug or Release and CPU type.
3. Install MASUnitTestApp's certificate 'MASUnitTestApp_TemporaryKey.pfx' to your computer's Trusted Root Certificate Authorities store
4. Run vstest.console.exe 'Path to the appx of MASUnitTestApp'  For example: vstest.console.exe "C:\GitHub\Windows-SDK\MASUnitTestApp\AppPackages\MASUnitTestApp_1.0.0.0_x86_Debug_Test\MASUnitTestApp_1.0.0.0_x86_Debug.appx" 
5. Progress and results are reported as tests are run.


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
+ Unregister the device.  [MASDevice.Current.UnregisterAsync](@ref MASFoundation::MASDevice::UnregisterAsync)
+ Get user information and logoff the user.  [MASUser.Current.GetInfoAsync](@ref MASFoundation::MASUser::GetInfoAsync) and [MASUser.Current.LogoffAsync](@ref MASFoundation::MASUser::LogoffAsync)
	
### Mobile Single Sign On ###

The SDK supports mobile single sign on for applications that have the same publisher.  This means that two applications from the same publisher can share the same login.

1. User signs into application A
2. User then starts application B and both application A and B are from the same publisher.  The user is automatically signed in to application B.

To enable this functionality both applications need to have the the following extension set in their package.appxmanifest.  See the following MSDN link for more information on shared cache folders: https://msdn.microsoft.com/en-us/library/windows/apps/windows.storage.applicationdata.getpublishercachefolder.aspx

~~~~~~~~~~~~~~~{.xml}
  <Extensions>
    <Extension Category="windows.publisherCacheFolders">
      <PublisherCacheFolders>
        <Folder Name="keys" />
      </PublisherCacheFolders>
    </Extension>
  </Extensions>
~~~~~~~~~~~~~~~

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

## How You Can Contribute

Contributions are welcome and much appreciated. To learn more, see the [Contribution Guidelines][contributing].


## License

Copyright (c) 2016 CA. All rights reserved.

This software may be modified and distributed under the terms
of the MIT license. See the [LICENSE][license-link] file for details.

[mas.ca.com]: http://mas.ca.com/
[get-started]: http://mas.ca.com/get-started
[contributing]: /CONTRIBUTING.md
[license-link]: /LICENSE
[doxygen]: https://sourceforge.net/projects/doxygen/files
