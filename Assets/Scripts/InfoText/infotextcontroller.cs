using System.Collections;

public class infotextcontroller : infotextbase
{
      

    public IEnumerator SetMessageKey(string key, float timeToWaitAfecterText, object[] arguments = null)
    {
        var localizedstring = GenerateLocalizedStringInEditor(key);
        if (arguments != null)
        {
            localizedstring.Arguments = arguments;
        }
        localize.StringReference = localizedstring;
        yield return SetMessage(textinfo.text, timeToWaitAfecterText);
    }

}
