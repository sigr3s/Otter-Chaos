using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button startGame = null;

    void Start()
    {
        startGame.onClick.AddListener(OnStartButtonPressed);
    }

    void Update()
    {
        
    }

    public void OnStartButtonPressed()
    {
        Invoke("LoadSceneGame", 2.2f);
    }

    public void LoadSceneGame()
    {
        SceneManager.LoadScene(1);
    }
}
