using GameplayManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay_Pause_LogicHandler : MonoBehaviour
{
    public Manager_Game.GameState state => Manager_Game.GameState.UI;
    private int currentIndex = 0;
    private int MaxIndex => Current_TargetList.Count;
    private Gameplay_Pause_Button currentButton;

    private List<Gameplay_Pause_Button> Current_TargetList;

    public List<Gameplay_Pause_Button> MainList;
    public Transform Content_Container;

    private void Start()
    {
        Manager_Game.instance.OnChangeGameState += Instance_OnChangeGameState;

        Manager_Input.Event_Interract += Manager_Input_Event_Interract;
        Manager_Input.Event_Navigation += Manager_Input_Event_Navigation;
        Manager_Input.Event_Negate += Manager_Input_Event_Negate;
    }

    

    private void OnDisable()
    {
        Manager_Game.instance.OnChangeGameState -= Instance_OnChangeGameState;

        Manager_Input.Event_Interract -= Manager_Input_Event_Interract;
        Manager_Input.Event_Navigation -= Manager_Input_Event_Navigation;
        Manager_Input.Event_Negate -= Manager_Input_Event_Negate;
    }
    private void HandleScreenPopUp()
    {
        if (CheckGameState())
        {
            Content_Container.gameObject.SetActive(true);
        }
        else
        {
            Content_Container.gameObject.SetActive(false);
        }
        currentIndex = 0;
        currentButton?.State_Defocused();
        Current_TargetList = MainList;
        currentButton = null;
    }
    #region Events Logic
    private void Manager_Input_Event_Negate(Manager_Game.GameState gameState)
    {
        if (!CheckGameState(gameState)) return;
        Debug.Log("Negate UI");
        Manager_Game.instance.SetGameState(Manager_Game.GameState.Gameplay);
    }

    private void Manager_Input_Event_Navigation(float obj)
    {
        if (!CheckGameState()) return;
        if (currentButton == null)
        {
            currentButton = Current_TargetList[currentIndex];
            currentButton.State_Focused();
            return;
        }
        int input = (int)obj;
        int bufferIndex = input + currentIndex;
        if(bufferIndex >= 0 && bufferIndex < MaxIndex)
        {
            currentIndex = bufferIndex;
            currentButton?.State_Defocused();
            currentButton = Current_TargetList[currentIndex];
            currentButton?.State_Focused();
        }
    }

    private void Manager_Input_Event_Interract(Manager_Input.PressedState obj)
    {
        if (!CheckGameState()) return;
        currentButton?.State_Tap();
    }
    private void Instance_OnChangeGameState(Manager_Game.GameState obj)
    {
        HandleScreenPopUp();
    }
    #endregion
    public bool CheckGameState() => Manager_Game.instance.currentGameState == state;
    public bool CheckGameState(Manager_Game.GameState gameState) => Manager_Game.instance.currentGameState == gameState;
}
