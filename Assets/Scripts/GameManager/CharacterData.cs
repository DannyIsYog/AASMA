using UnityEngine;
using DecisionMaking.DecisionMaking.ForwardModel.Goal;

namespace Assets.Scripts.GameManager
{
    public class CharacterData
    {
        private bool symptoms { get; set; }
        private bool infected { get; set; }
        private bool quarantined { get; set; }

        public Goal[] goals { get; set; }
        public float time { get; set; }

        // may not be needed
        private Personality personality;
        public GameObject CharacterGameObject { get; private set; }

        public CharacterData(GameObject gameObject, Goal[] goals)
        {
            this.CharacterGameObject = gameObject;
            this.symptoms = false;
            this.infected = false;
            this.quarantined = false;
            this.goals = goals;
            this.time = 0;
        }
    }
}
