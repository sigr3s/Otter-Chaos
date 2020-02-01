using System.Collections.Generic;

[System.Serializable]
public class DataFrame : BaseResponse{
    public List<TwitchPlayerModel> new_players;
    public List<TwitchCommand> commands;
}