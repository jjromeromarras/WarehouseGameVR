using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

public class MessageDeep 
{
    public string Content { get; set; }
    public string Role { get; set; } 

    public string Name { get; set; }

    /// <summary>
    /// beta feature
    /// </summary>
    public bool? Prefix { get; set; }

    /// <summary>
    /// beta feature
    /// </summary>
    [Newtonsoft.Json.JsonProperty("reasoning_content")]
    public string ReasoningContent { get; set; }

    [Newtonsoft.Json.JsonProperty("tool_call_id")]
    public string ToolCallId { get; set; }

    public static MessageDeep NewUserMessage(string content)
    {
        return new MessageDeep
        {
            Content = content,
            Role = "user"
        };
    }

    public static MessageDeep NewSystemMessage(string content)
    {
        return new MessageDeep
        {
            Content = content,
            Role = "system"
        };
    }

    public static MessageDeep NewAssistantMessage(string content, bool prefix, string reasoningContent)
    {
        return new MessageDeep
        {
            Content = content,
            Role = "assistant",
            Prefix = prefix,
            ReasoningContent = reasoningContent
        };
    }

    public static MessageDeep NewToolMessage(string content, string toolCallId)
    {
        return new MessageDeep
        {
            Content = content,
            Role = "tool",
            ToolCallId = toolCallId
        };
    }
}
