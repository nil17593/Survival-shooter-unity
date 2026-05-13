using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles reading, writing, and deleting JSON save data on local disk.
/// </summary>
public class JsonSaveRepository
{
    readonly string filePath;

    // Initializes the repository with a persistent file path
    public JsonSaveRepository(string fileName)
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    // Serializes the data object and writes it to disk
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

    // Reads the JSON file from disk and deserializes it
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

    // Checks if a save file currently exists on disk
    public bool HasSave()
    {
        return File.Exists(filePath);
    }

    // Deletes the save file if it exists
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