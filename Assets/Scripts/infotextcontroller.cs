using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class infotextcontroller : MonoBehaviour
{
    public static infotextcontroller instance;
    public GameObject infopanel;

    public TextMeshProUGUI textinfo;
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
            foreach (var item in msg)
            {
                textinfo.text += item;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(timeToWaitAfecterText);
           
        }
    }
}
