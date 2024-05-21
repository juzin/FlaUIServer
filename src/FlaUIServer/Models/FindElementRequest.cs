using System.Text.Json.Serialization;
using FlaUIServer.Enums;

namespace FlaUIServer.Models;

public class FindElementRequest
{
    public string Using { get; set; }
    
    public string Value { get; set; }
    
    [JsonIgnore]
    public FindBy FindBy {
        get
        {
            return Using switch
            {
                "accessibility id" => FindBy.AutomationId,
                "class name" => FindBy.ClassName,
                "tag name" => FindBy.TagName,
                "name" => FindBy.Name,
                "xpath" => FindBy.Xpath,
                _ => throw new NotSupportedException(
                    $"By '{Using}' is not supported. Supported methods are 'accessibility id', 'className', 'tag name', 'name', 'xpath'")
            };
        }
    }
}