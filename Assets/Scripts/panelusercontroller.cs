using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class panelusercontroller : MonoBehaviour
{
    public TextMeshProUGUI score;
    public LocalizeStringEvent localizescore;

    private string keyscore = "Puntuacion";


    public void SetScore(object[] arguments)
    {
        var localizedstring = GenerateLocalizedStringInEditor(keyscore);
        localizedstring.Arguments = arguments;
        localizescore.StringReference = localizedstring;
    }

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var collection = LocalizationEditorSettings.GetStringTableCollection("StringsGames");
        var entry = collection.SharedData.GetEntry(key);
        return new LocalizedString(collection.SharedData.TableCollectionNameGuid, entry.Id);
    }
}
