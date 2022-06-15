using System;
using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class SocialDistancing : WalkToTargetAndExecuteAction, IEquatable<SocialDistancing>
    {
        private AgentControl agent;
        private GameObject target;
        private float goalChange = 20.0f;
        public SocialDistancing(AgentControl agent, GameObject target) : base("SocialDistancing", agent, target)
        {
            this.agent = agent;
            this.target = target;
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
            if (agent.agentData.socialDistance)
                return true;

            return false;
        } 
        public override void Execute()
        {
            base.Execute();
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
