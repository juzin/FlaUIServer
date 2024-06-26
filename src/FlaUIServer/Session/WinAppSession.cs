﻿using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using FlaUIServer.Enums;
using FlaUIServer.Exceptions;
using FlaUIServer.Extensions;
using FlaUIServer.Models;
using FlaUIServer.Models.Gestures;
using FlaUIServer.Helpers;
using Application = FlaUI.Core.Application;

namespace FlaUIServer.Session;

/// <summary>
/// Represents Windows application automation session
/// </summary>
public sealed class WinAppSession : IDisposable
{
    private readonly ILogger<WinAppSession> _logger;
    private readonly bool _isRootSession;
    private readonly ServerOptions _options;
    private readonly UIA3Automation _automation;
    private readonly Application _application;
    private readonly Dictionary<Guid, AutomationElement> _elements = [];
    private readonly List<VirtualKeyShort> _pressedKeys = [];
    private Window _activeWindow;

    /// <summary>
    /// Active window
    /// </summary>
    private Window ActiveWindow
    {
        get
        {
            if (_activeWindow is not null) return _activeWindow;

            // In case window is null return application main window
            if (_isRootSession)
            {
                _automation.GetDesktop().AsWindow();
            }
            else
            {
                _activeWindow = _application.GetMainWindow(_automation);
            }
            return _activeWindow;
        }
        set => _activeWindow = value;
    }

    /// <summary>
    /// Session id
    /// </summary>
    public Guid SessionId { get; }

    /// <summary>
    /// Date and time of session creation
    /// </summary>
    public DateTimeOffset Created { get; }

    /// <summary>
    /// Date and time of last session action
    /// </summary>
    public DateTimeOffset LastActionAt { get; private set; }

    /// <summary>
    /// Initializes new instance of <see cref="WinAppSession"/>
    /// </summary>
    /// <param name="capabilities">Session capabilities</param>
    /// <param name="loggerFactory">Logger factory</param>
    /// <param name="options">Server options</param>
    public WinAppSession(Capabilities capabilities, ILoggerFactory loggerFactory, ServerOptions options)
    {
        ArgumentNullException.ThrowIfNull(capabilities);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(options);

        _logger = loggerFactory.CreateLogger<WinAppSession>();
        _options = options;
        _automation = new UIA3Automation();

        if (capabilities.AlwaysMatch.ApplicationTopLevelWindow is not null)
        {
            _application = Application.Attach(Convert.ToInt32(capabilities.AlwaysMatch.ApplicationTopLevelWindow, 16));
        }
        else if (capabilities.AlwaysMatch.Application == "Root")
        {
            _application = null;
            ActiveWindow = _automation.GetDesktop().AsWindow();
            _isRootSession = true;
        }
        else
        {
            _application = Application.Launch(capabilities.AlwaysMatch.Application);
            ActiveWindow = _application.GetMainWindow(_automation);
            _isRootSession = false;
        }

        Created = DateTimeOffset.Now;
        SessionId = Guid.NewGuid();
        LastActionAt = DateTimeOffset.Now;
    }

    #region Public Members

    /// <summary>
    /// Find element using locator strategy and selector
    /// </summary>
    /// <param name="searchParams">Locator strategy and selector</param>
    /// <returns>Element id</returns>
    /// <exception cref="ObjectNotFoundException"></exception>
    public Guid FindElement(FindElementRequest searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        var element = FindAutomationElement(ActiveWindow, searchParams);

        if (element is null)
        {
            throw new ObjectNotFoundException(
                $"Element by '{searchParams.Using}' value '{searchParams.Value}' not found");
        }

        var elementId = Guid.NewGuid();
        _elements[elementId] = element;

        return elementId;
    }

