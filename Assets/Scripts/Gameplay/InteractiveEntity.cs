using System;
using UnityEngine;

public class InteractiveEntity : MonoBehaviour
{
    public float HitPoints { get; private set; }

    [SerializeField] private float startHitPoints = 100;

    protected virtual void Awake()
    {
        HitPoints = startHitPoints;
    }

    public void TakeDamage(float damage)
    {
        HitPoints -= damage;
        if (HitPoints <= 0)
        {
            Destruct();
        }
    }

    protected virtual void Destruct()
    {
        
    }
}
