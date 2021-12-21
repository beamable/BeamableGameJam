using UnityEngine;
using UnityEngine.EventSystems;

public class SignalButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MorseDecoder _morseDecoder;
    [SerializeField] private MorseSoundGenerator _soundGenerator;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _morseDecoder.StartCounting();
            _soundGenerator.StartBeep();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _morseDecoder.StopCounting();
            _soundGenerator.StopBeep();
        }
#endif
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _morseDecoder.StartCounting();
        _soundGenerator.StartBeep();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _morseDecoder.StopCounting();
        _soundGenerator.StopBeep();
    }
}