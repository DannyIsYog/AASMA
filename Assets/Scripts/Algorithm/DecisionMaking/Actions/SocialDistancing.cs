using System;
using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class SocialDistancing : WalkToTargetAndExecuteAction, IEquatable<SocialDistancing>
    {
        private AgentControl agent;
        private GameObject target;
        private float goalChange = 2f;

        private float SOCIAL_INTERVAL = 10f;
        public SocialDistancing(AgentControl agent, GameObject target) : base("SocialDistancing", agent, target)
        {
            this.agent = agent;
            this.target = target;
            this.goalChange = this.agent.agentData.personality.protectValue;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.name == AgentControl.PROTECT_GOAL)
                change -= goalChange;
            return change;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return agent.agentData.socialDistance && !agent.doingTask;
        } 
        public override void Execute()
        {
            base.Execute();
            
            Vector3 direction = agent.transform.position + Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0) * (Vector3.right*2);
            //Vector3 direction = (agent.transform.position - target.transform.position).normalized;  
            agent.StartPathfinding(direction);
            agent.SocialDistance(target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AgentControl.PROTECT_GOAL);
            worldModel.SetGoalValue(AgentControl.PROTECT_GOAL, goalValue - goalChange);
        }
        
        public bool Equals(SocialDistancing obj)
        {
            if (obj == null) 
                return false;
            
            SocialDistancing objAsPart = obj as SocialDistancing;
            if (objAsPart == null)
                return false;
            else
                return target.Equals(objAsPart.target);
        }
        public override int GetHashCode()
        {
            return target.GetHashCode();
        }

    }
}
