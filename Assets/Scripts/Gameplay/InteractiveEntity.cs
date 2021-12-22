using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEntity : MonoBehaviour
{
    public float HitPoints { get; private set; }

    [SerializeField] private float startHitPoints = 100;

    [Header("UI References")]
    [SerializeField] private GameObject WorldHintBar;
    [SerializeField] private GameObject UIHintBar;

    private GameObject cachedHintBar;

    protected virtual void Awake()
    {
        HitPoints = startHitPoints;
        SetHintBar();
        Refresh();
    }

    public void TakeDamage(float damage)
    {
        HitPoints -= damage;

        Refresh();

        if (HitPoints <= 0)
        {
            Destruct();
        }
    }

    private void Refresh()
    {
        if (cachedHintBar != null)
            cachedHintBar.transform.localScale = new Vector3(Mathf.Clamp(HitPoints, 0, float.MaxValue) / startHitPoints, 1f, 1f);

        if (UIHintBar != null)
            UIHintBar.transform.localScale = new Vector3(Mathf.Clamp(HitPoints, 0, float.MaxValue) / startHitPoints, 1f, 1f);
    }

    private void SetHintBar()
    {
        if (cachedHintBar == null && WorldHintBar != null)
            cachedHintBar = Instantiate(WorldHintBar, transform.position, Quaternion.identity, this.transform);
    }

    protected virtual void Destruct()
    {

    }
}
