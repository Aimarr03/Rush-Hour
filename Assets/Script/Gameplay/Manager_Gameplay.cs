using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayManager
{
    public class Manager_Gameplay : MonoBehaviour
    {
        private Manager_Game.GameState state => Manager_Game.GameState.Gameplay;
        private bool canBeInterracted;
        
        private Gameplay_TrafficLight[] trafficLights;
        private Gameplay_TrafficLight currentTrafficLight;
        private int index = 0;
        private int maxIndex => trafficLights.Length;
        
        private void Awake()
        {
            trafficLights = FindObjectsOfType<Gameplay_TrafficLight>();
        }
        private void Start()
        {
            //Manager_Game.instance.OnChangeGameState += Instance_OnChangeGameState;
            Manager_Input.Event_Navigation += Manager_Input_Event_Navigation;
            Manager_Input.Event_Interract += Manager_Input_Event_Interract;
        }
        private void OnDisable()
        {
            //Manager_Game.instance.OnChangeGameState -= Instance_OnChangeGameState;
            Manager_Input.Event_Navigation -= Manager_Input_Event_Navigation;
            Manager_Input.Event_Interract -= Manager_Input_Event_Interract;
        }

        private void Manager_Input_Event_Navigation(float obj)
        {
            if (!CheckState()) return;
            //Debug.Log("<b>Gameplay\t</b>: On Navigate in Manager_Gameplay");
        }
        private void Manager_Input_Event_Interract(Manager_Input.PressedState obj)
        {
            if (!CheckState()) return;
            //Debug.Log("<b>Gameplay\t</b>: On Interract in Manager_Gameplay");
        }
        private bool CheckState() => state == Manager_Game.instance.currentGameState;
    }
}

