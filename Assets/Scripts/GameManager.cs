using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Data")]
    public Commands commands;
    public GameStatus gameStatus;
    public Dictionary<string, TwitchPlayer> players = new Dictionary<string, TwitchPlayer>();

    [Header("Scene references")]
    [Space(10)]
    public GameObject messageBox = null;
    public GameObject endGameBox = null;
    public Image winnerColorImage = null;
    public Text messageText = null;
    public Text timerText = null;
    public CanvasGroup scoreBoard;
    public CanvasGroup suddenDeath;

    [Space(10)]
    public TwitchPlayer playerPrefab;
    public List<FactoryLine> factoryLines;


    [Header("Game Settings")]
    [Space(10)]
    public float joinPhaseDuration;
    public int rounds = 3;
    public int lineFrames = 3;

    [Header("Server Settings")]
    [Space(10)]
    private float secondsSinceLastRequest = 0f;
    public float secondsBetweenRequests = 2f;

    [SerializeField] private int currentRound = 1;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        currentRound = 1;
        API.instance.StartGame(commands);
        messageBox.SetActive(false);
        endGameBox.SetActive(false);
        StartCoroutine(JoinPhase(OnJoinPhaseCompleted));
    }

    private void Update() {

        secondsSinceLastRequest += Time.deltaTime;

        if(secondsSinceLastRequest > secondsBetweenRequests){
            secondsSinceLastRequest = 0f;

            API.instance.GetFrame(ProcessFrame);
        }

        API.instance.GetLocalFrame(ProcessFrame);
    }

    public FactoryLine GetLineForNewUser(){
        FactoryLine line = null;

        foreach(FactoryLine fl in factoryLines){

            if(line == null || fl.GetPlayerCount() < line.GetPlayerCount()){
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
        messageText.text = "Get ready! Type '!join' in the chat.";
        timerText.text = (int)joinPhaseDuration+"";
        messageBox.SetActive(true);
        for (int i = (int)joinPhaseDuration; i >= 0; i--)
        {
            timerText.text = i+"";
            SoundManager.instance.PlayCountdownBeep();
            yield return new WaitForSeconds(1.0f);
        }
        messageBox.SetActive(false);
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
        SoundManager.instance.PlayEndRound();
        yield return new WaitForSeconds(3);

        OnPostRoundComplete();
    }

    private IEnumerator EndGame(FactoryLine factory){
        Debug.Log("End game!");
        SoundManager.instance.PlayEndRound();
        winnerColorImage.color = factory.color;
        API.instance.EndGame();
        endGameBox.SetActive(true);
        yield return new WaitForSeconds(5f);
        endGameBox.SetActive(false);
        SceneManager.LoadScene(0);
    }

    private IEnumerator CheckWinner(){
        yield return new WaitForSeconds(0.5f);

        int first = 0;
        int second = 0;

        FactoryLine currentWinner = null;
        foreach(FactoryLine fl in factoryLines){
            bool correctSol = fl.IsCorrectResult();

            if(correctSol){
                fl.wins ++;
            }

            if(fl.wins > first){
                currentWinner = fl;
                second = first;
                first = fl.wins;
            }
            else if(fl.wins > second){
                second = fl.wins;
            }
            fl.scoreText.text = "" + fl.wins;

            fl.NotifyWinner(correctSol);
        }

        if(rounds-currentRound <= 0){
            if(first > second){
                StartCoroutine(EndGame(currentWinner));
            }
            else{
                scoreBoard.DOFade(0, 0.25f);
                suddenDeath.DOFade(1, 0.25f);
                gameStatus = GameStatus.PostRound;
                StartCoroutine(PostRound(OnPostRoundComplete));
            }
        }
        else{
            if( (first - second) > (rounds - currentRound)){
                gameStatus = GameStatus.EndGame;
                StartCoroutine(EndGame(currentWinner));

            }
            else{
                gameStatus = GameStatus.PostRound;
                StartCoroutine(PostRound(OnPostRoundComplete));

                if((first - second) == 0 && (rounds - currentRound) < 2){
                    scoreBoard.DOFade(0, 0.25f);
                    suddenDeath.DOFade(1, 0.25f);
                }
            }
        }
    }

#endregion

#region Callbacks
    private void ProcessFrame(DataFrame dataFrame)
    {
        if(dataFrame.new_players != null){
            foreach(TwitchPlayerModel newPlayer in dataFrame.new_players){
            // spawn players
            // add player to players dict

            if(players.ContainsKey(newPlayer.id)){
                Debug.LogWarning("Arnau me cago en todo lo cagable que esta repetido el player");
                continue;
            }

            FactoryLine asignedLine = GetLineForNewUser();
            TwitchPlayer player = asignedLine.SpawnPlayer(playerPrefab);

            if(player == null){
                Debug.LogWarning("Max players!");
                continue;
            }

            player.SetData(newPlayer, asignedLine, commands);
            players.Add(newPlayer.id, player);
        }

        }
        if(dataFrame.commands != null){
            foreach(var action in dataFrame.commands){

                if(players.ContainsKey(action.player_id)){
                    players[action.player_id].ExecuteCommand(action.command);
                }

            }
        }
    }

    private void OnJoinPhaseCompleted()
    {
        if(players.Count > 1){
            gameStatus = GameStatus.PreRound;
            SoundManager.instance.PlayRoundStart();
            StartCoroutine(PreRound(OnPreRoundComplete));
        }
        else{
            StartCoroutine(JoinPhase(OnJoinPhaseCompleted));
        }

    }

    private void OnPreRoundComplete(){
        gameStatus = GameStatus.Round;

        int[] sequence = GenerateSequence();

        int propIndex = UnityEngine.Random.Range(0, 3);

        foreach(FactoryLine fl in factoryLines){
            fl.StartLine(sequence, OnRoundComplete, commands, propIndex);
        }

        Debug.Log("Start round");

    }

    private void OnRoundComplete(FactoryLine winner)
    {
        Debug.Log("Round complete");

        if(!winner.running){ 
            return;
        }

        foreach(FactoryLine fl in factoryLines){
            fl.StopLine();
        }

        StartCoroutine(CheckWinner());
    }

    private void OnPostRoundComplete(){
        currentRound ++;
        StartCoroutine(PreRound(OnPreRoundComplete));
    }

#endregion
}