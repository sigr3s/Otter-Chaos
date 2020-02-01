using System;
using System.Collections.Generic;
using UnityEngine;

public class TwitchPlayer : MonoBehaviour {
    public TwitchPlayerModel playerData;
    public Animator animator;
    public CommandDisplay commandDisplay;
    public GameObject arpon;

    [Header("Props")]
    public List<GameObject> hats;
    public Transform hatTransform;

    [Header("Debug")]
    public FactoryLine asignedLine;
    private Commands commands;

    private void Awake() {
        Color c = new Color(
            UnityEngine.Random.Range(0, 255) / 255f,
            UnityEngine.Random.Range(0, 255) / 255f,
            UnityEngine.Random.Range(0, 255) / 255f,
            255
        );

        Material m = arpon.GetComponent<MeshRenderer>().material;
        m.color = c;

        int prop = UnityEngine.Random.Range(0, hats.Count * 2);

        if(prop >= hats.Count){

        }
        else{
            GameObject hat = Instantiate(hats[prop], hatTransform);
            hat.transform.localPosition = Vector3.zero;
            hat.transform.localRotation = Quaternion.identity;
            hat.transform.localScale = Vector3.one;
        }
    }

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

        Color color = new Color(
            UnityEngine.Random.Range(0, 255) / 255f,
            UnityEngine.Random.Range(0, 255) / 255f,
            UnityEngine.Random.Range(0, 255) / 255f,
            255
        );

        ColorUtility.TryParseHtmlString(newPlayer.color, out color);

        arpon.GetComponent<MeshRenderer>().sharedMaterial.color = color;

        //TODO: Color y nombre del player
    }

    public void Lose(){
        commandDisplay.Display(commands.hate);
        animator.SetTrigger("lose");
    }

    public void Win()
    {
        commandDisplay.Display(commands.love);
        animator.SetTrigger("win");
    }

    public void ClearVote()
    {
        commandDisplay.Display(playerData.id.Substring(0,1));
        animator.SetTrigger("reset");
    }
}