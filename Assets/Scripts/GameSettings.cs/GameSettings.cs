using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public bool playVsAI = true;

    public static GameSettings EnsureInstance()
    {
        if (Instance != null)
            return Instance;

        GameObject settingsObject = new GameObject("GameSettings");
        Instance = settingsObject.AddComponent<GameSettings>();
        DontDestroyOnLoad(settingsObject);
        return Instance;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
