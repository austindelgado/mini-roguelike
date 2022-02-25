using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SyncVar] public GameObject parent;

    public Vector2 startingPos;
    public Vector2 dir;
    public float speed;
    public int damage;
    public CircleCollider2D circleCollider;
    public bool piercing;

    public float distanceTraveled;
    public float maxDistance;

    public virtual void Start()
    {
        startingPos = transform.position;
    }

    public virtual void Update()
    {
        Vector2 distanceVector = (Vector2)transform.position - startingPos;
        distanceTraveled = distanceVector.magnitude;

        if (maxDistance != 0 && distanceTraveled > maxDistance)
            NetworkServer.Destroy(gameObject);
    }

    public virtual void FixedUpdate()
    {
        // Happens on cluent and server
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);

        if(isServer)
        {
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
                        NetworkServer.Destroy(gameObject);
                }
                else
                    NetworkServer.Destroy(gameObject);
            }
        }
    }
}
