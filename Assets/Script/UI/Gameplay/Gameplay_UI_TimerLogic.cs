using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay_UI
{
    public class Gameplay_UI_TimerLogic : MonoBehaviour
    {
        private float TimerDefault = 120;
        private float currentTimer;

        public TextMeshProUGUI UI_TextTimer;

        private void Awake()
        {
            currentTimer = TimerDefault;
        }
        private void Update()
        {
            currentTimer -= Time.deltaTime;
            OnCalculateClockFormat();
        }
        private void OnCalculateClockFormat()
        {
            string DigitalClock_Format = "";
            int minuteFormat = (int)(currentTimer / 60); 
            int secondFormat = (int)(currentTimer % 60); 
            int miliSecondFormat = (int)((currentTimer % 1) * 100); 

            DigitalClock_Format = $"{minuteFormat:D2}:{secondFormat:D2}:{miliSecondFormat:D2}";

            UI_TextTimer.text = DigitalClock_Format;
        }
    }
}

