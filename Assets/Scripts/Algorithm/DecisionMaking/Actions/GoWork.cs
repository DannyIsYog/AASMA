using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoWork : DoTask
    {
        public float buildingDelay = 5f;
        public int peopleInBuilding {get; set;} = 0;
        private float increaseinDelay = 0.5f;

        public GoWork(AgentControl agent, GameObject target) : base("GoWork", agent, target)
        {
            base.SetDelay(this.buildingDelay + (target.GetComponent<Building>().numberOfPeople*increaseinDelay));
        }

    }
}