    /// <summary>
    /// Find element in element using locator strategy and selector
    /// </summary>
    /// <param name="parentElementId">Parent element id</param>
    /// <param name="searchParams">Locator strategy and selector</param>
    /// <returns>Element id</returns>
    /// <exception cref="ObjectNotFoundException"></exception>
    public Guid ElementFindElement(Guid parentElementId, FindElementRequest searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var parentElement = GetElement(parentElementId);
        var element = FindAutomationElement(parentElement, searchParams);

        if (element is null)
        {
            throw new ObjectNotFoundException(
                $"Element by '{searchParams.Using}' value '{searchParams.Value}' not found in parent element id '{parentElementId}'");
        }

        var elementId = Guid.NewGuid();
        _elements[elementId] = element;

        return elementId;
    }

    /// <summary>
    /// Find elements using locator strategy and selector
    /// </summary>
    /// <param name="searchParams">Locator strategy and selector</param>
    /// <returns>Element id</returns>
    /// <exception cref="ObjectNotFoundException"></exception>
    public Guid[] FindElements(FindElementRequest searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        var elements = FindAutomationElements(ActiveWindow, searchParams);

        if (elements.Length == 0)
        {
            throw new ObjectNotFoundException(
                $"Elements by '{searchParams.Using}' value '{searchParams.Value}' not found");
        }

        List<Guid> elementIds = [];

        foreach (var element in elements)
        {
            var elementId = Guid.NewGuid();
            elementIds.Add(elementId);
            _elements[elementId] = element;
        }

        return elementIds.ToArray();
    }

    /// <summary>
    /// Find elements in element using locator strategy and selector
    /// </summary>
    /// <param name="parentElementId">Parent element id</param>
    /// <param name="searchParams">Locator strategy and selector</param>
    /// <returns>Element ids</returns>
    /// <exception cref="ObjectNotFoundException"></exception>
    public Guid[] ElementFindElements(Guid parentElementId, FindElementRequest searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var parentElement = GetElement(parentElementId);
        var elements = FindAutomationElements(parentElement, searchParams);

        if (elements.Length == 0)
        {
            throw new ObjectNotFoundException(
                $"Elements by '{searchParams.Using}' value '{searchParams.Value}' not found in parent element id {parentElementId}");
        }

        List<Guid> elementIds = [];

        foreach (var element in elements)
        {
            var elementId = Guid.NewGuid();
            elementIds.Add(elementId);
            _elements[elementId] = element;
        }

        return elementIds.ToArray();
    }

    /// <summary>
    /// Click on existing element
    /// </summary>
    /// <param name="elementId">Element id</param>
    public void ElementCLick(Guid elementId)
    {
        var element = GetElement(elementId);
        element.WaitUntilClickable();
        element.AsButton().Click();
    }

    /// <summary>
    /// Fill text to existing element
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <param name="text">Text</param>
    public void ElementFillText(Guid elementId, string text)
    {
        var element = GetElement(elementId);
        element.Focus();
        element.AsTextBox().Enter(text);
    }

    /// <summary>
    /// Clear text in existing element
    /// </summary>
    /// <param name="elementId">Element id</param>
    public void ElementClearText(Guid elementId)
    {
        var element = GetElement(elementId);
        element.AsTextBox().Text = "";
    }

    /// <summary>
    /// Check if existing element is visible and on screen
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <returns>True if element is displayed otherwise false</returns>
    public bool IsElementDisplayed(Guid elementId)
    {
        var element = GetElement(elementId);
        return element.IsAvailable && !element.IsOffscreen;
    }

    /// <summary>
    /// Check if existing element is enabled
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <returns>True if element is enabled</returns>
    public bool IsElementEnabled(Guid elementId)
    {
        var element = GetElement(elementId);
        return element.IsEnabled;
    }

    /// <summary>
    /// Check if element is selected
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <returns>True if element is selected, otherwise false</returns>
    public bool IsElementSelected(Guid elementId)
    {
        var element = GetElement(elementId);

        var isSelected = false;
        if (element.Patterns.SelectionItem.IsSupported)
        {
            isSelected = element.Patterns.SelectionItem.PatternOrDefault.IsSelected.ValueOrDefault;
        }
        else if (element.Patterns.Toggle.IsSupported)
        {
            isSelected = element.Patterns.Toggle.PatternOrDefault.ToggleState.ValueOrDefault == ToggleState.On;
        }

        return isSelected;
    }

