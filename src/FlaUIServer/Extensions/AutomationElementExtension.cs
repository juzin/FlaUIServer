using System.Text;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Identifiers;

namespace FlaUIServer.Extensions;

public static class AutomationElementExtension
{
    /// <summary>
    /// Get runtime id as string
    /// </summary>
    /// <param name="element">Automation element</param>
    /// <returns>Runtime id as string</returns>
    public static string GetRuntimeId(this AutomationElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        
        if (element.Properties.RuntimeId.IsSupported)
        {
            return string.Join(string.Empty, element.Properties.RuntimeId.Value);
        }

        return null;
    }
    
    /// <summary>
    /// Try to get property from element
    /// </summary>
    /// <param name="element">Element</param>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Out value, null if gettyng property failed</param>
    /// <returns>True if getting property value succeeded, otherwise false</returns>
    public static bool TryGetProperty(this AutomationElement element, string propertyName, out object value)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        
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
        var sb = new StringBuilder();

        var descendants = element.FindAllChildren();

        if (descendants.Any())
        {
            sb.Append($"<{element.Properties.ControlType}{GetElementProperties(element)}>");
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
        sb.Append($" ProcessId=\"{element.Properties.ProcessId.ValueOrDefault}\"");
        sb.Append($" AutomationId=\"{element.Properties.AutomationId.ValueOrDefault}\"");
        sb.Append($" ClassName=\"{element.Properties.ClassName.ValueOrDefault}\"");
        sb.Append($" Name=\"{element.Properties.Name.ValueOrDefault}\"");
        sb.Append($" IsEnabled=\"{element.Properties.IsEnabled.ValueOrDefault}\"");
        sb.Append($" IsOffscreen=\"{element.Properties.IsOffscreen.ValueOrDefault}\"");
        sb.Append($" IsPassword=\"{element.Properties.IsPassword.ValueOrDefault}\"");
        sb.Append($" IsKeyboardFocusable=\"{element.Properties.IsKeyboardFocusable.ValueOrDefault}\"");
        sb.Append($" FrameworkId=\"{element.Properties.FrameworkId.ValueOrDefault}\"");

        if (element.Properties.BoundingRectangle.IsSupported)
        {
            sb.Append($" x=\"{element.Properties.BoundingRectangle.Value.X}\" y=\"{element.Properties.BoundingRectangle.Value.Y}\" width=\"{element.Properties.BoundingRectangle.Value.Width}\" height=\"{element.Properties.BoundingRectangle.Value.Height}\"");
        }
        
        return sb.ToString();
    }
}