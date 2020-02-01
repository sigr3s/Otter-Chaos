[System.Serializable]
public class TwitchPlayerModel{
    public TwitchPlayerModel(string playerId, string color){
        id = playerId;
        this.color = color;
    }
    
    public string id;
    public string color;
}