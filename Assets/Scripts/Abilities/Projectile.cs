using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject parent;

    public Vector2 startingPos;
    public Vector2 dir;
    public float speed;
    public int damage;
    public CircleCollider2D circleCollider;
    public bool piercing;

    public float distanceTraveled;
    public float maxDistance;

    private float catchupDistance = 0f;

    public virtual void Start()
    {
        startingPos = transform.position;
    }

    public virtual void Update()
    {
        Vector2 distanceVector = (Vector2)transform.position - startingPos;
        distanceTraveled = distanceVector.magnitude;

        if (maxDistance != 0 && distanceTraveled > maxDistance)
            Destroy(gameObject);
    }

    public virtual void FixedUpdate()
    {
        // Happens on client and server

        float moveValue = speed * Time.deltaTime;
        float catchupValue = 0f;

        if (catchupDistance > 0f)
        {
            float step = catchupDistance * Time.deltaTime;
            catchupDistance -= step;

            catchupValue = step;

            if (catchupDistance < (moveValue * 0.1f))
            {
                catchupValue += catchupDistance;
                catchupDistance = 0f;
            }
        }

        transform.Translate(Vector2.right * (moveValue + catchupValue));

        // Collision happens only on server
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius * transform.localScale.x);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == null || hit.gameObject == gameObject || hit.gameObject == parent)
                continue;

            // Hitting something not a wall and not yourself
            if (hit.gameObject.tag != "Wall" && hit.gameObject != parent)
            {
                if(!hit.gameObject.TryGetComponent<Health>(out var damageable))
                    continue;

                damageable.DealDamage(damage);

                if (!piercing)
                    Destroy(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }

    public void Initialize(GameObject parent, int damage, float duration)
    {
        catchupDistance = duration * speed;
        this.damage = damage;
        this.parent = parent;
    }
}
