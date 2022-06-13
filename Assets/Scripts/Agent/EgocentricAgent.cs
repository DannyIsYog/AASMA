using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent
{
    public class EgocentricAgent : Personality
    {
        protected override void SetPersonality() {
            tasksValue = 8f;
            protectValue = 2f;
            quickValue = 5f;

            //change rate
            tasksChangeRate = 0.2f;
            protectChangeRate = 0.5f;
            quickChangeRate = 0.2f;
        }

    }
}