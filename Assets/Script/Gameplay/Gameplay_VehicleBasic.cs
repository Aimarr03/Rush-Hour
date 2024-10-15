using GameplayManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
        private Vector2 directionDetection;

        private VehicleState currentState;
        private Gameplay_VehicleBasic frontVehicle;
        public VehicleState GetCurrentState => currentState;
        public Direction currentDirection;
        public MovingState currentMovingState;
        
        private Collider2D vehicle_Collider;
        private SpriteRenderer vehicle_Renderer;
        private Animator vehicle_Animator;



        [SerializeField] private Sprite UpDirectionSprite, RightDirectionSprite;
        [SerializeField] private int score;
        [SerializeField] private float detectionRange;
        [SerializeField] private Transform DetectionAnchor;
        
        public static event Action<Gameplay_VehicleBasic,int> OnReachGoal;
        public static event Action<Vector3> OnCrash;

        public TextMeshProUGUI Text_Score;
        #region MONOBEHAVIOUR CALLBACK
        private void Awake()
        {
            destinations = new Queue<Gameplay_RoadNode>();
            vehicle_Collider = GetComponent<Collider2D>();
            vehicle_Animator = GetComponent<Animator>();
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
                    CheckCrash();
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
                Vector3 rayEnd = boxCenter + (Vector3)directionDetection * detectionRange;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, rayEnd);
            }
            else if(currentState == VehicleState.Move)
            {
                Vector2 boxCenter = vehicle_Collider.bounds.center;
                Gizmos.DrawLine(transform.position, boxCenter + directionDetection * detectionRange);
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Gameplay_VehicleBasic vehicle))
            {
                if(vehicle.currentMovingState != currentMovingState && vehicle.currentState == VehicleState.Move && currentState == VehicleState.Move && vehicle != frontVehicle)
                {
                    Manager_Gameplay.instance.SetGameplayState(Manager_Game.GameplayState.Lose);
                    Debug.Log($"Lose {gameObject.name} {transform.position} => {vehicle.gameObject.name} {vehicle.transform.position}");
                    OnCrash?.Invoke(transform.position);
                    vehicle_Animator.SetTrigger("Crash");
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
            //Debug.Log(direction);
            bool hitVehicle = false;
            RaycastHit2D[] hitVehicleDatas = Physics2D.LinecastAll(boxCenter, boxCenter + directionDetection * 0.7f, vehicleMask);
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
                try
                {
                    targetPosition = destinations.Dequeue().worldPosition;
                }
                catch (Exception e)
                {
                    Debug.Log("Queue Empty! " + gameObject.name);
                }
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
        private void CheckCrash()
        {
            Vector2 boxCenter = vehicle_Collider.bounds.center;
            //Debug.Log(direction);
            RaycastHit2D[] hitVehicleDatas = Physics2D.LinecastAll(boxCenter, boxCenter + directionDetection * detectionRange, vehicleMask);
            Gameplay_VehicleBasic crashVehicle = null; 
            bool crash = false;
            foreach (RaycastHit2D RayCastHitVehicle in hitVehicleDatas)
            {
                Gameplay_VehicleBasic vehicle = RayCastHitVehicle.collider.gameObject.GetComponent<Gameplay_VehicleBasic>();
                if (vehicle.gameObject != gameObject && vehicle.currentState != VehicleState.Reach)
                {
                    if(vehicle.currentState != currentState && vehicle.currentDirection == currentDirection)
                    {
                        crashVehicle = vehicle;
                        crash = true;
                        break;
                    }
                    if (vehicle.currentMovingState != currentMovingState && vehicle.currentState == VehicleState.Move && currentState == VehicleState.Move && vehicle != frontVehicle)
                    {
                        crashVehicle = vehicle;
                        crash = true;
                        break;
                    }
                }
            }
            if (crash)
            {
                Manager_Gameplay.instance.SetGameplayState(Manager_Game.GameplayState.Lose);
                Debug.Log($"Lose {gameObject.name} {transform.position} {crashVehicle.gameObject.name}");
                OnCrash?.Invoke(transform.position);
                vehicle_Animator.SetTrigger("Crash");
            }
        }
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
                    OnSpawningScore();
                    currentState = VehicleState.Reach;
                    OnReachGoal?.Invoke(this, score);
                    Manager_VehicleProducer.Instance.AddVehicle(this);
                }
            }
        }
        private void OnSetCurrentDirection()
        {
            Vector3 calculateDistance = targetPosition - DetectionAnchor.position;
            directionDetection = new Vector2(calculateDistance.x, calculateDistance.y/2);
            float x = calculateDistance.x;
            float y = calculateDistance.y;
            if (x > 0 && y > 0)
            {
                currentDirection = Direction.Up;
                currentMovingState = MovingState.Vertical;
                vehicle_Renderer.sprite = UpDirectionSprite;
                directionDetection.y += 0.12f;
                vehicle_Renderer.flipX = false;
                targetPosition += new Vector3(-0.2f, 0);
            }
            else if (x < 0 && y < 0)
            {
                currentDirection = Direction.Down;
                currentMovingState = MovingState.Vertical;
                vehicle_Renderer.sprite = RightDirectionSprite;
                vehicle_Renderer.flipX = true;
                directionDetection.y -= 0.33f;
                targetPosition += new Vector3(0.2f, 0);
            }
            else if (x > 0 && y < 0)
            {
                currentDirection = Direction.Right;
                currentMovingState = MovingState.Horizontal;
                directionDetection.y -= 0.15f;
                Debug.Log(directionDetection);
                vehicle_Renderer.sprite = RightDirectionSprite;
                vehicle_Renderer.flipX = false;
                targetPosition += new Vector3(0, 0.175f);
            }
            else if (x < 0 && y > 0)
            {
                currentDirection = Direction.Left;
                directionDetection.y -= 0.07f;
                currentMovingState = MovingState.Horizontal;
                targetPosition += new Vector3(0, -0.175f);
                vehicle_Renderer.sprite = UpDirectionSprite;
                vehicle_Renderer.flipX = true;

            }
        }
        #endregion
        #region Score UI
        public async void OnSpawningScore()
        {
            Text_Score.text = score.ToString();
            Text_Score.gameObject.SetActive(true);
            Text_Score.canvas.transform.SetParent(null);
            await Task.Delay(800);
            Text_Score.gameObject.SetActive(false);
            Text_Score.canvas.transform.SetParent(gameObject.transform);
        }
        #endregion
    }
}
