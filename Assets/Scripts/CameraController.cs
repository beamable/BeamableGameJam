using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float lerpSpeed = 5;

    private float _yOffset;

    private void Start()
    {
        _yOffset = transform.position.y;

        if (!followTarget)
        {
            Debug.LogError("Camera follow target not set.");
        }
    }

    private void Update()
    {
        if (followTarget)
        {
            Vector3 targetPos = followTarget.position;
            targetPos.y = _yOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        }
    }
}
