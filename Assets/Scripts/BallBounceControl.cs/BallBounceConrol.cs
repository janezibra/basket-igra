using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallBounceControl : MonoBehaviour
{
    public float groundBounceFactor = 1.12f;
    public float horizontalBoostOnGround = 1.12f;
    public float maxHorizontalSpeedOnBounce = 24f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ground"))
            return;

        Vector2 velocity = rb.linearVelocity;

        if (velocity.y < 0f)
            velocity.y = -velocity.y * groundBounceFactor;

        velocity.x *= horizontalBoostOnGround;
        velocity.x = Mathf.Clamp(velocity.x, -maxHorizontalSpeedOnBounce, maxHorizontalSpeedOnBounce);

        if (Mathf.Abs(velocity.y) < 7.5f)
            velocity.y = 7.5f;

        rb.linearVelocity = velocity;
    }
}
