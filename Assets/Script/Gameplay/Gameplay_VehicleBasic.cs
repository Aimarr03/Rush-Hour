using GameplayManager;
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
            TrafficLightStop,
            Move,
            Reach
        }
        public enum Direction
        {
            Up, Left, Right, Down
        }
        [SerializeField, Range(5, 12)] private float movementSpeed;
        [SerializeField] LayerMask vehicleMask;
        private Queue<Gameplay_RoadNode> destinations;
        private Vector3 targetPosition;

        public Gameplay_TrafficLight currentTrafficLightToStop;
        private Vector3 bufferTargetPosition;

        private VehicleState currentState;
        public Direction currentDirection;
        
        private Collider2D vehicle_Collider;
        private SpriteRenderer vehicle_Renderer;
        [SerializeField] private Sprite UpDirectionSprite, RightDirectionSprite;

        #region MONOBEHAVIOUR CALLBACK
        private void Awake()
        {
            destinations = new Queue<Gameplay_RoadNode>();
            vehicle_Collider = GetComponent<Collider2D>();
            vehicle_Renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
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
            OnSetCurrentDirection();
        }
        void Update()
        {
            if (Manager_Game.instance.currentGameState != Manager_Game.GameState.Gameplay) return;
            switch (currentState)
            {
                case VehicleState.Stop:
                    break;
                case VehicleState.TrafficLightStop:
                    HandleTrafficStopLogic();
                    break;
                case VehicleState.Move:
                    HandleMovementLogic();
                    break;
                case VehicleState.Reach:
                    break;
            }
        }
        private void OnDrawGizmos()
        {
            if (currentState == VehicleState.TrafficLightStop || currentState == VehicleState.Stop && currentTrafficLightToStop != null)
            {
                Vector3 boxCenter = vehicle_Collider.bounds.center;
                Vector3 direction = (currentTrafficLightToStop.StopPosition.position - (Vector3)boxCenter).normalized;
                Vector3 rayEnd = boxCenter + direction * 0.6f;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, rayEnd);
            }
        }
        #endregion
        #region StateLogic
        public void SetState(VehicleState state)
        {
            Debug.Log("Set State to " + state);
            switch (state)
            {
                case VehicleState.TrafficLightStop:
                    bufferTargetPosition = targetPosition;
                    targetPosition = currentTrafficLightToStop.StopPosition.position;
                    Vector2 boxCenter = vehicle_Collider.bounds.center;
                    Vector2 distanceFromVehicle = targetPosition - (Vector3)boxCenter;
                    Vector2 direction = distanceFromVehicle.normalized;
                    break;
                case VehicleState.Move:
                    targetPosition = bufferTargetPosition;
                    CheckBufferPositionAgain();
                    OnSetCurrentDirection();
                    bufferTargetPosition = Vector3.zero;
                    break;
            }
            currentState = state;
        }
        public void SetTrafficLight(Gameplay_TrafficLight trafficLight)
        {
            currentTrafficLightToStop = trafficLight;
        }

        #endregion
        #region TrafficStopLogic
        private void HandleTrafficStopLogic()
        {
            Vector2 boxCenter = vehicle_Collider.bounds.center;
            Vector2 distanceFromVehicle = targetPosition - (Vector3)boxCenter;
            Vector2 direction = distanceFromVehicle.normalized;
            
            bool hitVehicle = false;
            RaycastHit2D[] hitVehicleDatas = Physics2D.LinecastAll(boxCenter, direction * 0.8f, vehicleMask);
            foreach(RaycastHit2D hitVehicleData in  hitVehicleDatas)
            {
                //Debug.Log(hitVehicleData.collider);
                if(hitVehicleData.collider.gameObject != gameObject)
                {
                    hitVehicle = true;
                    break;
                }
            }
          
            if(Vector3.Distance(transform.position, bufferTargetPosition) <= 0.01f)
            {
                bufferTargetPosition = destinations.Dequeue().worldPosition;
                Debug.Log("Dequeue more");
            }
            if (Vector2.Distance(transform.position, targetPosition) > 0.01f && !hitVehicle)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            }
            else
            {
                currentState = VehicleState.Stop;
            }
        }
        private void CheckBufferPositionAgain()
        {   
            Vector3 direction = targetPosition - transform.position;
            bool shouldIterate = calculateDirectionToReiterate(direction);
            while(shouldIterate)
            {
                targetPosition = destinations.Dequeue().worldPosition;
                direction = targetPosition - transform.position;
                shouldIterate = calculateDirectionToReiterate(direction);
            }
        }
        //This is to check whether the vehicle is behind the target position or not
        private bool calculateDirectionToReiterate(Vector3 direction)
        {
            bool result = false;
            switch (currentDirection)
            {
                case Direction.Up:
                    result = direction.x > 0 && direction.y > 0;
                    break;
                case Direction.Down:
                    result= direction.x < 0 && direction.y < 0;
                    break;
                case Direction.Left:
                    result = direction.x < 0 && direction.y > 0;
                    break;
                case Direction.Right:
                    result = direction.x > 0 && direction.y < 0;
                    break;
            }
            return !result;
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
                    OnSetCurrentDirection();
                    //Debug.Log("Change Destinations");
                }
                else
                {
                    currentState = VehicleState.Reach;
                }
            }
        }
        private void OnSetCurrentDirection()
        {
            Vector3 calculateDistance = targetPosition - transform.position;
            Debug.Log(calculateDistance);
            float x = calculateDistance.x;
            float y = calculateDistance.y;
            if (x > 0 && y > 0)
            {
                currentDirection = Direction.Up;
                vehicle_Renderer.sprite = UpDirectionSprite;
                vehicle_Renderer.flipX = false;
            }
            else if (x < 0 && y < 0)
            {
                currentDirection = Direction.Down;
                vehicle_Renderer.sprite = RightDirectionSprite;
                vehicle_Renderer.flipX = true;
            }
            else if (x > 0 && y < 0)
            {
                currentDirection = Direction.Right;
                vehicle_Renderer.sprite = RightDirectionSprite;
                vehicle_Renderer.flipX = false;
            }
            else if (x < 0 && y > 0)
            {
                currentDirection = Direction.Left;
                vehicle_Renderer.sprite = UpDirectionSprite;
                vehicle_Renderer.flipX = true;
                
            }
        }
        #endregion
    }
}
