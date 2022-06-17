using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent
{
    public class HypochondriacAgent : Personality
    {
        protected override void SetPersonality() {
            tasksValue = 2f;
            protectValue = 10f;
            quickValue = 2f;

            //change rate
            tasksChangeRate = 0.2f;
            protectChangeRate = 0.5f;
            quickChangeRate = 0.2f;

            proneToSymptoms = 4f;
        }
        
    }
}