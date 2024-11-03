using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using System.Globalization;

public class nivelText : MonoBehaviour
{

    [SerializeField] public LocalizeStringEvent localize;
    [SerializeField] public LocalizedStringTable table_tooltips;
    // Start is called before the first frame update

    public void SetPantallaTxt(string key, object[] arguments)
    {
        GameManager.Instance.WriteLog($"Set Nivel Text: {key} - Arguments: {string.Join(", ", arguments)}");
        var localizedstring = GenerateLocalizedStringInEditor(key);
        localizedstring.Arguments = arguments;
        localize.StringReference = localizedstring;
    }

    public LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var entry = table_tooltips.GetTable().GetEntry(key);
        return new LocalizedString(table_tooltips.TableReference, entry.KeyId);
    }
}
