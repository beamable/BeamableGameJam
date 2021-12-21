using TMPro;
using UnityEngine;

public class StatusPresenter : MonoBehaviour
{
    [SerializeField] private LoginManager loginManager;
    [SerializeField] private TextMeshProUGUI _status;

    private void Awake()
    {
        loginManager.OnStatusChanged += OnStatusChanged;
    }

    private void OnDestroy()
    {
        loginManager.OnStatusChanged -= OnStatusChanged;   
    }

    private void OnStatusChanged(string newStatus)
    {
        _status.text = newStatus;
    }
}