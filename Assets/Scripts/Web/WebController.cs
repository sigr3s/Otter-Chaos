using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebController : MonoBehaviour
{
    public Commands commands;

    public InputField userName;
    public InputField sessionID;
    public Button join;

    public GameObject controls;

    private void Update() {
        join.interactable = !string.IsNullOrEmpty(userName.text) && !string.IsNullOrEmpty(sessionID.text);
    }

    public void Call(){
        SessionData sd = new SessionData();
        sd.channel = "sigr3s";
        sd.commands = new List<string>();

        foreach(var c in commands.actions){
            sd.commands.Add(c.actionName);
        }

        StartCoroutine(API.instance.JoinWebController(new WebControllerJoinRequest(sessionID.text, userName.text), OnOk, OnKo));
    }

    private void OnKo()
    {
        controls.SetActive(false);
    }

    private void OnOk()
    {
        controls.SetActive(true);
    }
}
