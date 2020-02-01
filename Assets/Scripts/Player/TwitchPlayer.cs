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
        if(this.asignedLine != null){
            this.asignedLine.Unregister(this);
        }

        this.asignedLine = asignedLine;
        this.playerData = newPlayer;

        this.asignedLine.RegisterPlayer(this);

        //TODO: Color y nombre del player
    }
}