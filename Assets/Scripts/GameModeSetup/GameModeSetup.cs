using UnityEngine;

public class GameModeSetup : MonoBehaviour
{
    public static bool IsSelectionVisible { get; private set; }

    [Header("References")]
    public GameObject rightPlayer;
    public Player2Movement player2Movement;
    public SimpleAIBot aiBot;
    public PlayerBallControl rightPlayerBallControl;

    [Header("Mode Select")]
    public bool showModeSelectionOnStart = true;

    private bool showSelection;
    private Rect panelRect = new Rect(0f, 0f, 760f, 400f);

    private Texture2D whiteTex;

    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle controlsStyle;
    private GUIStyle keyStyle;
    private GUIStyle cardTitleStyle;
    private GUIStyle cardSubtitleStyle;
    private GUIStyle footerStyle;
    private GUIStyle badgeStyle;

    private readonly Color overlayColor = new Color(0.05f, 0.08f, 0.12f, 0.48f);
    private readonly Color panelBg = new Color(0.95f, 0.97f, 1f, 0.98f);
    private readonly Color panelInner = new Color(0.98f, 0.99f, 1f, 1f);
    private readonly Color borderDark = new Color(0.14f, 0.18f, 0.25f, 1f);
    private readonly Color borderLight = new Color(0.80f, 0.88f, 0.98f, 1f);
    private readonly Color textMain = new Color(0.10f, 0.15f, 0.24f, 1f);
    private readonly Color textSoft = new Color(0.37f, 0.45f, 0.58f, 1f);

    private readonly Color greenMain = new Color(0.18f, 0.95f, 0.50f, 1f);
    private readonly Color greenDark = new Color(0.06f, 0.44f, 0.22f, 1f);
    private readonly Color greenTint = new Color(0.94f, 1.00f, 0.96f, 1f);

    private readonly Color blueMain = new Color(0.24f, 0.58f, 1.00f, 1f);
    private readonly Color blueDark = new Color(0.08f, 0.24f, 0.56f, 1f);
    private readonly Color blueTint = new Color(0.94f, 0.97f, 1f, 1f);

    private readonly Color shadowColor = new Color(0f, 0f, 0f, 0.16f);
    private readonly Color lineGlow = new Color(0.30f, 0.54f, 0.90f, 0.12f);

    void Awake()
    {
        if (rightPlayer == null)
            rightPlayer = GameObject.Find("Player2");

        if (rightPlayer != null)
        {
            if (player2Movement == null)
                player2Movement = rightPlayer.GetComponent<Player2Movement>();

            if (aiBot == null)
                aiBot = rightPlayer.GetComponent<SimpleAIBot>();

            if (rightPlayerBallControl == null)
                rightPlayerBallControl = rightPlayer.GetComponent<PlayerBallControl>();
        }

    }

    void Start()
    {
        GameSettings.EnsureInstance();

        if (showModeSelectionOnStart)
        {
            showSelection = true;
            IsSelectionVisible = true;
            DisableRightPlayerControllers();
            Time.timeScale = 0f;
        }
        else
        {
            IsSelectionVisible = false;
            ApplyMode(GameSettings.Instance.playVsAI);
        }
    }

    void Update()
    {
        if (!showSelection)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            SelectVsAI();

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            SelectTwoPlayers();
    }

