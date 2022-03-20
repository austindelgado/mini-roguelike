using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : Entity
{
    private GameObject player; // Our Targetted enemy

    public float currSpeed;
    public float currAcceleration;
    public float currDeceleration;

    public float stoppingDistance;
    public float retreatDistance;

    public CircleCollider2D circleCollider;

    public Weapon weapon;
    public Transform weaponTransform;

    [SerializeField] private float startFireDelay = .5f;
    private float fireDelay;

    // TODO Really clean this up and turn as many components into scriptableObjects as possible

    // Update is called once per frame
    [Server]
    void Update()
    {
        if (hasAuthority)
            return;

        if (player == null)
            return;

        // // Get Player Position
        if (Vector2.Distance(transform.position, player.transform.position) > stoppingDistance)
            moveInput = (player.transform.position - transform.position).normalized;
        else if (Vector2.Distance(transform.position, player.transform.position) <= stoppingDistance && Vector2.Distance(transform.position, player.transform.position) >= retreatDistance)
        {
            moveInput = Vector2.zero;
            if (fireDelay <= 0)
            {
                weapon.ToggleFire(true);
                weapon.ToggleFire(false);
                fireDelay = startFireDelay;
            }
        }
        else if (Vector2.Distance(transform.position, player.transform.position) < retreatDistance)
            moveInput = (transform.position -  player.transform.position).normalized;


        // // Get player direction
        lookDir = (player.transform.position - transform.position).normalized;
        // float distance = Vector2.Distance(player.transform.position, transform.position);

        RotateWeapon();

        fireDelay -= Time.deltaTime;
    }

    [Server]
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
            if (hit.gameObject == gameObject || hit.gameObject.tag == "Projectile")
                continue;

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

    [Server]
    void RotateWeapon()
    {
        weaponTransform.rotation = Quaternion.AngleAxis(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    // On spawn, serve sets player we're targetting
    [Server]
    public void SetGridPlayer(GameObject player)
    {
        this.player = player;
        fireDelay = startFireDelay;
    }

    public override void Kill()
    {
        Destroy(gameObject); // Just destroy self right now, TODO let grid cell now so this can be tracked
    }
}
