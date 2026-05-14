using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Tracks and serializes the elapsed survival time of the player.
    /// </summary>
    public class SurvivalTimer : MonoBehaviour, ISaveable
    {
        #region Serialized Fields
        [SerializeField] PlayerHealth playerHealth;
        #endregion

        #region Private Fields
        float elapsedTime;
        #endregion

        #region Public Properties
        public float ElapsedTime { get { return elapsedTime; } }
        #endregion

        // Registers with the save manager when enabled
        void OnEnable()
        {
            if (SaveLoadManager.Instance != null)
            {
                SaveLoadManager.Instance.Register(this);
            }
        }

        void OnDisable()
        {
            if (SaveLoadManager.Instance != null && !SaveLoadManager.IsQuitting)
            {
                SaveLoadManager.Instance.Unregister(this);
            }
        }

        // Validates component dependencies
        void Awake()
        {
            if (playerHealth == null)
            {
                Debug.LogError("SurvivalTimer requires a PlayerHealth reference.");
                enabled = false;
            }
        }

        // Increments timer while the player is alive
        void Update()
        {
            if (playerHealth != null && playerHealth.CurrentHealth <= 0)
            {
                return;
            }

            elapsedTime += Time.deltaTime;
        }

        // Records elapsed time to the save data
        public void Save(GameSaveData data)
        {
            data.survivalElapsedTime = elapsedTime;
        }

        // Restores elapsed time from the save data
        public void Load(GameSaveData data)
        {
            elapsedTime = Mathf.Max(0f, data.survivalElapsedTime);
        }
    }
}