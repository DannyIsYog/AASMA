using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent{
    public class SelfAwareAgent : Personality
    {
        protected override void SetPersonality() {
            tasksValue = 5f;
            protectValue = 5f;
            quickValue = 5f;

            //change rate (probably number of people should influence)
            tasksChangeRate = 0.2f;
            protectChangeRate = 0.5f;
            quickChangeRate = 0.2f;
        }
    }
}