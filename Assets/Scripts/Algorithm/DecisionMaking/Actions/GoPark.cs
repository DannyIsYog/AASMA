using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoPark : DoTask
    {
        private float buildingDelay = 5f;
        public int peopleInBuilding {get; set;} = 0;
        private float increaseinDelay = 0.5f;
        public GoPark(AgentControl agent, GameObject target) : base("GoPark", agent, target)
        {
            base.SetDelay(this.buildingDelay + (target.GetComponent<Building>().numberOfPeople*increaseinDelay));
        }

        

    }
}
