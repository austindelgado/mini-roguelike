using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private GameObject player;

    public float currSpeed;
    public float currAcceleration;
    public float currDeceleration;

    public AbilitySlot ability1;
    public float attackRadius;

    public CircleCollider2D circleCollider;

    public override void Start()
    {
        base.Start();

        player = GameObject.Find("Player");

        currSpeed = speed;
        currAcceleration = acceleration;
        currDeceleration = deceleration;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Player Position
        moveInput = (player.transform.position - transform.position).normalized;
        currSpeed = speed + player.GetComponent<Player>().enemyKillCount * .1f;
        currAcceleration = acceleration + player.GetComponent<Player>().enemyKillCount * .075f;
        currDeceleration = deceleration + player.GetComponent<Player>().enemyKillCount * .075f;

        // Get player direction
        lookDir = moveInput;
        float distance = Vector2.Distance(player.transform.position, transform.position);
        Debug.Log(distance);

        if (ability1.available && distance < attackRadius)
            ability1.Trigger();

        if (currSpeed > 3)
            currSpeed = 3;
    }

    void FixedUpdate()
    {
        // Movement
        if (moveInput != Vector2.zero)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, currSpeed * moveInput.x, currAcceleration * Time.fixedDeltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, currSpeed * moveInput.y, currAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, currDeceleration * Time.fixedDeltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, 0, currDeceleration * Time.fixedDeltaTime);
        }
        transform.Translate(velocity * Time.fixedDeltaTime);

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit.gameObject.tag != "Player")
                continue;
            else if (hit.gameObject.tag == "Player")
                hit.gameObject.GetComponent<Player>().Kill();

            ColliderDistance2D colliderDistance = hit.Distance(circleCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            }
        }
    }

    public override void Kill()
    {
        Debug.Log("Enemy killed");
        player.GetComponent<Player>().enemyKillCount++;

        // Let spawner know
        GameObject.Find("EnemySpawner").GetComponent<Spawner>().EnemyRemoved();

        Destroy(gameObject);
    }
}
