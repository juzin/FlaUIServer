# FlaUI Server

Experimental implementation of Selenium like server using FlaUI. Server side of Windows application automation. Inspired by [FlaUI.WebDriver](https://github.com/FlaUI/FlaUI.WebDriver).

Compatible API with [Appium Windows Driver](https://github.com/appium/appium-windows-driver), for seamless switching between WinAppDriver and FlaUI. Custom client required.

Not all endpoints are implemented.

## Installation

- Install dotnet 8 sdk [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Download release zip file
- Unzip files

## Run server

.\FlaUIServer.exe --urls=http://127.0.0.1:4723 --log-response-body --use-swagger

Additional arguments:

* --use-swagger - Enables swagger by default is disabled
* --allow-powershell - Allows powershell execution by default is disabled
* --log-response-body - Enables logging of request/response body to console
* --cleanup-cycle=60 - Delay between cleaning of inactive session default value is 90 seconds, 0 disables cleanup service

## Start session

POST request to /wd/hub/session

When starting session there needs to be specified path to application executable (appium:app) or application process id (appium:appTopLevelWindow). For desktop session use "Root" in appium:app

```json
{
  "capabilities": {
    "alwaysMatch": {
      "appium:app": "C:\\Test\\Application.exe",
      "appium:appTopLevelWindow": "1234546456"
    }
  }
}
```

## Implemented endpoints

Use swagger to check endpoint examples

| Method | Path | Description |
|---|---|---|
| GET | /status | Get server status |
| POST | /session | Create new session |
| DELETE | /session/{sessionId} | Delete session |
| GET | /session/{sessionId}/source | Get app xml source |
| POST | /session/{sessionId}/window | Switch to window |
| DELETE | /session/{sessionId}/window | Close active window |
| POST | /session/{sessionId}/execute | Execute script or gesture |
| POST | /session/{sessionId}/keys | Keyboard action |
| POST | /session/{sessionId}/element | Find element |
| POST | /session/{sessionId}/element/{elementId}/element | Find element in parent element |
| POST | /session/{sessionId}/element/{elementId}/elements | Find elements in parent element |
| POST | /session/{sessionId}/elements | Find elements |
| POST | /session/{sessionId}/element/{elementId}/click | Click on element |
| POST | /session/{sessionId}/element/{elementId}/value | Fill element text |
| POST | /session/{sessionId}/element/{elementId}/clear | Clear element text |
| GET | /session/{sessionId}/title | Get active window title |
| GET | /session/{sessionId}/window/rect | Get active window size and location |
| GET | /session/{sessionId}/window_handle | Get active window handle |
| GET | /session/{sessionId}/window_handles | Get all available window handles |
| GET | /session/{sessionId}/element/{elementId}/text | Get element text |
| GET | /session/{sessionId}/element/{elementId}/displayed | Check if element is displayed |
| GET | /session/{sessionId}/element/{elementId}/enabled | Check that element is enabled |
| GET | /session/{sessionId}/element/{elementId}/selected | Check that element is selected |
| GET | /session/{sessionId}/element/{elementId}/rect | Get element size and location |
| GET | /session/{sessionId}/element/{elementId}/attribute/{name} | Get element attribute value |
| GET | /session/{sessionId}/screenshot | Take application screenshot |
| GET | /sessions | Get server active sessions (this endpoint is without '/wd/hub' path) |

## Gestures

POST /session/{sessionId}/execute

Send gesture (script) and string representation of json object as first member of args array

```json
{
  "script": "string",
  "args": [
    "string"
  ]
}
```

| Gesture | Parameters | Description |
|---|---|---|
| powerShell | - Commad - One line command - Script - Multiple line script | Execute powershell script or command |
| windows: click | - X</br> - Y </br>- Button - "left" \| "right" \| "middle" \| "back" \| "forward"</br> - Times - Number of clicks</br> - InterClickDelayMs - Delay between clicks in milliseconds | Click, multiclick on coordinates |
| windows: clickAndDrag | - StartX</br> - StartY</br> - EndX</br> - EndY | Drag from coordinates to coordinates |
| windows: hover | - StartX</br> - StartY</br> - EndX</br> - EndY | Move mouse cursor to coordinates |
| windows: scroll | - X</br> - Y</br> - DeltaX</br>  - DeltaY | Mouse scroll |
| windows: setClipboard | - ContentType - "plaintext" \| "image"</br> - B64Content - If content type is "image" content is base64 encoded string otherwise string | Set text or image to clipboard |
|  windows: getClipboard | - ContentType - "plaintext" \| "image"</br> - B64Content - If content type is "image" content is base64 encoded string otherwise string | Get text or image from clipboard |