using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, GameObject target) : base("GetManaPotion", character, target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana < this.Character.GameManager.characterData.MaxMana;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return (int)worldModel.GetProperty(Properties.MANA) < (int)worldModel.GetProperty(Properties.MAXMANA);
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetManaPotion(this.Target);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GET_MANA_GOAL) change -= goal.InsistenceValue;

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.MANA, (int)worldModel.GetProperty(Properties.MAXMANA));
            worldModel.SetGoalValue(AutonomousCharacter.GET_MANA_GOAL, 0f);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            float value = base.GetHValue(worldModel);
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            if (mana < (int)worldModel.GetProperty(Properties.MAXMANA))
            {
                // +0.01 so in case of draw, getting health is prefered
                return value + (mana - (int)worldModel.GetProperty(Properties.MAXMANA)) + 0.01f;
            }
            // it would not make sense to get a potion if the mana value is at max
            return 100.0f;
        }
    }
}
