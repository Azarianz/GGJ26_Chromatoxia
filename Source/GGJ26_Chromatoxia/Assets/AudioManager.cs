using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Menu")]
    public string mainMenuSceneName = "MainMenu";
    public AudioClip menuMusic;

    [Header("Gameplay (random between these)")]
    public List<AudioClip> gameplayTracks = new(); // put 2 tracks here
    [Tooltip("Scenes that count as gameplay (music will randomize when entering any of these). Leave empty = anything not menu is gameplay.")]
    public List<string> gameplaySceneNames = new(); // optional: StageGraph, RunStart, Boss, etc.

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    public bool loop = true;
    public bool rerollGameplayTrackOnEveryGameplaySceneLoad = false;

    string currentScenePlaying = "";
    AudioClip currentClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = loop;
        musicSource.volume = musicVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    void PlayForScene(string sceneName)
    {
        if (sceneName == currentScenePlaying)
            return;

        currentScenePlaying = sceneName;

        // MENU
        if (IsMenu(sceneName))
        {
            PlayClip(menuMusic);
            return;
        }

        // GAMEPLAY
        if (IsGameplay(sceneName))
        {
            if (!rerollGameplayTrackOnEveryGameplaySceneLoad && IsGameplayTrack(currentClip))
                return; // keep the current gameplay track across gameplay scenes

            PlayClip(PickRandomGameplayTrack());
            return;
        }

        // Fallback: stop if scene doesn't match anything
        musicSource.Stop();
        currentClip = null;
    }

    bool IsMenu(string sceneName)
    {
        return sceneName == mainMenuSceneName;
    }

    bool IsGameplay(string sceneName)
    {
        // If list is empty, treat "anything not menu" as gameplay
        if (gameplaySceneNames == null || gameplaySceneNames.Count == 0)
            return !IsMenu(sceneName);

        return gameplaySceneNames.Contains(sceneName);
    }

    bool IsGameplayTrack(AudioClip clip)
    {
        if (clip == null) return false;
        return gameplayTracks != null && gameplayTracks.Contains(clip);
    }

    AudioClip PickRandomGameplayTrack()
    {
        if (gameplayTracks == null || gameplayTracks.Count == 0)
            return null;

        // If 2 tracks, avoid repeating the same one when rerolling
        if (gameplayTracks.Count > 1 && currentClip != null)
        {
            const int tries = 10;
            for (int t = 0; t < tries; t++)
            {
                var c = gameplayTracks[Random.Range(0, gameplayTracks.Count)];
                if (c != null && c != currentClip) return c;
            }
        }

        return gameplayTracks[Random.Range(0, gameplayTracks.Count)];
    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            musicSource.Stop();
            currentClip = null;
            return;
        }

        if (currentClip == clip && musicSource.isPlaying)
            return;

        currentClip = clip;
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Optional API
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource) musicSource.volume = musicVolume;
    }

    public void StopMusic()
    {
        if (musicSource) musicSource.Stop();
        currentClip = null;
        currentScenePlaying = "";
    }
}