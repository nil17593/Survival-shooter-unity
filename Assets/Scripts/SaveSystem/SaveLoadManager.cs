using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveSystem
{
    /// <summary>
    /// Singleton manager that coordinates the saving, loading, and registration of ISaveable objects.
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private string saveFileName = "savegame.json";
        #endregion

        #region Static Properties
        public static bool IsQuitting { get; private set; } = false;
        public static SaveLoadManager Instance { get; private set; }

        private static GameSaveData pendingLoadData;
        #endregion

        #region Private Fields
        private readonly List<ISaveable> saveables = new List<ISaveable>();
        private JsonSaveRepository repository;
        #endregion

        // Initializes the singleton and repository
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            IsQuitting = false;
            Instance = this;
            DontDestroyOnLoad(gameObject);

            repository = new JsonSaveRepository(saveFileName);

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        // Auto-loads an existing save file on game launch
        private void Start()
        {
            if (HasSave())
            {
                Debug.Log("Found existing save file. Auto-loading on startup...");
                LoadGame();
            }
        }

        // Cleans up event subscriptions and singleton reference
        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            pendingLoadData = null;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            Instance = null;
        }

        // Registers an object to be included in the save state
        public void Register(ISaveable saveable)
        {
            if (!saveables.Contains(saveable))
            {
                saveables.Add(saveable);
            }
        }

        // Removes an object from the save state
        public void Unregister(ISaveable saveable)
        {
            if (saveables.Contains(saveable))
            {
                saveables.Remove(saveable);
            }
        }

        // Gathers data from all registered objects and writes to disk
        public bool SaveGame()
        {
            PlayerHealth player = FindAnyObjectByType<PlayerHealth>();

            if (player != null && player.CurrentHealth <= 0)
            {
                Debug.LogWarning("Save aborted: Cannot save the game while the player is dead.");
                return false;
            }
            GameSaveData data = CreateSaveData();

            RemoveMissingSaveables();

            foreach (ISaveable saveable in saveables)
            {
                saveable.Save(data);
            }

            bool saved = repository.SaveToFile(data);

            if (saved)
            {
                Debug.Log("Game saved successfully.");
            }
            else
            {
                Debug.LogWarning("Save failed.");
            }

            return saved;
        }

        // Reads data from disk and triggers the scene load
        public bool LoadGame()
        {
            if (!repository.HasSave())
            {
                Debug.LogWarning("Load failed: no save file found.");
                return false;
            }

            GameSaveData data = repository.LoadFromFile();

            if (data == null)
            {
                Debug.LogWarning("Load failed: save data is missing or invalid.");
                return false;
            }

            if (data.saveVersion != GameSaveData.CurrentSaveVersion)
            {
                Debug.LogWarning("Load failed: incompatible save version.");
                return false;
            }

            string sceneToLoad = string.IsNullOrEmpty(data.sceneName)
                ? SceneManager.GetActiveScene().name
                : data.sceneName;

            if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
            {
                pendingLoadData = null;
                Debug.LogWarning("Load failed: Scene '" + sceneToLoad + "' is not in the Build Settings.");
                return false;
            }

            pendingLoadData = data;
            SceneManager.LoadScene(sceneToLoad);
            return true;
        }

        // Returns true if a save file exists
        public bool HasSave()
        {
            return repository.HasSave();
        }

        // Deletes the current save file
        public bool DeleteSave()
        {
            return repository.DeleteSave();
        }

        private void OnApplicationQuit()
        {
            IsQuitting = true;
        }

        // Instantiates the base save data object with current scene and timestamp
        private GameSaveData CreateSaveData()
        {
            GameSaveData data = new GameSaveData();
            data.saveVersion = GameSaveData.CurrentSaveVersion;
            data.savedAt = DateTime.UtcNow.ToString("o");
            data.sceneName = SceneManager.GetActiveScene().name;
            return data;
        }

        // Intercepts the scene load to distribute pending save data
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (pendingLoadData == null)
            {
                return;
            }

            GameSaveData data = pendingLoadData;
            pendingLoadData = null;

            StartCoroutine(LoadDataAfterRegistration(data));
        }

        // Waits one frame to ensure objects run OnEnable before receiving load data
        private IEnumerator LoadDataAfterRegistration(GameSaveData data)
        {
            yield return new WaitForEndOfFrame();

            RemoveMissingSaveables();

            foreach (ISaveable saveable in saveables)
            {
                saveable.Load(data);
            }

            Debug.Log("Game loaded successfully.");
        }

        // Removes destroyed scene objects left behind by scene reloads before save/load iteration
        private void RemoveMissingSaveables()
        {
            saveables.RemoveAll(saveable =>
            {
                if (saveable == null)
                {
                    return true;
                }

                if (saveable is UnityEngine.Object unityObject)
                {
                    return unityObject == null;
                }

                return false;
            });
        }
    }
}