using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

public class OpenIAClient : ClientIA
{
    private const string apiKey = "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";
    private const string url = "https://api.openai.com/v1/chat/completions";


    UnityWebRequest Http;

    public OpenIAClient(): base(apiKey, url)
    {
        
    }
   
}
