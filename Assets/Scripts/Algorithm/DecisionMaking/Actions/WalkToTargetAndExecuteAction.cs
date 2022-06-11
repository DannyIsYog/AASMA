using Assets.Scripts.GameManager;
using UnityEngine;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public abstract class WalkToTargetAndExecuteAction : Action
    {
        protected PersonControl person { get; set; }

        public GameObject Target { get; set; }

        protected WalkToTargetAndExecuteAction(string actionName, PersonControl person, GameObject target) : base(actionName + "(" + target.name + ")")
        {
            this.person = person;
            this.Target = target;
        }

        public override float GetDuration()
        {
            return this.GetDuration(this.person.transform.position);
        }

        public override float GetDuration(WorldModel worldModel)
        {
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION);
            return this.GetDuration(position);
        }

        private float GetDuration(Vector3 currentPosition)
        {
            var distance = getDistance(currentPosition, Target.transform.position);
            var result = distance / this.person.maxSpeed;
            return result;
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.name == PersonControl.BE_QUICK_GOAL)
                return this.GetDuration();
            
            else return 0;
        }

        public override bool CanExecute()
        {
            return this.Target != null;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (this.Target == null) 
                return false;
                
            var targetEnabled = (bool)worldModel.GetProperty(this.Target.name);
            return targetEnabled;
        }

        public override void Execute()
        {
            Vector3 delta = this.Target.transform.position - this.person.transform.position;
            
            if (delta.sqrMagnitude > 5 )
               this.person.StartPathfinding(this.Target.transform.position);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var duration = this.GetDuration(worldModel);
            var quicknessValue = worldModel.GetGoalValue(PersonControl.BE_QUICK_GOAL);
            worldModel.SetGoalValue(PersonControl.BE_QUICK_GOAL, quicknessValue + duration * 0.1f);

            var time = (float)worldModel.GetProperty(Properties.TIME);
            worldModel.SetProperty(Properties.TIME, time + duration);

            worldModel.SetProperty(Properties.POSITION, Target.transform.position);
        }

        private float getDistance(Vector3 currentPosition, Vector3 targetPosition)
        {        
            var distance = this.person.GetDistanceToTarget(currentPosition, targetPosition);
            return distance;
        }

    }
}