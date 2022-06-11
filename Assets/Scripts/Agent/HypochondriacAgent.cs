using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent
{
    public class HypochondriacAgent : Personality
    {

        protected override void SetPersonality() {
            id = 2;
            doTasks = 0.5f;
            doPreventiveMeasures = 1.0f;
            test = 1.0f;
            quarantine = 1.0f;
            beQuick = 0f;
            noninfected = 1.0f;
        }

    }
}