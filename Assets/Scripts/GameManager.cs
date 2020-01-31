using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Commands commands;


    private void Awake() {
        instance = this;
    }

    private void Start() {
        
    }
}
