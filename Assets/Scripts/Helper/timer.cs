
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class timer : MonoBehaviour
{
    public float TimeLeft = 3f*60f;    
    public LocalizeStringEvent localizescore;

    private string keyscore = "tiempo";
    private bool TimerOn = false;

    // Start is called before the first frame update
    void Start()
    {
        TimerOn = false;
       
    }

    // Update is called once per frame
    void Update()
    {
        if(TimerOn)
        {
            if(TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                TimeLeft = 0;
                TimerOn = false;
            }
        }  
    }

    public void SetTimeLeft(float time)
    {
        TimeLeft = time;
        updateTimer(TimeLeft);
    }

    public void SetTimerOn(bool value)
    {
        TimerOn = value;
    }

    private void updateTimer(float currentTime)
    {
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime/60);
        float seconds = Mathf.FloorToInt(currentTime%60);
        SetTime(new object[] { string.Format("{0:00}:{1:00}", minutes, seconds) });
    }
   
    public void SetTime(object[] arguments)
    {
        var localizedstring = GenerateLocalizedStringInEditor(keyscore);
        localizedstring.Arguments = arguments;
        localizescore.StringReference = localizedstring;
    }

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var collection = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollection("StringsGames");
        var entry = collection.SharedData.GetEntry(key);
        return new LocalizedString(collection.SharedData.TableCollectionNameGuid, entry.Id);
    }
}
