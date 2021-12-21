using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Tower : MonoBehaviour
{
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private float rotationSpeed = 45;
    [SerializeField] private float fireCooldown = 2;

    private Transform _target;
    private CapsuleCollider _collider;
    private float cooldownTimer;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsHelper.TankTag))
        {
            _target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target && other.CompareTag(TagsHelper.TankTag))
        {
            _target = null;
        }
    }

    private void Update()
    {
        if (_target)
        {
            var dot = RotateTowardsTarget();
            if (Mathf.Approximately(dot, 1) && cooldownTimer <= 0)
            {
                Shoot();
            }
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private float RotateTowardsTarget()
    {
        Vector3 dir = _target.position - barrelTransform.position;
        var lookRotation = Quaternion.LookRotation(dir);
        barrelTransform.rotation = Quaternion.RotateTowards(barrelTransform.rotation, lookRotation, 
            rotationSpeed * Time.deltaTime);
        return Quaternion.Dot(barrelTransform.rotation, lookRotation);
    }

    private void Shoot()
    {
        cooldownTimer = fireCooldown;
    }

    private void OnDrawGizmos()
    {
        if (_collider)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _collider.radius);   
        }
    }
}
