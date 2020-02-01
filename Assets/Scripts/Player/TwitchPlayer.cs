using System;
using UnityEngine;

public class TwitchPlayer : MonoBehaviour {
    public TwitchPlayerModel playerData;
    public FactoryLine asignedLine;
    public CommandDisplay commandDisplay;

    private Commands commands;

    public void ExecuteCommand(int command)
    {
        //TODO: Player animation

        asignedLine.AddPlayerCommand(playerData.id, command);
        commandDisplay.Display(commands.actions[command].image);
    }

    public void SetData(TwitchPlayerModel newPlayer, FactoryLine asignedLine, Commands commands)
    {
        if(this.asignedLine != null){
            this.asignedLine.Unregister(this);
        }

        this.asignedLine = asignedLine;
        this.playerData = newPlayer;
        this.commands = commands;

        this.asignedLine.RegisterPlayer(this);

        //TODO: Color y nombre del player
    }

    public void Lose(){
        commandDisplay.Display(commands.hate);
    }

    public void Win()
    {
        commandDisplay.Display(commands.love);
    }

    public void ClearVote()
    {
        commandDisplay.Display(commands.bg);
    }
}