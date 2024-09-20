using UnityEngine;
using InputManager;
using UnityEngine.InputSystem.Interactions;
using System;

public class Manager_Input : MonoBehaviour
{
    private Input_PlayerOne playerOne_Input;
    public static event Action<float> Event_Navigation;

    private void Awake()
    {
        playerOne_Input = new Input_PlayerOne();
        playerOne_Input.Enable();
    }
    void Start()
    {
        Manager_Game.instance.OnChangeGameState += Instance_OnChangeGameState;

        playerOne_Input.Gameplay.Interract.performed += Gameplay_InterractController_Performed;
        playerOne_Input.Gameplay.Interract.canceled += Gameplay_InterractController_Canceled;
        playerOne_Input.Gameplay.Navigation.performed += Gameplay_Navigation_performed;
        playerOne_Input.Gameplay.Pause.performed += Gameplay_Pause_performed;

        playerOne_Input.UI.Navigation.performed += UI_Navigation_performed;
        playerOne_Input.UI.Interract.performed += UI_Interract_performed;
        playerOne_Input.UI.Resume.performed += UI_Resume_performed;
    }

    private void OnDisable()
    {
        playerOne_Input.Gameplay.Interract.performed -= Gameplay_InterractController_Performed;
        playerOne_Input.Gameplay.Interract.canceled -= Gameplay_InterractController_Canceled;
        playerOne_Input.Gameplay.Navigation.performed -= Gameplay_Navigation_performed;
        playerOne_Input.Gameplay.Pause.performed -= Gameplay_Pause_performed;

        playerOne_Input.UI.Navigation.performed -= UI_Navigation_performed;
        playerOne_Input.UI.Interract.performed -= UI_Interract_performed;
        playerOne_Input.UI.Resume.performed -= UI_Resume_performed;
    }
    #region Gameplay Input
    private void Gameplay_InterractController_Canceled(UnityEngine.InputSystem.InputAction.CallbackContext interraction)
    {
        if(interraction.interaction is HoldInteraction)
        {
            Debug.Log("<color=green><b>Gameplay</b></color>\t: Cancelled Hold Interraction");
        }
    }

    private void Gameplay_InterractController_Performed(UnityEngine.InputSystem.InputAction.CallbackContext interraction)
    {
        switch (interraction.interaction)
        {
            case TapInteraction:
                Debug.Log("<color=green><b>Gameplay</b></color>\t: Performed Tap Interraction");
                break;
            case HoldInteraction:
                Debug.Log("<color=green><b>Gameplay</b></color>\t: Performed Hold Interraction");
                break;
        }
    }
    private void Gameplay_Navigation_performed(UnityEngine.InputSystem.InputAction.CallbackContext navigation)
    {
        float navigation_value = playerOne_Input.Gameplay.Navigation.ReadValue<float>();
        Debug.Log($"<color=green><b>Gameplay</b></color>\t: Performed Navigation with Value: {navigation_value}");
        Event_Navigation?.Invoke(navigation_value);
    }
    private void Gameplay_Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log($"<color=green><b>Gameplay</b></color>\t: Performed Paused");
        Manager_Game.instance.SetGameState(Manager_Game.GameState.UI);
    }
    #endregion

    #region UI Input
    private void UI_Resume_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log($"<color=orange><b>UI</b></color>\t: Perforemd Resume");
        Manager_Game.instance.SetGameState(Manager_Game.GameState.Gameplay);
    }

    private void UI_Interract_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log($"<color=orange><b>UI</b></color>\t: Performed Interract ");
    }

    private void UI_Navigation_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        float read_value = playerOne_Input.UI.Navigation.ReadValue<float>();
        Debug.Log($"<color=orange><b>UI</b></color>\t: Performed Navigation => <b>{read_value}</b>");
    }
    #endregion
    private void Instance_OnChangeGameState(Manager_Game.GameState currentState)
    {
        playerOne_Input.Gameplay.Disable();
        playerOne_Input.UI.Disable();
        playerOne_Input.Dialgoue.Disable();
        switch (currentState)
        {
            case Manager_Game.GameState.Gameplay:
                playerOne_Input.Gameplay.Enable();
                break;
            case Manager_Game.GameState.Dialogue:
                playerOne_Input.Dialgoue.Enable();
                break;
            case Manager_Game.GameState.UI:
                playerOne_Input.UI.Enable();
                break;
        }
        Debug.Log($"Input Change to new State: {currentState}");
    }
}
