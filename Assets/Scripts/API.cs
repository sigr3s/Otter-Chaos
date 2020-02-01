using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour {
    public static API instance;
    public string channelID;
    public string baseUrl;
    public GameSession gameSession;



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


    private IEnumerator SendRequest<T>(string endpoint, Action<T> Success, Action Failure){
        using (UnityWebRequest webRequest = UnityWebRequest.Get(baseUrl + "/" + endpoint))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);

                Failure.Invoke();
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);

                T res = (T) JsonUtility.FromJson(webRequest.downloadHandler.text, typeof(T));

                Success.Invoke(res);
            }
        }
    }

    public void StartGame(Commands commands){
        SessionData sd = new SessionData();
        sd.channel = channelID;
        sd.commands = new List<string>();

        foreach(var c in commands.actions){
            sd.commands.Add(c.actionName);
        }

        StartCoroutine(StartGameSession(JsonUtility.ToJson(sd), GameStarted, StartError));
    }

    private void GameStarted(GameSession gameSession){

        this.gameSession = gameSession;

    }

    private void StartError(){
        Debug.LogError("Critical error");
    }

    private IEnumerator StartGameSession(string jsonData, Action<GameSession> callback, Action error){
        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "/game/start", jsonData))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                error.Invoke();
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                callback.Invoke(JsonUtility.FromJson<GameSession>(www.downloadHandler.text));
            }
        }
    }


    public void GetFrame(int lastTimestamp, Action<DataFrame> OnSuccess){
        if(!string.IsNullOrEmpty(gameSession.session_id)){
            StartCoroutine(SendRequest<DataFrame>("/game/" + gameSession.session_id, OnSuccess, () =>{
                //TODO: Handle this
            }));
        }
        else{
            //Debug.LogWarning("Session not started")
        }

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