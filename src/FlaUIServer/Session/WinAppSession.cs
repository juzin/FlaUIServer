using System.Drawing.Imaging;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using FlaUIServer.Enums;
using FlaUIServer.Exceptions;
using FlaUIServer.Extensions;
using FlaUIServer.Models;

namespace FlaUIServer.Session;

/// <summary>
/// Represents Windows application automation session
/// </summary>
public class WinAppSession : IDisposable
{
    private readonly UIA3Automation _automation;
    private readonly Application _application;
    private readonly Dictionary<Guid, AutomationElement> _elements = [];

    /// <summary>
    /// Session id
    /// </summary>
    public Guid SessionId { get; }

    /// <summary>
    /// Active window
    /// </summary>
    public Window ActiveWindow { get; private set; }
    
    public bool IsRootSession { get; }

    /// <summary>
    /// Date and time of session creation
    /// </summary>
    public DateTimeOffset Created { get; }

    /// <summary>
    /// Date and time of last session action
    /// </summary>
    public DateTimeOffset LastActionAt { get; private set; }

    public WinAppSession(Capabilities capabilities)
    {
        ArgumentNullException.ThrowIfNull(capabilities);
        ArgumentException.ThrowIfNullOrEmpty(capabilities.AlwaysMatch.Application);

        // TODO: handle root session
        // TODO: attach to running app
        var application = Application.Launch(capabilities.AlwaysMatch.Application);
        IsRootSession = false;
        Created = DateTimeOffset.Now;
        _application = application;
        SessionId = Guid.NewGuid();
        _automation = new UIA3Automation();
        ActiveWindow = _application.GetMainWindow(_automation);
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
            case "windows: click":
            case "windows: clickAndDrag":
            case "windows: hover":
            case "windows: scroll":
            case "windows: setClipboard":
            case "windows: getClipboard":
                throw new NotImplementedException();
            default:
                throw new NotSupportedException($"Script '{data.Script}' is not supported. Supported gestures 'powerShell', 'windows: click', 'windows: clickAndDrag', 'windows: hover', 'windows: scroll', 'windows: setClipboard', 'windows: getClipboard'");
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
        _automation?.Dispose();
    }
    
    private AutomationElement GetElement(Guid elementId)
    {
        if (_elements.TryGetValue(elementId, out var element))
        {
            return element;
        }

        throw new ObjectNotFoundException($"Element with '{elementId}' not found");
    }
}