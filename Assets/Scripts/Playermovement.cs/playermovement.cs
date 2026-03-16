using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    private Rigidbody2D rb;
    private float inputX;
    private bool isGrounded;

    private Vector3 baseScale;   // originalna velikost playerja

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
    }

    void Update()
    {
        // premik levo / desno
        inputX = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            inputX = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            inputX = 1f;

        // SKOK – samo ko je na tleh
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // obračanje lika brez spreminjanja velikosti
        if (inputX > 0f)
        {
            transform.localScale = new Vector3(Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
        }
        else if (inputX < 0f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
        }
    }

    void FixedUpdate()
    {
        // vodoravno premikanje
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    // zaznavanje tal preko TAG = Ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
