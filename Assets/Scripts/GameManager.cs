using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Commands commands;
    public GameStatus gameStatus;
    public Dictionary<string, TwitchPlayer> players = new Dictionary<string, TwitchPlayer>();


    [Header("Scene references")]
    public List<FactoryLine> factoryLines;

    [Space(10)]
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
        currentRound = 1;
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

    private int[] GenerateSequence(){
        int[] sequence = new int[lineFrames];

        for(int i = 0; i < lineFrames; i++){
            int sn = UnityEngine.Random.Range(0, commands.actions.Count);
            sequence[i] = sn;
            Debug.Log("Sequence number " +  i  +  " :   " + sn );
        }

        return sequence;
    }

#region GamePhases
    private IEnumerator JoinPhase(System.Action JoinPhaseCompleted)
    {
        Debug.Log("Start Join Phase!");
        yield return new WaitForSeconds(joinPhaseDuration);
        JoinPhaseCompleted();
    }

    private IEnumerator PreRound(System.Action OnPreRoundComplete)
    {
        Debug.Log("Start pre round");
        yield return new WaitForSeconds(3);
        OnPreRoundComplete();
    }

    private IEnumerator PostRound(Action OnPostRoundComplete){
        Debug.Log("Start post round");

        yield return new WaitForSeconds(3);

        OnPostRoundComplete();
    }

    private IEnumerator EndGame(){
        Debug.Log("End game!");

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
        if(players.Count > 1){
            gameStatus = GameStatus.PreRound;
            StartCoroutine(PreRound(OnPreRoundComplete));
        }
        else{
            StartCoroutine(JoinPhase(OnJoinPhaseCompleted));
        }

    }

    private void OnPreRoundComplete(){
        gameStatus = GameStatus.Round;

        int[] sequence = GenerateSequence();

        foreach(FactoryLine fl in factoryLines){
            fl.StartLine(sequence, OnRoundComplete, commands);
        }

        Debug.Log("Start round");

    }

    private void OnRoundComplete(FactoryLine winner)
    {
        Debug.Log("Round complete");

        winner.wins ++;

        int first = 0;
        int second = 0;

        foreach(FactoryLine fl in factoryLines){
            fl.StopLine();

            if(fl.wins > first){
                second = first;
                first = fl.wins;
            }
            else if(fl.wins > second){
                second = fl.wins;
            }
        }

        if( (first - second) > (rounds - currentRound)){
            gameStatus = GameStatus.EndGame;
            StartCoroutine(EndGame());

        }
        else{
            gameStatus = GameStatus.PostRound;
            StartCoroutine(PostRound(OnPostRoundComplete));
        }
    }

    private void OnPostRoundComplete(){
        currentRound ++;
        StartCoroutine(PreRound(OnPreRoundComplete));
    }
    
    #endregion
}