using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Data_Logic
{
    public class GameData
    {
        public class LevelData
        {
            public int score;
            public int starRating;
        }
        public bool newGame;
        public Dictionary<string, LevelData> List_LevelData;

        public GameData()
        {
            newGame = true;
            List_LevelData = new Dictionary<string, LevelData>();
        }
    }
    
}

