using UnityEngine;
using UnityEngine.Assertions;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Tower targetTower;

    private void Awake()
    {
        Assert.IsNotNull(targetTower);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsHelper.TankTag))
        {
            targetTower.TargetAcquired(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagsHelper.TankTag))
        {
            targetTower.TargetAcquired(null);
        }
    }
}
