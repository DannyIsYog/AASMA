using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return (int)worldModel.GetProperty(Properties.HP) < (int)worldModel.GetProperty(Properties.MAXHP);
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetHealthPotion(this.Target);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            float heal = this.Character.GameManager.characterData.MaxHP - this.Character.GameManager.characterData.HP;

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= heal;

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.HP, (int)worldModel.GetProperty(Properties.MAXHP));
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 5 - (int)worldModel.GetProperty(Properties.ShieldHP));
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            float value = base.GetHValue(worldModel);
            var hp = (int)worldModel.GetProperty(Properties.HP);
            
            if (hp < (int)worldModel.GetProperty(Properties.MAXHP))
            {
                return value + (hp - (int)worldModel.GetProperty(Properties.MAXHP));
            }
            // it would not make sense to get a potion if the health value is at max
            return 100.0f;
        }

    }
}
