using System.Collections.Generic;
using Beamable;
using Beamable.Common.Leaderboards;
using Beamable.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private CommandsManager _commandsManager;
    [SerializeField] private TankController _tankController;
    [SerializeField] private TextMeshProUGUI _ammoCounter;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private GameObject _mask;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _summaryText;
    [SerializeField] private TextMeshProUGUI _summaryTimer;
    [SerializeField] private LeaderboardRef _leaderboardRef;
    [SerializeField] private StatObject _aliasStatObject;
    [SerializeField] private TMP_InputField _nameField;
    
    private bool _isRunning;
    private float _missionTime;
    private bool _retreated;
    private int _destroyedTowers;

    private void Start()
    {
        _commandsManager.RegisterActionForCommand("A", _tankController.Attack);
        _commandsManager.RegisterActionForCommand("L", _tankController.Reload);
        _commandsManager.RegisterActionForCommand("P", _tankController.Boost);
        _commandsManager.RegisterActionForCommand("S", _tankController.Heal);
        
        _commandsManager.RegisterActionForCommand("ATTACK", _tankController.Attack);
        _commandsManager.RegisterActionForCommand("LOAD", _tankController.Reload);
        _commandsManager.RegisterActionForCommand("PUSH", _tankController.Boost);
        _commandsManager.RegisterActionForCommand("SOS", _tankController.Heal);

        _tankController.OnAmmoUpdated = OnAmmoUpdated;
        
        _summaryText.gameObject.SetActive(false);
        _summaryTimer.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(false);
        _nameField.gameObject.SetActive(false);
        
        SetTimer(0);

        _nameField.onValueChanged.AddListener(OnNameChanged);

        Tower.OnTowerDestroyed += OnTowerDestroyed;
    }

    private void OnTowerDestroyed()
    {
        _destroyedTowers++;
    }

    private void OnDestroy()
    {
        _tankController.OnAmmoUpdated = null;
        _nameField.onValueChanged.RemoveListener(OnNameChanged);
        Tower.OnTowerDestroyed -= OnTowerDestroyed;
    }

    private void OnNameChanged(string value)
    {
        _continueButton.interactable = value.Length >= 3;
    }

    private void Update()
    {
        if (!_isRunning) return;
        _missionTime += Time.deltaTime;
        SetTimer(_missionTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _retreated = false;
            StopMission();
        }
    }

    public void StartMission()
    {
        _mask.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(false);
        _summaryText.gameObject.SetActive(false);
        _summaryTimer.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(false);
        _nameField.gameObject.SetActive(false);
        
        _isRunning = true;
    }

    public void Retreat()
    {
        _retreated = true;
        StopMission();
    }

    public async void Continue()
    {
        if (!_retreated)
        {
            IBeamableAPI api = await Beamable.API.Instance;

            Dictionary<string, object> stats = new Dictionary<string, object>();
            stats.Add(_aliasStatObject.StatKey, _nameField.text);

            int score = (int)(_destroyedTowers * 10 / _missionTime * 100);

            await api.LeaderboardService.SetScore(_leaderboardRef, score, stats);
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
        _continueButton.interactable = false;
        _nameField.gameObject.SetActive(!_retreated);
        
        _summaryTimer.text = $"YOUR TIME: {_missionTime:F1} S";        
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