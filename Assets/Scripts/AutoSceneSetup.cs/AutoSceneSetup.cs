using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SetupSceneAfterLoad()
    {
        if (Application.isPlaying)
            ConfigureScene();
    }

    static void ConfigureScene()
    {
        GameObject ball = GameObject.FindWithTag("Ball");
        GameObject player1 = FindPlayer1();
        if (player1 == null)
            return;

        player1.name = "Player1";
        EnsurePlayer1Setup(player1, ball);

        GameObject player2 = FindPlayer2(player1);
        if (player2 != null)
            player2.name = "Player2";
        else
            Debug.LogWarning("AutoSceneSetup: Player2 was not found in the scene, so no clone was created.");

        EnsurePlayer2Setup(player2, ball);
        ApplyStartupLayout(player1, player2, ball);
        EnsureGameManager(ball, player1, player2);
        EnsureGameModeSetup(player2);
        EnsureHoopTriggers();
    }

    static void ApplyStartupLayout(GameObject player1, GameObject player2, GameObject ball)
    {
        if (player1 != null)
        {
            Vector3 position = player1.transform.position;
            position.x = -2.26f;
            position.y = -3.89f;
            player1.transform.position = position;
        }

        if (player2 != null)
        {
            Vector3 position = player2.transform.position;
            position.x = 7.27f;
            position.y = -3.93f;
            player2.transform.position = position;
        }

        if (ball != null)
        {
            Vector3 position = ball.transform.position;
            position.x = 2.55f;
            position.y = 4.92f;
            ball.transform.position = position;
        }
    }

    static GameObject FindPlayer1()
    {
        GameObject namedPlayer = GameObject.Find("Player1");
        if (namedPlayer != null)
            return namedPlayer;

        PlayerMovement movement = Object.FindFirstObjectByType<PlayerMovement>();
        return movement != null ? movement.gameObject : null;
    }

    static GameObject FindPlayer2(GameObject player1)
    {
        GameObject namedPlayer = GameObject.Find("Player2");
        if (namedPlayer != null && namedPlayer != player1)
            return namedPlayer;

        Player2Movement player2Movement = Object.FindFirstObjectByType<Player2Movement>();
        if (player2Movement != null && player2Movement.gameObject != player1)
            return player2Movement.gameObject;

        SimpleAIBot aiBot = Object.FindFirstObjectByType<SimpleAIBot>();
        if (aiBot != null && aiBot.gameObject != player1)
            return aiBot.gameObject;

        PlayerBallControl[] ballControls = Object.FindObjectsByType<PlayerBallControl>(FindObjectsSortMode.None);
        foreach (PlayerBallControl ballControl in ballControls)
        {
            if (ballControl != null && ballControl.gameObject != player1)
                return ballControl.gameObject;
        }

        SpriteRenderer[] renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer == null || renderer.gameObject == player1)
                continue;

            GameObject candidate = renderer.gameObject;
            if (candidate.transform.parent != null)
                continue;

            if (candidate.CompareTag("Ball"))
                continue;

            if (candidate.GetComponent<Rigidbody2D>() != null && candidate.GetComponent<Collider2D>() != null)
                return renderer.gameObject;
        }

        return null;
    }

    static void EnsurePlayer1Setup(GameObject player1, GameObject ball)
    {
        PlayerMovement movement = player1.GetComponent<PlayerMovement>();
        if (movement == null)
            movement = player1.AddComponent<PlayerMovement>();

        movement.moveSpeed = 16f;
        movement.jumpForce = 18f;
        movement.acceleration = 145f;
        movement.airControl = 0.94f;
        movement.leftKey = KeyCode.A;
        movement.rightKey = KeyCode.D;
        movement.jumpKey = KeyCode.W;

        PlayerBallControl ballControl = player1.GetComponent<PlayerBallControl>();
        if (ballControl == null)
            ballControl = player1.AddComponent<PlayerBallControl>();

        ballControl.ball = ball;
        ballControl.holdPoint = EnsureHoldPoint(player1.transform, "HoldPoint");
        ballControl.throwKey = KeyCode.Space;
        ballControl.pickUpDistance = 5.2f;
        ballControl.maxPickupBallSpeed = 9f;
        ballControl.pickUpCooldownTime = 0.1f;
        ballControl.throwForceX = 16f;
        ballControl.throwForceY = 18f;
    }

    static void EnsurePlayer2Setup(GameObject player2, GameObject ball)
    {
        if (player2 == null)
            return;

        PlayerMovement oldMovement = player2.GetComponent<PlayerMovement>();
        if (oldMovement != null)
            Object.Destroy(oldMovement);

        Player2Movement movement = player2.GetComponent<Player2Movement>();
        if (movement == null)
            movement = player2.AddComponent<Player2Movement>();

        movement.moveSpeed = 16f;
        movement.jumpForce = 18f;
        movement.acceleration = 145f;
        movement.airControl = 0.94f;
        movement.leftKey = KeyCode.LeftArrow;
        movement.rightKey = KeyCode.RightArrow;
        movement.jumpKey = KeyCode.UpArrow;

        SimpleAIBot ai = player2.GetComponent<SimpleAIBot>();
        if (ai == null)
            ai = player2.AddComponent<SimpleAIBot>();

        ai.ball = ball != null ? ball.transform : null;
        ai.moveSpeed = 10.5f;
        ai.jumpForce = 15f;
        ai.acceleration = 88f;
        ai.defendX = 4.8f;
        ai.chaseRange = 11f;
        ai.attackBias = 1.3f;
        ai.interceptOffset = 1.1f;

        PlayerBallControl ballControl = player2.GetComponent<PlayerBallControl>();
        if (ballControl == null)
            ballControl = player2.AddComponent<PlayerBallControl>();

        ballControl.ball = ball;
        ballControl.holdPoint = EnsureHoldPoint(player2.transform, "HoldPoint");
        ballControl.throwKey = KeyCode.Return;
        ballControl.pickUpDistance = 5.2f;
        ballControl.maxPickupBallSpeed = 9f;
        ballControl.pickUpCooldownTime = 0.1f;
        ballControl.throwForceX = 16f;
        ballControl.throwForceY = 18f;
    }

    static Transform EnsureHoldPoint(Transform playerTransform, string childName)
    {
        Transform holdPoint = playerTransform.Find(childName);
        if (holdPoint == null)
        {
            GameObject holdPointObject = new GameObject(childName);
            holdPoint = holdPointObject.transform;
            holdPoint.SetParent(playerTransform);
        }

        holdPoint.localRotation = Quaternion.identity;
        holdPoint.localPosition = new Vector3(0.55f, 0.2f, 0f);
        holdPoint.localScale = Vector3.one;
        return holdPoint;
    }

    static void EnsureGameManager(GameObject ball, GameObject player1, GameObject player2)
    {
        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        if (manager == null)
        {
            GameObject managerObject = new GameObject("GameManager");
            manager = managerObject.AddComponent<GameManager>();
        }

        manager.ball = ball;
        manager.leftPlayer = player1;
        manager.rightPlayer = player2;
        manager.playersCanPassThroughEachOther = true;
        manager.scoreToWin = 5;
    }

    static void EnsureGameModeSetup(GameObject player2)
    {
        GameModeSetup setup = Object.FindFirstObjectByType<GameModeSetup>();
        if (setup == null)
        {
            GameObject setupObject = new GameObject("GameModeSetup");
            setup = setupObject.AddComponent<GameModeSetup>();
        }

        setup.rightPlayer = player2;
        setup.showModeSelectionOnStart = true;
    }

    static void EnsureHoopTriggers()
    {
        GameObject leftTrigger = GameObject.Find("NetTriggerLeva");
        if (leftTrigger != null)
        {
            HoopNet leftHoop = leftTrigger.GetComponent<HoopNet>();
            if (leftHoop == null)
                leftHoop = leftTrigger.AddComponent<HoopNet>();
            leftHoop.hoopSide = HoopNet.HoopSide.Leva;
        }

        GameObject rightTrigger = GameObject.Find("NetTriggerDesna");
        if (rightTrigger != null)
        {
            HoopNet rightHoop = rightTrigger.GetComponent<HoopNet>();
            if (rightHoop == null)
                rightHoop = rightTrigger.AddComponent<HoopNet>();
            rightHoop.hoopSide = HoopNet.HoopSide.Desna;
        }
    }
}
