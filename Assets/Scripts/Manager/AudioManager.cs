using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip gameplayTheme;
    [SerializeField] private float musicVolume = 0.5f;

    [Header("Jingles")]
    [SerializeField] private AudioSource jingleSource;
    [SerializeField] private AudioClip dayStartJingle;
    [SerializeField] private AudioClip gameOverJingle;
    [SerializeField] private float jingleVolume = 0.7f;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip enemyKillSound;
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip createSandwichSound;
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private float sfxVolume = 0.6f;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
        }
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (jingleSource == null)
        {
            GameObject jingleObj = new GameObject("JingleSource");
            jingleObj.transform.SetParent(transform);
            jingleSource = jingleObj.AddComponent<AudioSource>();
        }
        jingleSource.loop = false;
        jingleSource.volume = jingleVolume;

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
        }
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    private void Start()
    {
        PlayGameplayTheme();
    }

    public void PlayGameplayTheme()
    {
        if (gameplayTheme != null && musicSource != null)
        {
            musicSource.clip = gameplayTheme;
            musicSource.Play();
        }
    }

    public void PlayDayStartJingle()
    {
        if (dayStartJingle != null && jingleSource != null)
        {
            StartCoroutine(PlayJingleAndResumeMusic(dayStartJingle));
        }
    }

    public void PlayGameOverJingle()
    {
        if (gameOverJingle != null && jingleSource != null)
        {
            musicSource.Stop();
            jingleSource.PlayOneShot(gameOverJingle);
        }
    }

    private IEnumerator PlayJingleAndResumeMusic(AudioClip jingle)
    {
        musicSource.Pause();
        jingleSource.PlayOneShot(jingle);
        yield return new WaitForSeconds(jingle.length);
        musicSource.UnPause();
    }

    public void PlayEnemyHitSound()
    {
        if (enemyHitSound != null)
        {
            sfxSource.PlayOneShot(enemyHitSound);
        }
    }

    public void PlayEnemyKillSound()
    {
        if (enemyKillSound != null)
        {
            sfxSource.PlayOneShot(enemyKillSound);
        }
    }

    public void PlayThrowSound()
    {
        if (throwSound != null)
        {
            sfxSource.PlayOneShot(throwSound);
        }
    }

    public void PlayCreateSandwichSound()
    {
        if (createSandwichSound != null)
        {
            sfxSource.PlayOneShot(createSandwichSound);
        }
    }

    public void PlayPlayerHitSound()
    {
        if (playerHitSound != null)
        {
            sfxSource.PlayOneShot(playerHitSound);
        }
    }

    public void StopAllMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}
