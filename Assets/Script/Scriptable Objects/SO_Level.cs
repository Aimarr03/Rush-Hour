using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName ="Level/Create Level Data")]
public class SO_Level : ScriptableObject
{
    [Header("Spawning")]
    public float MinDurationSpawn = 1f;
    public float MaxDurationToSpawn = 4f;
    public float IntervalSpawn = 1.25f;
    public int minQuantity = 1;
    public int maxQuantity = 3;

    [Header("Car Data")]
    public float minSpeed = 0.5f;
    public float maxSpeed = 0.75f;
    [Range(1f, 2f)]
    public float rushHourMultiplier = 1.1f;
    
}