    /// <summary>
    /// Get existing element text 
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <returns>Element text</returns>
    public string GetElementText(Guid elementId)
    {
        var element = GetElement(elementId);

        if (element.Patterns.Text.IsSupported)
        {
            return element.Patterns.Text.Pattern.DocumentRange.GetText(int.MaxValue);
        }

        if (element.Patterns.Value.IsSupported)
        {
            return element.Patterns.Value.Pattern.Value.ToString();
        }

        if (element.Patterns.RangeValue.IsSupported)
        {
            return element.Patterns.RangeValue.Pattern.Value.ToString();
        }

        return element.AsTextBox().Text;
    }

    /// <summary>
    /// Get element rectangle
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <returns>Element rectangle</returns>
    public RectangleResponse GetElementRectangle(Guid elementId)
    {
        var element = GetElement(elementId);
        return new RectangleResponse(element.BoundingRectangle.X, element.BoundingRectangle.Y, element.BoundingRectangle.Height, element.BoundingRectangle.Width);
    }

    /// <summary>
    /// Get element attribute value
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <param name="attributeName">Attribute name</param>
    /// <returns>Attribute value</returns>
    public string GetElementAttribute(Guid elementId, string attributeName)
    {
        var element = GetElement(elementId);
        element.TryGetProperty(attributeName, out var value);

        return value?.ToString();
    }

