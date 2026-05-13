using System;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public const int CurrentSaveVersion = 1;

    public int saveVersion = CurrentSaveVersion;
    public string savedAt;
    public string sceneName;

    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public int playerHealth;

    public int score;
    public float survivalElapsedTime;
}
