using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class infotextbase : MonoBehaviour
{

    [SerializeField] public AudioClip writesound;
    [SerializeField] public LocalizeStringEvent localize;
    [SerializeField] public GameObject infopanel;
    [SerializeField] public TextMeshProUGUI textinfo;
    [SerializeField] public LocalizedStringTable table_tooltips;
    public event Action onFinishInfoText;

    public bool writefulltext = false;
    public bool isWriting = false;

    // Start is called before the first frame update
    void Start()
    {
        this.WriteInfoText(string.Empty);
        isWriting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            isWriting = false;
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
            isWriting = true;
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
            isWriting = false;
            yield return new WaitForSeconds(timeToWaitAfecterText);

        }
    }

    public LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var entry = table_tooltips.GetTable().GetEntry(key);
        return new LocalizedString(table_tooltips.TableReference, entry.KeyId);
    }
}
