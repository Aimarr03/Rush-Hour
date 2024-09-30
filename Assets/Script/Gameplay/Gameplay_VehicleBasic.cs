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
            Move
        }
        [SerializeField, Range(5, 12)] private float movementSpeed;
        private Queue<Gameplay_RoadNode> destinations;
        private Vector3 targetPosition;

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
                    break;
                case VehicleState.Move:
                    HandleMovementLogic();
                    break;
            }
        }
        #endregion
        #region StateLogic
        public void SetState(VehicleState state)
        {
            currentState = state;
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
                    currentState = VehicleState.Stop;
                }
            }
        }
        #endregion
    }
}
