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
    [SerializeField] private AudioClip error = null;


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
        musicAudioSource.PlayOneShot(gameStart);
    }

    public void PlayEndRound()
    {
        musicAudioSource.PlayOneShot(dingding);
    }

    public void PlayeError()
    {
        musicAudioSource.PlayOneShot(error);
    }
}
