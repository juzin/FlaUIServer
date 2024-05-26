using System.Text;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Identifiers;

namespace FlaUIServer.Extensions;

public static class AutomationElementExtension
{
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
    
    public static string ToXml(this AutomationElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        
        var sb = new StringBuilder();

        var descendants = element.FindAllDescendants();

        if (descendants.Any())
        {
            sb.Append($"<{element.Properties.ControlType}  {GetElementProperties(element)}>");
            foreach (var descendant in descendants)
            {
                sb.Append(descendant.ToXml());
            }
            sb.Append($"</{element.Properties.ControlType}>");
        }
        else
        {
            sb.Append($"<{element.Properties.ControlType} {GetElementProperties(element)}/>");
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
        if (element.Properties.Name.IsSupported)
        {
            sb.Append($" Name=\"{element.Properties.Name.Value}\"");
        }
        if (element.Properties.ClassName.IsSupported)
        {
            sb.Append($" ClassName=\"{element.Properties.ClassName.Value}\"");
        }
        if (element.Properties.RuntimeId.IsSupported)
        {
            sb.Append($" RuntimeId=\"{element.Properties.RuntimeId.Value}\"");
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
            sb.Append($" Width=\"{element.Properties.BoundingRectangle.Value.Width}\" Height=\"{element.Properties.BoundingRectangle.Value.Height}\" X=\"{element.Properties.BoundingRectangle.Value.X}\" Y=\"{element.Properties.BoundingRectangle.Value.Y}\"");
        }
        
        return sb.ToString();
    }
}