using UnityEngine;
using Assets.Scripts.GameManager;
using Assets.Scripts.Agent;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class UseMask : Action {
        protected AgentControl agent { get; set; }
        private float goalChange = 2.0f;

        public UseMask(AgentControl agent) : base("UseMask")
        {
            this.agent = agent;
            this.goalChange = this.agent.agentData.personality.protectValue / 2;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return !agent.agentData.usingMask;
        }

        public override float GetGoalChange(Goal goal) {
            var change = base.GetGoalChange(goal);
            
            if (goal.name == AgentControl.PROTECT_GOAL) 
                change -= goalChange;

            //TODO verify if needed
            if (goal.name == AgentControl.BE_QUICK_GOAL) 
                change += 5;

            return change;
        }

        public override void Execute()
        {
            base.Execute();
            agent.UseMask();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AgentControl.PROTECT_GOAL);
            worldModel.SetGoalValue(AgentControl.PROTECT_GOAL, goalValue - 2.0f);
        }

    }


}