using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayManager
{
    public class Manager_Game : MonoBehaviour
    {
        public event Action<GameState> OnChangeGameState;
        public static Manager_Game instance;
        public GameState currentGameState { get; private set; }
        public enum GameState
        {
            UI,
            Gameplay,
            Dialogue
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log($"<b>Game Manager is more than one</b>: delete {gameObject} ");
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            SetGameState(GameState.Gameplay);
        }

        public void SetGameState(GameState gameState)
        {
            currentGameState = gameState;
            Debug.Log($"Game State Change to new State: {currentGameState}");
            OnChangeGameState?.Invoke(currentGameState);
        }
    }
}
