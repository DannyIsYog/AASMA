using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class Test : DoTask
    {
        private float goalChange = 5.0f;
        private AgentControl agent;

        public Test(AgentControl agent, GameObject target) : base("Test", agent, target)
        {
            this.agent = agent;
            this.goalChange = this.agent.agentData.personality.protectValue*2;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            
            // maybe use another goal (spreadGoal for example)
            if (goal.name == AgentControl.PROTECT_GOAL) 
                change -= goalChange;

            return change;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return agent.agentData.symptoms && !agent.agentData.tested;
        }

        public override void Execute()
        {
            base.Execute();
            agent.Test(this.target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AgentControl.PROTECT_GOAL);
            worldModel.SetGoalValue(AgentControl.PROTECT_GOAL, goalValue - goalChange);
        }
    }
}
