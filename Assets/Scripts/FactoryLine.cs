using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryLine : MonoBehaviour {
    public List<int> sequence;
    public bool running;
    public int playerCount;
    public int wins = 0;

    private Action<FactoryLine> OnComplete;
    private Dictionary<string, int> actions;


    public List<Transform> playerSlots;

    public void StartLine(List<int> sequence, Action<FactoryLine> OnComplete){
        this.sequence = sequence;
        this.OnComplete = OnComplete;
    }

    public void StopLine(){
        running = false;
    }

    public void AddPlayerCommand(string playerId, int command)
    {
        if(running){
            if(actions.ContainsKey(playerId)){
                actions[playerId] = command;
            }
            else{
                actions.Add(playerId, command);
            }
        }
    }

    public int ComputeActionResult(){
        return 0;
    }

    void Update()
    {
        if(running){

        }
    }

    public TwitchPlayer SpawnPlayer(TwitchPlayer playerPrefab)
    {
        TwitchPlayer player = Instantiate(playerPrefab, playerSlots[playerCount].position, Quaternion.identity);
        playerCount++;
        return player;
    }


    [ContextMenu("Win")]
    public void Win(){
        OnComplete(this);
    }
}