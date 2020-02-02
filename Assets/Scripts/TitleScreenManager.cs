﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button startGame = null;
    [SerializeField] private Button closeGame = null;
    [SerializeField] private InputField twitchChannel;

    void Start()
    {
        startGame.onClick.AddListener(OnStartButtonPressed);
        closeGame.onClick.AddListener(OnCloseButtonPressed);

        if(PlayerPrefs.HasKey("TwitchChannel")){
            twitchChannel.text = PlayerPrefs.GetString("TwitchChannel");
        }
    }

    void Update()
    {
        startGame.interactable =  !string.IsNullOrEmpty(twitchChannel.text);
    }

    public void OnStartButtonPressed()
    {
        PlayerPrefs.SetString("TwitchChannel", twitchChannel.text);
        PlayerPrefs.Save();
    
        SoundManager.instance.PlayGameStart();
        Invoke("LoadSceneGame", 2.2f);
    }

    public void OnCloseButtonPressed()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadSceneGame()
    {
        SceneManager.LoadScene(1);
    }
}
