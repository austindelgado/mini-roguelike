using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject parent;

    public Vector2 dir;
    public float speed;
    public int damage;
    public CircleCollider2D circleCollider;
    public bool piercing;

    void FixedUpdate()
    {
        transform.Translate(dir * speed * Time.fixedDeltaTime);

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject || gameObject.tag == hit.gameObject.tag)
                continue;

            // Hitting something not a wall and not yourself
            if (hit.gameObject.tag != "Wall" && hit.gameObject != parent)
            {
                hit.gameObject.GetComponent<Entity>().Damage(damage);
                if (!piercing)
                    Destroy(gameObject);
            }
            else if (hit.gameObject != parent)
                Destroy(gameObject);
        }
    }
}
