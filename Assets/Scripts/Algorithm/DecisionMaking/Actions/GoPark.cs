using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoPark : DoTask
    {

        public GoPark(AgentControl agent, GameObject target) : base("GoPark", agent, target)
        {
        }

    }
}
