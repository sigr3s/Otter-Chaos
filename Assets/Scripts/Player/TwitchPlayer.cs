using System;
using UnityEngine;

public class TwitchPlayer : MonoBehaviour {
    public TwitchPlayerModel playerData;
    public FactoryLine asignedLine;

    public void ExecuteCommand(int command)
    {
        //TODO: Player animation

        asignedLine.AddPlayerCommand(playerData.id, command);
    }

    public void SetData(TwitchPlayerModel newPlayer, FactoryLine asignedLine)
    {
        this.asignedLine = asignedLine;
        this.playerData = newPlayer;

        //TODO: Color y nombre del player
    }
}