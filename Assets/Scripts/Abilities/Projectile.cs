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
        transform.Translate(dir * speed * Time.fixedDeltaTime);

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius * transform.localScale.x);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == null || parent.gameObject == null || hit.gameObject.tag == "Friendly")
                continue;

            if (hit.gameObject == gameObject || parent.tag == hit.gameObject.tag)
                continue;

            if (parent.gameObject.tag == "Friendly" && hit.gameObject.tag == "Player")
                continue;

            // Hitting something not a wall and not yourself
            if (hit.gameObject.tag != "Wall" && hit.gameObject != parent)
            {
                hit.gameObject.GetComponent<Entity>().Damage(damage, parent);
                if (!piercing)
                    Destroy(gameObject);
            }
            else if (hit.gameObject != parent)
                Destroy(gameObject);
        }
    }
}
