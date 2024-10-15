using GameplayManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Data_Logic.GameData;

namespace Gameplay_RoadLogic 
{
    public class Gameplay_RoadVehicleSpawner : MonoBehaviour
    {
        VehicleSpawnerData data;
        public struct VehicleSpawnerData
        {
            public float intervalSpawner;
            public int minSpawnVehicle;
            public int maxSpawnVehicle;
            public VehicleSpawnerData(float intervalSpawner, int minSpawnVehicle, int maxSpawnVehicle)
            {
                this.intervalSpawner = intervalSpawner;
                this.minSpawnVehicle = minSpawnVehicle; 
                this.maxSpawnVehicle = maxSpawnVehicle;
            }
        }
        public float currentSpawnDuration = 0f;
        public void SetUpData(VehicleSpawnerData data)
        {
            this.data = data;
        }
        private void Update()
        {
            if (Manager_Game.instance.currentGameState != Manager_Game.GameState.Gameplay) return;
            if (Manager_Gameplay.instance.currentGameplayState != Manager_Game.GameplayState.Neutral) return;
            
            currentSpawnDuration += Time.deltaTime;
            if(currentSpawnDuration > data.intervalSpawner)
            {
                currentSpawnDuration = 0;
                Manager_VehicleProducer.Instance.SetData(transform.position,out Vector3 targetPosition);
                int RandomizeQuantity = Random.Range(data.minSpawnVehicle, data.maxSpawnVehicle);
                StartCoroutine(StartSpawning(RandomizeQuantity, targetPosition));
            }
        }
        private IEnumerator StartSpawning(int maxQuantity, Vector3 targetPosition)
        {
            int currentQuantity = 0;
            List<Gameplay_RoadNode> destinations = Manager_RoadPathfinding.instance.SetPathFromWorldPosition(transform.position, targetPosition);
            while (currentQuantity < maxQuantity)
            {
                Gameplay_VehicleBasic vehicleBasic = Instantiate(Manager_VehicleProducer.Instance.basicCar, transform.position, Quaternion.identity);
                vehicleBasic.transform.position = transform.position;
                vehicleBasic.SetUpDestination(destinations, Random.Range(Manager_VehicleProducer.Instance.level_data.minSpeed, Manager_VehicleProducer.Instance.level_data.maxSpeed));
                yield return new WaitForSeconds(0.35f);
            }

        }
    }
}

