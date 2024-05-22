using System;
using System.Collections;
using TMPro;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class infotextcontroller : MonoBehaviour
{
    public static infotextcontroller instance;
    public GameObject infopanel;

    public TextMeshProUGUI textinfo;
    public LocalizeStringEvent localize;
    public event Action onFinishInfoText;

    private bool writefulltext = false;
    private void Awake()
    {
        instance = this; 
    }
    // Start is called before the first frame update
    void Start()
    {
        SetActiveInfo(false);
        this.WriteInfoText(string.Empty);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!writefulltext)
            {
                writefulltext = true;
            } else
            {
                //SetActiveInfo(false);
                // this.WriteInfoText(string.Empty);
                if(onFinishInfoText != null)
                {
                    onFinishInfoText();
                }
            }
        }

    }
    public void WriteInfoText(string text)
    {
        if(textinfo != null)
        {
            textinfo.text = text;
        }
    }

    public void SetActiveInfo(bool value)
    {
        infopanel.SetActive(value);
    }

    // Update is called once per frame
    public IEnumerator SetMessage(string msg, float timeToWaitAfecterText)
    {
        
        if (textinfo != null)
        {

            textinfo.text = "";
            writefulltext = false;
            foreach (var item in msg)
            {
                textinfo.text += item;
                yield return new WaitForSeconds(0.05f);
                if(writefulltext)
                {
                    textinfo.text = msg;
                    break;
                }
            }
            writefulltext = true;
            yield return new WaitForSeconds(timeToWaitAfecterText);

        }
    }

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

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var collection = LocalizationEditorSettings.GetStringTableCollection("StringsGames");
        var entry = collection.SharedData.GetEntry(key);
        return new LocalizedString(collection.SharedData.TableCollectionNameGuid, entry.Id);
    }

}
