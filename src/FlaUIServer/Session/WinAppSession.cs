using System.Drawing.Imaging;
using FlaUI.Core;
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

namespace FlaUIServer.Session;

/// <summary>
/// Represents Windows application automation session
/// </summary>
public class WinAppSession : IDisposable
{
    private readonly ILogger<WinAppSession> _logger;
    private readonly UIA3Automation _automation;
    private readonly Application _application;
    private readonly Dictionary<Guid, AutomationElement> _elements = [];
    private Window _activeWindow;

    /// <summary>
    /// Session id
    /// </summary>
    public Guid SessionId { get; }

    /// <summary>
    /// Active window
    /// </summary>
    public Window ActiveWindow
    {
        get
        {
            // In case window is null return application main window
            if (_activeWindow is null)
            {
                if (IsRootSession)
                {
                    _automation.GetDesktop().AsWindow();
                }
                else
                {
                    _activeWindow = _application.GetMainWindow(_automation);
                }
            }
            return _activeWindow;
        }
        private set
        {
            _activeWindow = value;
        }
    }
    
    public bool IsRootSession { get; }

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
    public WinAppSession(Capabilities capabilities, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(capabilities);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        
        _logger = loggerFactory.CreateLogger<WinAppSession>();
        _automation = new UIA3Automation();
        
        
        if (capabilities.AlwaysMatch.ApplicationTopLevelWindow is not null)
        {
            _application = Application.Attach(Convert.ToInt32(capabilities.AlwaysMatch.ApplicationTopLevelWindow, 16));
        }
        else if (capabilities.AlwaysMatch.Application == "Root")
        {
            _application = null;
            ActiveWindow = _automation.GetDesktop().AsWindow();
            IsRootSession = true;
        }
        else
        {
            _application = Application.Launch(capabilities.AlwaysMatch.Application);
            ActiveWindow = _application.GetMainWindow(_automation);
            IsRootSession = false;
        }
        
        Created = DateTimeOffset.Now;
        SessionId = Guid.NewGuid();
        
        LastActionAt = DateTimeOffset.Now;
    }

    public Guid FindElement(FindElementRequest searchParams)
    {
        var elementId = Guid.NewGuid();
        AutomationElement element = null;

        switch (searchParams.FindBy)
        {
            case FindBy.AutomationId:
                element = ActiveWindow.FindFirstDescendant(x => x.ByAutomationId(searchParams.Value));
                break;
            case FindBy.ClassName:
                element = ActiveWindow.FindFirstDescendant(x => x.ByClassName(searchParams.Value));
                break;
            case FindBy.TagName:
                element = ActiveWindow.FindFirstDescendant(x =>
                    x.ByControlType(Enum.Parse<ControlType>(searchParams.Value)));
                break;
            case FindBy.Name:
                element = ActiveWindow.FindFirstDescendant(x => x.ByName(searchParams.Value));
                break;
            case FindBy.Xpath:
                element = ActiveWindow.FindFirstByXPath(searchParams.Value);
                break;
        }

        if (element is null)
        {
            throw new ObjectNotFoundException(
                $"Element by '{searchParams.Using}' value '{searchParams.Value}' not found");
        }

        _elements[elementId] = element;

        return elementId;
    }

    public Guid[] FindElements(FindElementRequest findElement)
    {
        List<Guid> elementIds = new();
        AutomationElement[] elements = null;

        switch (findElement.FindBy)
        {
            case FindBy.AutomationId:
                elements = ActiveWindow.FindAllDescendants(x => x.ByAutomationId(findElement.Value));
                break;
            case FindBy.ClassName:
                elements = ActiveWindow.FindAllDescendants(x => x.ByClassName(findElement.Value));
                break;
            case FindBy.TagName:
                elements = ActiveWindow.FindAllDescendants(x =>
                    x.ByControlType(Enum.Parse<ControlType>(findElement.Value)));
                break;
            case FindBy.Name:
                elements = ActiveWindow.FindAllDescendants(x => x.ByName(findElement.Value));
                break;
            case FindBy.Xpath:
                elements = ActiveWindow.FindAllByXPath(findElement.Value);
                break;
        }

        if (elements is null || elements.Length == 0)
        {
            throw new ObjectNotFoundException(
                $"Elements by '{findElement.Using}' value '{findElement.Value}' not found");
        }

        foreach (var element in elements)
        {
            var elementId = Guid.NewGuid();
            elementIds.Add(elementId);
            _elements[elementId] = element;
        }

        return elementIds.ToArray();
    }

