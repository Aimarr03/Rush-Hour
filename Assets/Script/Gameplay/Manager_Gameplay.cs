using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay_RoadLogic;
using System.Linq;
using Unity.VisualScripting;

namespace GameplayManager
{
    public class Manager_Gameplay : MonoBehaviour
    {
        private Manager_Game.GameState state => Manager_Game.GameState.Gameplay;
        private bool canBeInterracted;
        
        private List<Gameplay_TrafficLight>trafficLights;
        private Gameplay_TrafficLight currentTrafficLight;
        private int index = 0;
        private int maxIndex => trafficLights.Count;
        
        private void Awake()
        {
            trafficLights = FindObjectsOfType<Gameplay_TrafficLight>().ToList();
            foreach(Gameplay_TrafficLight trafficLight in trafficLights)
            {
                trafficLight.State_Defocused();
            }
            SortTrafficLights();
        }
        private void SortTrafficLights()
        {
            trafficLights = trafficLights.OrderBy(light => light.transform.position.x)
                                         .ThenBy(light => light.transform.position.y)
                                         .ToList();
        }
        private void Start()
        {
            //Manager_Game.instance.OnChangeGameState += Instance_OnChangeGameState;
            Manager_Input.Event_Navigation += Manager_Input_Event_Navigation;
            Manager_Input.Event_Interract += Manager_Input_Event_Interract;
            Manager_Input.Event_Negate += Manager_Input_Event_Negate;
            
            Manager_Game.instance.SetGameState(state);
        }
        private void OnDisable()
        {
            //Manager_Game.instance.OnChangeGameState -= Instance_OnChangeGameState;
            Manager_Input.Event_Navigation -= Manager_Input_Event_Navigation;
            Manager_Input.Event_Interract -= Manager_Input_Event_Interract;
            Manager_Input.Event_Negate -= Manager_Input_Event_Negate;
        }

        private void Manager_Input_Event_Navigation(float read_value)
        {
            if (!CheckState()) return;
            if(currentTrafficLight == null)
            {
                currentTrafficLight = trafficLights[index];
                currentTrafficLight.State_Focused();
                return;
            }

            int filtered_read_value = (int)read_value;
            int bufferedIndex = index + filtered_read_value;
            if(bufferedIndex >= 0 && bufferedIndex < maxIndex)
            {
                index = bufferedIndex;
                currentTrafficLight?.State_Defocused();
                currentTrafficLight = trafficLights[bufferedIndex];
                currentTrafficLight.State_Focused();
            }
            Debug.Log($"<b>Gameplay\t</b>: On Navigate New Traffic Light Focused: {currentTrafficLight.gameObject.name}");
        }
        private void Manager_Input_Event_Interract(Manager_Input.PressedState interractionState)
        {
            if (!CheckState() || currentTrafficLight == null)
            {
                return;
            }
            switch (interractionState)
            {
                case Manager_Input.PressedState.Tap:
                    currentTrafficLight.State_Tap();
                    break;
                case Manager_Input.PressedState.Hold:
                    currentTrafficLight.State_Hold();
                    break;
                case Manager_Input.PressedState.HoldCancel:
                    currentTrafficLight.State_CancelledHold();
                    break;
            }
        }
        private void Manager_Input_Event_Negate()
        {
            Manager_Game.instance.SetGameState(Manager_Game.GameState.UI);
        }
        private bool CheckState() => state == Manager_Game.instance.currentGameState;
    }
}

