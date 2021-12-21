using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankController : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField]
    NavMeshAgent navMeshAgent;

    [SerializeField]
    float tankBoostMultiplier;

    [SerializeField]
    float tankBoostTime;

    [Header("Pivots")]
    [SerializeField]
    Transform pivot;

    [SerializeField]
    Transform cannonPivot;

    [SerializeField]
    Transform bulletPivot;

    [SerializeField]
    Transform trackL_Pivot;

    [SerializeField]
    Transform trackR_Pivot;

    [Header("Colliders")]
    [SerializeField]
    CapsuleCollider2D mainCollider;

    [Header("Prefabs")]
    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    GameObject trackPrefab;

    [Header("Cannon")]
    [SerializeField]
    float cannonRotateSpeed;

    [SerializeField]
    float canonDelayShootTime;

    [SerializeField]
    float cannonShootErrorAngle;

    [SerializeField]
    float cannonRange;

    [SerializeField]
    float cannonBulletsAmount;

    [Header("Tracks")]
    [SerializeField]
    int tracksPoolSize;

    [SerializeField]
    float trackSpawnDelay;

    [Header("Parents")]
    [SerializeField]
    Transform bulletsParent;

    [SerializeField]
    Transform tracksParent;

    Vector3 target = Vector3.zero;
    bool isWaiting = false;
    bool canShoot = false;
    float currentShootDelay = 0;
    float currentTrackTime = 0;
    float currentCannonHeading;
    float currentBoostTime = 0;
    float cachedSpeed;
    float cachedCannonRotSpeed;

    int usedBullets;

    List<TankBullet> bulletsPool;
    List<GameObject> trackPool;

    void Start()
    {
        bulletsPool = new List<TankBullet>();
        trackPool = new List<GameObject>();

        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = true;

        cachedSpeed = navMeshAgent.speed;
        pivot.eulerAngles = new Vector3(90, 0, 0);
    }

    void Update()
    {
        if (Vector3.Angle(transform.forward, navMeshAgent.velocity) > 10 || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            navMeshAgent.updatePosition = false;
        else
        {
            if (!navMeshAgent.updatePosition)
                navMeshAgent.nextPosition = this.transform.position;

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
        Vector3 delta = cannonPivot.transform.position - target;

        float DesiredHeadingToPlayer = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        float AngleError = Mathf.DeltaAngle(DesiredHeadingToPlayer, currentCannonHeading);

        if (AngleError < cannonShootErrorAngle && currentShootDelay <= 0)
            canShoot = true;
        else
            canShoot = false;

        currentShootDelay -= Time.deltaTime;

        currentCannonHeading = Mathf.MoveTowardsAngle(currentCannonHeading, DesiredHeadingToPlayer, cachedCannonRotSpeed * Time.deltaTime);
        cannonPivot.transform.rotation = Quaternion.Euler(0, 0, currentCannonHeading + 90);

        ShootIfPossible();
    }

    void UpdateBoost()
    {
        currentBoostTime -= Time.deltaTime;

        if (currentBoostTime < 0)
        {
            navMeshAgent.speed = cachedSpeed;
            cachedCannonRotSpeed = cannonRotateSpeed;
        }
    }

    void ShootIfPossible()
    {
        if (canShoot && usedBullets < cannonBulletsAmount)
        {
            currentShootDelay = canonDelayShootTime;

            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = bulletPivot.transform.position;
            bullet.transform.localRotation = bulletPivot.transform.rotation;

            if (bulletsParent != null)
                bullet.transform.parent = bulletsParent;

            TankBullet tankBullet = bullet.GetComponent<TankBullet>();
            tankBullet.Init(cannonRange);

            Physics2D.IgnoreCollision(mainCollider, tankBullet.collider);
            bulletsPool.Add(tankBullet);

            for (int i = 0; i < bulletsPool.Count; i++)
            {
                if (!bulletsPool[i].gameObject.activeSelf)
                {
                    DestroyImmediate(bulletsPool[i].gameObject);
                    bulletsPool.RemoveAt(i);
                    i--;
                }
            }

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
