using UnityEngine;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        float xpChange; 
        public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite",character,target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                xpChange = 3.0f;
            }
        }
        
        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -this.xpChange;
            }
            
            return change;
        }

        public override bool CanExecute()
        {

            return base.CanExecute() && this.Target.tag.Equals("Skeleton") && this.Character.GameManager.characterData.Mana >= 2; 
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return (int)worldModel.GetProperty(Properties.MANA) >= 2;
        }

        /*public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.DivineSmite(this.Target);
        }*/

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            int xp = (int)worldModel.GetProperty(Properties.XP);
            int mana = (int)worldModel.GetProperty(Properties.MANA);

            base.ApplyActionEffects(worldModel);
         
            worldModel.SetProperty(this.Target.name, false);
            worldModel.SetProperty(Properties.XP, (int)(xp + this.xpChange));
            worldModel.SetProperty(Properties.MANA, (int)(mana - 2));
            worldModel.SetGoalValue(AutonomousCharacter.GET_MANA_GOAL, 0f);
        }

          public override float GetHValue(WorldModel worldModel)
        {
            // It's a good choice overall, just return a simple value
            return base.GetHValue(worldModel) - 5;
        }
    }
}
