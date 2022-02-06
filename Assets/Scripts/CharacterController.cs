using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed;
    public float acceleration;
    public float deceleration;

    private Vector2 velocity;
    private Vector2 moveInput;
    private Vector2 mouseInput;
    private Vector2 lookDir;

    public Camera cam;
    public CircleCollider2D circleCollider;

    public GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        mouseInput = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButtonDown("Fire1"))
            Shoot();
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

        // Aiming
        lookDir = (mouseInput - (Vector2)transform.position).normalized;

        // Collision
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == circleCollider || hit.gameObject.tag == "Projectile")
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

    // this is temporary and will probably move out of here
    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.GetComponent<Projectile>().dir = lookDir;
    }

    public void Kill()
    {
        Application.Quit();
    }
}
