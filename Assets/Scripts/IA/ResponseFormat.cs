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