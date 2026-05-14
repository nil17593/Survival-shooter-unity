using UnityEngine;
using SaveSystem;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the Load Game Button here to disable it if no save exists.")]
    public GameObject loadGameButton;

    [Header("Scene Settings")]
    [Tooltip("The exact name of your gameplay scene in the Build Settings.")]
    public string gameplaySceneName = "YourGameplaySceneNameHere";

    void Start()
    {
        Time.timeScale = 1f;

        if (SaveLoadManager.Instance != null)
        {
            loadGameButton.SetActive(SaveLoadManager.Instance.HasSave());
        }
        else
        {
            Debug.LogWarning("SaveLoadManager is missing! Ensure your persistent manager object is in this scene.");
        }
    }

    public void StartNewGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            if (SaveLoadManager.Instance.HasSave())
            {
                SaveLoadManager.Instance.DeleteSave();
            }
        }
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void LoadSavedGame()
    {
        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.HasSave())
        {
            SaveLoadManager.Instance.LoadGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
