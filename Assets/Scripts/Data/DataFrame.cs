using System.Collections.Generic;

[System.Serializable]
public class DataFrame{

    public List<TwitchPlayerModel> joined;

    public List<TwitchCommand> actions;

    public int timestamp;

}