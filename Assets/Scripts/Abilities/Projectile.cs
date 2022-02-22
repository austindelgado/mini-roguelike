using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
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
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius * transform.localScale.x);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == null || hit.gameObject.tag == "Friendly" || hit.gameObject.tag == "Player")
                continue;

            if (hit.gameObject == gameObject)
                continue;

            // Hitting something not a wall and not yourself
            if (hit.gameObject.tag != "Wall" && hit.gameObject != parent)
            {
                hit.gameObject.GetComponent<Entity>().Damage(damage, parent);
                if (!piercing)
                    NetworkServer.Destroy(gameObject);
            }
            else
                NetworkServer.Destroy(gameObject);
        }
    }
}
