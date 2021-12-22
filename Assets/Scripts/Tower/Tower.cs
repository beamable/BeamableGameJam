using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Tower : MonoBehaviour
{
    public static List<Tower> AllTowers { get; } = new List<Tower>();
    
    private const float FireEffectFadeDelay = .3f;
    
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform missileSpawnPoint;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject fireEffectPrefab;
    [SerializeField] private float rotationSpeed = 45;
    [SerializeField] private float fireCooldown = 2;

    private Transform _target;
    private CapsuleCollider _collider;
    private float _cooldownTimer;

    private void Awake()
    {
        AllTowers.Add(this);
        StartCoroutine(SortTowersAfterInit());
        
        _collider = GetComponent<CapsuleCollider>();
        Assert.IsNotNull(barrelTransform);
        Assert.IsNotNull(missileSpawnPoint);
        Assert.IsNotNull(missilePrefab);
        Assert.IsNotNull(fireEffectPrefab);
    }

    private IEnumerator SortTowersAfterInit()
    {
        yield return new WaitUntil(() => TankController.Instance != null);
        AllTowers.Sort((t1, t2) => t1.DistanceToTank() < t2.DistanceToTank() ? -1 : 1);
    }

    private float DistanceToTank() => Vector3.Distance(TankController.Instance.transform.position, transform.position);

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
            if (Mathf.Approximately(dot, 1) && _cooldownTimer <= 0)
            {
                Shoot();
            }
        }

        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
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
        Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation, null);
        var fireEffect = Instantiate(fireEffectPrefab, missileSpawnPoint.position, missileSpawnPoint.rotation,
            missileSpawnPoint);
        Destroy(fireEffect, FireEffectFadeDelay);
        _cooldownTimer = fireCooldown;
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
