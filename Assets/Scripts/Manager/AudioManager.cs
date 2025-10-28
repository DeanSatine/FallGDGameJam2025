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

    [Header("Pitch Randomization")]
    [SerializeField] private bool randomizePitch = true;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    [Header("Narration")]
    [SerializeField] private AudioSource narrationSource;
    [SerializeField] private AudioClip[] narrationClips;
    [SerializeField] private float narrationVolume = 0.8f;
    [SerializeField] private float narrationInterval = 5f;
    [SerializeField] private int maxNarrationLinesPerDay = 5;

    private static AudioManager instance;
    private Coroutine narrationCoroutine;
    private int currentNarrationIndex = 0;

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

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            PlayGameplayTheme();
        }
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

        if (narrationSource == null)
        {
            GameObject narrationObj = new GameObject("NarrationSource");
            narrationObj.transform.SetParent(transform);
            narrationSource = narrationObj.AddComponent<AudioSource>();
        }
        narrationSource.loop = false;
        narrationSource.volume = narrationVolume;
        narrationSource.spatialBlend = 0f;
    }

    private void Start()
    {
        PlayGameplayTheme();
    }

    private void PlaySFXWithRandomPitch(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        if (randomizePitch)
        {
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            sfxSource.pitch = 1f;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayGameplayTheme()
    {
        if (gameplayTheme != null && musicSource != null)
        {
            if (musicSource.isPlaying && musicSource.clip == gameplayTheme)
            {
                return;
            }

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

    public void StartNarrationForDay(int day)
    {
        if (narrationClips == null || narrationClips.Length == 0) return;

        int startIndex = (day - 1) * maxNarrationLinesPerDay;

        if (startIndex >= narrationClips.Length)
        {
            return;
        }

        currentNarrationIndex = startIndex;

        if (narrationCoroutine != null)
        {
            StopCoroutine(narrationCoroutine);
        }

        narrationCoroutine = StartCoroutine(PlayNarrationSequence(day));
    }

    private IEnumerator PlayNarrationSequence(int day)
    {
        int startIndex = (day - 1) * maxNarrationLinesPerDay;
        int endIndex = Mathf.Min(startIndex + maxNarrationLinesPerDay, narrationClips.Length);

        for (int i = startIndex; i < endIndex; i++)
        {
            if (narrationClips[i] != null && narrationSource != null)
            {
                narrationSource.PlayOneShot(narrationClips[i], narrationVolume);
            }

            yield return new WaitForSeconds(narrationInterval);
        }

        narrationCoroutine = null;
    }

    public void StopNarration()
    {
        if (narrationCoroutine != null)
        {
            StopCoroutine(narrationCoroutine);
            narrationCoroutine = null;
        }

        if (narrationSource != null && narrationSource.isPlaying)
        {
            narrationSource.Stop();
        }
    }

    public void PlayEnemyHitSound()
    {
        PlaySFXWithRandomPitch(enemyHitSound);
    }

    public void PlayEnemyKillSound()
    {
        PlaySFXWithRandomPitch(enemyKillSound);
    }

    public void PlayThrowSound()
    {
        PlaySFXWithRandomPitch(throwSound);
    }

    public void PlayCreateSandwichSound()
    {
        PlaySFXWithRandomPitch(createSandwichSound);
    }

    public void PlayPlayerHitSound()
    {
        PlaySFXWithRandomPitch(playerHitSound);
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

    public void SetNarrationVolume(float volume)
    {
        narrationVolume = Mathf.Clamp01(volume);
        if (narrationSource != null)
        {
            narrationSource.volume = narrationVolume;
        }
    }
}
