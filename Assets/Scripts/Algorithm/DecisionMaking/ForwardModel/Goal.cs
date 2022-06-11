namespace Assets.Scripts.Algorithm.DecisionMaking.ForwardModel
{
    public class Goal
    {
        public string name { get; private set; }
        public float insistenceValue { get; set; }
        public float changeRate { get; set; }
        public float weight { get; private set; }

        public Goal(string name, float weight)
        {
            this.name = name;
            this.weight = weight;
        }

        public override bool Equals(object obj)
        {
            var goal = obj as Goal;

            if (goal == null) 
                return false;
            else 
                return this.name.Equals(goal.name);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public float GetDiscontentment()
        {
            var insistence = this.insistenceValue;

            if (insistence <= 0) 
                return 0.0f;
            
            return this.weight * insistence * insistence;
        }

        public float GetDiscontentment(float goalValue)
        {
            if (goalValue <= 0) 
                return 0.0f;
                
            return this.weight*goalValue*goalValue;
        }
    }
}
