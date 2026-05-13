using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour, ISaveable
{

    public static int score;

    Text text;
    
    void Awake ()
    {
        text = GetComponent <Text> ();
        score = 0;
    }
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
    void Update ()
    {
        text.text = "Score: " + score;
    }
    public void Save(GameSaveData data)
    {
        data.score = score;
    }

    public void Load(GameSaveData data)
    {
        score = data.score;
        text.text = "Score: " + score;
    }
}