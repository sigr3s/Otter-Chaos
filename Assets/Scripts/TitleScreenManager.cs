using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button startGame = null;
    [SerializeField] private Button closeGame = null;
    [SerializeField] private InputField twitchChannel = null;
    private Camera mainCamera;


    [Header("Otter Love")]

    public List<TwitchPlayer> otterLovers;
    public CanvasGroup otterLove;

    void Start()
    {
        startGame.onClick.AddListener(OnStartButtonPressed);
        closeGame.onClick.AddListener(OnCloseButtonPressed);

        if(PlayerPrefs.HasKey("TwitchChannel")){
            twitchChannel.text = PlayerPrefs.GetString("TwitchChannel");
        }

        mainCamera = Camera.main;
    }

    void Update()
    {
        startGame.interactable =  !string.IsNullOrEmpty(twitchChannel.text);

        if(Input.GetKeyDown(KeyCode.Mouse0)){
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {

                if(hit.collider != null){
                    TwitchPlayer player = hit.collider.gameObject.GetComponentInParent<TwitchPlayer>();

                    if(player != null){
                        player.Pet();
                    }
                }
            }
        }
    
        bool loveIsInTheAir = true;

        foreach(var lover in otterLovers){
            loveIsInTheAir = loveIsInTheAir & lover.isPettinng;
        }

        if(loveIsInTheAir){
            otterLove.DOFade(1, 1.5f).OnComplete( () => {
                otterLove.DOFade(0f, 0.75f);
            });
        }
    }

    public void OnStartButtonPressed()
    {
        PlayerPrefs.SetString("TwitchChannel", twitchChannel.text);
        PlayerPrefs.Save();
    
        SoundManager.instance.PlayGameStart();
        Invoke("LoadSceneGame", 2.2f);
    }

    public void OnCloseButtonPressed()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadSceneGame()
    {
        SceneManager.LoadScene(1);
    }
}
