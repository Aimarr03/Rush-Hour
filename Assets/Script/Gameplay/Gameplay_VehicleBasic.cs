using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay_RoadLogic
{
    public class Gameplay_VehicleBasic : MonoBehaviour
    {
        public enum VehicleState
        {
            Stop,
            Move,
            Reach
        }
        [SerializeField, Range(5, 12)] private float movementSpeed;
        private Queue<Gameplay_RoadNode> destinations;
        private Vector3 targetPosition;

        public Gameplay_TrafficLight currentTrafficLightToStop;
        private Vector3 bufferTargetPosition;

        private VehicleState currentState;
        #region MONOBEHAVIOUR CALLBACK
        private void Awake()
        {
            destinations = new Queue<Gameplay_RoadNode>();
            currentState = VehicleState.Stop;
        }
        private void Start()
        {
            
        }
        public void SetUpDestination(List<Gameplay_RoadNode> destinations, float speed)
        {
            this.destinations.Clear();
            this.destinations = new Queue<Gameplay_RoadNode>(destinations);
            movementSpeed = speed;

            //Debug.Log(destinations.Count);
            targetPosition = this.destinations.Dequeue().worldPosition;
            currentState = VehicleState.Move;
        }
        void Update()
        {
            switch (currentState)
            {
                case VehicleState.Stop:
                    HandleStopLogic();
                    break;
                case VehicleState.Move:
                    HandleMovementLogic();
                    break;
                case VehicleState.Reach:
                    break;
            }
        }
        #endregion
        #region StateLogic
        public void SetState(VehicleState state)
        {
            switch (state)
            {
                case VehicleState.Stop:
                    bufferTargetPosition = targetPosition;
                    break;
                case VehicleState.Move:
                    targetPosition = bufferTargetPosition;
                    bufferTargetPosition = Vector3.zero;
                    break;
            }
            currentState = state;
        }
        public void SetTrafficLight(Gameplay_TrafficLight trafficLight)
        {
            if(trafficLight != null)
            {
                trafficLight.Event_ChangeLightState += CurrentTrafficLightToStop_Event_ChangeLightState;
            }
            else
            {
                currentTrafficLightToStop.Event_ChangeLightState -= CurrentTrafficLightToStop_Event_ChangeLightState;
            }
            currentTrafficLightToStop = trafficLight;
        }

        private void CurrentTrafficLightToStop_Event_ChangeLightState(Gameplay_TrafficLight.LightState trafficLight)
        {
            switch(trafficLight)
            {
                case Gameplay_TrafficLight.LightState.Green:
                    SetState(VehicleState.Move);
                    break;
            }
        }
        #endregion
        #region StopLogic
        private void HandleStopLogic()
        {
            if(targetPosition != null) bufferTargetPosition = targetPosition;
            if(Vector2.Distance(transform.position, currentTrafficLightToStop.StopPosition.position) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, currentTrafficLightToStop.StopPosition.position, movementSpeed * Time.deltaTime);
            }
        }
        #endregion
        #region MoveLogic
        private void HandleMovementLogic()
        {
            if(Vector2.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            }
            else
            {
                if(destinations.Count > 0)
                {
                    targetPosition = destinations.Dequeue().worldPosition;
                    //Debug.Log("Change Destinations");
                }
                else
                {
                    currentState = VehicleState.Reach;
                }
            }
        }
        #endregion
    }
}
