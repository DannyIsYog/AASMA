using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using Assets.Scripts.Agent;

namespace Assets.Scripts.Algorithm.DecisionMaking.Actions
{
    public class GoBank : DoTask
    {

        private AgentControl agent;
        public GoBank(AgentControl agent, GameObject target) : base("GoBank", agent, target)
        {
            this.agent = agent;
        }

        public override void Execute()
        {
            base.Execute();
            //agent.GoBuilding(this.target , 5f);
        }

    }
}
