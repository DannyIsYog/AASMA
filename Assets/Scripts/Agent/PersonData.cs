using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;

namespace Assets.Scripts.Agent
{
    public class PersonData
    {
        public bool symptoms { get; set; }
        public bool infected { get; set; }
        public bool quarantined { get; set; }
        public bool usingMask { get; set; }
        public Dictionary<string, Goal> goals;
        public int score { get; set; }
        public float time { get; set; }

        // may not be needed
        public Personality personality;
        public GameObject PersonGameObject { get; private set; }

        public PersonData(GameObject gameObject, Goal[] goals, Personality personality)
        {
            this.PersonGameObject = gameObject;
            this.symptoms = false;
            this.infected = false;
            this.quarantined = false;
            this.usingMask = false;
            this.score = 0;
            this.time = 0;
            this.personality = personality;
            GenerateGoals(goals);
        }

        public void GenerateGoals(Goal[] goals)
        {
            foreach (var goal in goals)
                this.goals.Add(goal.name,goal);
        }

    }
}
