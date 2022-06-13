using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoSupermarket : DoTask
    {

        public GoSupermarket(AgentControl agent, GameObject target) : base("GoSupermarket", agent, target)
        {
        }

    }
}
