using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Missile : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionTimer = 5;

    private void Awake()
    {
        Assert.IsNotNull(explosionPrefab);

        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionTimer);
        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsHelper.TankTag))
        {
            Explode();
            // deal damage
            
        }
    }

    private void Explode()
    {
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
        Destroy(explosion, 1);
        Destroy(gameObject);
    }
}
