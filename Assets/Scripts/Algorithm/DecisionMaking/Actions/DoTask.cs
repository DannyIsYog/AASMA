using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class DoTask : WalkToTargetAndExecuteAction
    {

        private AgentControl agent;

        public DoTask(string actionName, AgentControl agent, GameObject target) : base(actionName, agent, target)
        {
            this.agent = agent;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.name == AgentControl.DO_TASKS_GOAL) 
                change -= 2.0f;
            return change;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) 
                return false;
            return true;
        }

        public override void Execute()
        {
            base.Execute();
            agent.GoBuilding(this.target , 0f);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AgentControl.DO_TASKS_GOAL);
            worldModel.SetGoalValue(AgentControl.DO_TASKS_GOAL, goalValue - 2.0f);
        }

    }
}
