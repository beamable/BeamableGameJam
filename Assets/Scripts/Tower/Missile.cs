using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Missile : MonoBehaviour
{
    public new Collider collider;
    
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionTimer = 5;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float damage = 15;
    [SerializeField] private AudioClip explosionSfx;

    private void Awake()
    {
        Assert.IsNotNull(explosionPrefab);

        StartCoroutine(ExplodeAfterDelay());
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.Self);
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionTimer);
        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        var entity = other.GetComponent<InteractiveEntity>();
        
        if (entity)
        {
            Explode();
            entity.TakeDamage(damage);
        }
    }

    private void Explode()
    {
        AudioManager.Instance.PlayClip(explosionSfx, transform.position, .4f);
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
        Destroy(explosion, 1);
        Destroy(gameObject);
    }
}
