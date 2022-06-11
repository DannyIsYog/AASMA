using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class Test : WalkToTargetAndExecuteAction
    {
        private float protectChange = 10.0f;
        private PersonControl person;

        public Test(PersonControl person, GameObject target) : base("Test", person, target)
        {
            this.person = person;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            
            // maybe use another goal (spreadGoal for example)
            if (goal.name == PersonControl.PROTECT_GOAL) 
                change -= protectChange;

            return change;
        }

        public override bool CanExecute()
        {
           // if(character.)
           return true;
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
            //this.Character.GameManager.Test(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(PersonControl.PROTECT_GOAL);
            worldModel.SetGoalValue(PersonControl.PROTECT_GOAL, goalValue - protectChange);

            //check if positive or negative
            /*var money = (int)worldModel.GetProperty(Properties.MONEY);
            worldModel.SetProperty(Properties.MONEY, money + 5);*/

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }
    }
}
