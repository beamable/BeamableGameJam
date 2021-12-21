using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrack : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer renderer;

    Coroutine updateCoroutine;

    void Awake()
    {
        updateCoroutine = StartCoroutine(ColorUpdate());
    }

    private void OnDestroy()
    {
        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);

        updateCoroutine = null;
    }

    IEnumerator ColorUpdate()
    {
        while (true)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Mathf.Clamp(renderer.color.a - .01f, 0, 1));
            yield return new WaitForSeconds(.05f);
        }
    }
}
