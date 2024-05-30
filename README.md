# FlaUIServer

Experimental implementation of Selenium like server using FlaUI. Inspired by [FlaUI.WebDriver](https://github.com/FlaUI/FlaUI.WebDriver).

Compatible API with [Appium Windows Driver](https://github.com/appium/appium-windows-driver), for seamless switching between WinAppDriver and FlaUI. Custom client required.

Not all endpoints are implemented.

## Run server

.\FlaUIServer.exe --urls=http://127.0.0.1:4723 --log-response-body --use-swagger

Additional arguments:

* --use-swagger - Enables swagger by default is disabled
* --allow-powershell - Allows powershell execution by default is disabled
* --log-response-body - Enables logging of request/response body to console
* --cleanup-cycle=60 - Delay between cleaning of inactive session default value is 90 seconds, 0 disables cleanup service
