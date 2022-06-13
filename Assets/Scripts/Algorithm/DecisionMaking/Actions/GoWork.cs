using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoWork : DoTask
    {

        public GoWork(AgentControl agent, GameObject target) : base("GoWork", agent, target)
        {
        }

    }
}
