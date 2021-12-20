using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankController : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent navMeshAgent;

    [SerializeField]
    Transform pivot;

    [SerializeField]
    Transform cannonPivot;

    [SerializeField]
    float cannonRotateSpeed;

    Vector3 target;
    bool isWaiting = false;
    float currentCannonHeading;

    void Start()
    {
        navMeshAgent.updateRotation = true;
        navMeshAgent.updateUpAxis = true;
        pivot.eulerAngles = new Vector3(90, 0, 0);
    }

    private void Update()
    {
        if (Vector3.Angle(transform.forward, navMeshAgent.desiredVelocity) > 10)
            navMeshAgent.updatePosition = false;
        else
        {
            if (!navMeshAgent.updatePosition)
                navMeshAgent.nextPosition = this.transform.position;

            navMeshAgent.updatePosition = true;
        }

        navMeshAgent.SetDestination(target);

        if (navMeshAgent.remainingDistance < 0.1f && !isWaiting)
        {
            CalcNewTarget();
        }

        UpdateCanonRotation();
    }

    private void CalcNewTarget()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        float x = UnityEngine.Random.Range(-7, 7);
        float y = UnityEngine.Random.Range(-3f, 3f);

        target = new Vector3(x, y, 0);
    }

    private void UpdateCanonRotation()
    {
        Vector3 delta = cannonPivot.transform.position - target;

        float DesiredHeadingToPlayer = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        float AngleError = Mathf.DeltaAngle(DesiredHeadingToPlayer, currentCannonHeading);

        currentCannonHeading = Mathf.MoveTowardsAngle(currentCannonHeading, DesiredHeadingToPlayer, cannonRotateSpeed * Time.deltaTime);
        cannonPivot.transform.rotation = Quaternion.Euler(0, 0, currentCannonHeading + 90);
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(target, 0.1f);
    }

#endif
}
