using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using System.Collections.Generic;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.GameManager
{
    //class that represents a world model that corresponds to the current state of the world,
    //all required properties and goals are stored inside the game manager
    public class CurrentStateWorldModel : FutureStateWorldModel
    {
        private Dictionary<string, Goal> goals { get; set; } 
        private AgentData agentData;

        public CurrentStateWorldModel(AgentData agentData, GameManager gameManager, List<Action> actions, List<Goal> goals) : base(gameManager, actions)
        {
            this.parent = null;
            this.agentData = agentData;
            this.goals = new Dictionary<string, Goal>();

            foreach (var goal in goals)
                this.goals.Add(goal.name,goal);
        }

        public void Initialize()
        {
            this.actionEnumerator.Reset();
        }

        public override object GetProperty(string propertyName)
        {
            //TIP: this code can be optimized by using a dictionary with lambda functions instead of if's

            if (propertyName.Equals(Properties.SYMPTOMS)) 
                return agentData.symptoms;
            
            if (propertyName.Equals(Properties.INFECTED)) 
                return agentData.infected;

            if (propertyName.Equals(Properties.QUARANTINED)) 
                return agentData.quarantined;

            if (propertyName.Equals(Properties.USING_MASK)) 
                return agentData.usingMask;

            if (propertyName.Equals(Properties.TIME)) 
                return agentData.time;

            if (propertyName.Equals(Properties.POSITION))
                return agentData.agentGameObject.transform.position;

            return this.agentData.ContainsAction(propertyName);
        }

        public override float GetGoalValue(string goalName)
        {
            return goals[goalName].insistenceValue;
        }

        public override void SetGoalValue(string goalName, float goalValue)
        {
            //this method does nothing, because you should not directly set a goal value of the CurrentStateWorldModel
        }

        public override void SetProperty(string propertyName, object value)
        {
            //this method does nothing, because you should not directly set a property of the CurrentStateWorldModel
        }

        public override int GetNextPlayer()
        {
            //in the current state, the next player is always player 0
            return 0;
        }
    }
}
