using UnityEngine;

/// <summary>
/// Tracks and serializes the elapsed survival time of the player.
/// </summary>
public class SurvivalTimer : MonoBehaviour, ISaveable
{
    [SerializeField] PlayerHealth playerHealth;

    float elapsedTime;

    public float ElapsedTime { get { return elapsedTime; } }

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