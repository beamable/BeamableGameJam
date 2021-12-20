using UnityEngine;

public class MorseSoundGenerator : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private AudioSource _source;
#pragma warning restore CS0649

    private void Start()
    {
        _source.volume = 0.0f;
    }
    
    public void StartBeep()
    {
        _source.volume = 1.0f;
    }

    public void StopBeep()
    {
        _source.volume = 0.0f;
    }
}