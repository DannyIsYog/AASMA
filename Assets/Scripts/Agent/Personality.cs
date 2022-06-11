using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent{
    public abstract class Personality : MonoBehaviour
    {
        //personality quirks
        public int id;
        public float doTasks;
        public float doPreventiveMeasures;
        public float test;
        public float quarantine;
        public float beQuick;
        public float noninfected;

        //global stats

        protected abstract void SetPersonality();
    }
}