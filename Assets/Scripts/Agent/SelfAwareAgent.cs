using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent{
    public class SelfAwareAgent : Personality
    {

        protected override void SetPersonality() {
            id = 0;
            doTasks = 0.5f;
            doPreventiveMeasures = 0.5f;
            test = 0.5f;
            quarantine = 0.5f;
            beQuick = 0.5f;
            noninfected = 0.5f;
        }
    }
}