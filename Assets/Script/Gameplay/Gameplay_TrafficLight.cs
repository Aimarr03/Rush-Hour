using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay_RoadLogic
{

    public class Gameplay_TrafficLight : MonoBehaviour, I_Navigation
    {
        public TrafficState Current_TrafficState;
        public Gameplay_VehicleBasic.Direction targetDirection;
        public LightState Current_LightState;

        private float currentDuration = 0;
        private float maxBufferDuration = 0;
        private float intervalSwitch = 0.5f;
        private float intervalHold = 1.5f;

        [SerializeField] private Collider2D AreaDetection;

        [SerializeField] private SpriteRenderer visualIndicator;
        [SerializeField] public Transform StopPosition;

        private List<Gameplay_VehicleBasic> ListVehicle;
        public event Action<LightState> Event_ChangeLightState;
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

        private void Awake()
        {
            Current_TrafficState = TrafficState.Idle;
            ListVehicle = new List<Gameplay_VehicleBasic>();
            SetTrafficState(LightState.Red);
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
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Enter Vehicle");
            if(collision.TryGetComponent(out Gameplay_VehicleBasic vehicleBasic) && Current_TrafficState == TrafficState.Idle)
            {
                if (vehicleBasic.currentDirection != targetDirection) return;
                vehicleBasic.SetTrafficLight(this);
                vehicleBasic.SetState(Gameplay_VehicleBasic.VehicleState.TrafficLightStop);
                ListVehicle.Add(vehicleBasic);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Gameplay_VehicleBasic vehicleBasic) && Current_TrafficState != TrafficState.Idle)
            {
                ListVehicle.Remove(vehicleBasic);
            }
        }
        #region Traffic State Logic
        public void SetTrafficState(LightState lightState)
        {
            Current_LightState = lightState;
            switch (lightState)
            {
                case LightState.Red:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=red>Red Light</color>");
                    break;
                case LightState.Yellow:
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=yellow>Yellow Light</color>");
                    break;
                case LightState.Green:
                    foreach(Gameplay_VehicleBasic vehicleBasic in ListVehicle)
                    {
                        vehicleBasic.SetState(Gameplay_VehicleBasic.VehicleState.Move);
                    }
                    Debug.Log($"<b>{gameObject.name}</b>: Change To <color=green>Green Light</color>");
                    break;
            }
            Event_ChangeLightState?.Invoke(lightState);
        }
        private void TrafficState_Tap()
        {
            currentDuration += Time.deltaTime;
            if (currentDuration > maxBufferDuration && Current_LightState != LightState.Green)
            {
                currentDuration = 0;
                switch (Current_LightState)
                {
                    case LightState.Red:
                        SetTrafficState(LightState.Yellow);
                        break;
                    case LightState.Yellow:
                        maxBufferDuration = intervalHold;
                        SetTrafficState(LightState.Green);
                        break;
                }
            }
            else if (currentDuration > maxBufferDuration && Current_LightState == LightState.Green)
            {
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
                        SetTrafficState(LightState.Yellow);
                        break;
                    case LightState.Yellow:
                        SetTrafficState(LightState.Green);
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
                        SetTrafficState(LightState.Yellow);
                        break;
                    case LightState.Yellow:
                        SetTrafficState(LightState.Red);
                        Current_TrafficState = TrafficState.Idle;
                        currentDuration = 0;
                        maxBufferDuration = 0;
                        break;
                }
            }
        }
        #endregion

        #region Interface Override
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
        #endregion
    }
}