    void EnsureGUIInitialized()
    {
        if (titleStyle != null)
            return;

        whiteTex = Texture2D.whiteTexture;

        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 40;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = textMain;

        subtitleStyle = new GUIStyle(GUI.skin.label);
        subtitleStyle.alignment = TextAnchor.MiddleCenter;
        subtitleStyle.fontSize = 17;
        subtitleStyle.fontStyle = FontStyle.Normal;
        subtitleStyle.normal.textColor = textSoft;

        controlsStyle = new GUIStyle(GUI.skin.label);
        controlsStyle.alignment = TextAnchor.MiddleCenter;
        controlsStyle.fontSize = 13;
        controlsStyle.fontStyle = FontStyle.Bold;
        controlsStyle.normal.textColor = textMain;

        keyStyle = new GUIStyle(GUI.skin.label);
        keyStyle.alignment = TextAnchor.MiddleCenter;
        keyStyle.fontSize = 26;
        keyStyle.fontStyle = FontStyle.Bold;
        keyStyle.normal.textColor = Color.white;

        cardTitleStyle = new GUIStyle(GUI.skin.label);
        cardTitleStyle.alignment = TextAnchor.MiddleLeft;
        cardTitleStyle.fontSize = 25;
        cardTitleStyle.fontStyle = FontStyle.Bold;
        cardTitleStyle.normal.textColor = textMain;

        cardSubtitleStyle = new GUIStyle(GUI.skin.label);
        cardSubtitleStyle.alignment = TextAnchor.MiddleLeft;
        cardSubtitleStyle.fontSize = 13;
        cardSubtitleStyle.fontStyle = FontStyle.Normal;
        cardSubtitleStyle.normal.textColor = textSoft;

        footerStyle = new GUIStyle(GUI.skin.label);
        footerStyle.alignment = TextAnchor.MiddleCenter;
        footerStyle.fontSize = 12;
        footerStyle.fontStyle = FontStyle.Bold;
        footerStyle.normal.textColor = textSoft;

        badgeStyle = new GUIStyle(GUI.skin.label);
        badgeStyle.alignment = TextAnchor.MiddleCenter;
        badgeStyle.fontSize = 11;
        badgeStyle.fontStyle = FontStyle.Bold;
        badgeStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        if (!showSelection)
            return;

        EnsureGUIInitialized();

        panelRect.width = 760f;
        panelRect.height = 400f;
        panelRect.x = Mathf.Floor((Screen.width - panelRect.width) * 0.5f);
        panelRect.y = Mathf.Floor((Screen.height - panelRect.height) * 0.5f);

        DrawOverlay();
        DrawMainPanel();
        DrawHeader();
        DrawControlsBar();

        DrawModeCard(
            new Rect(panelRect.x + 42f, panelRect.y + 196f, 308f, 138f),
            "1",
            "VS AI",
            "Quick solo arcade match",
            "SOLO",
            greenMain,
            greenDark,
            greenTint,
            true,
            SelectVsAI
        );

        DrawModeCard(
            new Rect(panelRect.x + 410f, panelRect.y + 196f, 308f, 138f),
            "2",
            "2 PLAYERS",
            "Local versus on one keyboard",
            "LOCAL",
            blueMain,
            blueDark,
            blueTint,
            false,
            SelectTwoPlayers
        );

        GUI.Label(
            new Rect(panelRect.x + 20f, panelRect.y + 354f, panelRect.width - 40f, 22f),
            "Press 1 or 2 to start",
            footerStyle
        );
    }

    void DrawOverlay()
    {
        DrawRect(new Rect(0f, 0f, Screen.width, Screen.height), overlayColor);
        DrawRect(new Rect(0f, 0f, Screen.width, Screen.height * 0.30f), new Color(1f, 1f, 1f, 0.05f));
    }

    void DrawMainPanel()
    {
        Rect shadow = new Rect(panelRect.x + 10f, panelRect.y + 12f, panelRect.width, panelRect.height);
        DrawRect(shadow, shadowColor);

        DrawPixelPanel(panelRect, panelBg, borderLight, borderDark);

        Rect inner = new Rect(panelRect.x + 8f, panelRect.y + 8f, panelRect.width - 16f, panelRect.height - 16f);
        DrawRect(inner, panelInner);

        DrawRect(new Rect(panelRect.x + 16f, panelRect.y + 16f, panelRect.width - 32f, 6f), new Color(0.88f, 0.93f, 1f, 1f));
        DrawRect(new Rect(panelRect.x + 24f, panelRect.y + 28f, 130f, 6f), greenMain);
        DrawRect(new Rect(panelRect.x + panelRect.width - 154f, panelRect.y + 28f, 130f, 6f), blueMain);
        DrawRect(new Rect(panelRect.x + 24f, panelRect.y + panelRect.height - 34f, panelRect.width - 48f, 4f), new Color(0.86f, 0.90f, 0.98f, 1f));
    }

    void DrawHeader()
    {
        GUI.Label(
            new Rect(panelRect.x + 20f, panelRect.y + 46f, panelRect.width - 40f, 42f),
            "PIXEL HOOPS",
            titleStyle
        );

        GUI.Label(
            new Rect(panelRect.x + 20f, panelRect.y + 92f, panelRect.width - 40f, 24f),
            "Choose solo AI or local versus and start the match.",
            subtitleStyle
        );
    }

    void DrawControlsBar()
    {
        Rect infoRect = new Rect(panelRect.x + 52f, panelRect.y + 132f, panelRect.width - 104f, 42f);
        DrawPixelPanel(infoRect, new Color(0.97f, 0.98f, 1f, 1f), borderLight, borderDark);

        DrawRect(new Rect(infoRect.x + 10f, infoRect.y + 10f, 8f, infoRect.height - 20f), greenMain);
        DrawRect(new Rect(infoRect.x + infoRect.width - 18f, infoRect.y + 10f, 8f, infoRect.height - 20f), blueMain);

        GUI.Label(
            new Rect(infoRect.x + 24f, infoRect.y + 10f, infoRect.width - 48f, 22f),
            "P1  A D MOVE   W JUMP   SPACE THROW      P2  ARROWS MOVE   ENTER THROW",
            controlsStyle
        );
    }

