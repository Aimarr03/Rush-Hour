using Gameplay_RoadLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameplayManager.Manager_Game;

namespace GameplayManager
{
    public class Manager_Gameplay : MonoBehaviour
    {
        public GameplayState currentGameplayState;
        public int score = 0;
        public event Action<GameplayState> OnChangeGameplayState;
        public static Manager_Gameplay instance;
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            SetGameplayState(GameplayState.Neutral);
            Gameplay_VehicleBasic.OnReachGoal += Gameplay_VehicleBasic_OnReachGoal;
        }
        private void OnDisable()
        {
            Gameplay_VehicleBasic.OnReachGoal -= Gameplay_VehicleBasic_OnReachGoal;
        }
        private void Gameplay_VehicleBasic_OnReachGoal(Gameplay_VehicleBasic vehicle,int score)
        {
            this.score += score;
        }
        public void SetGameplayState(GameplayState state)
        {
            currentGameplayState = state;
            OnChangeGameplayState?.Invoke(currentGameplayState);
        }
    }

}
