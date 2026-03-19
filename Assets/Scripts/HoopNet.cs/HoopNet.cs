using System.Collections.Generic;
using UnityEngine;

public class HoopNet : MonoBehaviour
{
    public enum HoopSide { Leva, Desna }

    public HoopSide hoopSide;

    private readonly HashSet<int> trackedBalls = new HashSet<int>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball"))
            return;

        Rigidbody2D ballRb = other.attachedRigidbody;
        if (ballRb == null)
            return;

        bool enteredFromAbove = other.bounds.center.y > transform.position.y;
        bool movingDown = ballRb.linearVelocity.y <= 0.05f;

        if (enteredFromAbove && movingDown)
            trackedBalls.Add(other.GetInstanceID());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Ball"))
            return;

        int ballId = other.GetInstanceID();
        if (!trackedBalls.Contains(ballId))
            return;

        trackedBalls.Remove(ballId);

        if (GameManager.Instance == null)
            return;

        bool exitedBelow = other.bounds.center.y < transform.position.y;
        if (!exitedBelow)
            return;

        if (hoopSide == HoopSide.Leva)
            GameManager.Instance.ScoreRight();
        else
            GameManager.Instance.ScoreLeft();
    }
}
