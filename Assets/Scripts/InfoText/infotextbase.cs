using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class infotextbase : MonoBehaviour
{

    [SerializeField] public AudioClip writesound;
    [SerializeField] public LocalizeStringEvent localize;
    [SerializeField] public GameObject infopanel;
    [SerializeField] public TextMeshProUGUI textinfo;
    [SerializeField] public TextMeshProUGUI textcontinuar;
    [SerializeField] public LocalizedStringTable table_tooltips;
    public event Action onFinishInfoText;

    public bool writefulltext = false;
    public bool isWriting = false;
    public bool isFinish = false;
    public bool executeFinish = true;

    // Start is called before the first frame update
    void Start()
    {
        this.WriteInfoText(string.Empty);
        isWriting = false;
        executeFinish = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (executeFinish)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                isWriting = false;
                if (!writefulltext)
                {
                    writefulltext = true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Joystick1Button1))
            {
                isFinish = true;
                if (onFinishInfoText != null && executeFinish)
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
    
    public void SetContinueMessage()
    {
        if(GameManager.Instance != null && executeFinish)
        {
            var tecla = "B";
                         
                // Mando Xbox
                if (LocalizationSettings.Instance.GetSelectedLocale().LocaleName.Contains("es"))
                {
                    tecla = "(B) or espacio";
                } else if (LocalizationSettings.Instance.GetSelectedLocale().LocaleName.Contains("en"))
                {
                    tecla = "(B) or space";
                }
                else
                {
                    tecla = "(B) or espace";
                }
            

            if(textcontinuar!= null)
            {
                if (LocalizationSettings.Instance.GetSelectedLocale().LocaleName.Contains("(es)"))
                {
                    textcontinuar.text = $"Pulsar {tecla} para continuar.";
                }
                else if (LocalizationSettings.Instance.GetSelectedLocale().LocaleName.Contains("(en)"))
                {
                    textcontinuar.text = $"Press {tecla} to continue.";
                }
                else
                {
                    textcontinuar.text = $"Pulsar {tecla} pour continuer.";
                }

            }
        } else
        {
            textcontinuar.text = string.Empty ; 
        }
    }

    public IEnumerator SetMessage(string msg, float timeToWaitAfecterText)
    {

        if (textinfo != null)
        {
            isFinish = false;
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
            isFinish = true;
            SetContinueMessage();
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
