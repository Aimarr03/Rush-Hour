using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay_Pause_Button : MonoBehaviour, I_Navigation
{
    public Image FocusIndicator;
    private void Awake()
    {
        if (FocusIndicator == null)
        {
            FocusIndicator = transform.GetChild(0).GetComponent<Image>();
        }
    }
    public void State_CancelledHold()
    {
        
    }

    public void State_Defocused()
    {
        FocusIndicator.gameObject.SetActive(false);
    }

    public void State_Focused()
    {
        FocusIndicator.gameObject.SetActive(true);
    }
    //Alternative for Negate Button()
    public void State_Hold()
    {
        
    }

    public void State_Tap()
    {
        Debug.Log($"{gameObject.name} is Interracted");
    }
}
