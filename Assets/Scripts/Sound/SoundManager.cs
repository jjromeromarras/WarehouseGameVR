using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource effectsSource, musicSource, textSource;
    public static SoundManager SharedInstance;

    private void Awake()
    {
        if (SharedInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            SharedInstance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        PlayClip(effectsSource, clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        PlayClip(musicSource, clip);
    }

    public void PlayWrite(AudioClip clip)
    {
        PlayClip(textSource, clip);
    }    

    private void PlayClip(AudioSource audio, AudioClip clip)
    {
        audio.Stop();
        audio.clip = clip;
        audio.Play();
    }
}
