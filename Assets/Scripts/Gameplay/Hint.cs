using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hint;

    public void Setup(string hint)
    {
        _hint.text = hint;
    }
    
}