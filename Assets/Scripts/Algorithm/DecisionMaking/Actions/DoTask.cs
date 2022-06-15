using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class DoTask : WalkToTargetAndExecuteAction
    {

        private AgentControl agent;
        public float delay;
        private float goalChange;

        public DoTask(string actionName, AgentControl agent, GameObject target) : base(actionName, agent, target)
        {
            this.agent = agent;
            this.delay = 0f;
            this.goalChange = this.agent.agentData.personality.tasksValue;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.name == AgentControl.DO_TASKS_GOAL) 
                change -= goalChange;
            return change;

            
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return base.CanExecute(worldModel);
        }

        public override void Execute()
        {
            base.Execute();
            agent.GoBuilding(this.target , delay);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AgentControl.DO_TASKS_GOAL);
            worldModel.SetGoalValue(AgentControl.DO_TASKS_GOAL, goalValue - goalChange);
        }

        public void SetDelay(float delay)
        {
            this.delay = delay;
        }

    }
}
