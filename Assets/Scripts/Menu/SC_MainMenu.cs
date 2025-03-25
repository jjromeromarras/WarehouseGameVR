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
    [SerializeField] public infotextcontroller infotext;

    private bool changelanguage = false;
    private int currentlevel = 1;
    private UnityEngine.AsyncOperation asyncLoad;
    public bool waitreading;
    private StateGame state;
    private bool enableactions = false;

    #region Public Methods

    private void Start()
    {
        //SoundManager.SharedInstance.PlayMusic(menuMusic);
        //selected[0].SetActive(true);
        if (GameManager.Instance != null)
        {
            state = StateGame.ShowOptions;
            Options();
        }
        if (infotext != null)
        {
            infotext.onFinishInfoText += FinishInfoText;
        }



    }

    public void FinishInfoText()
    {
        waitreading = false;
        NextStep();
    }
    public void NextStep()
    {

        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    showTextoKey("showencuesta");
                    state = StateGame.ShowEncuesta;
                    enableactions = false;
                    break;
                }
            case StateGame.ShowEncuesta:
                {
                    infotext.textcontinuar.gameObject.SetActive(false);
                    showTextoKey("encuestageneral1");
                    state = StateGame.ShowEncuestageneral1;
                    infotext.executeFinish = false;
                    enableactions = true;
                    if (GameManager.Instance.UsedIA)
                    {
                        GameManager.Instance.InitialIA();
                    }
                    break;
                }
            case StateGame.ShowEncuestageneral1:
                {
                    showTextoKey("encuestageneral2");
                    enableactions = true;
                    state = StateGame.ShowEncuestageneral2;
                    break;
                }
            case StateGame.ShowEncuestageneral2:
                {
                    showTextoKey("encuestarecep1");
                    enableactions = true;
                    state = StateGame.ShowEncuestaRecep1;
                    break;
                }
            case StateGame.ShowEncuestaRecep1:
                {
                    showTextoKey("encuestarecep2");
                    enableactions = true;
                    state = StateGame.ShowEncuestaRecep2;
                    break;
                }
            case StateGame.ShowEncuestaRecep2:
                {
                    showTextoKey("encuestapicking1");
                    enableactions = true;
                    state = StateGame.ShowEncuestaPicking1;
                    break;
                }
            case StateGame.ShowEncuestaPicking1:
                {
                    showTextoKey("encuestapicking2");
                    state = StateGame.ShowEncuestaPicking2;
                    enableactions = true;
                    break;
                }
            case StateGame.ShowEncuestaPicking2:
                {
                    showTextoKey("encuestaubicacion1");
                    state = StateGame.ShowEncuestaUbicacion1;
                    enableactions = true;
                    break;
                }
            case StateGame.ShowEncuestaUbicacion1:
                {
                    showTextoKey("encuestaubicacion2");
                    state = StateGame.ShowEncuestaUbicacion2;
                    enableactions = true;
                    break;
                }
            case StateGame.ShowEncuestaUbicacion2:
                {
                    showTextoKey("encuestacarretilla1");
                    state = StateGame.ShowEncuestaCarretilla1;
                    enableactions = true;
                    break;
                }
            case StateGame.ShowEncuestaCarretilla1:
                {
                    showTextoKey("encuestacarretilla2");
                    state = StateGame.ShowEncuestaCarretilla2;
                    enableactions = true;
                    break;
                }
            case StateGame.ShowEncuestaCarretilla2:
                {
                    GameManager.Instance.player.playerClassification.CalculateClassifyPlayer();
                    showTextoKey("finalencuesta");
                    infotext.textcontinuar.gameObject.SetActive(true);
                    infotext.executeFinish = true;
                    GameManager.Instance.player.Survery = true;
                    state = StateGame.ShowMenu;
                    enableactions = true;
                    break;
                }
            case StateGame.ShowMenu:
                {
                    infotext.SetActiveInfo(false);
                    break;
                }

        }

    }



    void Update()
    {      

        if (state == StateGame.ShowInicio && !changelanguage)
        {
            state = StateGame.ShowBienVenido;
            infotext.SetActiveInfo(true);
            showTextoKey("PrimerBienvenida");
        }

        if (state == StateGame.ShowFinalEncuesta && (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick1Button1)))
        {
            FinishInfoText();
        }
       
        if (infotext.isFinish && state != StateGame.ShowBienVenido && state != StateGame.ShowEncuesta && enableactions)
        {
            var penalizacion = 0;
            bool respuestaencuesta = false;

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                penalizacion = 0;
                respuestaencuesta = true;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                penalizacion = 1;
                respuestaencuesta = true;
            }
            else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                penalizacion = 2;
                respuestaencuesta = true;
            }
            if (respuestaencuesta)
            {
                enableactions = false;
                switch (state)
                {
                    case StateGame.ShowEncuestageneral1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("General", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestageneral2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("General", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaRecep1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("RecepcionMateriales", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaRecep2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("RecepcionMateriales", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaPicking1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("PreparacionPedidos", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaPicking2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("PreparacionPedidos", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaUbicacion1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("UbicacionMateriales", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaUbicacion2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("UbicacionMateriales", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaCarretilla1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("ManejoCarretillas", penalizacion);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaCarretilla2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("ManejoCarretillas", penalizacion);
                            FinishInfoText();
                            break;
                        }

                }
            }

        }
    }

    public void showTextoKey(string key)
    {
        if (!waitreading)
        {
            infotext.SetActiveInfo(true);
            waitreading = true;
            StartCoroutine(infotext.SetMessageKey(key, 2f));
        }
    }

    public void showMsg(string msg)
    {
        if (!waitreading)
        {
            infotext.SetActiveInfo(true);
            waitreading = true;
            StartCoroutine(infotext.SetMessageText(msg, 2f));
        }
    }

    public void PlayNowButton()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        
        if (currentlevel == 1)
        {
            GameManager.Instance.minlevel = 0;
            GameManager.Instance.maxlevel = 2;
        }
        else if (currentlevel == 2)
        {
            GameManager.Instance.minlevel = 3;
            GameManager.Instance.maxlevel = 3;
        }
        else if (currentlevel == 4)
        {
            GameManager.Instance.minlevel = 4;
            GameManager.Instance.maxlevel = 5;
        }
        StartCoroutine(LoadAsyncScene()); //call to begin loading scene

    }

    public void PlayGame()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.minlevel = 1;
        GameManager.Instance.maxlevel = 6;
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

    public void ChangeLLM(int LLMid)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        if (LLMid == 0)
        {
            GameManager.Instance.UsedIA = false;
        }
        else
        {
            GameManager.Instance.UsedIA = true;
            GameManager.Instance.IAmodelIndx = LLMid - 1;
        }
    }

    public void ChangeLevel(int Levelid)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        if (Levelid != 0)
        {
            GameManager.Instance.player.Survery = true;
            switch (Levelid)
            {
                case 1:
                    GameManager.Instance.player.playerClassification.SetClassifyPlayer(PlayerClassification.LevelCategory.Principiante);
                        break;
                case 2:
                    GameManager.Instance.player.playerClassification.SetClassifyPlayer(PlayerClassification.LevelCategory.Medio);
                    break;
                case 3:
                    GameManager.Instance.player.playerClassification.SetClassifyPlayer(PlayerClassification.LevelCategory.Avanzado);
                    break;
                case 4:
                    GameManager.Instance.player.playerClassification.SetClassifyPlayer(PlayerClassification.LevelCategory.Experto);
                    break;
            }

        }
    }

    IEnumerator SetLocale(int _locale)
    {
        changelanguage = true;
        GameManager.Instance.localize = _locale;

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
        if (!GameManager.Instance.player.Survery)
        {
            state = StateGame.ShowBienVenido;
            infotext.SetActiveInfo(true);
            NextStep();
        }
        else
        {
            infotext.gameObject.SetActive(false);
            state = StateGame.ShowMenu;
            if (GameManager.Instance.UsedIA)
                GameManager.Instance.InitialIA();
        }

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
        GameManager.Instance.UsedIA = value;

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
        panellevels.SetActive(false);
        mainmenu.SetActive(true);
    }

    public void NextLevel()
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        if (currentlevel < 5) currentlevel++;
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
        var localizedstring = GenerateLocalizedStringInEditor("nivel" + currentlevel.ToString());
        textlevel.StringReference = localizedstring;

        localizedstring = GenerateLocalizedStringInEditor("level" + currentlevel.ToString());
        txttitulolevel.StringReference = localizedstring;
        imagelock.SetActive(false);
        bottoncomenzar.SetActive(true);
    }

    private LocalizedString GenerateLocalizedStringInEditor(string key)
    {
        // The main advantage to using a table Guid and entry Id is that references will not be lost when changes are made to the Table name or Entry name.
        var entry = table_tooltips.GetTable().GetEntry(key);
        return new LocalizedString(table_tooltips.TableReference, entry.KeyId);
    }
    #endregion

    internal enum StateGame
    {
        ShowOptions,
        ShowInicio,
        ShowBienVenido,
        ShowEncuesta,
        ShowEncuestageneral1,
        ShowEncuestageneral2,
        ShowEncuestaRecep1,
        ShowEncuestaRecep2,
        ShowEncuestaPicking1,
        ShowEncuestaPicking2,
        ShowEncuestaUbicacion1,
        ShowEncuestaUbicacion2,
        ShowEncuestaCarretilla1,
        ShowEncuestaCarretilla2,
        ShowFinalEncuesta,
        ShowRecomendacion,
        ShowIAResult,
        ShowMenu,

    }
}
