using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithm.DecisionMaking.Actions;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Algorithm.DecisionMaking.GOB;
using Assets.Scripts.GameManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Assets.Scripts.Agent
{
    public class AgentControl : MonoBehaviour
    {
        //constants
        public const string DO_TASKS_GOAL = "DoTasks";
        public const string PROTECT_GOAL = "Noninfected";
        public const string BE_QUICK_GOAL = "BeQuick";

        public const float DECISION_MAKING_INTERVAL = 20.0f;
        public const float GOAL_CHANGING_INTERVAL = 20.0f;
        public const float TEST_INTERVAL = 20.0f;
        public const float TRIGGER_INTERVAL = 1.0f;

        //UI Variables
        private Text DoTasksGoalText;
        private Text ProtectGoalText;
        private Text BeQuickGoalText;
        private Text DiscontentmentText;
        private Text TotalProcessingTimeText;
        private Text BestDiscontentmentText;
        private Text ProcessedActionsText;
        private Text BestActionText;
        private Text BestActionSequence;
        private Text DiaryText;

        public GameManager.GameManager GameManager { get; private set; }

        [Header("Character Settings")]
        public float controlledSpeed;
        public List<AgentData> agentsAroundMeList = new List<AgentData>();

        //Goals
        public Goal doTasksGoal { get; private set; }
        public Goal protectGoal { get; private set; }
        public Goal beQuickGoal { get; private set; }

        public List<Goal> Goals { get; set; }
        public List<Action> Actions { get; set; }
        public Action currentAction { get; private set; }
        public Action previousAction { get; private set; }
        public DepthLimitedGOAPDecisionMaking GOAPDecisionMaking { get; set; }

        //private fields for internal use only
        private NavMeshAgent agent;
        private float nextUpdateTime = 0.0f;

        //This speed is only a pointer to the NavMeshAgent's speed
        public float maxSpeed { get; private set; }
        public TextMeshProUGUI playerText;
        private GameObject closestObject;

        public Vector3 origin { get; private set; }
        private Vector3 previousTarget;
        public int actionsNumber = 3;
        public bool init = false;

        //agent variables
        public AgentData agentData;
        public GameObject quarantineCenter;
        public bool worldChanged = false;
        public float testInterval = 0.0f;
        //private int agentsAroundMe = 0;
        private float triggerTime = 0;
        public float timeOfContact = 0f;
        public bool doingTask = false;

        public GameObject infectedVisualIndicator;

        public void Init(GameObject person, Personality p, bool infected)
        {

            //This is the actual speed of the agent
            this.agent = this.GetComponent<NavMeshAgent>();
            this.agentData = new AgentData(person, new Goal[0], p);
            this.agentData.infected = infected;
            if (infected) infectedVisualIndicator.SetActive(true);
            maxSpeed = this.agent.speed;
            playerText.text = "";

            GameManager = GameObject.Find("GameManager").GetComponent<GameManager.GameManager>();

            // Initializing UI Text
            this.DoTasksGoalText = GameObject.Find("DoTasksGoal").GetComponent<Text>();
            this.ProtectGoalText = GameObject.Find("ProtectGoal").GetComponent<Text>();
            this.BeQuickGoalText = GameObject.Find("BeQuickGoal").GetComponent<Text>();

            this.DiscontentmentText = GameObject.Find("Discontentment").GetComponent<Text>();
            this.TotalProcessingTimeText = GameObject.Find("ProcessTime").GetComponent<Text>();
            this.BestDiscontentmentText = GameObject.Find("BestDis").GetComponent<Text>();
            this.ProcessedActionsText = GameObject.Find("ProcComb").GetComponent<Text>();
            this.BestActionText = GameObject.Find("BestAction").GetComponent<Text>();
            this.BestActionSequence = GameObject.Find("BestActionSequence").GetComponent<Text>();
            this.DiaryText = GameObject.Find("DiaryText").GetComponent<Text>();

            // generate agent goals
            GenerateGoals();
            // generate actions
            StartCoroutine(GenerateActions());

            // Initialization of Decision Making Algorithms
            var worldModel = new CurrentStateWorldModel(agentData, GameManager, this.Actions, this.Goals);
            this.GOAPDecisionMaking = new DepthLimitedGOAPDecisionMaking(worldModel, this.Actions, this.Goals);
        }

        void Update()
        {
            if (!init || GameManager.gameEnded || agentData.quarantined || doingTask)
                return;

            //Every x amount of times we've got to update things
            if (Time.time > this.nextUpdateTime || this.worldChanged)
            {
                int r = Random.Range(0, 100);

                if (r < GameManager.symptomsProbability * agentData.personality.proneToSymptoms && !agentData.symptoms)
                {
                    agentData.symptoms = true;
                    GameManager.symptomsNumber++;
                }

                this.worldChanged = false;
                this.nextUpdateTime = Time.time + DECISION_MAKING_INTERVAL;

                //first step, perceptions
                //update the agent's goals based on the state of the world
                this.doTasksGoal.insistenceValue += this.doTasksGoal.changeRate; //- tasks left to do ;
                if (this.doTasksGoal.insistenceValue > 10.0f)
                    this.doTasksGoal.insistenceValue = 100.0f;

                if (agentData.tested && !agentData.quarantined)
                    this.protectGoal.insistenceValue += this.protectGoal.changeRate * 5;
                else
                    this.protectGoal.insistenceValue += this.protectGoal.changeRate;
                if (this.protectGoal.insistenceValue > 10.0f)
                    this.protectGoal.insistenceValue = 100.0f;

                this.beQuickGoal.insistenceValue += GOAL_CHANGING_INTERVAL * this.beQuickGoal.changeRate;
                if (this.beQuickGoal.insistenceValue > 10.0f)
                    this.beQuickGoal.insistenceValue = 10.0f;


                this.DoTasksGoalText.text = "Do Tasks: " + this.doTasksGoal.insistenceValue.ToString("F1");
                this.ProtectGoalText.text = "Protect: " + this.protectGoal.insistenceValue.ToString("F1");
                this.BeQuickGoalText.text = "Be Quick: " + this.beQuickGoal.insistenceValue.ToString("F1");
                this.DiscontentmentText.text = "Discontentment: " + this.CalculateDiscontentment().ToString("F1");

                this.currentAction = null;
                this.GOAPDecisionMaking.InitializeDecisionMakingProcess();
            }

            this.UpdateDLGOAP();

            if (this.currentAction != null)
                if (this.currentAction.CanExecute())
                    this.currentAction.Execute();
        }

        public void GenerateGoals()
        {
            //this.Goals = agentData.GenerateGoals();
            this.doTasksGoal = new Goal(DO_TASKS_GOAL, 5f)
            {
                changeRate = agentData.personality.tasksChangeRate
            };

            this.protectGoal = new Goal(PROTECT_GOAL, 5f)
            {
                changeRate = agentData.personality.protectChangeRate
            };

            this.beQuickGoal = new Goal(BE_QUICK_GOAL, 3f)
            {
                changeRate = agentData.personality.quickChangeRate
            };

            this.Goals = new List<Goal>();
            this.Goals.Add(this.doTasksGoal);
            this.Goals.Add(this.beQuickGoal);
            this.Goals.Add(this.protectGoal);
        }

        public System.Collections.IEnumerator GenerateActions()
        {
            this.Actions = new List<Action>();

            // Egocentric types do not protect, test or quarantine
            if (agentData.personality.GetType() != typeof(EgocentricAgent))
            {

                this.Actions.Add(new UseMask(this));

                // spawn of agent
                this.Actions.Add(new Quarantine(this, quarantineCenter));

                foreach (var hospital in GameObject.FindGameObjectsWithTag("Hospital"))
                    this.Actions.Add(new Test(this, hospital));


            }

            foreach (var goal in GameObject.FindGameObjectsWithTag("Goals"))
                this.Actions.Add(new GoalsFinished(this, goal));

            foreach (var a in GameObject.FindGameObjectsWithTag("Agent"))
            {
                if (a == this.gameObject)
                    continue;
                this.Actions.Add(new SocialDistancing(this, a));
            }

            for (int i = 0; i < actionsNumber; i++)
            {

                //TODO verification no more actions than those that exist
                List<int> randomList = new List<int>();
                int r = Random.Range(1, 6);

                while (randomList.Contains(r))
                    r = Random.Range(1, 6);

                randomList.Add(r);

                Building.BuildingTypes actionName = (Building.BuildingTypes)r;
                switch (actionName)
                {
                    case Building.BuildingTypes.Hospital:
                        foreach (var hospital in GameObject.FindGameObjectsWithTag("Hospital"))
                            this.Actions.Add(new GoHospital(this, hospital));
                        this.agentData.AddDisposableAction("Hospital");
                        break;
                    case Building.BuildingTypes.Supermarket:
                        foreach (var supermarket in GameObject.FindGameObjectsWithTag("Supermarket"))
                            this.Actions.Add(new GoSupermarket(this, supermarket));
                        this.agentData.AddDisposableAction("Supermarket");
                        break;
                    case Building.BuildingTypes.Bank:
                        foreach (var bank in GameObject.FindGameObjectsWithTag("Bank"))
                            this.Actions.Add(new GoBank(this, bank));
                        this.agentData.AddDisposableAction("Bank");
                        break;
                    case Building.BuildingTypes.Restaurant:
                        foreach (var restaurant in GameObject.FindGameObjectsWithTag("Restaurant"))
                            this.Actions.Add(new GoRestaurant(this, restaurant));
                        this.agentData.AddDisposableAction("Restaurant");
                        break;
                    case Building.BuildingTypes.Park:
                        foreach (var park in GameObject.FindGameObjectsWithTag("Park"))
                            this.Actions.Add(new GoPark(this, park));
                        this.agentData.AddDisposableAction("Park");
                        break;
                    case Building.BuildingTypes.Work:
                        foreach (var work in GameObject.FindGameObjectsWithTag("Work"))
                            this.Actions.Add(new GoWork(this, work));
                        this.agentData.AddDisposableAction("Work");
                        break;
                }
            }

            yield return null;

            init = true;
        }

        public void AddToDiary(string s)
        {
            DiaryText.text += s + "\n";

            //If the diary gets too large we cut it. Plain and simple
            if (DiaryText.text.Length > 600)
                DiaryText.text = DiaryText.text.Substring(500);
        }

        private void UpdateDLGOAP()
        {
            if (this.GOAPDecisionMaking.InProgress)
            {
                //choose an action using the GOB Decision Making process
                var action = this.GOAPDecisionMaking.ChooseAction();

                if (action != null && action != this.currentAction)
                    this.currentAction = action;
            }

            this.TotalProcessingTimeText.text = "Process. Time: " + this.GOAPDecisionMaking.TotalProcessingTime.ToString("F");
            this.BestDiscontentmentText.text = "Best Discontentment: " + this.GOAPDecisionMaking.BestDiscontentmentValue.ToString("F");
            this.ProcessedActionsText.text = "Act. comb. processed: " + this.GOAPDecisionMaking.TotalActionCombinationsProcessed;

            if (this.GOAPDecisionMaking.BestAction != null)
            {
                var actionText = "";
                foreach (var action in this.GOAPDecisionMaking.BestActionSequence)
                    if (action != null) actionText += "\n" + action.name;

                this.BestActionSequence.text = "Best Action Sequence: " + actionText;
                this.BestActionText.text = "Best Action: " + GOAPDecisionMaking.BestAction.name;
            }
            else
            {
                this.BestActionSequence.text = "Best Action Sequence:\nNone";
                this.BestActionText.text = "Best Action: \n Node";
            }
        }

        public void StartPathfinding(Vector3 targetPosition)
        {
            //if the targetPosition received is the same as a previous target, then this a request for the same target
            //no need to redo the pathfinding search
            if (!this.previousTarget.Equals(targetPosition))
            {
                this.previousTarget = targetPosition;
                agent.SetDestination(targetPosition);

            }
        }

        // Simple way of calculating distance left to target using Unity's navmesh
        public float GetDistanceToTarget(Vector3 originalPosition, Vector3 targetPosition)
        {
            var distance = 0.0f;

            NavMeshPath result = new NavMeshPath();
            var r = agent.CalculatePath(targetPosition, result);
            if (r == true)
            {
                var currentPosition = originalPosition;
                foreach (var c in result.corners)
                {
                    //Rough estimate, it does not account for shortcuts so we have to multiply it
                    distance += Vector3.Distance(currentPosition, c) * 0.65f;
                    currentPosition = c;
                }
                return distance;
            }

            //Default value
            return 100;
        }

        public float CalculateDiscontentment()
        {
            var discontentment = 0.0f;

            foreach (var goal in this.Goals)
            {
                discontentment += goal.GetDiscontentment();
            }
            return discontentment;
        }

        public void GoBuilding(GameObject building, float delay)
        {
            if (InBuildingRange(building))
            {
                List<string> newList = new List<string>(agentData.disposableActions);

                foreach (string action in agentData.disposableActions)
                    if (action.Equals(building.name))
                        newList.Remove(action);

                agentData.disposableActions = new List<string>(newList);
                if (!agentData.disposableActions.Any() && !agentData.goalsDone)
                {
                    agentData.goalsDone = true;
                    GameManager.goalsNumber++;
                }

                StartCoroutine(WaitAction(building, delay));
                building.GetComponent<Building>().numberOfPeople--;
            }
        }

        public void UseMask()
        {
            agentData.usingMask = true;
            GameManager.protectingNumber++;
            this.worldChanged = true;
        }

        public void Test(GameObject building)
        {
            if (InBuildingRange(building))
            {
                int r = Random.Range(0, 100);
                this.AddToDiary(" I tested myself and it came back " + agentData.infected);

                if (agentData.infected)
                    agentData.tested = true;
                else
                {
                    agentData.symptoms = false;
                    GameManager.symptomsNumber--;
                }

                testInterval = Time.time + TEST_INTERVAL;
                this.worldChanged = true;
            }
        }

        public void Quarantine(GameObject building, float delay)
        {
            if (InBuildingRange(building))
            {
                this.AddToDiary(" I went to quarantine");
                agentData.quarantined = true;
                StartCoroutine(WaitAction(null, delay));
                doingTask = true;
                GameManager.quarantinedNumber++;
            }
        }
        public void GoalsDone(GameObject building, float delay)
        {
            if (InBuildingRange(building))
            {
                this.AddToDiary(" I finished everything");
                StartCoroutine(WaitAction(null, delay));
            }
        }

        public void SocialDistance(GameObject target)
        {
            StartCoroutine(Distancing(3f));
        }

        int getInfectedAgentsAround()
        {
            int count = 0;

            foreach (AgentData data in agentsAroundMeList)
            {
                if (data.infected) count++;
            }
            return count;
        }

        private bool CheckForInfection()
        {
            int r = Random.Range(0, 100);
            float usingMask = 0;

            if (agentData.usingMask)

                usingMask = 10;

            if (agentData.quarantined)
            {
                /*agentData.symptoms = false;
                agentData.infected = false;
                agentData.quarantined = false;
                return false;*/
                return true;
            }
            if (r < ((getInfectedAgentsAround() * GameManager.infectionProbability) + timeOfContact - usingMask))
            {
                Debug.Log((getInfectedAgentsAround() * GameManager.infectionProbability) + timeOfContact - usingMask);
                Debug.Log(timeOfContact);
            }

            return r < ((getInfectedAgentsAround() * GameManager.infectionProbability) + timeOfContact - usingMask);
        }

        private System.Collections.IEnumerator Distancing(float delay)
        {
            doingTask = true;
            yield return new WaitForSeconds(delay);
            doingTask = false;

            if (this.previousAction != null && previousAction.name != "SocialDistancing")
            {
                currentAction = previousAction;
                if (this.currentAction.CanExecute())
                    this.currentAction.Execute();
            }
            else
                worldChanged = true;

            agentData.socialDistance = false;

        }

        private System.Collections.IEnumerator WaitAction(GameObject building, float delay)
        {
            if (building != null)
                building.GetComponent<Building>().numberOfPeople++;
            doingTask = true;

            yield return new WaitForSeconds(delay);

            if (building != null)
                building.GetComponent<Building>().numberOfPeople--;
            doingTask = false;
            this.worldChanged = true;
        }

        private bool CheckRange(GameObject obj, float maximumSqrDistance)
        {
            var distance = (obj.transform.position - this.transform.position).sqrMagnitude;
            return distance <= maximumSqrDistance;
        }

        public bool InBuildingRange(GameObject building)
        {
            return this.CheckRange(building, 16.0f);
        }

        void OnTriggerEnter(Collider coll)
        {
            if (!doingTask && !agentData.goalsDone)
            {
                if (agentData.personality.GetType() != typeof(EgocentricAgent))
                {
                    previousAction = currentAction;
                    protectGoal.insistenceValue += protectGoal.changeRate;
                    agentData.socialDistance = true;
                    worldChanged = true;
                }
                var agentControl = coll.gameObject.GetComponent<AgentControl>();
                if (agentControl) { agentsAroundMeList.Add(agentControl.agentData); }
            }
        }

        void OnTriggerStay(Collider coll)
        {

            if (Time.time > triggerTime && !agentData.symptoms)
            {
                triggerTime += Time.time + TRIGGER_INTERVAL;
                if (!agentData.infected && CheckForInfection())
                {
                    agentData.infected = true;
                    GameManager.infectedNumber++;
                    infectedVisualIndicator.SetActive(true);

                    int r = Random.Range(0, 100);

                    if (r <= 50)
                    {
                        agentData.symptoms = true;
                        GameManager.symptomsNumber++;
                    }

                }

                if (coll.GetComponent<AgentControl>() != null && coll.GetComponent<AgentControl>().agentData.infected)
                    timeOfContact += 0.5f;
            }

        }

        void OnTriggerExit(Collider coll)
        {
            if (agentData.personality.GetType() != typeof(EgocentricAgent))
                protectGoal.insistenceValue -= protectGoal.changeRate;

            var agentControl = coll.gameObject.GetComponent<AgentControl>();
            if (agentControl) agentsAroundMeList.Remove(agentControl.agentData);
        }
    }
}