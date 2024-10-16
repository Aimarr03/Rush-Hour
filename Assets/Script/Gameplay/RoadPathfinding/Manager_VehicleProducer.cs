using Gameplay_RoadLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameplayManager
{
    public class Manager_VehicleProducer : MonoBehaviour
    {
        public Gameplay_VehicleBasic basicCar;
        public Gameplay_RoadVehicleSpawner roadVehicleSpawner;

        public SO_Level level_data;
        public Transform VehicleContainer;
        public Queue<Gameplay_VehicleBasic> Queue_BasicVehicle;

        Manager_RoadPathfinding roadPathfinding;
        List<Gameplay_RoadNode> List_EdgeNodes => roadPathfinding.EdgeTilesList;
        


        private float currentDuration;
        private float currentIntervalToSpawn;
        private float intervalSpawnVehicle = 0.75f;
        
        private float minDuration = 2.5f;
        private float maxDuration = 4.2f;
        private float intervalToSpawn => Random.Range(minDuration, maxDuration);

        public static Manager_VehicleProducer Instance { get; private set; }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            Queue_BasicVehicle = new Queue<Gameplay_VehicleBasic>();
            for(int index = 0; index < 30; index++)
            {
                Gameplay_VehicleBasic vehicle = Instantiate(basicCar, VehicleContainer);
                vehicle.gameObject.name = $"Vehicle {index}";
                vehicle.transform.parent = VehicleContainer;
                vehicle.gameObject.SetActive(false);
                Queue_BasicVehicle.Enqueue(vehicle);
            }
            roadPathfinding = GetComponent<Manager_RoadPathfinding>();
            currentDuration = 0;
            currentIntervalToSpawn = intervalToSpawn;
        }
        private void Start()
        {
            SetLevelData(level_data);
        }
        
        public void SetLevelData(SO_Level level)
        {
            level_data = level;
            minDuration = level_data.MinDurationSpawn;
            maxDuration = level_data.MaxDurationToSpawn;
            intervalSpawnVehicle = level_data.IntervalSpawn;

            foreach(Gameplay_RoadNode roadNode in List_EdgeNodes)
            {
                Gameplay_RoadVehicleSpawner vehicleSpawner = Instantiate(roadVehicleSpawner, roadNode.worldPosition, Quaternion.identity);
                vehicleSpawner.transform.parent = Manager_Gameplay.instance.transform;
                float IntervalToSpawn = Random.Range(level_data.IntervalSpawn - 0.2f, level_data.IntervalSpawn + 0.15f);
                vehicleSpawner.SetUpData(new Gameplay_RoadVehicleSpawner.VehicleSpawnerData(
                    level_data.MinDurationSpawn, 
                    level_data.MaxDurationToSpawn,
                    level_data.minQuantity, 
                    level_data.maxQuantity,
                    IntervalToSpawn
                    ));
            }
        }

        public void SetData(Vector3 startPosition,out Vector3 targetPosition)
        {
            Gameplay_RoadNode endNode;
            do
            {
                endNode = List_EdgeNodes[Random.Range(0, List_EdgeNodes.Count)];
            } while (startPosition == endNode.worldPosition);
            
            targetPosition = endNode.worldPosition;
            
        }
        public void AddVehicle(Gameplay_VehicleBasic vehicle)
        {
            vehicle.gameObject.SetActive(false);
            Queue_BasicVehicle.Enqueue(vehicle);
        }
        public Gameplay_VehicleBasic GetVehicle()
        {
            if(Queue_BasicVehicle.Count == 0)
            {
                Gameplay_VehicleBasic basicVehicle =  Instantiate(basicCar);
                basicVehicle.transform.position = transform.position;
                basicVehicle.transform.rotation = Quaternion.identity;
                return basicVehicle;
            }
            else
            {
                Gameplay_VehicleBasic basicVehicle = Queue_BasicVehicle.Dequeue();
                return basicVehicle;
            }
        }
        /*private IEnumerator SpawnVehicleWithCount(Vector3 startPos, Vector3 endPos, int maxCount)
        {
            List<Gameplay_RoadNode> destinations = roadPathfinding.SetPathFromWorldPosition(startPos, endPos);
            for (int index = 0; index < maxCount; index++)
            {
                Gameplay_VehicleBasic newCar = Instantiate(basicCar);
                newCar.transform.position = startPos;
                newCar.SetUpDestination(destinations, Random.Range(level_data.minSpeed, level_data.maxSpeed));
                yield return new WaitForSeconds(intervalSpawnVehicle);
            }
            yield return null;
        }*/
    }
}
