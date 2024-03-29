﻿using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class Quarantine : WalkToTargetAndExecuteAction
    {
        private float goalChange = 10.0f;
        private AgentControl agent;

        public Quarantine(AgentControl agent, GameObject target) : base("Quarantine",agent,target)
        {
            this.agent = agent;
            this.goalChange = float.MaxValue;
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
            return agent.agentData.tested && agent.agentData.infected;
        }

        public override void Execute()
        {
            
            base.Execute();
            this.agent.Quarantine(this.target, 500.0f);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AgentControl.PROTECT_GOAL);
            worldModel.SetGoalValue(AgentControl.PROTECT_GOAL, goalValue - goalChange);
        }

    }
}
