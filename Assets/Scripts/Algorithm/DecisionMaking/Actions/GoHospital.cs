using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoHospital : DoTask
    {

        private float buildingDelay = 5f;
        public int peopleInBuilding {get; set;} = 0;
        private float increaseinDelay = 0.5f;

        public GoHospital(AgentControl agent, GameObject target) : base("GoHospital", agent, target)
        {
            base.SetDelay(this.buildingDelay + (target.GetComponent<Building>().numberOfPeople*increaseinDelay));

        }
        

        

    }
}
