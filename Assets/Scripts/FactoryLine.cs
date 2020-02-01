using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryLine : MonoBehaviour {
    public float frameTime = 10f;
    public List<Transform> playerSlots;
    public ResultMode resultMode = ResultMode.Democracy;

    [Header("Debug")]
    public bool running;
    public int[] sequence;
    public int[] result;
    public int[] votes;

    public int wins = 0;
    public int playerCount;

    private Commands commands;
    private Action<FactoryLine> OnComplete;
    private Dictionary<string, int> actions = new Dictionary<string, int>();
    [SerializeField] private List<TwitchPlayer> players = new List<TwitchPlayer>();
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private int currentFrame = 0;

    public void StartLine(int[] sequence, Action<FactoryLine> OnComplete, Commands commands){
        this.sequence = sequence;
        this.OnComplete = OnComplete;
        this.commands = commands;
        currentTime = 0f;
        currentFrame = 0;
        result = new int[sequence.Length];
        votes = new int[commands.actions.Count];
        running = true;
    }

    public void StopLine(){
        running = false;
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

    public int ComputeActionResult(){
        return 0;
    }

    void Update()
    {
        if(running){
            currentTime += Time.deltaTime;

            if(currentTime > frameTime){
                currentTime = 0f;
                CheckFrame();
            }
        }
    }

    private void CheckFrame()
    {
        int res = GetPlayersResult();
        result[currentFrame] = res;

        if(currentFrame == sequence.Length - 1){
            if(CheckResults()){
                Win();
            }
            else{
                currentFrame = 0;
            }
        }
        else{
            currentFrame++;
        }

    }

    private bool CheckResults(){
        bool correct = true;

        for(int i = 0; i < sequence.Length; i++ ){
            correct = correct & (sequence[i] == result[i]);
        }

        return correct;
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

    public TwitchPlayer SpawnPlayer(TwitchPlayer playerPrefab)
    {
        TwitchPlayer player = Instantiate(playerPrefab, playerSlots[playerCount].position, Quaternion.identity);
        players.Add(player);
        playerCount++;
        return player;
    }


    [ContextMenu("Win")]
    public void Win(){
        OnComplete(this);
    }
}

public enum ResultMode{
    Democracy,
    Anarchy,
    King
}