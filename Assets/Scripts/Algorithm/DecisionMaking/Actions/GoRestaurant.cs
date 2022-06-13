using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Agent;
using UnityEngine;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoRestaurant : DoTask
    {

        public GoRestaurant(AgentControl agent, GameObject target) : base("GoRestaurant", agent, target)
        {
        }

    }
}
