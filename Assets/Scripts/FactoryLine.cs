using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryLine : MonoBehaviour {

    [Header("ID")]
    public string name;
    public Color color;

    [Header("Config")]
    public float animationSpeed = 0.5f;
    public List<Transform> playerSlots;
    public List<CommandDisplay> displays;

    [Space(20)]

    [Header("Lights")]
    public List<GameObject> lights;
    public Color correct;
    public Color incorrect;
    public Color missplaced;
    public Color disabled;

    [Space(20)]

    [SerializeField] Animator animator = default;
    public ResultMode resultMode = ResultMode.Democracy;
    public Text scoreText = null;

    [Header("Debug")]
    public bool running;
    public int[] sequence;
    public int[] result;
    public int[] votes;
    public int wins = 0;

    private Commands commands;
    private Action<FactoryLine> OnComplete;
    private Dictionary<string, int> actions = new Dictionary<string, int>();
    [SerializeField] private List<TwitchPlayer> players = new List<TwitchPlayer>();
    [SerializeField] private int currentFrame = 0;


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

    public void StartLine(int[] sequence, Action<FactoryLine> OnComplete, Commands commands){
        this.sequence = sequence;
        this.OnComplete = OnComplete;
        this.commands = commands;
        currentFrame = 0;
        result = new int[sequence.Length];
        votes = new int[commands.actions.Count];
        running = true;

        animator.Play(AnimatorHash.Move, -1);
        animator.speed = animationSpeed;

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

    private void CheckResults(){
        bool correct = true;

        for(int i = 0; i < sequence.Length; i++ ){
            correct = correct & (sequence[i] == result[i]);
        }

        if(correct){
            animator.speed = 0;
            Win();
        }
    }

    [ContextMenu("Win")]
    public void Win(){
        OnComplete(this);
        foreach(var player in players){
            player.Win();
        }
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