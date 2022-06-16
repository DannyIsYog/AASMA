using System.Collections.Generic;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.Agent
{
    public class AgentData
    {
        public bool symptoms { get; set; }
        public bool infected { get; set; }
        public bool tested { get; set; }
        public bool quarantined { get; set; }
        public bool usingMask { get; set; }
        public bool socialDistance { get; set; }
        public bool goalsDone { get; set; }
        public float time { get; set; }
        public List<string> disposableActions { get; set; }



        // may not be needed
        public Personality personality;
        public GameObject agentGameObject { get; private set; }
        public List<Goal> goals;
        public Goal doTasksGoal;
        public Goal protectGoal;
        public Goal beQuickGoal;

        public AgentData(GameObject gameObject, Goal[] goals, Personality personality)
        {
            this.agentGameObject = gameObject;
            this.symptoms = false;
            this.infected = false;
            this.tested = false;
            this.quarantined = false;
            this.usingMask = false;
            this.socialDistance = false;
            this.goalsDone = false;
            this.time = 0;
            this.personality = personality;
            disposableActions = new List<string>();
            //GenerateGoals(goals);
        }

        public List<Goal> GenerateGoals()
        {
            doTasksGoal = new Goal(AgentControl.DO_TASKS_GOAL, personality.tasksValue)
            {
                changeRate = personality.tasksChangeRate
            };

            protectGoal = new Goal(AgentControl.PROTECT_GOAL, personality.protectValue)
            {
                changeRate = personality.protectChangeRate
            };

            beQuickGoal = new Goal(AgentControl.BE_QUICK_GOAL, personality.quickValue)
            {
                changeRate = personality.quickChangeRate
            };

            this.goals = new List<Goal>();
            this.goals.Add(doTasksGoal);
            this.goals.Add(beQuickGoal);
            this.goals.Add(protectGoal);

            return this.goals;
        }

        public void AddDisposableAction(string action)
        {
            disposableActions.Add(action);
        }

        public bool ContainsAction(string action)
        {
            foreach (string a in disposableActions)
                if (a.Equals(action))
                    return true;

            return false;
        }

    }
}
