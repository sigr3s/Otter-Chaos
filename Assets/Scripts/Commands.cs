using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Commands", menuName = "Twitch/Commands", order = 0)]
public class Commands : ScriptableObject {
    public string join;
    public List<TwitchAction> actions;
}

[System.Serializable]
public class TwitchAction{
    public string actionName;
    public Sprite image;
}