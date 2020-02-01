using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager instance { get { return _instance; } }

    [Header("Audiosources")]
    [SerializeField] private AudioSource musicAudioSource = null;
    [SerializeField] private AudioSource sfxAudioSource = null;

    [Header("AudioClips")]
    [SerializeField] private AudioClip bgMusic = null;
    [SerializeField] private AudioClip dingding = null;
    [SerializeField] private AudioClip gameStart = null;
    [SerializeField] private AudioClip roundStart = null;
    [SerializeField] private AudioClip error = null;
    [SerializeField] private AudioClip ui_beep = null;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        StartMusic();
    }

    void Update()
    {
        
    }

    public void StartMusic()
    {
        musicAudioSource.clip = bgMusic;
        if (!musicAudioSource.isPlaying) musicAudioSource.Play();
    }

    public void StopMusic()
    {
        if (!musicAudioSource.isPlaying) musicAudioSource.Pause();
    }

    public void PlayGameStart()
    {
        sfxAudioSource.PlayOneShot(gameStart);
    }

    public void PlayRoundStart()
    {
        sfxAudioSource.PlayOneShot(roundStart);
    }

    public void PlayEndRound()
    {
        sfxAudioSource.PlayOneShot(dingding);
    }

    public void PlayError()
    {
        sfxAudioSource.PlayOneShot(error);
    }

    public void PlayCountdownBeep()
    {
        sfxAudioSource.PlayOneShot(ui_beep);
    }
}
