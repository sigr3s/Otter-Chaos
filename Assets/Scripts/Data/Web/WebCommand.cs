[System.Serializable]
public class WebCommand{
    public string session_id;
    public string user_id;
    public int command;

    public WebCommand(string sid, string uid, int c){
        session_id = sid;
        user_id = uid;
        command = c;
    }
}