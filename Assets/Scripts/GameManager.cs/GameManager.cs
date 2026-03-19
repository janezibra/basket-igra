using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private const string GameplaySceneName = "SampleScene";

    [Header("Score")]
    public int leftScore = 0;
    public int rightScore = 0;
    public int scoreToWin = 5;

    [Header("UI")]
    public TMP_Text leftScoreText;
    public TMP_Text rightScoreText;
    public TMP_Text winText;
    public GameObject gameOverPanel;

    [Header("References")]
    public Transform ballSpawnPoint;
    public GameObject ball;
    public Transform leftPlayerSpawn;
    public Transform rightPlayerSpawn;
    public GameObject leftPlayer;
    public GameObject rightPlayer;

    [Header("Gameplay")]
    public bool playersCanPassThroughEachOther = true;
    public float countdownDuration = 3f;

    private Rigidbody2D ballRb;
    private Rigidbody2D leftPlayerRb;
    private Rigidbody2D rightPlayerRb;
    private PlayerBallControl[] playerBallControls;
    private string winnerMessage = string.Empty;
    private string roundMessage = string.Empty;
    private float roundMessageTimer;
    private float countdownTimer;
    private bool controlsLocked = true;
    private bool roundEnded;
    private Vector3 initialBallPosition;
    private Vector3 initialLeftPlayerPosition;
    private Vector3 initialRightPlayerPosition;

    public bool CanPlayersControl()
    {
        return !controlsLocked && !roundEnded && Time.timeScale > 0f;
    }

    public bool IsCountdownActive()
    {
        return countdownTimer > 0f;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AutoAssignReferences();

        if (ball != null)
            ballRb = ball.GetComponent<Rigidbody2D>();

        if (leftPlayer != null)
            leftPlayerRb = leftPlayer.GetComponent<Rigidbody2D>();

        if (rightPlayer != null)
            rightPlayerRb = rightPlayer.GetComponent<Rigidbody2D>();

        playerBallControls = FindObjectsByType<PlayerBallControl>(FindObjectsSortMode.None);

        if (playersCanPassThroughEachOther)
            IgnorePlayerCollisions();

        if (ball != null)
            initialBallPosition = ball.transform.position;

        if (leftPlayer != null)
            initialLeftPlayerPosition = leftPlayer.transform.position;

        if (rightPlayer != null)
            initialRightPlayerPosition = rightPlayer.transform.position;

        UpdateScoreUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        BeginRoundCountdown();
    }

    void Update()
    {
        if (roundMessageTimer > 0f)
            roundMessageTimer -= Time.unscaledDeltaTime;

        if (countdownTimer > 0f)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
                StartLiveRound();
        }
    }

    void AutoAssignReferences()
    {
        if (leftPlayer == null)
            leftPlayer = GameObject.Find("Player1");

        if (rightPlayer == null)
            rightPlayer = GameObject.Find("Player2");

        if (ball == null)
            ball = GameObject.FindWithTag("Ball");

        if (ballSpawnPoint == null && ball != null)
            ballSpawnPoint = ball.transform;

        if (leftPlayerSpawn == null && leftPlayer != null)
            leftPlayerSpawn = leftPlayer.transform;

        if (rightPlayerSpawn == null && rightPlayer != null)
            rightPlayerSpawn = rightPlayer.transform;
    }

    void IgnorePlayerCollisions()
    {
        if (leftPlayer == null || rightPlayer == null)
            return;

        Collider2D[] leftColliders = leftPlayer.GetComponentsInChildren<Collider2D>(true);
        Collider2D[] rightColliders = rightPlayer.GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D leftCollider in leftColliders)
        {
            foreach (Collider2D rightCollider in rightColliders)
            {
                if (leftCollider != null && rightCollider != null)
                    Physics2D.IgnoreCollision(leftCollider, rightCollider, true);
            }
        }
    }

    public void ScoreLeft()
    {
        if (roundEnded)
            return;

        leftScore++;
        roundMessage = "PLAYER 1 SCORED!";
        roundMessageTimer = 1.2f;
        UpdateScoreUI();
        CheckWin();

        if (!roundEnded)
            ResetRound();
    }

    public void ScoreRight()
    {
        if (roundEnded)
            return;

        rightScore++;
        roundMessage = "PLAYER 2 SCORED!";
        roundMessageTimer = 1.2f;
        UpdateScoreUI();
        CheckWin();

        if (!roundEnded)
            ResetRound();
    }

    void UpdateScoreUI()
    {
        if (leftScoreText != null)
            leftScoreText.text = leftScore.ToString();

        if (rightScoreText != null)
            rightScoreText.text = rightScore.ToString();
    }

    void CheckWin()
    {
        if (leftScore >= scoreToWin)
            EndGame("PLAYER 1 WINS!");
        else if (rightScore >= scoreToWin)
            EndGame("PLAYER 2 WINS!");
    }

    void EndGame(string message)
    {
        winnerMessage = message;
        roundMessage = string.Empty;
        roundMessageTimer = 0f;
        roundEnded = true;
        controlsLocked = true;
        FreezeRoundBodies(true);
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (winText != null)
            winText.text = message;
    }

    public void ResetRound()
    {
        if (roundEnded)
            return;

        controlsLocked = true;

        if (playerBallControls == null || playerBallControls.Length == 0)
            playerBallControls = FindObjectsByType<PlayerBallControl>(FindObjectsSortMode.None);

        foreach (PlayerBallControl playerBallControl in playerBallControls)
        {
            if (playerBallControl != null)
                playerBallControl.ReleaseBall();
        }

        if (ball != null)
            ball.transform.position = initialBallPosition;

        if (leftPlayer != null)
            leftPlayer.transform.position = initialLeftPlayerPosition;

        if (rightPlayer != null)
            rightPlayer.transform.position = initialRightPlayerPosition;

        ZeroBodies();
        FreezeRoundBodies(true);
        BeginRoundCountdown();
    }

    void BeginRoundCountdown()
    {
        countdownTimer = countdownDuration;
        controlsLocked = true;
    }

    void StartLiveRound()
    {
        countdownTimer = 0f;
        controlsLocked = false;
        FreezeRoundBodies(false);
        ZeroBodies();
    }

    void FreezeRoundBodies(bool freeze)
    {
        if (ballRb == null && ball != null)
            ballRb = ball.GetComponent<Rigidbody2D>();

        if (leftPlayerRb == null && leftPlayer != null)
            leftPlayerRb = leftPlayer.GetComponent<Rigidbody2D>();

        if (rightPlayerRb == null && rightPlayer != null)
            rightPlayerRb = rightPlayer.GetComponent<Rigidbody2D>();

        if (ballRb != null)
            ballRb.simulated = !freeze;

        if (leftPlayerRb != null)
            leftPlayerRb.simulated = !freeze;

        if (rightPlayerRb != null)
            rightPlayerRb.simulated = !freeze;
    }

    void ZeroBodies()
    {
        if (ballRb == null && ball != null)
            ballRb = ball.GetComponent<Rigidbody2D>();

        if (leftPlayerRb == null && leftPlayer != null)
            leftPlayerRb = leftPlayer.GetComponent<Rigidbody2D>();

        if (rightPlayerRb == null && rightPlayer != null)
            rightPlayerRb = rightPlayer.GetComponent<Rigidbody2D>();

        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Dynamic;
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        if (leftPlayerRb != null)
        {
            leftPlayerRb.linearVelocity = Vector2.zero;
            leftPlayerRb.angularVelocity = 0f;
        }

        if (rightPlayerRb != null)
        {
            rightPlayerRb.linearVelocity = Vector2.zero;
            rightPlayerRb.angularVelocity = 0f;
        }
    }

    void OnGUI()
    {
        bool showGameplayUi = !GameModeSetup.IsSelectionVisible;

        if (showGameplayUi)
            DrawScoreOverlay();

        if (showGameplayUi && roundMessageTimer > 0f && !string.IsNullOrEmpty(roundMessage))
            DrawRoundMessage();

        if (showGameplayUi && IsCountdownActive() && !roundEnded && Time.timeScale > 0f)
            DrawCountdownOverlay();

        if (Time.timeScale == 0f && !string.IsNullOrEmpty(winnerMessage))
            DrawWinnerOverlay();
    }

    void DrawScoreOverlay()
    {
        Rect boardRect = new Rect((Screen.width - 382f) * 0.5f, 18f, 382f, 78f);

        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.18f);
        GUI.DrawTexture(new Rect(boardRect.x + 6f, boardRect.y + 6f, boardRect.width, boardRect.height), Texture2D.whiteTexture);
        GUI.color = new Color(0.89f, 0.94f, 1f, 0.96f);
        GUI.DrawTexture(boardRect, Texture2D.whiteTexture);
        GUI.color = new Color(0.22f, 0.46f, 0.84f, 1f);
        GUI.DrawTexture(new Rect(boardRect.x, boardRect.y, boardRect.width, 10f), Texture2D.whiteTexture);
        GUI.color = new Color(0.53f, 0.32f, 0.12f, 1f);
        GUI.DrawTexture(new Rect(boardRect.x, boardRect.y + boardRect.height - 8f, boardRect.width, 8f), Texture2D.whiteTexture);
        GUI.color = new Color(0.9f, 0.39f, 0.18f, 1f);
        GUI.DrawTexture(new Rect(boardRect.x + 16f, boardRect.y + 18f, 8f, boardRect.height - 34f), Texture2D.whiteTexture);
        GUI.color = new Color(0.18f, 0.59f, 0.37f, 1f);
        GUI.DrawTexture(new Rect(boardRect.x + boardRect.width - 24f, boardRect.y + 18f, 8f, boardRect.height - 34f), Texture2D.whiteTexture);
        GUI.color = previousColor;

        GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
        nameStyle.alignment = TextAnchor.MiddleCenter;
        nameStyle.fontSize = 14;
        nameStyle.fontStyle = FontStyle.Bold;
        nameStyle.normal.textColor = new Color(0.2f, 0.28f, 0.39f);

        GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
        scoreStyle.alignment = TextAnchor.MiddleCenter;
        scoreStyle.fontSize = 34;
        scoreStyle.fontStyle = FontStyle.Bold;
        scoreStyle.normal.textColor = new Color(0.12f, 0.21f, 0.36f);

        GUIStyle centerStyle = new GUIStyle(scoreStyle);
        centerStyle.fontSize = 24;
        centerStyle.normal.textColor = new Color(0.9f, 0.39f, 0.18f);

        GUI.Label(new Rect(boardRect.x + 34f, boardRect.y + 16f, 100f, 20f), "PLAYER 1", nameStyle);
        GUI.Label(new Rect(boardRect.x + 226f, boardRect.y + 16f, 100f, 20f), "PLAYER 2", nameStyle);
        GUI.Label(new Rect(boardRect.x + 42f, boardRect.y + 34f, 84f, 34f), leftScore.ToString(), scoreStyle);
        GUI.Label(new Rect(boardRect.x + 234f, boardRect.y + 34f, 84f, 34f), rightScore.ToString(), scoreStyle);
        GUI.Label(new Rect(boardRect.x + 165f, boardRect.y + 34f, 52f, 26f), "VS", centerStyle);
    }

    void DrawRoundMessage()
    {
        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontSize = 24;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;

        GUIStyle shadowStyle = new GUIStyle(textStyle);
        shadowStyle.normal.textColor = new Color(0f, 0f, 0f, 0.4f);

        Rect pillRect = new Rect((Screen.width - 300f) * 0.5f, 112f, 300f, 40f);

        Color previousColor = GUI.color;
        GUI.color = new Color(0.08f, 0.12f, 0.18f, 0.78f);
        GUI.DrawTexture(pillRect, Texture2D.whiteTexture);
        GUI.color = previousColor;

        GUI.Label(new Rect(pillRect.x + 1f, pillRect.y + 2f, pillRect.width, pillRect.height), roundMessage, shadowStyle);
        GUI.Label(pillRect, roundMessage, textStyle);
    }

    void DrawCountdownOverlay()
    {
        string countdownText = countdownTimer > 2f ? "3" : countdownTimer > 1f ? "2" : countdownTimer > 0.1f ? "1" : "GO";

        GUIStyle countdownStyle = new GUIStyle(GUI.skin.label);
        countdownStyle.alignment = TextAnchor.MiddleCenter;
        countdownStyle.fontSize = 72;
        countdownStyle.fontStyle = FontStyle.Bold;
        countdownStyle.normal.textColor = Color.white;

        GUIStyle shadowStyle = new GUIStyle(countdownStyle);
        shadowStyle.normal.textColor = new Color(0f, 0f, 0f, 0.35f);

        Rect rect = new Rect((Screen.width - 220f) * 0.5f, (Screen.height - 120f) * 0.5f, 220f, 120f);
        GUI.Label(new Rect(rect.x + 4f, rect.y + 4f, rect.width, rect.height), countdownText, shadowStyle);
        GUI.Label(rect, countdownText, countdownStyle);
    }

    void DrawWinnerOverlay()
    {
        Rect panelRect = new Rect((Screen.width - 480f) * 0.5f, (Screen.height - 270f) * 0.5f, 480f, 270f);

        Color previousColor = GUI.color;
        GUI.color = new Color(0.03f, 0.05f, 0.08f, 0.54f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = new Color(0.97f, 0.97f, 0.94f, 0.98f);
        GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
        GUI.color = new Color(0.91f, 0.65f, 0.16f, 1f);
        GUI.DrawTexture(new Rect(panelRect.x, panelRect.y, panelRect.width, 10f), Texture2D.whiteTexture);
        GUI.color = previousColor;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 32;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = new Color(0.1f, 0.16f, 0.24f);

        GUIStyle infoStyle = new GUIStyle(GUI.skin.label);
        infoStyle.alignment = TextAnchor.MiddleCenter;
        infoStyle.fontSize = 16;
        infoStyle.normal.textColor = new Color(0.34f, 0.39f, 0.47f);

        GUI.Label(new Rect(panelRect.x + 24f, panelRect.y + 38f, panelRect.width - 48f, 40f), winnerMessage, titleStyle);
        GUI.Label(new Rect(panelRect.x + 24f, panelRect.y + 86f, panelRect.width - 48f, 24f), "Final Score  " + leftScore + " : " + rightScore, infoStyle);
        GUI.Label(new Rect(panelRect.x + 24f, panelRect.y + 112f, panelRect.width - 48f, 24f), "First to 5 points wins the match.", infoStyle);

        DrawOverlayButton(new Rect(panelRect.x + 48f, panelRect.y + 178f, 170f, 46f), "Play Again", new Color(0.16f, 0.62f, 0.4f, 1f), RestartGame);
        DrawOverlayButton(new Rect(panelRect.x + 262f, panelRect.y + 178f, 170f, 46f), "Mode Select", new Color(0.17f, 0.43f, 0.84f, 1f), BackToMenu);
    }

    void DrawOverlayButton(Rect rect, string text, Color color, System.Action onClick)
    {
        Event currentEvent = Event.current;
        bool hovered = rect.Contains(currentEvent.mousePosition);

        Color previousColor = GUI.color;
        GUI.color = hovered ? color * 1.06f : color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;

        GUIStyle buttonLabelStyle = new GUIStyle(GUI.skin.label);
        buttonLabelStyle.alignment = TextAnchor.MiddleCenter;
        buttonLabelStyle.fontSize = 18;
        buttonLabelStyle.fontStyle = FontStyle.Bold;
        buttonLabelStyle.normal.textColor = Color.white;

        GUI.Label(rect, text, buttonLabelStyle);

        if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            onClick();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameplaySceneName);
    }
}
