using Cinemachine;
using DG.Tweening;
using Gameplay_RoadLogic;
using UnityEngine;

namespace GameplayManager
{
    public class Gameplay_CameraController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera defaultCamera;
        [SerializeField] CinemachineVirtualCamera crashCamera;
        private void Start()
        {
            Manager_Gameplay.instance.OnChangeGameplayState += Instance_OnChangeGameplayState;
            Gameplay_VehicleBasic.OnCrash += Gameplay_VehicleBasic_OnCrash;
        }

        private void OnDisable()
        {
            Manager_Gameplay.instance.OnChangeGameplayState -= Instance_OnChangeGameplayState;
            Gameplay_VehicleBasic.OnCrash -= Gameplay_VehicleBasic_OnCrash;
        }

        private void Gameplay_VehicleBasic_OnCrash(Vector3 targetMove)
        {
            targetMove.z = -10;
            crashCamera.transform.DOMove(targetMove, 0.65f).SetEase(Ease.OutBounce);
        }

        private void Instance_OnChangeGameplayState(Manager_Game.GameplayState state)
        {
            if(state == Manager_Game.GameplayState.Lose)
            {
                defaultCamera.gameObject.SetActive(false);
                crashCamera.gameObject.SetActive(true);
            }
        }
    }
}

