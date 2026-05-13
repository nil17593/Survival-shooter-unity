using System;
using System.IO;
using UnityEngine;

public class JsonSaveRepository
{
    readonly string filePath;

    public JsonSaveRepository(string fileName)
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    public bool SaveToFile(GameSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to save game data: " + exception.Message);
            return false;
        }
    }

    public GameSaveData LoadFromFile()
    {
        if (!HasSave())
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

            if (data == null)
            {
                Debug.LogWarning("Save file could not be read.");
                return null;
            }

            return data;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to load save game data: " + exception.Message);
            return null;
        }
    }

    public bool HasSave()
    {
        return File.Exists(filePath);
    }

    public bool DeleteSave()
    {
        if (!HasSave())
        {
            return true;
        }

        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to delete save game data: " + exception.Message);
            return false;
        }
    }
}
