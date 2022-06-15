using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoBank : DoTask
    {

        private float increaseinDelay = 0.5f;

        public GoBank(AgentControl agent, GameObject target) : base("GoBank", agent, target)
        {
            base.SetDelay(target.GetComponent<Building>().delay + (target.GetComponent<Building>().numberOfPeople*increaseinDelay));
        }


    }
}
