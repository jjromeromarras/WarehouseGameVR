using TMPro;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class rfcontroller : MonoBehaviour
{
   

  

    [Header("UI elements")]
    public TextMeshProUGUI pantallatxt;
    public TextMeshProUGUI title;
    public LocalizeStringEvent localize;
    public LocalizeStringEvent localizePantalla;
    public LocalizedStringTable table_tooltips;


    void Start()
    {
        ResetRf();
    }

   
    public void ResetRf()
    {
        writeText(pantallatxt, string.Empty);
        writeText(title, string.Empty);
    }
    
    public void writeText(TextMeshProUGUI UIText, string text)
    {
        if (UIText != null)
            UIText.text = text;
    }

    public void SetPantallaTxt(string key , object[] arguments)
    {
        GameManager.Instance.WriteLog($"SetPantallaTxt: {key} - Arguments: {string.Join(", ",arguments)}");
        var localizedstring = GenerateLocalizedStringInEditor(key);
        localizedstring.Arguments = arguments;
        localizePantalla.StringReference = localizedstring;
    }
    // Update is called once per frame

    public void SetTitle(string key)
    {
        var localizedstring = GenerateLocalizedStringInEditor(key);        
        localize.StringReference = localizedstring;        
    }

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var entry = table_tooltips.GetTable().GetEntry(key);
        return new LocalizedString(table_tooltips.TableReference, entry.KeyId);
    }
}
