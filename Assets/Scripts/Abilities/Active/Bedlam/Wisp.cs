using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : Entity
{
    public AbilitySlot ability1;
    public float attackRadius;

    public GameObject parent;
    public int damage;

    public CircleCollider2D circleCollider;

    public GameObject target;

    void Update()
    {
        if (target != null)
        {
            lookDir = (target.transform.position - transform.position).normalized;
            ability1.Trigger();
        }
    }

    void FixedUpdate()
    {
        //transform.Translate(dir * speed * Time.fixedDeltaTime);
        transform.RotateAround(parent.transform.position, new Vector3(0, 0, 1), speed * Time.fixedDeltaTime);

        // // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius * transform.localScale.x);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.tag == "Enemy")
            {
                target = hit.gameObject;
            }
        }
    }
}
