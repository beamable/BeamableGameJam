using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManualPresenter : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private UnityEvent _backButtonAction;

    private void Awake()
    {
        _backButton.onClick.AddListener(_backButtonAction.Invoke);
    }
}