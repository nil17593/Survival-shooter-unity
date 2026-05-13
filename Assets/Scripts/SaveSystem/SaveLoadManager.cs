using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] string saveFileName = "savegame.json";

    static GameSaveData pendingLoadData;

    JsonSaveRepository repository;

    void Awake()
    {
        repository = new JsonSaveRepository(saveFileName);
    }

    public bool SaveGame()
    {
        GameSaveData data = CreateSaveData();
        List<ISaveable> saveables = DiscoverSaveables();

        foreach (ISaveable saveable in saveables)
        {
            saveable.Save(data);
        }

        return repository.SaveToFile(data);
    }

    public bool LoadGame()
    {
        GameSaveData data = repository.LoadFromFile();

        if (data == null)
        {
            return false;
        }

        pendingLoadData = data;
        SceneManager.sceneLoaded -= RestoreAfterSceneLoaded;
        SceneManager.sceneLoaded += RestoreAfterSceneLoaded;

        string sceneToLoad = string.IsNullOrEmpty(data.sceneName)
            ? SceneManager.GetActiveScene().name
            : data.sceneName;

        try
        {
            SceneManager.LoadScene(sceneToLoad);
            return true;
        }
        catch (Exception exception)
        {
            SceneManager.sceneLoaded -= RestoreAfterSceneLoaded;
            pendingLoadData = null;
            Debug.LogWarning("Failed to load saved scene: " + exception.Message);
            return false;
        }
    }

    public bool HasSave()
    {
        return repository.HasSave();
    }

    public bool DeleteSave()
    {
        return repository.DeleteSave();
    }

    GameSaveData CreateSaveData()
    {
        return new GameSaveData
        {
            saveVersion = GameSaveData.CurrentSaveVersion,
            savedAt = DateTime.UtcNow.ToString("o"),
            sceneName = SceneManager.GetActiveScene().name
        };
    }

    static void RestoreAfterSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= RestoreAfterSceneLoaded;

        if (pendingLoadData == null)
        {
            return;
        }

        GameSaveData data = pendingLoadData;
        pendingLoadData = null;

        List<ISaveable> saveables = DiscoverSaveables();

        foreach (ISaveable saveable in saveables)
        {
            saveable.Load(data);
        }
    }

    static List<ISaveable> DiscoverSaveables()
    {
        var saveables = new List<ISaveable>();
        MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>(true);

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is ISaveable saveable)
            {
                saveables.Add(saveable);
            }
        }

        return saveables;
    }
}
