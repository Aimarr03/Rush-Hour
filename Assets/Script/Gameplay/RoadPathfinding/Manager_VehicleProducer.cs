using Gameplay_RoadLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameplayManager
{
    public class Manager_VehicleProducer : MonoBehaviour
    {
        [SerializeField] private Gameplay_VehicleBasic basicCar;

        public SO_Level level_data;

        Manager_RoadPathfinding roadPathfinding;
        List<Gameplay_RoadNode> List_EdgeNodes => roadPathfinding.EdgeTilesList;

        private float currentDuration;
        private float currentIntervalToSpawn;
        private float intervalSpawnVehicle = 0.75f;
        
        private float minDuration = 2.5f;
        private float maxDuration = 4.2f;
        private float intervalToSpawn => Random.Range(minDuration, maxDuration);
        private void Awake()
        {
            roadPathfinding = GetComponent<Manager_RoadPathfinding>();
            currentDuration = 0;
            currentIntervalToSpawn = intervalToSpawn;
        }
        private void Start()
        {
            SetLevelData(level_data);
        }
        private void Update()
        {
            if (Manager_Game.instance.currentGameState != Manager_Game.GameState.Gameplay) return;
            if (Manager_Gameplay.instance.currentGameplayState != Manager_Game.GameplayState.Neutral) return;
            
            currentDuration += Time.deltaTime;
            if (currentDuration > currentIntervalToSpawn)
            {
                currentDuration = 0;
                currentIntervalToSpawn = intervalToSpawn;
                SpawnVehicle();
            }
        }
        public void SetLevelData(SO_Level level)
        {
            level_data = level;
            minDuration = level_data.MinDurationSpawn;
            maxDuration = level_data.MaxDurationToSpawn;
            intervalSpawnVehicle = level_data.IntervalSpawn;
        }

        private void SpawnVehicle()
        {
            Gameplay_RoadNode startNode = List_EdgeNodes[Random.Range(0, List_EdgeNodes.Count)];
            Gameplay_RoadNode endNode;
            do
            {
                endNode = List_EdgeNodes[Random.Range(0, List_EdgeNodes.Count)];
            } while (startNode == endNode);
            int range = Random.Range(level_data.minQuantity, level_data.maxQuantity);
            StartCoroutine(SpawnVehicleWithCount(startNode.worldPosition, endNode.worldPosition, range));
        }
        private IEnumerator SpawnVehicleWithCount(Vector3 startPos, Vector3 endPos, int maxCount)
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
        }
    }
}
