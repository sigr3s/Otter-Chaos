using System;
using UnityEngine;

public class API : MonoBehaviour {
    public static API instance;
    public string channelID;
    public string baseUrl;


    void Awake()
    {
        if(PlayerPrefs.HasKey("TwitchChannel")){
            channelID = PlayerPrefs.GetString("TwitchChannel");
        }
    }

    public void StartGame(){

    }


    public void GetFrame(int lastTimestamp, Action<DataFrame> OnSuccess){

        
    }

    public void EndGame(){

    }



}