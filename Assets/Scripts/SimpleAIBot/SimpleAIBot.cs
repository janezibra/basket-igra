using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleAIBot : MonoBehaviour
{
    public Transform ball;
    public Transform targetHoop;
    public float moveSpeed = 10.5f;
    public float jumpForce = 15f;
    public float acceleration = 88f;
    public float followDistance = 0.2f;
    public float defendX = 4.8f;
    public float chaseRange = 11f;
    public float jumpBallHeightOffset = 1.15f;
    public float attackBias = 1.3f;
    public float interceptOffset = 1.1f;
    public float shootRange = 4.8f;
    public float minShootForceX = 8f;
    public float maxShootForceX = 15f;
    public float shootForceY = 13.5f;
    public float jumpCooldown = 0.65f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 baseScale;
    private PlayerBallControl ballControl;
    private float jumpCooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
        ballControl = GetComponent<PlayerBallControl>();
    }

    void Start()
    {
        if (ball == null)
        {
            GameObject ballObject = GameObject.FindWithTag("Ball");
            if (ballObject != null)
                ball = ballObject.transform;
        }

        if (targetHoop == null)
        {
            GameObject hoopObject = GameObject.Find("NetTriggerLeva");
            if (hoopObject != null)
                targetHoop = hoopObject.transform;
        }
    }

    void Update()
    {
        if (ball == null)
            return;

        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        if (GameManager.Instance != null && !GameManager.Instance.CanPlayersControl())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (ballControl != null && ballControl.IsHoldingBall)
        {
            PlayAttackWithBall();
            return;
        }

        if (ballControl != null)
            ballControl.TryPickUpBallExternal();

        MoveTowardsTarget(GetTargetX());
        TryJumpForBall();
    }

    void MoveTowardsTarget(float targetX)
    {
        float delta = targetX - transform.position.x;
        float dir = Mathf.Abs(delta) > followDistance ? Mathf.Sign(delta) : 0f;
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, dir * moveSpeed, acceleration * Time.deltaTime);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

        if (dir > 0f)
            transform.localScale = new Vector3(Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
        else if (dir < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
    }

    void TryJumpForBall()
    {
        if (!isGrounded || jumpCooldownTimer > 0f)
            return;

        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        bool ballAbove = ball.position.y > transform.position.y + jumpBallHeightOffset;
        bool ballClose = Mathf.Abs(ball.position.x - transform.position.x) < 1.4f;
        bool ballComingDown = ballRb == null || ballRb.linearVelocity.y < 1.2f;

        if (ballAbove && ballClose && ballComingDown)
            Jump();
    }

    void PlayAttackWithBall()
    {
        if (targetHoop == null || ballControl == null)
            return;

        float desiredX = targetHoop.position.x + 2.1f;
        MoveTowardsTarget(desiredX);

        float hoopDistance = Mathf.Abs(targetHoop.position.x - transform.position.x);
        bool closeEnoughToShoot = hoopDistance <= shootRange;
        bool roughlyAligned = transform.position.x > targetHoop.position.x + 0.35f;

        if (isGrounded && closeEnoughToShoot && roughlyAligned)
        {
            float horizontalForce = Mathf.Clamp((targetHoop.position.x - transform.position.x) * 2.1f, -maxShootForceX, -minShootForceX);
            ballControl.ThrowBallExternal(horizontalForce, shootForceY);
            Jump(jumpForce * 0.75f);
        }
    }

    float GetTargetX()
    {
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        float predictedBallX = ball.position.x;

        if (ballRb != null)
            predictedBallX += ballRb.linearVelocity.x * interceptOffset;

        bool ballOnAttackSide = predictedBallX > 0f;
        bool shouldChase = Mathf.Abs(predictedBallX - transform.position.x) <= chaseRange || ballOnAttackSide;

        if (!shouldChase)
            return defendX;

        float attackTarget = predictedBallX + attackBias;
        return Mathf.Max(defendX - 0.55f, attackTarget);
    }

    void Jump()
    {
        Jump(jumpForce);
    }

    void Jump(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        jumpCooldownTimer = jumpCooldown;
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
