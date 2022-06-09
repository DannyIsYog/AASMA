using UnityEngine;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class Rest : Action {
        protected AutonomousCharacter Character { get; set; }

        public Rest(AutonomousCharacter character) : base("Rest")
        {
            this.Character = character;
        }

        public override bool CanExecute() {
            return this.Character.GameManager.characterData.MaxHP != this.Character.GameManager.characterData.HP;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return (int)worldModel.GetProperty(Properties.MAXHP) != (int)worldModel.GetProperty(Properties.HP);
        }

        public override float GetGoalChange(Goal goal) {
            var change = base.GetGoalChange(goal);
            
            float heal = Mathf.Min(2, this.Character.GameManager.characterData.MaxHP - this.Character.GameManager.characterData.HP);
            
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= heal;
            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL) change += 5;

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            float surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            float heal = Mathf.Min(2, (int)worldModel.GetProperty(Properties.MAXHP) - (int)worldModel.GetProperty(Properties.HP));

            worldModel.SetProperty(Properties.HP, (int)worldModel.GetProperty(Properties.HP) + (int)heal);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + heal);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, (int)worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL) + 5);
            worldModel.SetProperty(Properties.TIME, (float)worldModel.GetProperty(Properties.TIME) + 5);
        }

        public override void Execute() {
            this.Character.GameManager.Rest();
        }

        public override float GetHValue(WorldModel worldModel)
        {
            // Penalize the closer we are to the death of the heat wave (aka time 200)
            bool needsHealing = (int)worldModel.GetProperty(Properties.MAXHP) - 2 > (int)worldModel.GetProperty(Properties.HP);
            bool closeToTimeLimit = 195 > (int)worldModel.GetProperty(Properties.TIME);
            if(needsHealing && !closeToTimeLimit) {
                return -5;
            }
            return 100;
        }

    }


}