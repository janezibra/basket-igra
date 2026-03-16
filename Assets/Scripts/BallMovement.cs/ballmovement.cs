using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallMovement : MonoBehaviour
{
    [Header("")]
    public float hitForce = 6f;          
    [Header("")]
    public float minHorizontalSpeed = 2f;
    public float bounceMultiplier = 1f;  
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1) udarec igralca
        if (collision.collider.CompareTag("Player"))
        {
            // smer od playerja proti žogi
            Vector2 dir = (transform.position - collision.transform.position).normalized;
            // samo vodoravno porivanje
            dir.y = 0f;
            rb.AddForce(dir * hitForce, ForceMode2D.Impulse);
        }
        // 2) odboj od leve/desne stene
        if (collision.collider.CompareTag("SideWall"))
        {
            Vector2 v = rb.linearVelocity;
            // obrnemo X hitrost
            v.x = -v.x * bounceMultiplier;
            // če je skoraj 0, jo malo porinemo, da se ne zatakne
            if (Mathf.Abs(v.x) < minHorizontalSpeed)
            {
                float dir = (transform.position.x > collision.transform.position.x) ? 1f : -1f;
                v.x = dir * minHorizontalSpeed;
            }
            rb.linearVelocity = v;
        }
    }
}
