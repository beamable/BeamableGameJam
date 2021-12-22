using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private CommandsManager _commandsManager;
    [SerializeField] private TankController _tankController;
    
    private void Start()
    {
        _commandsManager.RegisterActionForCommand("A", _tankController.Attack);
        _commandsManager.RegisterActionForCommand("L", _tankController.Reload);
        _commandsManager.RegisterActionForCommand("P", _tankController.Boost);
    }
}