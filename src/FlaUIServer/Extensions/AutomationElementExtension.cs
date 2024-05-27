using System.Text;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Identifiers;

namespace FlaUIServer.Extensions;

public static class AutomationElementExtension
{
    /// <summary>
    /// Get runtime id as string
    /// </summary>
    /// <param name="window">Window</param>
    /// <returns>Runtime id as string</returns>
    public static string GetRuntimeId(this AutomationElement window)
    {
        ArgumentNullException.ThrowIfNull(window);
        
        if (window.Properties.RuntimeId.TryGetValue(out var runtimeId))
        {
            return string.Join(string.Empty, runtimeId);
        }

        return null;
    }
    
    public static bool TryGetProperty(this AutomationElement element, string propertyName, out object value)
    {
        var library = element.FrameworkAutomationElement.PropertyIdLibrary;

        if (library.GetType().GetProperty(propertyName) is { } propertyInfo &&
            propertyInfo.GetValue(library) is PropertyId propertyId)
        {
            element.FrameworkAutomationElement.TryGetPropertyValue(propertyId, out value);
            return true;
        }

        value = null;
        return false;
    }
    
    /// <summary>
    /// Traverse through all elements and create xml structure
    /// </summary>
    /// <param name="element">Parent element</param>
    /// <returns>xml representation of elements structure</returns>
    public static string ToXml(this AutomationElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        
        var sb = new StringBuilder();

        var descendants = element.FindAllDescendants();

        if (descendants.Any())
        {
            sb.Append($"<?xml version=\"1.0\" encoding=\"utf-16\"?><{element.Properties.ControlType}{GetElementProperties(element)}>");
            foreach (var descendant in descendants)
            {
                sb.Append(descendant.ToXml());
            }
            sb.Append($"</{element.Properties.ControlType}>");
        }
        else
        {
            sb.Append($"<{element.Properties.ControlType}{GetElementProperties(element)} />");
        }
        
        return sb.ToString();
    }

    private static string GetElementProperties(AutomationElement element)
    {
        var sb = new StringBuilder();

        if (element.Properties.AutomationId.IsSupported)
        {
            sb.Append($" AutomationId=\"{element.Properties.AutomationId.Value}\"");
        }
        if (element.Properties.ClassName.IsSupported)
        {
            sb.Append($" ClassName=\"{element.Properties.ClassName.Value}\"");
        }
        if (element.Properties.Name.IsSupported)
        {
            sb.Append($" Name=\"{element.Properties.Name.Value}\"");
        }
        
        sb.Append($" AcceleratorKey=\"{element.Properties.AcceleratorKey.ValueOrDefault}\"");
        sb.Append($" AccessKey=\"{element.Properties.AccessKey.ValueOrDefault}\"");
        sb.Append($" FrameworkId=\"{element.Properties.FrameworkId.ValueOrDefault}\"");
        sb.Append($" HasKeyboardFocus=\"{element.Properties.HasKeyboardFocus.ValueOrDefault}\"");
        sb.Append($" IsControlElement=\"{element.Properties.IsControlElement.ValueOrDefault}\"");
        sb.Append($" IsContentElement=\"{element.Properties.IsContentElement.ValueOrDefault}\"");
        sb.Append($" HelpText=\"{element.Properties.HelpText.ValueOrDefault}\"");
        
        if (element.Properties.RuntimeId.IsSupported)
        {
            sb.Append($" RuntimeId=\"{element.GetRuntimeId()}\"");
        }
        if (element.Properties.IsEnabled.IsSupported)
        {
            sb.Append($" IsEnabled=\"{element.Properties.IsEnabled.Value}\"");
        }
        if (element.Properties.IsOffscreen.IsSupported)
        {
            sb.Append($" IsOffscreen=\"{element.Properties.IsOffscreen.Value}\"");
        }
        if (element.Properties.IsPassword.IsSupported)
        {
            sb.Append($" IsPassword=\"{element.Properties.IsPassword.Value}\"");
        }
        if (element.Properties.IsDialog.IsSupported)
        {
            sb.Append($" IsDialog=\"{element.Properties.IsDialog.Value}\"");
        }
        if (element.Properties.BoundingRectangle.IsSupported)
        {
            sb.Append($" x=\"{element.Properties.BoundingRectangle.Value.X}\" y=\"{element.Properties.BoundingRectangle.Value.Y}\" width=\"{element.Properties.BoundingRectangle.Value.Width}\" height=\"{element.Properties.BoundingRectangle.Value.Height}\"");
        }
        
        return sb.ToString();
    }
}