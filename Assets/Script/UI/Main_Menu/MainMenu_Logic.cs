using GameplayManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MainMenuManager
{
    public class MainMenu_Logic : MonoBehaviour
    {
        public Manager_Game.GameState state => Manager_Game.GameState.UI;
        public List<MainMenu_Button> List_MainMenuButton;

        public List<MainMenu_Button> CurrentList_NavigationButtons;

        private MainMenu_Button currentButton;
        private int currentIndex = 0;
        public int Max_ListCount => CurrentList_NavigationButtons == null? 0 : CurrentList_NavigationButtons.Count;
        private void Awake()
        {
            
        }
        private void Start()
        {
            Manager_Game.instance?.SetGameState(state);
            CurrentList_NavigationButtons = List_MainMenuButton;

            Manager_Input.Event_Interract += Manager_Input_Event_Interract;
            Manager_Input.Event_Navigation += Manager_Input_Event_Navigation;
        }
        private void OnDisable()
        {
            Manager_Input.Event_Interract -= Manager_Input_Event_Interract;
            Manager_Input.Event_Navigation -= Manager_Input_Event_Navigation;
        }
        
        private void Manager_Input_Event_Navigation(float value)
        {
            if(!CheckState()) return;
            if(currentButton == null)
            {
                currentButton = CurrentList_NavigationButtons[currentIndex];
                currentButton.State_Focused();
                return;
            }
            
            int bufferIndex = currentIndex + (int)value;
            if (bufferIndex < 0) bufferIndex = Max_ListCount;
            else if (bufferIndex > Max_ListCount) bufferIndex = 0;
            
            currentIndex = bufferIndex;
            currentButton?.State_Defocused();
            currentButton = CurrentList_NavigationButtons[currentIndex];
            currentButton.State_Focused();
        }

        private void Manager_Input_Event_Interract(Manager_Input.PressedState obj)
        {
            if (!CheckState()) return;
            currentButton?.State_Tap();
        }
        private bool CheckState() => Manager_Game.instance?.currentGameState == state;
    }
}

