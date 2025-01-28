using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

public class MessageDeep 
{
    public string content { get; set; }
    public string role { get; set; } 


    public static MessageDeep NewUserMessage(string content)
    {
        return new MessageDeep
        {
            content = content,
            role = "user"
        };
    }

    public static MessageDeep NewSystemMessage(string content)
    {
        return new MessageDeep
        {
            content = content,
            role = "system"
        };
    }

}
