using TMPro;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class panelusercontroller : MonoBehaviour
{
    public TextMeshProUGUI score;
    public LocalizeStringEvent localizescore;
    public LocalizedStringTable table_tooltips;
    private string keyscore = "puntuacion";


    public void SetScore(object[] arguments)
    {
        var localizedstring = GenerateLocalizedStringInEditor(keyscore);
        localizedstring.Arguments = arguments;
        localizescore.StringReference = localizedstring;
    }

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var entry = table_tooltips.GetTable().GetEntry(key);
        return new LocalizedString(table_tooltips.TableReference, entry.KeyId);
    }
}

