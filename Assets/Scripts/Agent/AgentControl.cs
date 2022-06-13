using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Assets.Scripts.GameManager;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using Assets.Scripts.Algorithm.DecisionMaking.GOB;
using Assets.Scripts.Algorithm.DecisionMaking.Actions;
using TMPro;

namespace Assets.Scripts.Agent
{
    public class AgentControl : MonoBehaviour
    {
        //constants
        public const string DO_TASKS_GOAL = "DoTasks";
        public const string PROTECT_GOAL = "Noninfected";
        public const string BE_QUICK_GOAL = "BeQuick";

        public const float DECISION_MAKING_INTERVAL = 200.0f;
        public const float GOAL_CHANGING_INTERVAL = 20.0f;
        public const float TEST_INTERVAL = 20.0f;
        /*public const float RESTING_INTERVAL = 5.0f;
        public const int REST_HP_RECOVERY = 2;*/

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

        [Header("Decision Algorithm Options")]
        public bool GOBActive;
        public bool GOAPActive;

        [Header("Character Info")]
        public bool symptoms = false;
        public bool infected = false;
        public bool quarantine = false;
        //public bool Resting = false;
        public float StopRestTime;

        //Goals
        public Goal doTasksGoal { get; private set; }
        public Goal protectGoal { get; private set; }
        public Goal beQuickGoal { get; private set; }
        public Goal goHospitalGoal { get; private set; }
        public Goal goSupermarketGoal { get; private set; }
        public Goal goBankGoal { get; private set; }
        public Goal goRestaurantGoal { get; private set; }
        public Goal goParkGoal { get; private set; }
        public Goal goWorkGoal { get; private set; }

    

        public List<Goal> Goals { get; set; }
        public List<Action> Actions { get; set; }
        public Action currentAction { get; private set; }
        public GOBDecisionMaking GOBDecisionMaking { get; set; }
        public DepthLimitedGOAPDecisionMaking GOAPDecisionMaking { get; set; }

        //private fields for internal use only
        private NavMeshAgent agent;
        private float nextUpdateTime = 0.0f;

        //This speed is only a pointer to the NavMeshAgent's speed
        public float maxSpeed {get; private set;}
        public TextMeshProUGUI playerText;
        private GameObject closestObject;

        public Vector3 origin { get; private set; }
        private Vector3 previousTarget;
        public int actionsNumber = 3;
        public bool init = false;

        //agent variables
        public AgentData agentData;
        public Building quarantineCenter;
        public bool worldChanged = false;
        public float testInterval = 0.0f;


        public void Init(GameObject person, Personality p)
        {

            //This is the actual speed of the agent
            this.agent = this.GetComponent<NavMeshAgent>();
            this.agentData = new AgentData(person, new Goal[0], p);
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

            this.origin = this.transform.position;

            // generate agent goals
            GenerateGoals();
            // generate actions
            GenerateActions();

            // Initialization of Decision Making Algorithms
            var worldModel = new CurrentStateWorldModel(agentData, GameManager, this.Actions, this.Goals);
            //this.GOBDecisionMaking = new GOBDecisionMaking(this.Actions, this.Goals);
            this.GOAPDecisionMaking = new DepthLimitedGOAPDecisionMaking(worldModel, this.Actions, this.Goals);
            
            DiaryText.text += "My Diary \n I awoke. What a wonderful day to NOT spread a virus! \n";
        }

        void Update()
        {
            if (!init || GameManager.gameEnded || agentData.quarantined) 
                return;

            //Every x amount of times we've got to update things
            if (Time.time > this.nextUpdateTime || this.worldChanged)
            {
                this.worldChanged = false;
                this.nextUpdateTime = Time.time + DECISION_MAKING_INTERVAL;

                //first step, perceptions
                //update the agent's goals based on the state of the world
                /*this.SurviveGoal.InsistenceValue = (GameManager.characterData.MaxHP - GameManager.characterData.HP) + (5 - GameManager.characterData.ShieldHP);

                this.GetManaGoal.InsistenceValue = GameManager.characterData.MaxMana - GameManager.characterData.Mana;*/

                this.doTasksGoal.insistenceValue += GOAL_CHANGING_INTERVAL * this.doTasksGoal.changeRate;
                /*if(this.doTasksGoal.insistenceValue > 10.0f)
                    this.doTasksGoal.insistenceValue = 100.0f;*/

                this.protectGoal.insistenceValue += GOAL_CHANGING_INTERVAL * this.protectGoal.changeRate;
                /*if(this.protectGoal.insistenceValue > 10.0f)
                    this.protectGoal.insistenceValue = 100.0f;*/

                this.beQuickGoal.insistenceValue += GOAL_CHANGING_INTERVAL * this.beQuickGoal.changeRate;
                if(this.beQuickGoal.insistenceValue > 10.0f)
                    this.beQuickGoal.insistenceValue = 10.0f;


                this.DoTasksGoalText.text = "Do Tasks: " + this.doTasksGoal.insistenceValue;
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
                changeRate = 0.5f
            };

            this.protectGoal = new Goal(PROTECT_GOAL, 5f)
            {
                changeRate = 0.5f
            };

            this.beQuickGoal = new Goal(BE_QUICK_GOAL, 3f)
            {
                changeRate = 0.02f
            };

            this.Goals = new List<Goal>();
            this.Goals.Add(this.doTasksGoal);
            this.Goals.Add(this.beQuickGoal);
            this.Goals.Add(this.protectGoal);
        }

