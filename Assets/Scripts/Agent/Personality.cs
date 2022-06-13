using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Agent{
    public abstract class Personality : MonoBehaviour
    {
        //personality quirks
        public float tasksValue = 0f;
        public float protectValue = 0f;
        public float quickValue = 0f;

        //change rate
        public float tasksChangeRate = 0f;
        public float protectChangeRate = 0f;
        public float quickChangeRate = 0f;

        protected abstract void SetPersonality();

    }
}