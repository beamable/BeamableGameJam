using System.Collections;
using System.Collections.Generic;
using Beamable.Modules.Leaderboards;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LeaderboardsPresenter _leaderboardsPresenter;
    
    void Start()
    {
    }

    void Update()
    {
    }

    public void StartGame()
    {
        
    }

    public void ShowInstructions()
    {
        
    }

    public void OpenLeaderboards()
    {
        _leaderboardsPresenter.gameObject.SetActive(true);
    }

    public void CloseLeaderboards()
    {
        _leaderboardsPresenter.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}