using System.Collections.Generic;

namespace Assets.Scripts.Algorithm.DecisionMaking.ForwardModel
{
    public class Action
    {
        private static int actionID = 0; 
        public string name { get; set; }
        public int id { get; set; }
        private Dictionary<Goal, float> goalEffects { get; set; }
        public float duration { get; set; }
        public bool hasHValue = false;
        public float hValue = 0;

        public Action(string name)
        {
            this.id = Action.actionID++;
            this.name = name;
            this.goalEffects = new Dictionary<Goal, float>();
            this.duration = 0.0f;

        }

        public void AddEffect(Goal goal, float goalChange)
        {
            this.goalEffects[goal] = goalChange;
        }

        // Used for GOB Decison Making
        public virtual float GetGoalChange(Goal goal)
        {
            if (this.goalEffects.ContainsKey(goal))
            {
                return this.goalEffects[goal];
            }
            else return 0.0f;
        }

        public virtual float GetDuration()
        {
            return this.duration;
        }

        public virtual float GetDuration(WorldModel worldModel)
        {
            return this.duration;
        }
        
        public virtual bool CanExecute(WorldModel woldModel)
        {
            return true;
        }

        public virtual bool CanExecute()
        {
            return true;
        }

        public virtual void Execute()
        {
        }

        // Used for GOAP Decison Making
        public virtual void ApplyActionEffects(WorldModel worldModel)
        {
        }


    }
}
