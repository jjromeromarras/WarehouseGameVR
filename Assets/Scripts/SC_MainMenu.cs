using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SC_MainMenu : MonoBehaviour
{

    [SerializeField] GameObject mainmenu;
    [SerializeField] GameObject paneloptions;
    [SerializeField] GameObject panellevels;
    [SerializeField] UnityEngine.UI.Image imagelevel;
    [SerializeField] GameObject imagelock;
    [SerializeField] Sprite[] imageslevels;
    [SerializeField] LocalizeStringEvent textlevel;
    [SerializeField] LocalizeStringEvent txttitulolevel;
    [SerializeField] GameObject bottoncomenzar;
    [SerializeField] GameObject panelloading;
    [SerializeField] UnityEngine.UI.Image ui_barra;

    private bool changelanguage = false;   
    private int currentlevel = 1;
   
    
    private UnityEngine.AsyncOperation asyncLoad;
    #region Public Methods
    public void PlayNowButton()
    {

        StartCoroutine(LoadAsyncScene()); //call to begin loading scene
       
    }

    IEnumerator LoadAsyncScene()
    {

        panelloading.SetActive(true);
        panellevels.SetActive(false);
        ui_barra.fillAmount = 0;
        yield return new WaitForSeconds(1f);
        asyncLoad = SceneManager.LoadSceneAsync("Level_"+currentlevel.ToString());
        SceneManager.LoadSceneAsync("StaticGame", LoadSceneMode.Additive);
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
        mainmenu.SetActive(false);
        paneloptions.SetActive(false);  
        panellevels.SetActive(true);
        SetLevel();
    }

    public void ChangeLocale(int localeId)
    {
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
       Application.Quit();
    }

    public void Options()
    {
        mainmenu.SetActive(false);
        paneloptions.SetActive(true);
    } 

    public void ExistOption()
    {
        mainmenu.SetActive(true);
        paneloptions.SetActive(false);
    }

    public void ChangePenalizacion(bool value)
    {
        
        GameManager.Instance.penalización = value;
    }

    public void ChangeAyuda(bool value)
    {
        GameManager.Instance.showayuda = value;
        
    }

    public void Changemap(bool value)
    {
        GameManager.Instance.showminimap = value;
        
    }

    public void CerrarLevels()
    {
        panellevels.SetActive(false );
        mainmenu.SetActive(true);
    }

    public void NextLevel()
    {
        if (currentlevel < 6) currentlevel++;
        SetLevel();
    }
    public void BackLevel()
    {
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
        imagelock.SetActive(currentlevel != 1);
        bottoncomenzar.SetActive(currentlevel == 1);
    }

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var collection = LocalizationEditorSettings.GetStringTableCollection("StringsGames");
        var entry = collection.SharedData.GetEntry(key);
        return new LocalizedString(collection.SharedData.TableCollectionNameGuid, entry.Id);
    }
    #endregion
}
