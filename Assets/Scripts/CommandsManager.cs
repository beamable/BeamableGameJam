using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandsManager : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private MorseDecoder _morseDecoder;
    [SerializeField] private CrewSounds _crewSounds;
#pragma warning restore CS0649

    private readonly Dictionary<string, Action> _registeredActions = new Dictionary<string, Action>();

    public void IssueCommand()
    {
        string currentCommand = _morseDecoder.GetCurrentCommand().ToUpper();

        if (_registeredActions.TryGetValue(currentCommand, out Action currentAction))
        {
            currentAction?.Invoke();
            _crewSounds.PlaySound(currentCommand);
        }
        else
        {
            _crewSounds.PlaySound("N");
        }

        _morseDecoder.Reset();
    }

    public void RegisterActionForCommand(string command, Action action)
    {
        _registeredActions.Add(command.ToUpper(), action);
    }
}