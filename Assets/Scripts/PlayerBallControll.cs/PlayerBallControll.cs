using UnityEngine;

public class PlayerBallControl : MonoBehaviour
{
    [Header("References")]
    public GameObject ball;          // žogo nastaviš v Inspectorju
    public Transform holdPoint;      // točka, kjer jo držiš

    [Header("Pickup settings")]
    public float pickUpDistance = 2.5f;  // koliko blizu mora biti player
    public float pickUpCooldownTime = 0.3f; // čas po metu, ko je ne more takoj pobrati

    [Header("Throw settings")]
    public float throwForceX = 6f;   // vodoravna sila meta
    public float throwForceY = 10f;  // navpična sila meta

    private Rigidbody2D ballRb;
    private Collider2D ballCollider;
    private bool holdingBall = false;
    private float pickUpCooldown = 0f;

    void Start()
    {
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody2D>();
            ballCollider = ball.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogWarning("PlayerBallControl: ball ni nastavljena v Inspectorju!");
        }

        if (holdPoint == null)
        {
            Debug.LogWarning("PlayerBallControl: holdPoint ni nastavljen v Inspectorju!");
        }
    }

    void Update()
    {
        if (ball == null || holdPoint == null) return;

        // odštevanje cooldowna za pickup
        if (pickUpCooldown > 0f)
            pickUpCooldown -= Time.deltaTime;

        if (!holdingBall)
        {
            TryPickUpBall();
        }
        else
        {
            HoldBall();

            // MET z ↑ ali W
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                ThrowBall();
            }
        }
    }

    void TryPickUpBall()
{
    if (pickUpCooldown > 0f) return;
    if (ballRb == null) return;

    float dist = Vector2.Distance(transform.position, ball.transform.position);

    // lahko jo pobereš, če je dovolj blizu
    // in ni prehitro (ampak ni treba, da je čisto mrtva)
    if (dist <= pickUpDistance && ballRb.linearVelocity.magnitude < 2f)
    {
        Debug.Log("Poberem žogo!");
        holdingBall = true;

        ballRb.bodyType = RigidbodyType2D.Kinematic;
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        if (ballCollider != null)
            ballCollider.enabled = false;
    }
}


    void HoldBall()
    {
        // žoga sledi točki holdPoint
        ball.transform.position = holdPoint.position;
    }

void ThrowBall()
{
    if (ballRb == null) return;

    holdingBall = false;

    // nazaj na fiziko
    ballRb.bodyType = RigidbodyType2D.Dynamic;

    if (ballCollider != null)
        ballCollider.enabled = true;

    float dir = transform.localScale.x >= 0 ? 1f : -1f;

    // resetiraj staro hitrost pred metom
    ballRb.linearVelocity = Vector2.zero;
    ballRb.angularVelocity = 0f;

    // met v smer pogleda
    ballRb.linearVelocity = new Vector2(dir * throwForceX, throwForceY);

    pickUpCooldown = pickUpCooldownTime;

    Debug.Log("Vržem žogo " + (dir > 0 ? "DESNO" : "LEVO") + "!");
}


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpDistance);
    }
}
