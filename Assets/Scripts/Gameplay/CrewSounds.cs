using UnityEngine;

public class CrewSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _help;
    [SerializeField] private AudioClip _attack;
    [SerializeField] private AudioClip _load;
    [SerializeField] private AudioClip _negative;
    [SerializeField] private AudioClip _push;

    public void PlaySound(string command)
    {
        switch (command)
        {
            case "ATTACK":
            case "A":
                _audioSource.clip = _attack;
                break;
            case "PUSH":
            case "P":
                _audioSource.clip = _push;
                break;
            case "LOAD":
            case "L":
                _audioSource.clip = _load;
                break;
            case "NEGATIVE":
            case "N":
                _audioSource.clip = _negative;
                break;
            case "SOS":
                _audioSource.clip = _help;
                break;
        }
        
        _audioSource.Play();
    }
}