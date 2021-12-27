using System;
using Beamable;
using Beamable.Api;
using Beamable.Common.Api.Auth;
using Beamable.Service;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private ConfigsRepository _configsRepository;
    [SerializeField] private CommandsConfigRef _commandsConfig;
    
    private IBeamableAPI _api;

    public event Action<string> OnStatusChanged;

    private async void Start()
    {
        OnStatusChanged?.Invoke("Connecting...");
        
        _api = await Beamable.API.Instance;

        bool deviceIdAvailable = await _api.AuthService.IsThisDeviceIdAvailable();

        if (deviceIdAvailable)
        {
            OnStatusChanged?.Invoke("Registering...");
            await _api.AuthService.RegisterDeviceId().Then(OnDeviceRegistered);
        }
        else
        {
            var platform = ServiceManager.Resolve<PlatformService>();
            platform.ClearToken();
        }

        OnStatusChanged?.Invoke("Logging in...");
        var tokenResponse = await _api.AuthService.LoginDeviceId();
        await _api.ApplyToken(tokenResponse);
        OnDeviceLogin(tokenResponse);
        
        OnStatusChanged?.Invoke("Downloading content...");
        await _api.ContentService.GetContent(_commandsConfig).Then(OnCommandsConfigReceived);
        
        OnStatusChanged?.Invoke("Loading main menu...");
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnCommandsConfigReceived(CommandsConfig config)
    {
        OnStatusChanged?.Invoke("Commands config received...");
        _configsRepository.CommandsConfig = config;
    }

    private void OnDeviceRegistered(User obj)
    {
        OnStatusChanged?.Invoke("Registered...");
    }

    private void OnDeviceLogin(TokenResponse tr)
    {
        OnStatusChanged?.Invoke("Logged in...");
    }
}