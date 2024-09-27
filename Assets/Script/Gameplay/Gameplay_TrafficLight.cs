using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay_TrafficLight : MonoBehaviour, I_Navigation
{
    public TrafficState Current_TrafficState;
    public LightState Current_LightState;

    private float currentDuration = 0;
    private float maxBufferDuration = 0;
    private float intervalSwitch = 0.5f;
    private float intervalHold = 1.5f;

    [SerializeField] private SpriteRenderer visualIndicator;
    public enum TrafficState
    {
        Idle,
        Tap,
        Hold,
        OnTransitionExit
    }
    public enum LightState
    {
        Red,
        Yellow,
        Green
    }
    



    public void State_Defocused()
    {
        visualIndicator.gameObject.SetActive(false);
    }

    public void State_Focused()
    {
        visualIndicator.gameObject.SetActive(true);
    }

    public void State_Hold()
    {
        if (Current_TrafficState != TrafficState.Idle) return;
        Debug.Log($"<b>{gameObject.name}</b>\t: Set to Hold State");
        Current_TrafficState = TrafficState.Hold;
        currentDuration = 0;
        maxBufferDuration = intervalSwitch;
    }
    public void State_CancelledHold()
    {
        if (Current_TrafficState != TrafficState.Hold) return;
        Debug.Log($"<b>{gameObject.name}</b>\t: Set to Transition Exit State From Hold State");
        Current_TrafficState = TrafficState.OnTransitionExit;
        currentDuration = 0;
    }
    public void State_Tap()
    {
        if (Current_TrafficState != TrafficState.Idle) return;
        Debug.Log($"<b>{gameObject.name}</b>\t: Set to Tap State");
        Current_TrafficState = TrafficState.Tap;
        currentDuration = 0;
        maxBufferDuration = intervalSwitch;
    }



    private void Awake()
    {
        Current_TrafficState = TrafficState.Idle;
        Current_LightState = LightState.Red;
    }

    private void Update()
    {
        switch (Current_TrafficState)
        {
            case TrafficState.Idle:
                break;
            case TrafficState.Tap:
                TrafficState_Tap();
                break;
            case TrafficState.Hold:
                TrafficState_Hold();
                break;
            case TrafficState.OnTransitionExit:
                TrafficState_OnTransitionExit();
                break;
        }
    }
    private void TrafficState_Tap()
    {
        currentDuration += Time.deltaTime;
        if(currentDuration > maxBufferDuration && Current_LightState != LightState.Green)
        {
            currentDuration = 0;
            switch (Current_LightState)
            {
                case LightState.Red:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=yellow>Yellow Light</color>");
                    Current_LightState = LightState.Yellow;
                    break;
                case LightState.Yellow:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=green>Green Light</color>");
                    Current_LightState = LightState.Green;
                    maxBufferDuration = intervalHold;
                    break;
            }
        }
        else if (currentDuration > maxBufferDuration && Current_LightState == LightState.Green)
        {
            Debug.Log($"<b>{gameObject.name}</b>: Change To Transition Exit State");
            Current_TrafficState = TrafficState.OnTransitionExit;
            currentDuration = 0;
            maxBufferDuration = intervalSwitch;
        }
    }
    private void TrafficState_Hold()
    {
        if (Current_LightState == LightState.Green) return;
        currentDuration += Time.deltaTime;
        if (currentDuration > maxBufferDuration && Current_LightState != LightState.Green)
        {
            currentDuration = 0;
            switch (Current_LightState)
            {
                case LightState.Red:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=yellow>Yellow Light</color>");
                    Current_LightState = LightState.Yellow;
                    break;
                case LightState.Yellow:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=green>Green Light</color>");
                    Current_LightState = LightState.Green;
                    break;
            }
        }
    }
    private void TrafficState_OnTransitionExit()
    {
        currentDuration += Time.deltaTime;
        if (currentDuration > maxBufferDuration && Current_LightState != LightState.Red)
        {
            currentDuration = 0;
            switch (Current_LightState)
            {
                case LightState.Green:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=yellow>Yellow Light</color>");
                    Current_LightState = LightState.Yellow;
                    break;
                case LightState.Yellow:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=red>Red Light</color>");
                    Debug.Log($"<b>{gameObject.name}</b>: Change To Idle State");
                    Current_LightState = LightState.Red;
                    Current_TrafficState = TrafficState.Idle;
                    currentDuration = 0;
                    maxBufferDuration = 0;
                    break;
            }
        }
    }
}
