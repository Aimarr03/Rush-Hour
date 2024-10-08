using Gameplay_RoadLogic;
using GameplayManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gameplay_UI_ScoreLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI VisualText;
    
    private void Start()
    {
        Gameplay_VehicleBasic.OnReachGoal += Gameplay_VehicleBasic_OnReachGoal;
    }


    private void OnDisable()
    {
        Gameplay_VehicleBasic.OnReachGoal -= Gameplay_VehicleBasic_OnReachGoal;
    }
    private void Gameplay_VehicleBasic_OnReachGoal(Gameplay_VehicleBasic vehicle, int obj)
    {
        int score = Manager_Gameplay.instance.score;
        VisualText.text = $"Score: {score}";
    }
}
