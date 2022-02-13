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

    public float distance;
    public float maxDistance;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        Vector2 distanceVector = transform.position - startingPos;
        distance = distanceVector.magnitude;

        if (maxDistance != 0 && distance > maxDistance)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.Translate(dir * speed * Time.fixedDeltaTime);

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject || parent.tag == hit.gameObject.tag)
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
