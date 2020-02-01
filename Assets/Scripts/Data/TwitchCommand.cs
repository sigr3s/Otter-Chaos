[System.Serializable]
public class TwitchCommand{
    public TwitchCommand(string id, int c){
        player_id = id;
        command = c;
    }
    
    public string player_id;
    public int command;
}