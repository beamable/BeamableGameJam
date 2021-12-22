using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSourcePrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayClip(AudioClip clip, Vector3 position, float volume = 1)
    {
        var source = Instantiate(audioSourcePrefab, position, Quaternion.identity, transform);
        source.volume = volume;
        source.PlayOneShot(clip);
        StartCoroutine(DestroyAudioSourceWhenFinished(source));
    }

    private IEnumerator DestroyAudioSourceWhenFinished(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        Destroy(source.gameObject);
    }
}
