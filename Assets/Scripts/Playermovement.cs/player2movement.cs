using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player2Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 16f;
    public float jumpForce = 18f;
    public float acceleration = 145f;
    public float airControl = 0.94f;

    [Header("Controls")]
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jumpKey = KeyCode.UpArrow;

    private Rigidbody2D rb;
    private float inputX;
    private bool isGrounded;
    private Vector3 baseScale;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanPlayersControl())
        {
            inputX = 0f;
            return;
        }

        inputX = 0f;

        if (Input.GetKey(leftKey))
            inputX = -1f;
        else if (Input.GetKey(rightKey))
            inputX = 1f;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (inputX > 0f)
            FaceDirection(1f);
        else if (inputX < 0f)
            FaceDirection(-1f);
    }

    void FaceDirection(float direction)
    {
        transform.localScale = new Vector3(Mathf.Abs(baseScale.x) * Mathf.Sign(direction), Mathf.Abs(baseScale.y), Mathf.Abs(baseScale.z));

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction < 0f;
    }

    void FixedUpdate()
    {
        float controlMultiplier = isGrounded ? 1f : airControl;
        float targetVelocityX = inputX * moveSpeed;
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelocityX, acceleration * controlMultiplier * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = false;
    }
}
