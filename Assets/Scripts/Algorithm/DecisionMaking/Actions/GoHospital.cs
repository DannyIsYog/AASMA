using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoHospital : DoTask
    {

        public GoHospital(AgentControl agent, GameObject target) : base("GoHospital", agent, target)
        {
        }

    }
}
