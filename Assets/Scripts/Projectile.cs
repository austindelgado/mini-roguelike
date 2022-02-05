using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 dir;
    public float speed;
    public CircleCollider2D circleCollider;

    void FixedUpdate()
    {
        transform.Translate(dir * speed * Time.fixedDeltaTime);

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider and player
            if (hit.gameObject.tag != "Projectile" && hit.gameObject.tag != "Player")
                Destroy(gameObject);
        }
    }
}
