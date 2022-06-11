using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent
{
    public class EgocentricAgent : Personality
    {

        protected override void SetPersonality() {
            id = 1;
            doTasks = 1.0f;
            doPreventiveMeasures = 0f;
            test = 0f;
            quarantine = 0f;
            beQuick = 1.0f;
            noninfected = 0f;
        }

    }
}