    void DrawModeCard(
        Rect rect,
        string keyText,
        string title,
        string subtitle,
        string badgeText,
        Color mainColor,
        Color darkColor,
        Color tintColor,
        bool greenSide,
        System.Action onClick
    )
    {
        Rect shadow = new Rect(rect.x + 6f, rect.y + 8f, rect.width, rect.height);
        DrawRect(shadow, shadowColor);

        DrawPixelPanel(rect, tintColor, borderLight, borderDark);

        Rect inner = new Rect(rect.x + 6f, rect.y + 6f, rect.width - 12f, rect.height - 12f);
        DrawRect(inner, new Color(1f, 1f, 1f, 0.90f));

        if (greenSide)
        {
            DrawRect(new Rect(rect.x + 6f, rect.y + 6f, 10f, rect.height - 12f), mainColor);
            DrawRect(new Rect(rect.x + rect.width - 18f, rect.y + 6f, 12f, rect.height - 12f), darkColor);
        }
        else
        {
            DrawRect(new Rect(rect.x + 6f, rect.y + 6f, 10f, rect.height - 12f), darkColor);
            DrawRect(new Rect(rect.x + rect.width - 18f, rect.y + 6f, 12f, rect.height - 12f), mainColor);
        }

        Rect keyBox = new Rect(rect.x + 22f, rect.y + 28f, 58f, 58f);
        DrawPixelPanel(keyBox, mainColor, new Color(1f, 1f, 1f, 0.18f), borderDark);
        GUI.Label(new Rect(keyBox.x, keyBox.y + 3f, keyBox.width, keyBox.height), keyText, keyStyle);

        Rect badgeRect = new Rect(rect.x + rect.width - 88f, rect.y + 18f, 60f, 24f);
        DrawPixelPanel(badgeRect, mainColor, new Color(1f, 1f, 1f, 0.12f), borderDark);
        GUI.Label(badgeRect, badgeText, badgeStyle);

        GUI.Label(
            new Rect(rect.x + 100f, rect.y + 30f, rect.width - 198f, 30f),
            title,
            cardTitleStyle
        );

        GUI.Label(
            new Rect(rect.x + 100f, rect.y + 66f, rect.width - 128f, 24f),
            subtitle,
            cardSubtitleStyle
        );

        DrawRect(new Rect(rect.x + 22f, rect.y + 104f, rect.width - 44f, 6f), new Color(0.87f, 0.90f, 0.96f, 1f));
        DrawRect(new Rect(rect.x + 22f, rect.y + 114f, rect.width - 44f, 8f), mainColor);
        DrawRect(new Rect(rect.x + 22f, rect.y + 124f, rect.width - 44f, 2f), lineGlow);

        if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            onClick();
    }

    void DrawPixelPanel(Rect rect, Color fill, Color lightBorder, Color darkBorder)
    {
        DrawRect(rect, fill);

        DrawRect(new Rect(rect.x, rect.y, rect.width, 3f), lightBorder);
        DrawRect(new Rect(rect.x, rect.y, 3f, rect.height), lightBorder);

        DrawRect(new Rect(rect.x, rect.y + rect.height - 3f, rect.width, 3f), darkBorder);
        DrawRect(new Rect(rect.x + rect.width - 3f, rect.y, 3f, rect.height), darkBorder);

        DrawRect(new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, 1f), new Color(1f, 1f, 1f, 0.06f));
    }

    void DrawRect(Rect rect, Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTex);
        GUI.color = old;
    }

    void DisableRightPlayerControllers()
    {
        if (player2Movement != null)
            player2Movement.enabled = false;

        if (aiBot != null)
            aiBot.enabled = false;

        if (rightPlayerBallControl != null)
            rightPlayerBallControl.enabled = false;
    }

    public void SelectVsAI()
    {
        ApplyMode(true);
    }

    public void SelectTwoPlayers()
    {
        ApplyMode(false);
    }

    void ApplyMode(bool playVsAI)
    {
        GameSettings.EnsureInstance().playVsAI = playVsAI;

        if (player2Movement != null)
            player2Movement.enabled = !playVsAI;

        if (aiBot != null)
            aiBot.enabled = playVsAI;

        if (rightPlayerBallControl != null)
            rightPlayerBallControl.enabled = !playVsAI;

        showSelection = false;
        IsSelectionVisible = false;
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
            GameManager.Instance.ResetRound();
    }

    void OnDisable()
    {
        IsSelectionVisible = false;
    }
}
