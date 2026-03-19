using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const string GameplaySceneName = "SampleScene";

    public void PlayVsAI()
    {
        GameSettings.EnsureInstance().playVsAI = true;
        SceneManager.LoadScene(GameplaySceneName);
    }

    public void Play2Players()
    {
        GameSettings.EnsureInstance().playVsAI = false;
        SceneManager.LoadScene(GameplaySceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
