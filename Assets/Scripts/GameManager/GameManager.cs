using Assets.Scripts.Agent;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameManager
{
    public class GameManager : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 2.0f;
        public const int TIME_LIMIT = 200;
        //public fields, seen by Unity in Editor


        [Header("UI Objects")]
        public Text SymptomsText;
        public Text InfectedText;
        public Text QuarantinedText;
        public Text UsingMaskText;
        public Text GoalsText;
        public Text TimeText;
        public Text DiaryText;
        public GameObject GameEnd;

        //world stats
        [HideInInspector]
        public int symptomsNumber, infectedNumber, quarantinedNumber, protectingNumber, goalsNumber;

        [Header("People specs")]
        public int number;
        public GameObject agentPrefab;
        private GameObject[] agents;
        public int selfAwarePercentage;
        public int egocentricPercentage;
        public int hypochondriacPercentage;

        public int initialInfectedPercentage;

        public WorldManager worldManager;

        private float nextUpdateTime = 0.0f;
        private float enemyAttackCooldown = 0.0f;
        public bool gameEnded { get; set; } = false;
        public Vector3 initialPosition { get; set; }
        public int infectionProbability;
        public int symptomsProbability;



        void Awake()
        {
            agents = new GameObject[number];
            GeneratePeople(number);
            AgentData agentData;
            foreach (GameObject agent in agents)
            {
                agentData = agent.GetComponent<AgentControl>().agentData;
                if (agentData.infected) infectedNumber++;
                if (agentData.symptoms) symptomsNumber++;
                if (agentData.quarantined) quarantinedNumber++;
                if (agentData.usingMask) protectingNumber++;
            }

        }

        public void Update()
        {
            AgentData agentData = this.agents[0].GetComponent<AgentControl>().agentData;

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + UPDATE_INTERVAL;
                agentData.time += UPDATE_INTERVAL;
            }

            //TODO to change when we have multiple agents
            this.SymptomsText.text = symptomsNumber + " contracted symptoms";
            this.InfectedText.text = infectedNumber + " infected";
            this.QuarantinedText.text = quarantinedNumber + " are in quarantine";
            this.UsingMaskText.text = protectingNumber + " are protected";
            this.GoalsText.text = goalsNumber + " finished their tasks";
            //this.TimeText.text = "Time: " + agentData.time;

        }

        public void GeneratePeople(int number)
        {
            int selfAwareAgents = (number * selfAwarePercentage) / 100;
            int egocentricAgents = (number * egocentricPercentage) / 100;
            int hypochondriacAgents = (number * hypochondriacPercentage) / 100;

            int selfAwareInfected = (selfAwareAgents * initialInfectedPercentage) / 100;
            int egocentricInfected = (egocentricAgents * initialInfectedPercentage) / 100;
            int hypochondriacInfected = (hypochondriacAgents * initialInfectedPercentage) / 100;

            bool infectedAgent = false;

            worldManager.spawner();

            for (int i = 0; i < number; i++)
            {
                GameObject agent = Instantiate(agentPrefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
                agent.transform.parent = GameObject.Find("Agents").transform;
                worldManager.spawnAgent(agent);
                infectedAgent = false;
                Personality p = null;
                //this.initialPosition = this.character.transform.position;

                //self-aware
                if (i < selfAwareAgents)
                {
                    agent.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue);
                    if (selfAwareInfected > 0)
                    {
                        infectedAgent = true;
                        selfAwareInfected--;
                    }
                    p = new SelfAwareAgent();
                }
                //egocentric
                else if (i >= selfAwareAgents && i < selfAwareAgents + egocentricAgents)
                {
                    agent.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
                    if (egocentricInfected > 0)
                    {
                        infectedAgent = true;
                        egocentricInfected--;
                    }
                    p = new EgocentricAgent();
                }
                //hypochondriac
                else
                {
                    agent.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
                    if (hypochondriacInfected > 0)
                    {
                        infectedAgent = true;
                        hypochondriacInfected--;
                    }
                    p = new HypochondriacAgent();
                }

                agent.GetComponent<AgentControl>().Init(agent, p, infectedAgent);
                this.agents[i] = agent;
            }
        }
    }
}
