using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SendCommand : MonoBehaviour
{
    public Commands commands;
    public Image image;

    private Button _button;

    private Button button{
        get{
            if(_button == null){
                _button = GetComponent<Button>();
            }

            return _button;
        }
    }


    public int commandID = 0;

    void Start()
    {
        image.sprite = commands.actions[commandID].image;
        button.onClick.AddListener(SendWebCommand);
    }

    private void SendWebCommand()
    {
        API.instance.SendWebCommand(commandID);
    }
}
