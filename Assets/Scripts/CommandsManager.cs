using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandsManager : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private MorseDecoder _morseDecoder;
    [SerializeField] private List<string> _commands = new List<string>();
#pragma warning restore CS0649
    
    private readonly Dictionary<string, Action> _registeredActions = new Dictionary<string, Action>();

    public void IssueCommand()
    {
        string currentCommand = _morseDecoder.GetCurrentCommand().ToUpper();
        if (_commands.Contains(currentCommand))
        {
            if(_registeredActions.TryGetValue(currentCommand, out Action currentAction))
            {
                currentAction?.Invoke();
            }
            else
            {
                Debug.Log($"Command {currentCommand} has no action registered");
            }
        }
        else
        {
            Debug.Log($"Command {currentCommand} doesn't exist");
        }
        
        _morseDecoder.Reset();
    }

    public void RegisterActionForCommand(string command, Action action)
    {
        if (!_commands.Contains(command))
        {
            Debug.Log($"Command {command} is not allowed");
            return;
        }
        
        _registeredActions.Add(command.ToUpper(), action);
    }
}