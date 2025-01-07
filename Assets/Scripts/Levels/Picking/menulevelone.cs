using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class menulevelone : MonoBehaviour
{

    [SerializeField] public GameObject options;
    [SerializeField] public GameObject controles;
    [SerializeField] AudioClip bottonClip;

    private bool changelanguage = false;
    private void Start()
    {
        options.SetActive(true);
        controles.SetActive(false);
    }

    public void SetOpciones()
    {
        options.SetActive(true);
        controles.SetActive(false);
    }

    public void SetControles()
    {
        options.SetActive(false);
        controles.SetActive(true);
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

    public void ChangePenalizacion(bool value)
    {
        SoundManager.SharedInstance.PlaySound(bottonClip);
        GameManager.Instance.penalización = value;
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
}


