using UnityEngine;

public class PlayerBallControl : MonoBehaviour
{
    [Header("References")]
    public GameObject ball;
    public Transform holdPoint;

    [Header("Pickup settings")]
    public float pickUpDistance = 6.8f;
    public float pickUpCooldownTime = 0.04f;
    public float maxPickupBallSpeed = 18f;

    [Header("Throw settings")]
    public float throwForceX = 6f;
    public float throwForceY = 10f;
    public KeyCode throwKey = KeyCode.Space;

    private static PlayerBallControl currentBallOwner;

    private Rigidbody2D ballRb;
    private Collider2D ballCollider;
    private bool holdingBall;
    private float pickUpCooldown;

    public bool IsHoldingBall => holdingBall;

    void Start()
    {
        if (ball == null)
            ball = GameObject.FindWithTag("Ball");

        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody2D>();
            ballCollider = ball.GetComponent<Collider2D>();
        }

        if (holdPoint == null)
            Debug.LogWarning("PlayerBallControl: holdPoint is not assigned in the Inspector.");
    }

    void Update()
    {
        if (ball == null || holdPoint == null)
            return;

        if (pickUpCooldown > 0f)
            pickUpCooldown -= Time.deltaTime;

        bool canControl = GameManager.Instance == null || GameManager.Instance.CanPlayersControl();

        if (!canControl)
        {
            if (holdingBall)
                HoldBall();
            return;
        }

        if (!holdingBall)
        {
            TryPickUpBall();
        }
        else
        {
            HoldBall();

            if (Input.GetKeyDown(throwKey))
                ThrowBall();
        }
    }

    void TryPickUpBall()
    {
        if (pickUpCooldown > 0f || ballRb == null)
            return;

        if (currentBallOwner != null && currentBallOwner != this && currentBallOwner.IsHoldingBall)
            return;

        Vector2 ballPosition = ball.transform.position;
        Vector2 playerPosition = transform.position;
        Vector2 holdPosition = holdPoint.position;
        Vector2 colliderPoint = ballCollider != null ? ballCollider.ClosestPoint(holdPosition) : ballPosition;

        float playerDistance = Vector2.Distance(playerPosition, ballPosition);
        float handDistance = Vector2.Distance(holdPosition, colliderPoint);
        float bodyDistance = Vector2.Distance(playerPosition, colliderPoint);
        float distanceToBall = Mathf.Min(playerDistance, handDistance, bodyDistance);

        if (distanceToBall > pickUpDistance)
            return;

        if (ballRb.linearVelocity.magnitude > maxPickupBallSpeed)
            return;

        holdingBall = true;
        currentBallOwner = this;
        ballRb.bodyType = RigidbodyType2D.Kinematic;
        ballRb.simulated = true;
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        if (ballCollider != null)
            ballCollider.enabled = false;

        HoldBall();
        pickUpCooldown = pickUpCooldownTime;
    }

    public bool TryPickUpBallExternal()
    {
        if (holdingBall)
            return true;

        bool wasHoldingBall = holdingBall;
        TryPickUpBall();
        return !wasHoldingBall && holdingBall;
    }

    void HoldBall()
    {
        ball.transform.position = holdPoint.position;
        ball.transform.rotation = Quaternion.identity;
    }

    void ThrowBall()
    {
        if (ballRb == null)
            return;

        holdingBall = false;
        if (currentBallOwner == this)
            currentBallOwner = null;
        ballRb.bodyType = RigidbodyType2D.Dynamic;
        ballRb.simulated = true;

        if (ballCollider != null)
            ballCollider.enabled = true;

        float direction = transform.localScale.x >= 0f ? 1f : -1f;

        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        ballRb.linearVelocity = new Vector2(direction * throwForceX, throwForceY);

        pickUpCooldown = pickUpCooldownTime;
    }

    public void ThrowBallExternal(float horizontalForce, float verticalForce)
    {
        if (ballRb == null || !holdingBall)
            return;

        holdingBall = false;
        if (currentBallOwner == this)
            currentBallOwner = null;
        ballRb.bodyType = RigidbodyType2D.Dynamic;
        ballRb.simulated = true;

        if (ballCollider != null)
            ballCollider.enabled = true;

        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        ballRb.linearVelocity = new Vector2(horizontalForce, verticalForce);

        pickUpCooldown = pickUpCooldownTime;
    }

    public void ReleaseBall()
    {
        holdingBall = false;
        if (currentBallOwner == this)
            currentBallOwner = null;

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Dynamic;
            ballRb.simulated = true;
        }

        if (ballCollider != null)
            ballCollider.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = holdPoint != null ? holdPoint.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, pickUpDistance);
    }

    public void ForceHoldBall()
    {
        if (ball == null || holdPoint == null)
            return;

        if (ballRb == null)
            ballRb = ball.GetComponent<Rigidbody2D>();

        if (ballCollider == null)
            ballCollider = ball.GetComponent<Collider2D>();

        holdingBall = true;
        currentBallOwner = this;

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            ballRb.simulated = true;
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        if (ballCollider != null)
            ballCollider.enabled = false;

        HoldBall();
    }
}
