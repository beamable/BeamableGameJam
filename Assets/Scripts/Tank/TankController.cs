using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TankController : InteractiveEntity
{
    public static TankController Instance { get; private set; }
    public Action<float> OnAmmoUpdated;
    public Action OnDefeat;
    
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
    [SerializeField] private GameObject explosionPrefab;

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

    [Header("Sounds")]
    [SerializeField] private AudioClip shotSfx;
    [SerializeField] private AudioClip explosionSfx;
    [SerializeField] private AudioClip engineSfx;

    Vector3 movementTarget = Vector3.zero;
    Vector3 enemyTarget = Vector3.zero;

    bool isWaiting = false;
    bool canShoot = false;
    float currentShootDelay = 0;
    float currentTrackTime = 0;
    float currentCannonHeading;
    float currentBoostTime = 0;

    float cachedSpeed, cachedAngularSpeed;
    float cachedCannonRotSpeed;
    Tower cachedAttackedTower;

    int usedBullets;

    List<GameObject> bulletsPool;
    List<GameObject> trackPool;

    AudioSource engineAudioSource;

    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            base.Awake();
        }
    }

    private int AmmoLeft => (int) (cannonBulletsAmount - usedBullets);

    void Start()
    {
        bulletsPool = new List<GameObject>();
        trackPool = new List<GameObject>();

        cachedAttackedTower = null;

        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = true;

        cachedSpeed = navMeshAgent.speed;
        cachedAngularSpeed = navMeshAgent.angularSpeed;

        pivot.eulerAngles = new Vector3(90, 0, 0);
        
        OnAmmoUpdated?.Invoke(AmmoLeft);
        
        engineAudioSource = AudioManager.Instance.PlayLoop(engineSfx, transform);
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

        if (engineSfx != null)
        {
            UpdateEngineSound();
        }

        if (movementTarget != Vector3.zero)
        {
            navMeshAgent.SetDestination(movementTarget);

            UpdateCannon();
            UpdateTracks();
            UpdateBoost();
        }
    }

    void CalcNewTarget()
    {
        if (Tower.AllTowers.Count > 0)
        {
            Tower.AllTowers.First();

        }

        Tower nearest = Tower.AllTowers.First();

        if (nearest != null)
        {
            cachedAttackedTower = nearest;
            Vector3 randomNearestPos = nearest.transform.position + UnityEngine.Random.insideUnitSphere * 1f;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;

            if (NavMesh.SamplePosition(randomNearestPos, out hit, 10f, 1))
            {
                finalPosition = hit.position;
            }

            movementTarget = finalPosition;
            enemyTarget = nearest.transform.position;
        }
     
    }

    void UpdateCannon()
    {
        Vector3 delta = enemyTarget - cannonPivot.transform.position;
        var lookRotation = Quaternion.LookRotation(delta);

        cannonPivot.transform.rotation = Quaternion.RotateTowards(cannonPivot.transform.rotation, lookRotation, 
            cachedCannonRotSpeed * Time.deltaTime);

        float dot = Quaternion.Dot(cannonPivot.transform.rotation, lookRotation);
        float dist = Vector3.Distance(enemyTarget, cannonPivot.transform.position);
        if (Mathf.Approximately(dot, 1) && currentShootDelay <= 0 && dist <= cannonRange && (cachedAttackedTower!= null && cachedAttackedTower.enabled))
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

    void UpdateEngineSound()
    {
        if (navMeshAgent.velocity.magnitude > 0)
        {
            if (engineAudioSource.pitch < 1.05f)
            {
                engineAudioSource.pitch = Random.Range(1.05f, 1.2f);
            }
        }
        else
        {
            engineAudioSource.pitch = 1f;
        }
    }

    void ShootIfPossible()
    {
        if (canShoot && usedBullets < cannonBulletsAmount)
        {
            AudioManager.Instance.PlayClip(shotSfx, transform.position);
            
            currentShootDelay = canonDelayShootTime;

            GameObject missile = Instantiate(missilePrefab, bulletPivot.position, bulletPivot.rotation, null);
            var fireEffect = Instantiate(fireEffectPrefab, bulletPivot.position, bulletPivot.rotation,
                bulletPivot);
            Destroy(fireEffect, .3f);

            Physics.IgnoreCollision(mainCollider, missile.GetComponent<Missile>().collider);

            usedBullets++;
            OnAmmoUpdated?.Invoke(AmmoLeft);
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
        OnAmmoUpdated?.Invoke(AmmoLeft);
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

    protected override void Destruct()
    {
        OnDefeat?.Invoke();
        AudioManager.Instance.PlayClip(explosionSfx, transform.position, .5f);
        Destroy(gameObject);
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
        Destroy(explosion, 1);
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(movementTarget, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(enemyTarget, 0.1f);
    }

#endif
    public void Heal()
    {
        HitPoints = startHitPoints;
        Refresh();
    }
}