        public void GenerateActions()
        {
            this.Actions = new List<Action>();

            // Egocentric types do not protect, test or quarantine
            if (agentData.personality.GetType() != typeof(EgocentricAgent)){

                this.Actions.Add(new UseMask(this));

                // spawn of agent
                foreach (var quarantine in GameObject.FindGameObjectsWithTag("Quarantine"))
                    this.Actions.Add(new Quarantine(this, quarantine));

                foreach (var hospital in GameObject.FindGameObjectsWithTag("Hospital"))
                    this.Actions.Add(new Test(this, hospital));
            }


            for(int i = 0; i < actionsNumber; i++){

                //TODO verification no more actions than those that exist
                if (actionsNumber > System.Enum.GetValues(typeof(Building.BuildingTypes)).Length)
                    return;

                List<int> randomList = new List<int>();
                int r = Random.Range(1,6);

                while(randomList.Contains(r))
                    r = Random.Range(1,6);

                randomList.Add(r);

                Building.BuildingTypes actionName = (Building.BuildingTypes) r;
                Debug.Log(actionName.ToString());
                switch(actionName)
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
            bool newDecision = false;
            if (this.GOAPDecisionMaking.InProgress)
            {
                //choose an action using the GOB Decision Making process
                var action = this.GOAPDecisionMaking.ChooseAction();

                if (action != null && action != this.currentAction)
                {
                    this.currentAction = action;
                    newDecision = true;
                }
            }

            this.TotalProcessingTimeText.text = "Process. Time: " + this.GOAPDecisionMaking.TotalProcessingTime.ToString("F");
            this.BestDiscontentmentText.text = "Best Discontentment: " + this.GOAPDecisionMaking.BestDiscontentmentValue.ToString("F");
            this.ProcessedActionsText.text = "Act. comb. processed: " + this.GOAPDecisionMaking.TotalActionCombinationsProcessed;

            if (this.GOAPDecisionMaking.BestAction != null)
            {
                if (newDecision)
                    AddToDiary("I decided to " + GOAPDecisionMaking.BestAction.name);

                var actionText = "";
                foreach (var action in this.GOAPDecisionMaking.BestActionSequence)
                    if(action != null) actionText += "\n" + action.name;

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
            if(!this.previousTarget.Equals(targetPosition))
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
            if(InBuildingRange(building))
            {
                                
                this.AddToDiary("I went to " + building.name );

                List<string> newList = new List<string>(agentData.disposableActions);

                foreach(string action in agentData.disposableActions)
                    if (action.Equals(building.name))
                        newList.Remove(action);
                
                agentData.disposableActions = new List<string>(newList);

                StartCoroutine(WaitAction(building.name, delay));

                if(CheckForSymptoms()){
                    agentData.symptoms = true;
                    GameManager.symptomsNumber++;
                }

                this.worldChanged = true;

            }
        }

        public void UseMask()
        {
            this.AddToDiary(" I put on a mask");
            agentData.usingMask = true;
            GameManager.protectingNumber++;
            this.worldChanged = true;
        }

        public void Test(GameObject building)
        {
            if(InBuildingRange(building)){
                int r = Random.Range(0,100);
                bool result = false;
                if (r < 50)
                    result = true;
            
                this.AddToDiary("I tested myself and it came back " + result);

                if(result){
                    agentData.infected = true;
                    GameManager.infectedNumber++;
                }
                else
                    agentData.symptoms = false;

                testInterval = Time.time + TEST_INTERVAL;
                this.worldChanged = true;
            }
        }

        public void Quarantine(GameObject building, float delay)
        {
            if(InBuildingRange(building))
            {
                this.AddToDiary( " I went to " + building.name );
                StartCoroutine(WaitAction(building.name, delay));
                agentData.quarantined = true;
                GameManager.quarantinedNumber++;
                this.worldChanged = true;
            }
        }

        private bool CheckForSymptoms()
        {
            int r = Random.Range(0,100);

            if(agentData.usingMask)
                return r < 25;
            
            if(agentData.quarantined){
                /*agentData.symptoms = false;
                agentData.infected = false;
                agentData.quarantined = false;
                return false;*/
                return true;
            }

            return r < 50;
        }

        private System.Collections.IEnumerator WaitAction(string building, float delay){
            yield return new WaitForSeconds(delay);
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
    }
}