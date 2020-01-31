using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Commands commands;
    public GameStatus gameStatus;
    public Dictionary<string, TwitchPlayer> players;


    [Header("Scene references")]
    public List<FactoryLine> factoryLines;
    public TwitchPlayer playerPrefab;

    [Header("Game Settings")]
    public float joinPhaseDuration;
    public int rounds = 3;
    public int lineFrames = 3;


    [Header("Server Settings")]
    private float secondsSinceLastRequest = 0f;
    public float secondsBetweenRequests = 2f;

    private int lastTimestamp = 0;
    private int currentRound = 0;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        API.instance.StartGame();
        StartCoroutine(JoinPhase(OnJoinPhaseCompleted));
    }

    private void Update() {

        secondsSinceLastRequest += Time.deltaTime;

        if(secondsSinceLastRequest > secondsBetweenRequests){
            secondsSinceLastRequest = 0f;

            API.instance.GetFrame(lastTimestamp, OnSuccess);
        }
    }

    public FactoryLine GetLineForNewUser(){
        FactoryLine line = null;

        foreach(FactoryLine fl in factoryLines){

            if(line == null || fl.playerCount < line.playerCount){
                line = fl;
            }
        }

        return line;
    }

    private List<int> GenerateSequence(){
        List<int> sequence = new List<int>();

        for(int i = 0; i < lineFrames; i++){
            sequence.Add(UnityEngine.Random.Range(0, commands.actions.Count));
        }

        return sequence;
    }
#region GamePhases
    private IEnumerator JoinPhase(System.Action JoinPhaseCompleted)
    {
        yield return new WaitForSeconds(joinPhaseDuration);
        JoinPhaseCompleted();
    }

    private IEnumerator PreRound(System.Action OnPreRoundComplete)
    {

        yield return null;
        OnPreRoundComplete();
    }

    private IEnumerator PostRound(Action OnPostRoundComplete){
        yield return null;
    }

#endregion

#region Callbacks
    private void OnSuccess(DataFrame dataFrame)
    {
        foreach(TwitchPlayerModel newPlayer in dataFrame.joined){
            // spawn players
            // add player to players dict

            if(players.ContainsKey(newPlayer.id)){
                Debug.LogWarning("Arnau me cago en todo lo cagable que esta repetido el player");
                continue;
            }

            FactoryLine asignedLine = GetLineForNewUser();
            TwitchPlayer player = asignedLine.SpawnPlayer(playerPrefab);

            player.SetData(newPlayer, asignedLine);
            players.Add(newPlayer.id, player);
        }

        foreach(var action in dataFrame.actions){

            if(players.ContainsKey(action.playerID)){
                players[action.playerID].ExecuteCommand(action.command);
            }

        }
    }

    private void OnJoinPhaseCompleted()
    {
        gameStatus = GameStatus.PreRound;
        StartCoroutine(PreRound( ()=>{} ));

    }


    private void OnPreRoundComplete(){
        gameStatus = GameStatus.Round;

        List<int> sequence = GenerateSequence();

        foreach(FactoryLine fl in factoryLines){
            fl.StartLine(sequence, OnRoundComplete);
        }
    }

    private void OnRoundComplete(FactoryLine winner)
    {
        winner.wins ++;

        foreach(FactoryLine fl in factoryLines){
            fl.StopLine();

            if(fl != winner){

            }
        }



        StartCoroutine(PostRound(OnPostRoundComplete));
    }

    private void OnPostRoundComplete(){
        Debug.Log("Round complete");
    }
    #endregion
}

