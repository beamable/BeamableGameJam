using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBullet : MonoBehaviour
{
    [SerializeField]
    public BoxCollider2D collider;

    [SerializeField]
    Rigidbody2D rigidbody2D;

    [SerializeField]
    float speed;

    float range;
    Vector2 startPos;

    public void Init(float range)
    {
        this.range = range;
        startPos = this.transform.position;
        rigidbody2D.velocity = transform.up * speed;
    }

    public void Update()
    {
        if (Vector2.Distance(transform.position,startPos) > range)
        {
            rigidbody2D.velocity = Vector2.zero;
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogError($"Tank Bullet collide with: {collision.name}");
        this.gameObject.SetActive(false);
    }
}
