using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    public Text scoreText;

    public float speed;
    public float acceleration;
    public float deceleration;

    private Vector2 velocity;
    private Vector2 moveInput;

    public CircleCollider2D circleCollider;

    void Start()
    {
        player = GameObject.Find("Player");
        scoreText = GameObject.Find("Score").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get Player Position
        moveInput = (player.transform.position - transform.position).normalized;
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
            if (hit.gameObject.tag != "Player")
                continue;
            else if (hit.gameObject.tag == "Player")
                hit.gameObject.GetComponent<CharacterController>().Kill();

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

    public void Kill()
    {
        Debug.Log("Enemy killed");
        player.GetComponent<CharacterController>().enemyKillCount++;
        scoreText.text = player.GetComponent<CharacterController>().enemyKillCount.ToString();
        Destroy(gameObject);
    }
}
