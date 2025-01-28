using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseFormat
{
    public string Type
    {
        get; set;
    }

    public ResponseFormat()
    {
        Type = ResponseFormatTypes.Text;
    }
}