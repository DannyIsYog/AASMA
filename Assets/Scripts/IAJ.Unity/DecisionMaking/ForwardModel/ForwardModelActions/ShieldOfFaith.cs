using Assets.Scripts.GameManager;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class ShieldOfFaith : Action {
        protected AutonomousCharacter Character { get; set; }

        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
        {
            this.Character = character;
        }

        public override bool CanExecute() {
            return this.Character.GameManager.characterData.Mana >= 5 && this.Character.GameManager.characterData.ShieldHP < 5;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return (int)worldModel.GetProperty(Properties.MANA) >= 5 && (int)worldModel.GetProperty(Properties.ShieldHP) < 5;
        }

        public override float GetGoalChange(Goal goal) {
            var change = base.GetGoalChange(goal);

            // Make this actually a bad goal if the character recharges the shield with too much already there
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change += -10 + 3 * this.Character.GameManager.characterData.ShieldHP;
            if (goal.Name == AutonomousCharacter.GET_MANA_GOAL) change += -5;

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            float surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);

            float surviveGoalChange = -10 + 3 * this.Character.GameManager.characterData.ShieldHP;
            float manaGoalChange = ((int)worldModel.GetProperty(Properties.MANA) - 5);
            worldModel.SetProperty(Properties.ShieldHP, 5);
            worldModel.SetProperty(Properties.MANA, (int)manaGoalChange);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + surviveGoalChange);
            worldModel.SetGoalValue(AutonomousCharacter.GET_MANA_GOAL, manaGoalChange);
        }
           public override float GetHValue(WorldModel worldModel)
        {
            var shieldHP = (int)worldModel.GetProperty(Properties.ShieldHP);
            var hp = (int)worldModel.GetProperty(Properties.HP);
            
            if (shieldHP == 0 && hp < (int)worldModel.GetProperty(Properties.MAXHP))
            {
                return hp/1.5f;
            }
            // if the shield is already active there is no point in activating again unless the hp is very low and there are no potions
            return shieldHP+hp/1.5f;
        }
    }


}