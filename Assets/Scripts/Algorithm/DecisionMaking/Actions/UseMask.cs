using Assets.Scripts.GameManager;
using Assets.Scripts.Agent;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;

namespace Assets.Scripts.Algporithm.DecisionMaking.Actions
{
    public class UseMask : Action {
        protected PersonControl person { get; set; }
        private float protectChange = 2.0f;

        public UseMask(PersonControl person) : base("UseMask")
        {
            this.person = person;
        }

        public override bool CanExecute() {
            //return this.Person.GameManager.characterData.personality.id != 1;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            //return (int)worldModel.GetProperty(Properties.MAXHP) != (int)worldModel.GetProperty(Properties.HP);
            return true;
        }

        public override float GetGoalChange(Goal goal) {
            var change = base.GetGoalChange(goal);
            
            if (goal.name == PersonControl.PROTECT_GOAL) 
                change -= protectChange;

            //TODO verify if needed
            if (goal.name == PersonControl.BE_QUICK_GOAL) 
                change += 5;

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            float protectValue = worldModel.GetGoalValue(PersonControl.PROTECT_GOAL);

            worldModel.SetProperty(Properties.PROTECTION, (int)worldModel.GetProperty(Properties.PROTECTION) + (int)protectChange);
            worldModel.SetGoalValue(PersonControl.PROTECT_GOAL, protectValue + protectChange);
            worldModel.SetGoalValue(PersonControl.BE_QUICK_GOAL, (int)worldModel.GetGoalValue(PersonControl.BE_QUICK_GOAL) + 5);
            worldModel.SetProperty(Properties.TIME, (float)worldModel.GetProperty(Properties.TIME) + 5);
        }

        public override void Execute() {
            //this.person.GameManager.Rest();
        }

    }


}