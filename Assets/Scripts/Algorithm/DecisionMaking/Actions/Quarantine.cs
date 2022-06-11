﻿using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class Quarantine : WalkToTargetAndExecuteAction
    {

        public Quarantine(PersonControl character, GameObject target) : base("Quarantine",character,target)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            /*if (goal.name == person.GET_RICH_GOAL) 
                change -= 2.0f;*/
            return change;
        }

        public override bool CanExecute()
        {

            if (!base.CanExecute())
                return false;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return true;
        }

        public override void Execute()
        {
            
            base.Execute();
            //this.Character.GameManager.PickUpChest(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            /*var goalValue = worldModel.GetGoalValue(Person.GET_RICH_GOAL);
            worldModel.SetGoalValue(Person.GET_RICH_GOAL, goalValue - 2.0f);

            var money = (int)worldModel.GetProperty(Properties.MONEY);
            worldModel.SetProperty(Properties.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);*/
        }

    }
}