using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    public Text scoreText;

    public float baseSpeed;
    public float currSpeed;

    public float baseAcceleration;
    public float currAcceleration;
    
    public float baseDeceleration;
    public float currDeceleration;

    private Vector2 velocity;
    private Vector2 moveInput;

    public CircleCollider2D circleCollider;

    void Start()
    {
        player = GameObject.Find("Player");
        scoreText = GameObject.Find("Score").GetComponent<Text>();

        currSpeed = baseSpeed;
        currAcceleration = baseAcceleration;
        currDeceleration = baseDeceleration;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Player Position
        moveInput = (player.transform.position - transform.position).normalized;
        currSpeed = baseSpeed + player.GetComponent<CharacterController>().enemyKillCount * .1f;
        currAcceleration = baseAcceleration + player.GetComponent<CharacterController>().enemyKillCount * .075f;
        currDeceleration = baseDeceleration + player.GetComponent<CharacterController>().enemyKillCount * .075f;
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