    /// <summary>
    /// Take active window screenshot
    /// </summary>
    /// <returns>Base64 encoded png screenshot string</returns>
    public string GetScreenshot()
    {
        using var result = Capture.MainScreen();
        // using var bitmap = ActiveWindow.Capture();
        using var ms = new MemoryStream();
        // bitmap.Save(ms, ImageFormat.Png);
        result.Bitmap.Save(ms, ImageFormat.Png);

        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// Get active window title
    /// </summary>
    /// <returns>Window title</returns>
    public string WindowTitle => ActiveWindow.Title;

    /// <summary>
    /// Get active window rectangle
    /// </summary>
    /// <returns></returns>
    public RectangleResponse WindowRectangle => new (ActiveWindow.BoundingRectangle.X, ActiveWindow.BoundingRectangle.Y, ActiveWindow.BoundingRectangle.Height, ActiveWindow.BoundingRectangle.Width);

    /// <summary>
    /// Get active window handle
    /// </summary>
    /// <returns>Window handle</returns>
    public string GetWindowHandle()
    {
        return ActiveWindow.GetRuntimeId();
    }

    /// <summary>
    /// Get all window handles
    /// </summary>
    /// <returns>Window handles</returns>
    public string[] GetWindowHandles()
    {
        var windows = _application.GetAllTopLevelWindows(_automation);
        return windows.Select(x => x.GetRuntimeId()).ToArray();
    }

    /// <summary>
    /// Switch to window by handle id
    /// </summary>
    /// <param name="name"></param>
    public void SwitchToWindow(string name)
    {
        var windows = _application.GetAllTopLevelWindows(_automation);
        var window = windows.FirstOrDefault(x => x.GetRuntimeId().Equals(name));

        ActiveWindow = window ?? throw new ObjectNotFoundException($"Window with handle '{name}' not found");
    }

    /// <summary>
    /// Get application source xml
    /// </summary>
    /// <returns>Xml representation of application source</returns>
    public string WindowGetSource()
    {
        return $"<?xml version=\"1.0\" encoding=\"utf-16\"?>{ActiveWindow.ToXml()}";
    }

    /// <summary>
    /// Close window session
    /// </summary>
    public void CloseActiveWindow()
    {
        ActiveWindow.Close();
    }

    /// <summary>
    /// Executes script or gesture
    /// </summary>
    /// <returns>Gesture result or null</returns>
    public async Task<string> ExecuteScript(ExecuteScriptRequest data, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(data);

        switch (data.Script)
        {
            case "powerShell":
                return await ExecutePowershell(GestureRequestHelper.DeserializeGestureRequest<PowerShellCommandRequest>(data.Args), ct);
            case "windows: click":
                await Task.Run(() => ClickGesture(GestureRequestHelper.DeserializeGestureRequest<ClickGestureRequest>(data.Args)), ct);
                break;
            case "windows: clickAndDrag":
                await Task.Run(() => DragAndDropGesture(GestureRequestHelper.DeserializeGestureRequest<MoveGestureRequest>(data.Args)), ct);
                break;
            case "windows: hover":
                await Task.Run(() => HoverGesture(GestureRequestHelper.DeserializeGestureRequest<MoveGestureRequest>(data.Args)), ct);
                break;
            case "windows: scroll":
                await Task.Run(() => ScrollGesture(GestureRequestHelper.DeserializeGestureRequest<ScrollGestureRequest>(data.Args)), ct);
                break;
            case "windows: setClipboard":
                await Task.Run(() => SetClipboard(GestureRequestHelper.DeserializeGestureRequest<ClipboardGestureRequest>(data.Args)), ct);
                break;
            case "windows: getClipboard":
                return await Task.Run(() => GetClipboard(GestureRequestHelper.DeserializeGestureRequest<ClipboardGestureRequest>(data.Args)), ct);
            default:
                throw new NotSupportedException($"Script '{data.Script}' is not supported. Supported gestures 'powerShell', 'windows: click', 'windows: clickAndDrag', 'windows: hover', 'windows: scroll', 'windows: setClipboard', 'windows: getClipboard'");
        }

        return null;
    }

    /// <summary>
    /// Type using keyboard. Modifier key is pressed until same key release it or is released at the end
    /// </summary>
    /// <param name="keys">Request with keys to type</param>
    public void KeyboardType(KeyInputRequest keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        foreach (var key in keys.Value)
        {
            var modifierKey = KeyboardHelper.GetModifierKey(key);

            if (modifierKey is not null)
            {
                // Press modifier key, if same key is present second time release it
                if (_pressedKeys.Contains(modifierKey.Value))
                {
                    _logger.LogDebug("Release key: {Key}", modifierKey.Value.ToString());
                    Keyboard.Release(modifierKey.Value);
                    _pressedKeys.Remove(modifierKey.Value);
                }
                else
                {
                    _logger.LogDebug("Press key: {Key}", modifierKey.Value.ToString());
                    Keyboard.Press(modifierKey.Value);
                    _pressedKeys.Add(modifierKey.Value);
                }
            }
            //Null key - release all
            else if (key == '\uE000')
            {
                ReleaseAllKeys();
            }
            else
            {
                _logger.LogDebug("Type: {Key}", key);
                Keyboard.Type(key);
            }
        }
    }

    /// <summary>
    /// Close application
    /// </summary>
    public void Close()
    {
        if (!_isRootSession && _application != null && !_application.HasExited)
        {
            _application.Close();
        }
    }

    /// <summary>
    /// Updates last action time
    /// </summary>
    public void UpdateLastActionTime()
    {
        LastActionAt = DateTimeOffset.Now;
    }

    /// <summary>
    /// Dispose session
    /// </summary>
    public void Dispose()
    {
        _elements.Clear();
        _application?.Dispose();
        _automation.Dispose();
    }

    #endregion

    #region Private Members

    private static AutomationElement FindAutomationElement(AutomationElement parent, FindElementRequest searchParams)
    {
        return searchParams.FindBy switch
        {
            FindBy.AutomationId => parent.FindFirstDescendant(x => x.ByAutomationId(searchParams.Value)),
            FindBy.ClassName => parent.FindFirstDescendant(x => x.ByClassName(searchParams.Value)),
            FindBy.TagName => parent.FindFirstDescendant(x => x.ByControlType(Enum.Parse<ControlType>(searchParams.Value))),
            FindBy.Name => parent.FindFirstDescendant(x => x.ByName(searchParams.Value)),
            FindBy.Xpath => parent.FindFirstByXPath(searchParams.Value),
            _ => throw new NotSupportedException(
                $"Find by '{searchParams.FindBy}' is not supported, supported strategies are 'AutomationId', 'ClassName', 'TagName', 'Name', 'Xpath'")
        };
    }

    private static AutomationElement[] FindAutomationElements(AutomationElement parent, FindElementRequest searchParams)
    {
        return searchParams.FindBy switch
        {
            FindBy.AutomationId => parent.FindAllDescendants(x => x.ByAutomationId(searchParams.Value)),
            FindBy.ClassName => parent.FindAllDescendants(x => x.ByClassName(searchParams.Value)),
            FindBy.TagName => parent.FindAllDescendants(x => x.ByControlType(Enum.Parse<ControlType>(searchParams.Value))),
            FindBy.Name => parent.FindAllDescendants(x => x.ByName(searchParams.Value)),
            FindBy.Xpath => parent.FindAllByXPath(searchParams.Value),
            _ => throw new NotSupportedException(
                $"Find by '{searchParams.FindBy}' is not supported, supported strategies are 'AutomationId', 'ClassName', 'TagName', 'Name', 'Xpath'")
        };
    }

    /// <summary>
    /// Get element from collection by id
    /// </summary>
    /// <param name="elementId">Element id</param>
    /// <returns>Element</returns>
    /// <exception cref="ObjectNotFoundException">When element is not found in collection</exception>
    private AutomationElement GetElement(Guid elementId)
    {
        if (_elements.TryGetValue(elementId, out var element))
        {
            return element;
        }

        throw new ObjectNotFoundException($"Element with '{elementId}' not found");
    }

    private void ClickGesture(ClickGestureRequest data)
    {
        _logger.LogDebug("Click gesture x: {X}, y: {Y}, click times: {Times}, inter click delay: {InterClickDelay} ms", data.X, data.Y, data.Times, data.InterClickDelayMs);

        var button = data.MouseButton;
        Mouse.MoveTo(data.X, data.Y);
        Wait.UntilInputIsProcessed();

        for (var i = 0; i < data.Times; i++)
        {
            Mouse.Down(button);
            Mouse.Up(button);
            Thread.Sleep(data.InterClickDelayMs);
        }
    }

    private void HoverGesture(MoveGestureRequest data)
    {
        _logger.LogDebug("Move to gesture x: {X}, y: {Y}", data.EndX, data.EndY);
        Mouse.MoveTo(data.EndX, data.EndY);
    }

    private void DragAndDropGesture(MoveGestureRequest data)
    {
        _logger.LogDebug("Drag and drop gesture startX: {StartX}, startY: {StartY}, endX: {EndX}, endY: {EndY}", data.StartX, data.StartY, data.EndX, data.EndY);
        Mouse.MoveTo(data.StartX, data.StartY);
        Wait.UntilInputIsProcessed();
        Mouse.Down();
        Wait.UntilInputIsProcessed();
        Mouse.MoveTo(data.EndX, data.EndY);
        Wait.UntilInputIsProcessed();
        Mouse.Up();
        Wait.UntilInputIsProcessed();
    }

    private void ScrollGesture(ScrollGestureRequest data)
    {
        if (data.DeltaX is null && data.DeltaY is null)
        {
            throw new RequestValidationException("Body parameter DeltaX or DeltaY must be specified");
        }
        if (data.DeltaX is not null && data.DeltaY is not null)
        {
            throw new RequestValidationException("Only one body parameter DeltaX or DeltaY cab be specified");
        }

        _logger.LogDebug("Moving to X: {X}, Y: {Y}, Scroll by DeltaX: {DeltaX}, DeltaY: {DeltaY}", data.X, data.Y, data.DeltaX, data.DeltaY);
        Mouse.MoveTo(data.X, data.Y);
        Wait.UntilInputIsProcessed();

        if (data.DeltaX is not null)
        {
            Mouse.Scroll(data.DeltaX.Value);
            Wait.UntilInputIsProcessed();
        }
        else if(data.DeltaY is not null)
        {
            Mouse.HorizontalScroll(data.DeltaY.Value);
            Wait.UntilInputIsProcessed();
        }
    }

    private void SetClipboard(ClipboardGestureRequest data)
    {
        if (data.ContentType != "plaintext" 
            && data.ContentType != "image")
        {
            throw new RequestValidationException($"Body parameter ContentType value must be either 'plaintext' or 'image', received '{data.ContentType}'");
        }

        _logger.LogDebug("Set clipboard content type: {ContentType}", data.ContentType);

        if (data.ContentType == "plaintext")
        {
            Clipboard.SetText(data.Base64Content);
        }
        else if (data.ContentType == "image")
        {
            if (data.Base64Content is null)
            {
                throw new RequestValidationException("Body parameter B64Content is null");
            }
            var bytes = Convert.FromBase64String(data.Base64Content);
            using var ms = new MemoryStream(bytes);
            using var image = Image.FromStream(ms);
            Clipboard.SetImage(image);
        }
    }

    private string GetClipboard(ClipboardGestureRequest data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.ContentType != "plaintext" 
            && data.ContentType != "image")
        {
            throw new RequestValidationException($"Bo parameter ContentType value must be either 'plaintext' or 'image', received '{data.ContentType}'");
        }

        _logger.LogDebug("Get clipboard content type: {ContentType}", data.ContentType);

        if (data.ContentType == "plaintext")
        {
            return Clipboard.GetText();
        }
        else
        {
            using var image = Clipboard.GetImage();
            if (image is null)
            {
                return null;
            }

            using var ms = new MemoryStream();
            image.Save(ms,image.RawFormat);

            return Convert.ToBase64String(ms.ToArray());
        }
    }
    
