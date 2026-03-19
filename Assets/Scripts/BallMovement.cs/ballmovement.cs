using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallMovement : MonoBehaviour
{
    public float hitForce = 9f;
    public float upwardHitBoost = 2.8f;
    public float minHorizontalSpeed = 4f;
    public float maxHorizontalSpeed = 18f;
    public float bounceMultiplier = 1.15f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsPlayerObject(collision.collider))
        {
            Vector2 dir = (transform.position - collision.transform.position).normalized;
            dir.y = Mathf.Max(0.2f, dir.y + 0.2f);
            Vector2 force = new Vector2(dir.x * hitForce, upwardHitBoost);
            rb.AddForce(force, ForceMode2D.Impulse);

            Vector2 velocity = rb.linearVelocity;
            velocity.x = Mathf.Clamp(velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
            rb.linearVelocity = velocity;
        }

        if (collision.collider.CompareTag("SideWall"))
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.x = -velocity.x * bounceMultiplier;

            if (Mathf.Abs(velocity.x) < minHorizontalSpeed)
            {
                float direction = transform.position.x > collision.transform.position.x ? 1f : -1f;
                velocity.x = direction * minHorizontalSpeed;
            }

            velocity.x = Mathf.Clamp(velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
            rb.linearVelocity = velocity;
        }
    }

    bool IsPlayerObject(Collider2D colliderToCheck)
    {
        return colliderToCheck.GetComponent<PlayerMovement>() != null
            || colliderToCheck.GetComponent<Player2Movement>() != null
            || colliderToCheck.GetComponent<SimpleAIBot>() != null
            || colliderToCheck.CompareTag("Player");
    }
}
