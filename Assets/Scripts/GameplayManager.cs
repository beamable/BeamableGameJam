using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private CommandsManager _commandsManager;
    [SerializeField] private TankController _tankController;
    [SerializeField] private TextMeshProUGUI _ammoCounter;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private GameObject _mask;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private TextMeshProUGUI _summaryText;
    [SerializeField] private TextMeshProUGUI _summaryTimer;
    
    private bool _isRunning;
    private float _missionTime;
    private bool _retreated;

    private void Start()
    {
        _commandsManager.RegisterActionForCommand("A", _tankController.Attack);
        _commandsManager.RegisterActionForCommand("L", _tankController.Reload);
        _commandsManager.RegisterActionForCommand("P", _tankController.Boost);

        _tankController.OnAmmoUpdated = OnAmmoUpdated;
        
        _summaryText.gameObject.SetActive(false);
        _summaryTimer.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(false);
        
        SetTimer(0);
    }

    private void Update()
    {
        if (!_isRunning) return;
        _missionTime += Time.deltaTime;
        SetTimer(_missionTime);
    }

    public void StartMission()
    {
        _mask.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(false);
        _summaryText.gameObject.SetActive(false);
        _summaryTimer.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(false);
        
        _isRunning = true;
    }

    public void Retreat()
    {
        _retreated = true;
        StopMission();
    }

    public void Continue()
    {
        if (!_retreated)
        {
            // TODO: add score to leaderboard
        }

        SceneManager.LoadScene("MainMenuScene");
    }

    private void StopMission()
    {
        _mask.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(false);
        _summaryText.gameObject.SetActive(true);
        _summaryTimer.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(true);
        
        _summaryTimer.text = $"YOUR TIME: {_missionTime.ToString(CultureInfo.InvariantCulture)}";        
        _isRunning = false;
    }

    private void OnAmmoUpdated(float amount)
    {
        SetAmmoCounter(amount);
    }

    private void SetAmmoCounter(float amount)
    {
        if (_ammoCounter != null)
        {
            _ammoCounter.text = $"AMMO LEFT: {amount}";
        }
    }

    private void SetTimer(float value)
    {
        if (_timer != null)
        {
            _timer.text = $"TIME: {value:F1} S";
        }
    }
}