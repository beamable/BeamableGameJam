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
        if (clip == null)
        {
            Debug.LogError("Trying to play NULL audio clip");
        }
        
        var source = Instantiate(audioSourcePrefab, position, Quaternion.identity, transform);
        source.volume = volume;
        source.PlayOneShot(clip);
        StartCoroutine(DestroyAudioSourceWhenFinished(source));
    }

    public AudioSource PlayLoop(AudioClip clip, Transform parent, float volume = 1)
    {
        if (clip == null)
        {
            Debug.LogError("Trying to play NULL audio clip");
        }
        
        var source = Instantiate(audioSourcePrefab, parent);
        source.volume = volume;
        source.loop = true;
        source.clip = clip;
        source.Play();
        
        return source;
    }

    private IEnumerator DestroyAudioSourceWhenFinished(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        Destroy(source.gameObject);
    }
}
