using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Serializes and deserializes the player's world position and rotation.
    /// </summary>
    public class PlayerTransformSave : MonoBehaviour, ISaveable
    {
        // Registers with the save manager when enabled
        private void OnEnable()
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

        // Records the current transform state
        public void Save(GameSaveData data)
        {
            data.playerPosition = transform.position;
            data.playerRotation = transform.rotation;
        }

        // Restores position and rotation, using Rigidbody when physics is present
        public void Load(GameSaveData data)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = data.playerPosition;
                rb.rotation = data.playerRotation;
            }

            transform.position = data.playerPosition;
            transform.rotation = data.playerRotation;

            // Forces Unity's physics engine to instantly accept the new position
            Physics.SyncTransforms();
        }
    }
}