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
}