using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_Button : MonoBehaviour, I_Navigation
{
    public Image imageFocus;
    private void Awake()
    {
        imageFocus = GetComponent<Image>();
    }
    public void State_CancelledHold()
    {
        
    }

    public void State_Defocused()
    {
        imageFocus.enabled = false;
    }

    public void State_Focused()
    {
        imageFocus.enabled = true;
    }

    public void State_Hold()
    {
        Debug.Log($"Interract with {gameObject.name} Button");
    }

    public void State_Tap()
    {
        
    }

}
