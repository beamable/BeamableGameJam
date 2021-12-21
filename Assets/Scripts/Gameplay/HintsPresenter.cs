using System.Collections.Generic;
using UnityEngine;

public class HintsPresenter : MonoBehaviour
{
    [SerializeField] private MorseDecoder _morseDecoder;
    [SerializeField] private Hint _hintPrefab;
    [SerializeField] private RectTransform _hintsParent;
    
    private void Start()
    {
        ConfigsRepository configsRepository = FindObjectOfType<ConfigsRepository>();
        List<string> availableCommands = configsRepository?.CommandsConfig?.AvailableCommands;

        if (availableCommands == null) return;
        
        foreach (string command in availableCommands)
        {
            Hint hint = Instantiate(_hintPrefab, _hintsParent, false);

            string code = _morseDecoder.GetCodeForCommand(command);
            hint.Setup($"{command}: {code}");
        }
    }
}