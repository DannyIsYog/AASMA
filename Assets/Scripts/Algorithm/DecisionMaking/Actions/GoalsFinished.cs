using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoalsFinished: WalkToTargetAndExecuteAction
    {
        private float goalsChange = 5.0f;
        private AgentControl agent;

        public GoalsFinished(AgentControl agent, GameObject target) : base("GoalsFinished",agent,target)
        {
            this.agent = agent;
            this.goalsChange = this.agent.agentData.personality.tasksValue + this.agent.agentData.personality.quickValue;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.name == AgentControl.DO_TASKS_GOAL) 
                change -= goalsChange;
            return change;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return agent.agentData.goalsDone && !agent.agentData.tested;
        }

        public override void Execute()
        {
            
            base.Execute();
            this.agent.GoalsDone(this.target, 500.0f);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AgentControl.PROTECT_GOAL);
            worldModel.SetGoalValue(AgentControl.PROTECT_GOAL, goalValue - goalsChange);
        }

    }
}
