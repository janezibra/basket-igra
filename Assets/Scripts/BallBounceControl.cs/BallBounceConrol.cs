using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallBounceControl : MonoBehaviour
{
public float groundBounceFactor = 0.90f;    // prej 0.85, zdaj močnejši bounce
public float wallBounceFactor = 0.65f;      // malo močnejši odboj od stene
public float horizontalDampOnGround = 0.20f; // prej 0.25 → manj dušenja
public float maxHorizontalSpeedOnBounce = 14f; 
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 v = rb.linearVelocity;

        // ODBOJ OD TAL
        if (collision.collider.CompareTag("Ground"))
        {
            if (v.y < 0f)
                v.y = -v.y * groundBounceFactor;

            v.x *= (1f - horizontalDampOnGround);

            v.x = Mathf.Clamp(v.x, -maxHorizontalSpeedOnBounce, maxHorizontalSpeedOnBounce);
        }

        // ODBOJ OD STRANSKE STENE
        else if (collision.collider.CompareTag("SideWall"))
        {
            v.x = -v.x * wallBounceFactor;
        }

        rb.linearVelocity = v;
    }
}