    private async Task<string> ExecutePowershell(PowerShellCommandRequest ps, CancellationToken ct)
    {
        if (!_options.AllowPowershell)
        {
            throw new RequestValidationException("Executing of powershell is disabled. Start server with '--allow-powershell' argument to enable powershell execution");
        }

        if (ps.Command is null && ps.Script is null)
        {
            throw new RequestValidationException("Body parameter Command or Script must be specified");
        }

        using Process process = new();

        if (ps.Command is not null)
        {
            _logger.LogDebug("Executing powershell command: {Command}", ps.Command);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{ps.Command.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            process.StartInfo = processStartInfo;
            process.Start();

            var result = await process.StandardOutput.ReadToEndAsync(ct);
            await HandlePowershellError(process, ct);
            await process.WaitForExitAsync(ct);

            return result;
        }

        var scriptPath = $"{SessionId}.ps1";

        try
        {
            await File.WriteAllTextAsync(scriptPath, ps.Script, ct);
            _logger.LogDebug("Executing powershell script: {Script}", ps.Script);

            var processInfo =
                new ProcessStartInfo("powershell.exe", "-ExecutionPolicy Bypass -File \"" + scriptPath + "\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

            process.StartInfo = processInfo;
            process.Start();
            
            var result = await process.StandardOutput.ReadToEndAsync(ct);
            await HandlePowershellError(process, ct);
            await process.WaitForExitAsync(ct);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to execute powershell script. {Exception}", e);
            throw;
        }
        finally
        {
            File.Delete(scriptPath);
        }
    }

    private async Task HandlePowershellError(Process process, CancellationToken ct)
    {
        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(ct);
            _logger.LogError("Failed to execute powershell. {Error}", error);
            throw new FailedToExecutePowershellException(error);
        }
    }

    /// <summary>
    /// Release all modifier keys
    /// </summary>
    private void ReleaseAllKeys()
    {
        foreach (var key in _pressedKeys.Reverse<VirtualKeyShort>())
        {
            _logger.LogDebug("Release key: {Key}", key.ToString());
            Keyboard.Release(key);
        }
        _pressedKeys.Clear();
    }

    #endregion
}