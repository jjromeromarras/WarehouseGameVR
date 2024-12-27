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
    private float time;
    private float resetTime = 0.2f;
    private bool enableactions = false;

    #region Public Methods

    private void Start()
    {
        //SoundManager.SharedInstance.PlayMusic(menuMusic);

        selected[0].SetActive(true);
        state = StateGame.ShowInicio;
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
        if (!changelanguage)
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
                        showTextoKey("encuestageneral1");
                        state = StateGame.ShowEncuestageneral1;
                        infotext.executeFinish = false;
                        enableactions = true;
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

                        GameManager.Instance.player.playerClassification.ClassifyPlayer();
                        string prompt = $"A continuación te indico los resultados de las preguntas realizada al jugador y el nivel para cada categoría:";
                        foreach (var categoria in GameManager.Instance.player.playerClassification.playerResponses)
                        {
                            prompt += $"{categoria.Key}={categoria.Value.level.ToString()},";
                        }
                        prompt += $"el nivel global del jugador {GameManager.Instance.player.playerClassification.overallLevel}. Con estos datos necesitos que recomiendes al jugador que categorías debería enfocarse primero." +
                            $"La repuesta debe ser una frase de máximo 3 líneas. Escribe la respuesta como le estuvieras hablando al jugador. Si su nivel global bajo lo ideal sería recomendarle que siga el entrenamiento general. Y necesito que este en el idioma:{GameManager.Instance.GetLanguage()} ";
                        GameManager.Instance.SendIAMsg(prompt);
                        showTextoKey("finalencuesta");
                        state = StateGame.ShowFinalEncuesta;
                        enableactions = true;
                        break;
                    }
                case StateGame.ShowRecomendacion:
                    infotext.SetActiveInfo(false);
                    break;

            }
        }
    }



    void Update()
    {
        // Respuesta A -> X (2) 2 Puntos
        // Respuesta B -> A (0) 1 puntos
        // Respuesta C -> B (1) 0 puntos
        /*  { "General", new CategoryScore() },
        { "RecepcionMateriales", new CategoryScore() },
        { "PreparacionPedidos", new CategoryScore() },
        { "UbicacionMateriales", new CategoryScore() },
        { "ManejoCarretillas", new CategoryScore() }
        */

        if (state == StateGame.ShowInicio && !changelanguage)
        {
            state = StateGame.ShowBienVenido;
            infotext.SetActiveInfo(true);
            showTextoKey("PrimerBienvenida");
        }

        if (state == StateGame.ShowFinalEncuesta)
        {
            if (!GameManager.Instance.wait4IAResponse)
            {
                waitreading = false;
                showMsg(GameManager.Instance.IAResponse);
                state = StateGame.ShowRecomendacion;
                infotext.executeFinish = true;
            }

        }
        if (infotext.isFinish && state != StateGame.ShowBienVenido && state != StateGame.ShowEncuesta && enableactions)
        {
            var points = 0;
            bool respuestaencuesta = false;

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                points = 2;
                respuestaencuesta = true;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                points = 1;
                respuestaencuesta = true;
            }
            else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                respuestaencuesta = true;
            }
            if (respuestaencuesta)
            {
                enableactions = false;
                switch (state)
                {
                    case StateGame.ShowEncuestageneral1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("General", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestageneral2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("General", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaRecep1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("RecepcionMateriales", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaRecep2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("RecepcionMateriales", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaPicking1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("PreparacionPedidos", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaPicking2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("PreparacionPedidos", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaUbicacion1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("UbicacionMateriales", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaUbicacion2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("UbicacionMateriales", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaCarretilla1:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta1("ManejoCarretillas", points);
                            FinishInfoText();
                            break;
                        }
                    case StateGame.ShowEncuestaCarretilla2:
                        {
                            GameManager.Instance.player.playerClassification.setCategoriaPregunta2("ManejoCarretillas", points);
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
        GameManager.Instance.minlevel = currentlevel;
        if (currentlevel == 1)
        {
            GameManager.Instance.maxlevel = 3;
        }
        else if (currentlevel == 4)
        {
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
        var localizedstring = GenerateLocalizedStringInEditor("nivel" + currentlevel.ToString());
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

    internal enum StateGame
    {
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

    }
}
