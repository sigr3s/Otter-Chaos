using System;
using UnityEngine;

public class API : MonoBehaviour {
    public static API instance;
    public string channelID;
    public string baseUrl;



    [Header("Debug")]
    public DataFrame joinDataFrame;
    public DataFrame debugDataFrame;
    public bool sendJoin = false;
    public bool sendDebug = false;


    void Awake()
    {
        instance = this;
        if(PlayerPrefs.HasKey("TwitchChannel")){
            channelID = PlayerPrefs.GetString("TwitchChannel");
        }
    }

    public void StartGame(){

    }


    public void GetFrame(int lastTimestamp, Action<DataFrame> OnSuccess){

        if(sendDebug){
            DataFrame df = debugDataFrame;
            sendDebug = false;
            OnSuccess(df);
        }

        if(sendJoin){
            DataFrame df = joinDataFrame;
            sendJoin = false;
            OnSuccess(df);
        }
    }

    public void EndGame(){

    }



}