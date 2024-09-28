using Gameplay_RoadLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayManager
{
    public class Manager_VehicleProducer : MonoBehaviour
    {
        [SerializeField] private Gameplay_VehicleBasic basicCar;
        Manager_RoadPathfinding roadPathfinding;
        List<Gameplay_RoadNode> List_EdgeNodes => roadPathfinding.EdgeTilesList;

        private float currentDuration;
        private float currentIntervalToSpawn;
        
        private float minDuration = 0.5f;
        private float maxDuration = 1.2f;
        private float intervalToSpawn => Random.Range(minDuration, maxDuration);
        private void Awake()
        {
            roadPathfinding = GetComponent<Manager_RoadPathfinding>();
            currentDuration = 0;
            currentIntervalToSpawn = intervalToSpawn;
        }
        private void Start()
        {

        }
        private void Update()
        {
            currentDuration += Time.deltaTime;
            if(currentDuration > currentIntervalToSpawn)
            {
                currentDuration = 0;
                currentIntervalToSpawn = intervalToSpawn;
                SpawnVehicle();
            }
        }

        private void SpawnVehicle()
        {
            Gameplay_RoadNode startNode = List_EdgeNodes[Random.Range(0, List_EdgeNodes.Count)];
            Gameplay_RoadNode endNode;
            do
            {
                endNode = List_EdgeNodes[Random.Range(0, List_EdgeNodes.Count)];
            } while (startNode == endNode);

            List<Gameplay_RoadNode> destinations = roadPathfinding.SetPathFromWorldPosition(startNode.worldPosition, endNode.worldPosition);
            Gameplay_VehicleBasic newCar = Instantiate(basicCar);
            newCar.transform.position = startNode.worldPosition;
            newCar.SetUpDestination(destinations);

        }
    }
}
