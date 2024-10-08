using GameplayManager;
using System;
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
        public enum MovingState
        {
            Horizontal, Vertical
        }
        [SerializeField, Range(0, 3)] private float movementSpeed;
        [SerializeField] LayerMask vehicleMask;
        private Queue<Gameplay_RoadNode> destinations;
        private Vector3 targetPosition;

        public Gameplay_TrafficLight currentTrafficLightToStop;
        private Vector3 bufferTargetPosition;

        private VehicleState currentState;
        private Gameplay_VehicleBasic frontVehicle;
        public VehicleState GetCurrentState => currentState;
        public Direction currentDirection;
        public MovingState currentMovingState;
        
        private Collider2D vehicle_Collider;
        private SpriteRenderer vehicle_Renderer;
        [SerializeField] private Sprite UpDirectionSprite, RightDirectionSprite;
        [SerializeField] private int score;
        
        public static event Action<Gameplay_VehicleBasic,int> OnReachGoal;
        public static event Action<Vector3> OnCrash;

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

            targetPosition = this.destinations.Dequeue().worldPosition;
            currentState = VehicleState.Move;
            OnSetCurrentDirection();
        }
        void Update()
        {
            if (Manager_Game.instance.currentGameState != Manager_Game.GameState.Gameplay) return;
            if (Manager_Gameplay.instance.currentGameplayState != Manager_Game.GameplayState.Neutral) return;
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
                Vector3 rayEnd = boxCenter + direction * 0.1f;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, rayEnd);
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Gameplay_VehicleBasic vehicle))
            {
                if(vehicle.currentMovingState != currentMovingState && vehicle.currentState == VehicleState.Move && currentState == VehicleState.Move && vehicle != frontVehicle)
                {
                    Manager_Gameplay.instance.SetGameplayState(Manager_Game.GameplayState.Lose);
                    OnCrash?.Invoke(transform.position);
                }
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
            //Debug.Log(direction);
            bool hitVehicle = false;
            RaycastHit2D[] hitVehicleDatas = Physics2D.LinecastAll(boxCenter, boxCenter + direction * 0.35f, vehicleMask);
            foreach(RaycastHit2D RayCastHitVehicle in  hitVehicleDatas)
            {
                Gameplay_VehicleBasic hitVehicleData =  RayCastHitVehicle.collider.gameObject.GetComponent<Gameplay_VehicleBasic>();
                if(hitVehicleData.gameObject != gameObject)
                {
                    if (hitVehicleData.currentDirection != currentDirection) continue;
                    hitVehicle = true;
                    frontVehicle = hitVehicleData;
                    break;
                }
            }
          
            
            if (Vector2.Distance(transform.position, targetPosition) > 0.01f && !hitVehicle)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            }
            else
            {
                currentState = VehicleState.Stop;
            }
            if (Vector3.Distance(transform.position, bufferTargetPosition) <= 0.01f)
            {
                bufferTargetPosition = destinations.Dequeue().worldPosition;
                //Debug.Log($"{destinations.Count} remaining");
                //Debug.Log("Dequeue more");
            }
        }
        private void CheckBufferPositionAgain()
        {   
            Vector3 direction = targetPosition - transform.position;
            Debug.Log($"{gameObject.name} + {direction}");
            bool shouldIterate = calculateDirectionToReiterate(direction);
            while(shouldIterate)
            {
                //Debug.Log($"{destinations.Count} remaining");
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
                    OnReachGoal?.Invoke(this, score);
                }
            }
        }
        private void OnSetCurrentDirection()
        {
            Vector3 calculateDistance = targetPosition - transform.position;
            float x = calculateDistance.x;
            float y = calculateDistance.y;
            if (x > 0 && y > 0)
            {
                currentDirection = Direction.Up;
                currentMovingState = MovingState.Vertical;
                vehicle_Renderer.sprite = UpDirectionSprite;
                vehicle_Renderer.flipX = false;
                targetPosition += new Vector3(-0.175f, 0);
            }
            else if (x < 0 && y < 0)
            {
                currentDirection = Direction.Down;
                currentMovingState = MovingState.Vertical;
                vehicle_Renderer.sprite = RightDirectionSprite;
                vehicle_Renderer.flipX = true;
                targetPosition += new Vector3(0.175f, 0);
            }
            else if (x > 0 && y < 0)
            {
                currentDirection = Direction.Right;
                currentMovingState = MovingState.Horizontal;
                vehicle_Renderer.sprite = RightDirectionSprite;
                vehicle_Renderer.flipX = false;
                targetPosition += new Vector3(0, 0.175f);
            }
            else if (x < 0 && y > 0)
            {
                currentDirection = Direction.Left;
                currentMovingState = MovingState.Horizontal;
                vehicle_Renderer.sprite = UpDirectionSprite;
                vehicle_Renderer.flipX = true;

            }
        }
        #endregion
    }
}
