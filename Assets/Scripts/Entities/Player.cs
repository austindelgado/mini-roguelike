using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Player : Entity
{
    public GameObject playerUI;
    public GameObject projectilePrefab;

    public Transform weapon;

    public Vector2 mouseInput;

    public Camera cam;
    public CircleCollider2D circleCollider;

    // Move to Entity class in future
    public bool canMove;
    public bool canCast;

    public int enemyKillCount;

    public KeyCode key1;
    public AbilitySlot ability1;
    public KeyCode key2;
    public AbilitySlot ability2;
    public KeyCode key3;
    public AbilitySlot ability3;
    public KeyCode key4;
    public AbilitySlot ability4;

    public override void OnStartAuthority()
    {
        playerUI.SetActive(true);
    }

    public override void Start()
    {
        base.Start();

        // Get the camera
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        GameEvents.current.onRoundStart += RoundStart;
        GameEvents.current.onRoundEnd += RoundEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (canCast)
        {
            // Ability inputs
            if (Input.GetKeyDown(key1))
            {
                CmdShoot();
            }
            if (Input.GetKey(key2))
            {
                ability2.Trigger();
            }
            if (Input.GetKey(key3))
            {
                ability3.Trigger();
            }
            if (Input.GetKey(key4))
            {
                ability4.Trigger();
            }
        }

        RotateWeapon();
    }

    void RotateWeapon()
    {
        mouseInput = cam.ScreenToWorldPoint((Vector2)Input.mousePosition);
        lookDir = mouseInput - (Vector2)transform.position;
        weapon.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    [Command]
    void CmdShoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, weapon.transform.position, weapon.transform.rotation);
        NetworkServer.Spawn(projectile);
    }

    void FixedUpdate()
    {
        // Movement
        if (moveInput != Vector2.zero)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput.x, acceleration * Time.fixedDeltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, speed * moveInput.y, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.fixedDeltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, 0, deceleration * Time.fixedDeltaTime);
        }
        transform.Translate(velocity * Time.fixedDeltaTime);

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == circleCollider || hit.gameObject.tag == "Projectile" || hit.gameObject.tag == "Friendly")
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

    public void RoundStart(int round)
    {
        canMove = true;
        canCast = true;
    }

    public void RoundEnd(int round)
    {
        canMove = false;
        canCast = false;

        // Clear input
        moveInput = Vector2.zero;

        // Reset position
        transform.position = new Vector3(0,0,0);
    }

    public override void Kill()
    {
        SceneManager.LoadScene(0);
    }
}
