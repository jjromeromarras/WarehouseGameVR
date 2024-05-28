using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class infotextbase : MonoBehaviour
{

    [SerializeField] public AudioClip writesound;
    [SerializeField] public LocalizeStringEvent localize;
    [SerializeField] public GameObject infopanel;
    [SerializeField] public TextMeshProUGUI textinfo;

    public event Action onFinishInfoText;

    protected bool writefulltext = false;
    

    // Start is called before the first frame update
    void Start()
    {
        SetActiveInfo(false);
        this.WriteInfoText(string.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!writefulltext)
            {
                writefulltext = true;
            }
            else
            {
                //SetActiveInfo(false);
                // this.WriteInfoText(string.Empty);
                if (onFinishInfoText != null)
                {
                    onFinishInfoText();
                }
            }
        }
    }

    public void WriteInfoText(string text)
    {
        if (textinfo != null)
        {
            textinfo.text = text;
        }
    }

    public void SetActiveInfo(bool value)
    {
        infopanel.SetActive(value);
    }

    public IEnumerator SetMessage(string msg, float timeToWaitAfecterText)
    {

        if (textinfo != null)
        {

            textinfo.text = "";
            writefulltext = false;
            foreach (var item in msg)
            {
                textinfo.text += item;
                SoundManager.SharedInstance.PlayWrite(writesound);
                yield return new WaitForSeconds(0.03f);
                if (writefulltext)
                {
                    textinfo.text = msg;
                    break;
                }
            }
            writefulltext = true;
            yield return new WaitForSeconds(timeToWaitAfecterText);

        }
    }

    public LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var collection = LocalizationEditorSettings.GetStringTableCollection("StringsGames");
        var entry = collection.SharedData.GetEntry(key);
        return new LocalizedString(collection.SharedData.TableCollectionNameGuid, entry.Id);
    }
}
