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
    public string player1 = "sigr3s";
    public string player2 = "drixide";
    private DataFrame joinDataFrame;
    private bool sendJoin = false;
    public DataFrame debugDataFrame = new DataFrame();

    void Awake()
    {
        instance = this;
        if(PlayerPrefs.HasKey("TwitchChannel")){
            channelID = PlayerPrefs.GetString("TwitchChannel");
        }
    }


    private void Update() {
        if(Input.GetKeyDown(KeyCode.J)){
            joinDataFrame = new DataFrame();
            joinDataFrame.new_players = new List<TwitchPlayerModel>();
            joinDataFrame.new_players.Add(new TwitchPlayerModel(player1, "#b975f0"));
            joinDataFrame.new_players.Add(new TwitchPlayerModel(player2, "#7bd12a"));
            sendJoin = true;
        }

        if(debugDataFrame.commands == null){
            debugDataFrame.commands = new List<TwitchCommand>();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)){
            debugDataFrame.commands.Add(new TwitchCommand(player1, 0));
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            debugDataFrame.commands.Add(new TwitchCommand(player1, 1));
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            debugDataFrame.commands.Add(new TwitchCommand(player1, 2));
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            debugDataFrame.commands.Add(new TwitchCommand(player1, 3));
        }

        if(Input.GetKeyDown(KeyCode.V)){
            debugDataFrame.commands.Add(new TwitchCommand(player2, 0));
        }
        if(Input.GetKeyDown(KeyCode.B)){
            debugDataFrame.commands.Add(new TwitchCommand(player2, 1));
        }
        if(Input.GetKeyDown(KeyCode.N)){
            debugDataFrame.commands.Add(new TwitchCommand(player2, 2));
        }
        if(Input.GetKeyDown(KeyCode.M)){
            debugDataFrame.commands.Add(new TwitchCommand(player2, 3));
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
        using (UnityWebRequest www = UnityWebRequest.Put(baseUrl + "/game", jsonData))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.url);
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

    public void GetFrame(Action<DataFrame> OnSuccess){
        /*
        if(!string.IsNullOrEmpty(gameSession.session_id)){
            StartCoroutine(SendRequest<DataFrame>("/game/" + gameSession.session_id, OnSuccess, () =>{
                //TODO: Handle this
            }));
        }
        else{
            //Debug.LogWarning("Session not started")
        }*/
    }

    public void GetLocalFrame(Action<DataFrame> OnSuccess){
        if(debugDataFrame.commands != null && debugDataFrame.commands.Count > 0){
            DataFrame df = debugDataFrame;
            debugDataFrame = new DataFrame();
            OnSuccess(df);
        }

        if(sendJoin){
            DataFrame df = joinDataFrame;
            sendJoin = false;
            OnSuccess(df);
        }
    }

    public void EndGame(){
        if(!string.IsNullOrEmpty(gameSession.session_id)){
            StartCoroutine(CloseGame());
        }
    }

    private IEnumerator CloseGame()
    {

        using (UnityWebRequest www = UnityWebRequest.Delete(baseUrl + "/game/" + gameSession.session_id))
        {
            yield return www.SendWebRequest();

        }
    }

    private void OnDestroy() {
        if(!string.IsNullOrEmpty(gameSession.session_id)){
            using (UnityWebRequest www = UnityWebRequest.Delete(baseUrl + "/game/" + gameSession.session_id))
            {
                www.SendWebRequest();

                while(!www.isDone && !www.isNetworkError){

                }
            }
        }
    }
}