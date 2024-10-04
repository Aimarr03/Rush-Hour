
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Data_Logic
{
    public class Manager_Data : MonoBehaviour
    {
        [SerializeField] private string directivePath;
        [SerializeField] private string fileName;
        public static Manager_Data instance;
        public GameData gameData;
        private FileHandler_Logic fileHandler;
        List<I_DataPersistance> ListDataPersistance;
        public void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("There is one more! Destroy newest one");
                Destroy(this.gameObject);
                return;
            }
            instance = this;
            ListDataPersistance = new List<I_DataPersistance>();
            fileHandler = new FileHandler_Logic(Application.persistentDataPath, fileName);
            DontDestroyOnLoad(this.gameObject);
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ListDataPersistance = GetListDataPersistance();
            LoadGame();
        }

        private List<I_DataPersistance> GetListDataPersistance()
        {
            IEnumerable<I_DataPersistance> ListDataPersistance = FindObjectsOfType<MonoBehaviour>().OfType<I_DataPersistance>();
            return new List<I_DataPersistance>(ListDataPersistance);
        }

        public void NewGame()
        {
            gameData = new GameData();
            Debug.Log("Created new Game Data");
            fileHandler.NewData();
            fileHandler.SaveData(gameData);
        }
        public void LoadGame()
        {
            gameData = fileHandler.LoadData();
            if (gameData != null)
            {
                foreach (I_DataPersistance iDataPersistance in ListDataPersistance)
                {
                    iDataPersistance.LoadData(gameData);
                }
                return;
            }
            else
            {
                Debug.LogWarning("No Data was Found!");
            }
        }
        public void SaveGame()
        {
            if (gameData == null)
            {
                Debug.LogWarning("No data was found!");
                return;
            }
            foreach (I_DataPersistance iDataPersistance in ListDataPersistance)
            {
                iDataPersistance.SaveData(ref gameData);
            }
            fileHandler.SaveData(gameData);
        }

        public bool HasGameData()
        {
            return gameData != null;
        }
    }
}

