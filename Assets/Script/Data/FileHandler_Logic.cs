using System;
using System.IO;
using UnityEngine;

namespace Data_Logic
{
    public class FileHandler_Logic : MonoBehaviour
    {
        public string directoryPath;
        public string fileName;

        public FileHandler_Logic(string directoryPath, string fileName)
        {
            this.directoryPath = directoryPath;
            this.fileName = fileName;
        }

        public void SaveData(GameData gameData)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, directoryPath, fileName);
            try
            {
                if (!Directory.Exists(Path.Combine(Application.persistentDataPath, directoryPath)))
                {
                    Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, directoryPath));
                }

                string data = JsonUtility.ToJson(gameData);
                File.WriteAllText(fullPath, data);
                Debug.Log("Data saved successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data to {fullPath}. Exception: {e}");
            }
        }

        public GameData LoadData()
        {
            string fullPath = Path.Combine(Application.persistentDataPath, Path.Combine(directoryPath, fileName));
            if (File.Exists(fullPath))
            {
                try
                {
                    string data = File.ReadAllText(fullPath);
                    GameData gameData = JsonUtility.FromJson<GameData>(data);
                    Debug.Log($"Data Loaded successfully from {fullPath}");

                    return gameData;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return null;
        }
        public void NewData()
        {
            if (File.Exists(Path.Combine(directoryPath, fileName)))
            {
                File.Delete(Path.Combine(directoryPath, fileName));
            }
        }
    }
}

