using Assets.Scripts.GameManager;
using UnityEngine;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public abstract class WalkToTargetAndExecuteAction : Action
    {
        protected AgentControl agent { get; set; }

        public GameObject target { get; set; }

        protected WalkToTargetAndExecuteAction(string actionName, AgentControl agent, GameObject target) : base(actionName + "(" + target.name + ")")
        {
            this.agent = agent;
            this.target = target;
        }

        public override float GetDuration()
        {
            return this.GetDuration(this.agent.transform.position);
        }

        public override float GetDuration(WorldModel worldModel)
        {
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION);
            return this.GetDuration(position);
        }

        private float GetDuration(Vector3 currentPosition)
        {
            var distance = getDistance(currentPosition, target.transform.position);
            var result = distance / this.agent.maxSpeed;
            return result;
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.name == AgentControl.BE_QUICK_GOAL)
                return this.GetDuration();
            
            else return 0;
        }

        public override bool CanExecute()
        {
            return this.target != null;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (this.target == null) 
                return false;

            var targetEnabled = (bool) worldModel.GetProperty(this.target.name);
            return targetEnabled;
        }

        public override void Execute()
        {
            Vector3 delta = this.target.transform.position - this.agent.transform.position;
            
            if (delta.sqrMagnitude > 5 )
               this.agent.StartPathfinding(this.target.transform.position);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var duration = this.GetDuration(worldModel);
            var quicknessValue = worldModel.GetGoalValue(AgentControl.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AgentControl.BE_QUICK_GOAL, quicknessValue + duration * 0.1f);

            var time = (float)worldModel.GetProperty(Properties.TIME);
            worldModel.SetProperty(Properties.TIME, time + duration);

            worldModel.SetProperty(Properties.POSITION, target.transform.position);
        }

        private float getDistance(Vector3 currentPosition, Vector3 targetPosition)
        {        
            var distance = this.agent.GetDistanceToTarget(currentPosition, targetPosition);
            return distance;
        }

    }
}