    public void ElementCLick(Guid elementId)
    {
        var element = GetElement(elementId);
        element.AsButton().Click();
    }

    public void ElementFillText(Guid elementId, string text)
    {
        var element = GetElement(elementId);
        element.Focus();
        element.AsTextBox().Enter(text);
    }

    public void ElementClearText(Guid elementId)
    {
        var element = GetElement(elementId);
        element.AsTextBox().Text = "";
    }

    public bool IsElementDisplayed(Guid elementId)
    {
        var element = GetElement(elementId);
        return element.IsAvailable && !element.IsOffscreen;
    }

    public bool IsElementEnabled(Guid elementId)
    {
        var element = GetElement(elementId);
        return element.IsEnabled;
    }

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

    public RectangleResponse GetElementRectangle(Guid elementId)
    {
        var element = GetElement(elementId);
        return new RectangleResponse(element.BoundingRectangle.X, element.BoundingRectangle.Y, element.BoundingRectangle.Height, element.BoundingRectangle.Width);
    }

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
    public string GetWindowTitle()
    {
        return ActiveWindow.Title;
    }

    /// <summary>
    /// Get active window rectangle
    /// </summary>
    /// <returns></returns>
    public RectangleResponse GetWindowRectangle()
    {
        return new RectangleResponse(ActiveWindow.BoundingRectangle.X, ActiveWindow.BoundingRectangle.Y, ActiveWindow.BoundingRectangle.Height, ActiveWindow.BoundingRectangle.Width);
    }

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
    /// Close window session
    /// </summary>
    public void CloseActiveWindow()
    {
        ActiveWindow.Close();
    }

    /// <summary>
    /// Executes script or gesture
    /// </summary>
    /// <returns></returns>
    public string ExecuteScript(ExecuteScriptRequest data)
    {
        ArgumentNullException.ThrowIfNull(data);
        
        switch (data.Script)
        {
            case "powerShell":
                throw new NotImplementedException();
            case "windows: click":
                ClickGesture(GestureRequestHelper.DeserializeGestureRequest<ClickGestureRequest>(data.Args));
                break;
            case "windows: clickAndDrag":
                DragAndDropGesture(GestureRequestHelper.DeserializeGestureRequest<MoveGestureRequest>(data.Args));
                break;
            case "windows: hover":
                HoverGesture(GestureRequestHelper.DeserializeGestureRequest<MoveGestureRequest>(data.Args));
                break;
            case "windows: scroll":
            case "windows: setClipboard":
            case "windows: getClipboard":
                throw new NotImplementedException();
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
        var keyModifiers = new List<VirtualKeyShort>();
        
        foreach (var key in keys.Value)
        {
            var modifierKey = KeyboardHelper.GetModifierKey(key);
            
            if (modifierKey is not null)
            {
                // Press modifier key, if same key is present second time release it
                if (keyModifiers.Contains(modifierKey.Value))
                {
                    _logger.LogDebug("Depressing key {Key}", modifierKey.Value.ToString());
                    Keyboard.Release(modifierKey.Value);
                    keyModifiers.Remove(modifierKey.Value);
                }
                else
                {
                    _logger.LogDebug("Pressing key {Key}", modifierKey.Value.ToString());
                    Keyboard.Press(modifierKey.Value);
                    keyModifiers.Add(modifierKey.Value);
                }
            }
            else
            {
                //Null key - release all
                if (key == '\uE000')
                {
                    ReleaseAll();
                }
                else
                {
                    Keyboard.Type(key);
                }
            }
        }

        ReleaseAll();
        return;

        //Release all modifier keys
        void ReleaseAll()
        {
            foreach (var key in keyModifiers.Reverse<VirtualKeyShort>())
            {
                _logger.LogDebug("Depressing key {Key}", key.ToString());
                Keyboard.Release(key);
            }
            keyModifiers.Clear();
        }
    }
    
    /// <summary>
    /// Close application
    /// </summary>
    public void Close()
    {
        if (!IsRootSession && _application != null && !_application.HasExited)
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
        Mouse.Down();
        Mouse.MoveTo(data.EndX, data.EndY);
        Mouse.Up();
    }
}