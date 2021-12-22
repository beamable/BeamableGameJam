using Beamable.Modules.Leaderboards;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LeaderboardsPresenter _leaderboardsPresenter;
    [SerializeField] private ManualPresenter _manualPresenter;
    
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OpenManual()
    {
        _manualPresenter.gameObject.SetActive(true);
    }

    public void CloseManual()
    {
        _manualPresenter.gameObject.SetActive(false);
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