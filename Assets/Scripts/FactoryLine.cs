using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryLine : MonoBehaviour {

    [Header("ID")]
    public string lineName;
    public Color color;

    [Header("Config")]
    [Space(10)]
    public float animationSpeed = 0.5f;
    public ResultMode resultMode = ResultMode.Democracy;


    [Header("Lights")]
    [Space(10)]
    public List<GameObject> lights;
    public Color correct;
    public Color incorrect;
    public Color missplaced;
    public Color disabled;

    [Header("Scene refernces")]
    public Text scoreText = null;
    public List<Transform> playerSlots;
    public List<CommandDisplay> displays;

    [Header("Props")]
    public GameObject[] propList = null;

    [Header("Debug")]
    [Space(30)]
    public bool running;
    public int[] sequence;
    public int[] result;
    public int[] votes;
    public int wins = 0;

    private Commands commands;
    private Action<FactoryLine> OnComplete;
    private Dictionary<string, int> actions = new Dictionary<string, int>();
    private List<TwitchPlayer> players = new List<TwitchPlayer>();
    private int currentFrame = 0;

    private Animator _animator = null;
    private Animator animator{
        get{
            if(_animator == null){
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }


    static class AnimatorHash
    {
        public static readonly int Move = Animator.StringToHash("move");
    }

    private void Awake() {
        foreach(GameObject l in lights){
            Material m = l.GetComponent<MeshRenderer>().material;
            m.color = disabled;
        }
    }

    public void StartLine(int[] sequence, Action<FactoryLine> OnComplete, Commands commands, int propIndex){
        this.sequence = sequence;
        this.OnComplete = OnComplete;
        this.commands = commands;
        currentFrame = 0;
        result = new int[sequence.Length];
        votes = new int[commands.actions.Count];
        running = true;

        animator.Play(AnimatorHash.Move, -1);
        animator.speed = animationSpeed;

        for (int i = 0; i < propList.Length; ++i)
        {
            if (i == propIndex) propList[i].SetActive(true);
            else propList[i].SetActive(false);
        }

        foreach(GameObject l in lights){
            Material m = l.GetComponent<MeshRenderer>().sharedMaterial;
            m.color = disabled;
        }

        foreach(var player in players){
            player.ClearVote();
        }
    }

    public void StopLine(){
        running = false;
        animator.speed = 0;
    }

    private void CheckFrame()
    {
        int res = GetPlayersResult();
        result[currentFrame] = res;
        displays[currentFrame].Display(commands.actions[res].image);

        if(currentFrame == sequence.Length - 1){
            currentFrame = 0;
        }
        else{
            currentFrame++;
        }

        foreach(var player in players){
            player.ClearVote();
        }
    }

    public bool IsCorrectResult(){
        bool correct = true;

        for(int i = 0; i < sequence.Length; i++ ){
            correct = correct & (sequence[i] == result[i]);
        }

        return correct;
    }

    private void CheckResults(){
        bool correct = true;

        int correctSlots = 0;
        int correctCommands = 0;

        bool[] possible = new bool[commands.actions.Count];
        bool[] incorrectGuess = new bool[commands.actions.Count];

        for(int i = 0; i < sequence.Length; i++ ){
            correct = correct & (sequence[i] == result[i]);

            if((sequence[i] == result[i])){
                correctSlots ++;
            }
            else{
                possible[sequence[i]] = true;
                incorrectGuess[result[i]] = true;
            }
        }

        for(int i = 0; i < possible.Length; i++){
            if(possible[i] && incorrectGuess[i]){
                correctCommands++;
            }
        }

        for(int l = 0; l < lights.Count; l++){
            if(correctSlots > 0){
                correctSlots--;
                lights[l].GetComponent<MeshRenderer>().sharedMaterial.color = this.correct;
            }
            else if(correctCommands > 0){
                correctCommands--;
                lights[l].GetComponent<MeshRenderer>().sharedMaterial.color = this.missplaced;
            }
            else{
                lights[l].GetComponent<MeshRenderer>().sharedMaterial.color = this.incorrect;
            }
        }

        if(correct){
            animator.speed = 0;
            Win();
        }
    }

    [ContextMenu("Win")]
    public void Win(){
        OnComplete(this);
    }

#region Player
    public TwitchPlayer SpawnPlayer(TwitchPlayer playerPrefab)
    {
        if(players.Count < playerSlots.Count){
            TwitchPlayer player = Instantiate(playerPrefab, playerSlots[players.Count].position, Quaternion.identity);
            return player;
        }
        else{
            return null;
        }
    }

    private int GetPlayersResult()
    {
        int result = 0;

        if(actions.Count == 0){
            return result;
        }

        switch(resultMode){
            case ResultMode.Democracy:

                int maxCount = 0;

                for(int i = 0; i < votes.Length; i++){
                    if(votes[i] > maxCount){
                        result = i;
                        maxCount = votes[i];
                    }
                }

            break;
        }

        actions = new Dictionary<string, int>();
        votes = new int[commands.actions.Count];

        return result;
    }

    public void AddPlayerCommand(string playerId, int command)
    {
        if(running){
            if(actions.ContainsKey(playerId)){
                int old = actions[playerId];
                actions[playerId] = command;

                votes[old] -= 1;
                votes[command] += 1;
            }
            else{
                actions.Add(playerId, command);
                votes[command] += 1;
            }
        }
    }

    public void NotifyWinner(bool hasWon)
    {
        foreach(var player in players){
            if(hasWon){
                player.Win();
            }
            else{
                player.Lose();
            }
        }
    }

    public void Unregister(TwitchPlayer twitchPlayer)
    {
        if(players.Contains(twitchPlayer)){
            players.Remove(twitchPlayer);
        }
    }

    public void RegisterPlayer(TwitchPlayer twitchPlayer)
    {
        players.Add(twitchPlayer);
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }
#endregion
}

public enum ResultMode{
    Democracy,
    Anarchy,
    King
}