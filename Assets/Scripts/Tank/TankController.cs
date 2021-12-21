using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankController : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [Header("Pivots")]
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform cannonPivot;
    [SerializeField] private Transform bulletPivot;
    [SerializeField] private Transform trackL_Pivot;
    [SerializeField] private Transform trackR_Pivot;

    [Header("Colliders")]
    [SerializeField] private CapsuleCollider mainCollider;

    [Header("Prefabs")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject fireEffectPrefab;
    [SerializeField] private GameObject trackPrefab;

    [Header("Parameters")]
    [SerializeField] private float cannonRotateSpeed;
    [SerializeField] private float canonDelayShootTime;
    [SerializeField] private float cannonBulletsAmount;
    [SerializeField] private float cannonRange;

    [SerializeField] private float tankBoostMultiplier;
    [SerializeField] private float tankBoostTime;

    [SerializeField] private float movementAngleCheckValue;

    [Header("Tracks")]
    [SerializeField] private int tracksPoolSize;
    [SerializeField] private float trackSpawnDelay;
    [SerializeField] private Transform tracksParent;

    Vector3 target = Vector3.zero;
    bool isWaiting = false;
    bool canShoot = false;
    float currentShootDelay = 0;
    float currentTrackTime = 0;
    float currentCannonHeading;
    float currentBoostTime = 0;

    float cachedSpeed, cachedAngularSpeed;
    float cachedCannonRotSpeed;

    int usedBullets;

    List<GameObject> bulletsPool;
    List<GameObject> trackPool;

    void Start()
    {
        bulletsPool = new List<GameObject>();
        trackPool = new List<GameObject>();

        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = true;

        cachedSpeed = navMeshAgent.speed;
        cachedAngularSpeed = navMeshAgent.angularSpeed;

        pivot.eulerAngles = new Vector3(90, 0, 0);
    }

    void Update()
    {
        if (Vector3.Angle(transform.forward, navMeshAgent.velocity) > movementAngleCheckValue || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.updatePosition = false;
        }
        else
        {
            if (!navMeshAgent.updatePosition)
            {
                navMeshAgent.nextPosition = this.transform.position;
            }

            navMeshAgent.updatePosition = true;
        }

        if (target != Vector3.zero)
        {
            navMeshAgent.SetDestination(target);

            UpdateCannon();
            UpdateTracks();
            UpdateBoost();
        }
    }

    void CalcNewTarget()
    {
        // TO DO GET NEXT TARGET

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, 1))
        {
            finalPosition = hit.position;
        }
        target = finalPosition;
    }

    void UpdateCannon()
    {
        Vector3 delta = target - cannonPivot.transform.position;
        var lookRotation = Quaternion.LookRotation(delta);

        cannonPivot.transform.rotation = Quaternion.RotateTowards(cannonPivot.transform.rotation, lookRotation, 
            cachedCannonRotSpeed * Time.deltaTime);

        float dot = Quaternion.Dot(cannonPivot.transform.rotation, lookRotation);
        float dist = Vector3.Distance(target, cannonPivot.transform.position);
        if (Mathf.Approximately(dot, 1) && currentShootDelay <= 0 && dist <= cannonRange)
            canShoot = true;
        else
            canShoot = false;

        currentShootDelay -= Time.deltaTime;

        ShootIfPossible();
    }


    void UpdateBoost()
    {
        currentBoostTime -= Time.deltaTime;

        if (currentBoostTime < 0)
        {
            navMeshAgent.speed = cachedSpeed;
            navMeshAgent.angularSpeed = cachedAngularSpeed;
            cachedCannonRotSpeed = cannonRotateSpeed;
        }
    }

    void ShootIfPossible()
    {
        if (canShoot && usedBullets < cannonBulletsAmount)
        {
            currentShootDelay = canonDelayShootTime;

            GameObject missile = Instantiate(missilePrefab, bulletPivot.position, bulletPivot.rotation, null);
            var fireEffect = Instantiate(fireEffectPrefab, bulletPivot.position, bulletPivot.rotation,
                bulletPivot);
            Destroy(fireEffect, .3f);

             Physics.IgnoreCollision(mainCollider, missile.GetComponent<Missile>().collider);

            usedBullets++;
        }
    }

    void UpdateTracks()
    {
        currentTrackTime += Time.deltaTime;

        if (currentTrackTime > trackSpawnDelay)
        {
            SpawnTrack(trackL_Pivot);
            SpawnTrack(trackR_Pivot);

            if (trackPool.Count > tracksPoolSize)
            {
                for (int i = 0; i < 2 && i < trackPool.Count; i++)
                {
                    DestroyImmediate(trackPool[0].gameObject);
                    trackPool.RemoveAt(0);
                }
            }

            currentTrackTime = 0;
        }
    }

    public void Reload()
    {
        usedBullets = 0;
    }

    public void Attack()
    {
        CalcNewTarget();
    }

    public void Boost()
    {
        currentBoostTime = tankBoostTime;
        navMeshAgent.speed = cachedSpeed * tankBoostMultiplier;
        navMeshAgent.angularSpeed = cachedAngularSpeed * tankBoostMultiplier;
        cachedCannonRotSpeed = cannonRotateSpeed * tankBoostMultiplier;
    }

    void SpawnTrack(Transform trackPivot)
    {
        GameObject track = Instantiate(trackPrefab);
        track.transform.position = trackPivot.position;
        track.transform.localRotation = trackPivot.rotation;

        if (tracksParent != null)
            track.transform.parent = tracksParent;

        trackPool.Add(track);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < bulletsPool.Count; i++)
            Destroy(bulletsPool[i].gameObject);

        for (int i = 0; i < trackPool.Count; i++)
            Destroy(trackPool[i].gameObject);

        bulletsPool.Clear();
        trackPool.Clear();

    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(target, 0.1f);
    }

#endif
}
