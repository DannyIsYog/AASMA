using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;

namespace Assets.Scripts.Agent
{
    public class AgentData
    {
        public bool symptoms { get; set; }
        public bool infected { get; set; }
        public bool quarantined { get; set; }
        public bool usingMask { get; set; }
        public Dictionary<string, Goal> goals = new Dictionary<string, Goal>();
        public int score { get; set; }
        public float time { get; set; }
        public List<string> disposableActions { get; set; }


        // may not be needed
        public Personality personality;
        public GameObject agentGameObject { get; private set; }

        public AgentData(GameObject gameObject, Goal[] goals, Personality personality)
        {
            this.agentGameObject = gameObject;
            this.symptoms = false;
            this.infected = false;
            this.quarantined = false;
            this.usingMask = false;
            this.score = 0;
            this.time = 0;
            this.personality = personality;
            disposableActions = new List<string>();
            GenerateGoals(goals);
        }

        public void GenerateGoals(Goal[] goals)
        {
            foreach (var goal in goals)
                this.goals.Add(goal.name,goal);
        }

        public void AddDisposableAction(string action)
        {
            disposableActions.Add(action);
        }

        public bool ContainsAction(string action)
        {
            foreach(string a in disposableActions)
                if (a.Equals(action))
                    return true;

            return false;
        }

    }
}
