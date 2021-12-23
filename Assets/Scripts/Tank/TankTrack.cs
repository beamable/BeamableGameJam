using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrack : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

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
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Clamp(spriteRenderer.color.a - .01f, 0, 1));
            yield return new WaitForSeconds(.05f);
        }
    }
}
