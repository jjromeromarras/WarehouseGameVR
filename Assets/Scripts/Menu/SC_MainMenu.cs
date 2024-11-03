using System.Collections;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class SC_MainMenu : MonoBehaviour
{

    [SerializeField] GameObject mainmenu;
    [SerializeField] GameObject paneloptions;
    [SerializeField] GameObject panellevels;
    [SerializeField] GameObject panelconsentimiento;
    [SerializeField] UnityEngine.UI.Image imagelevel;
    [SerializeField] GameObject imagelock;
    [SerializeField] Sprite[] imageslevels;
    [SerializeField] LocalizeStringEvent textlevel;
    [SerializeField] LocalizeStringEvent txttitulolevel;
    [SerializeField] GameObject bottoncomenzar;
    [SerializeField] GameObject panelloading;
    [SerializeField] UnityEngine.UI.Image ui_barra;
    [SerializeField] AudioClip menuMusic, bottonClip;
    [SerializeField] public LocalizedStringTable table_tooltips;
    [SerializeField] GameObject[] selected;
    private bool changelanguage = false;   
    private int currentlevel = 1;
    private UnityEngine.AsyncOperation asyncLoad;
    #region Public Methods

    private void Start()
    {
        //SoundManager.SharedInstance.PlayMusic(menuMusic);
        //selected[0].SetActive(true);
    }

    
    public void PlayNowButton()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.minlevel = currentlevel;
        if (currentlevel == 1)
        {
            GameManager.Instance.maxlevel = 3;
        } else if (currentlevel == 4)
        {
            GameManager.Instance.maxlevel = 5;
        }
        StartCoroutine(LoadAsyncScene()); //call to begin loading scene
       
    }

    public void PlayGame()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.minlevel = 1;
        GameManager.Instance.minlevel = 5;
        ShowConsentimiento();
        //StartCoroutine(LoadAsyncScene()); //call to begin loading scene

    }

    public void ShowConsentimiento()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        panelconsentimiento.SetActive(true);
        panellevels.SetActive(false);
    }


    IEnumerator LoadAsyncScene()
    {
       
        panelloading.SetActive(true);
        panelconsentimiento.SetActive(false);
        ui_barra.fillAmount = 0;
        yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("Warehouse");   
        asyncLoad.allowSceneActivation = false;
        float progress = 0f;
        //wait until the asynchronous scene fully loads
        while (progress <= .9f)
        {

            ui_barra.fillAmount = progress;

            //progressText.text = Mathf.Round(progress * 100f) + "%";

            progress += .01f;

            yield return new WaitForSeconds(.01f);
        }

        while (!asyncLoad.isDone && progress >= 0.9f)
        {
            Debug.Log("Loading progress: " + (asyncLoad.progress * 100) + "%");
            ui_barra.fillAmount = Mathf.Clamp01(asyncLoad.progress / .9f);
            //scene has loaded as much as possible,
            // the last 10% can't be multi-threaded
           
                asyncLoad.allowSceneActivation = true;
            
            yield return null;
        }
      
        panelloading.SetActive(false);
       

    }

    public void ComenzarButton()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        mainmenu.SetActive(false);
        paneloptions.SetActive(false);  
        panellevels.SetActive(true);
        SetLevel();
    }

    public void ChangeLocale(int localeId)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        if (changelanguage)
            return;
        StartCoroutine(SetLocale(localeId));
    }

    IEnumerator SetLocale(int _locale)
    {

        changelanguage = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_locale];
        changelanguage = false;
    }

    public void Exit()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        Application.Quit();
    }

    public void Options()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        mainmenu.SetActive(false);
        paneloptions.SetActive(true);
    } 

    public void ExistOption()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        mainmenu.SetActive(true);
        paneloptions.SetActive(false);
    }

    public void ChangePenalizacion(bool value)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.penalización = value;
    }

    public void ChangeMando(bool value)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.mandoxbox = value;
    }

    public void ChangeAyuda(bool value)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.showayuda = value;
        
    }

    

    public void Changemap(bool value)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.showminimap = value;
        
    }

    public void CerrarLevels()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        panelconsentimiento.SetActive(false);
        mainmenu.SetActive(true);
    }

    public void NextLevel()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        if (currentlevel < 6) currentlevel++;
        SetLevel();
    }
    public void BackLevel()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        if (currentlevel > 0) currentlevel--;
        SetLevel();
    }
    #endregion

    #region Private Methods
    private void SetLevel()
    {        
        imagelevel.sprite = imageslevels[currentlevel - 1];
        var localizedstring = GenerateLocalizedStringInEditor("nivel"+currentlevel.ToString());
        textlevel.StringReference = localizedstring;

        localizedstring = GenerateLocalizedStringInEditor("level" + currentlevel.ToString());
        txttitulolevel.StringReference = localizedstring;
        imagelock.SetActive(currentlevel != 1 && currentlevel != 4);
        bottoncomenzar.SetActive(currentlevel == 1 || currentlevel == 4);
    }

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var entry = table_tooltips.GetTable().GetEntry(key);
        return new LocalizedString(table_tooltips.TableReference, entry.KeyId);
    }
    #endregion
}
