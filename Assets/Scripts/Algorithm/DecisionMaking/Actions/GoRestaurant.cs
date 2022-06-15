using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoRestaurant : DoTask
    {
        private float buildingDelay = 5;
        public int peopleInBuilding {get; set;} = 0;
        private float increaseinDelay = 0.5f;

        public GoRestaurant(AgentControl agent, GameObject target) : base("GoRestaurant", agent, target)
        {
            base.SetDelay(this.buildingDelay + (target.GetComponent<Building>().numberOfPeople*increaseinDelay));
        }

        

    }
}
