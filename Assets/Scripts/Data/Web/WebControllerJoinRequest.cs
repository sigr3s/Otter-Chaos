using UnityEngine;

[System.Serializable]
public class WebControllerJoinRequest{
    public string session_id;
    public string user_id;
    public string color;

    public WebControllerJoinRequest(string session, string user_id){
        this.session_id = session;
        this.user_id = user_id;

        Color c = new Color(
            UnityEngine.Random.Range(0, 255) / 255f,
            UnityEngine.Random.Range(0, 255) / 255f,
            UnityEngine.Random.Range(0, 255) / 255f,
            255
        );

        this.color = ColorUtility.ToHtmlStringRGB(c);
    }
}