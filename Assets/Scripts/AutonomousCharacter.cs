using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;

namespace Assets.Scripts
{
    public class AutonomousCharacter : MonoBehaviour
    {
        //constants
        public const string DO_TASKS_GOAL = "DoTasks";
        public const string NONINFECTED_GOAL = "Noninfected";
        public const string BE_QUICK_GOAL = "BeQuick";
        
        /*public const string GET_RICH_GOAL = "GetRich";
        public const string GET_MANA_GOAL = "GetMana";*/

        public const float DECISION_MAKING_INTERVAL = 200.0f;
        public const float GOAL_CHANING_INTERVAL = 20.0f;
        /*public const float RESTING_INTERVAL = 5.0f;
        public const int REST_HP_RECOVERY = 2;*/

        //UI Variables
        private Text DoTasksGoalText;
        private Text NoninfectedGoalText;
        private Text BeQuickGoalText;
        /*private Text GetRichGoalText;
        private Text GetManaGoalText;*/
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
        public bool Resting = false;
        public float StopRestTime;

        public Goal BeQuickGoal { get; private set; }
        public Goal DoTasksGoal { get; private set; }
        public Goal NoninfectedGoal { get; private set; }

        /*public Goal GetRichGoal { get; private set; }
        public Goal GetManaGoal { get; private set; }*/

        public List<Goal> Goals { get; set; }
        public List<Action> Actions { get; set; }
        public Action CurrentAction { get; private set; }
        public GOBDecisionMaking GOBDecisionMaking { get; set; }
        public DepthLimitedGOAPDecisionMaking GOAPDecisionMaking { get; set; }

        //private fields for internal use only
        private NavMeshAgent agent;
        private float nextUpdateTime = 0.0f;
        /*private float previousGold = 0.0f;
        private int previousLevel = 1;
        private Vector3 previousTarget;*/

        //This speed is only a pointer to the NavMeshAgent's speed
        public float maxSpeed {get; private set;}
        public TextMesh playerText;
        private GameObject closestObject;

        public Vector3 origin { get; private set; }

        public void Start()
        {
            //This is the actual speed of the agent
            this.agent = this.GetComponent<NavMeshAgent>();
            maxSpeed = this.agent.speed;
            GameManager = GameObject.Find("Manager").GetComponent<GameManager.GameManager>();
            playerText.text = "";

            // Initializing UI Text
            this.BeQuickGoalText = GameObject.Find("BeQuickGoal").GetComponent<Text>();
            this.DoTasksGoalText = GameObject.Find("SurviveGoal").GetComponent<Text>();
            this.NoninfectedGoalText = GameObject.Find("GainXP").GetComponent<Text>();
            
            /*this.GetRichGoalText = GameObject.Find("GetRichGoal").GetComponent<Text>();
            this.GetManaGoalText = GameObject.Find("GetManaGoal").GetComponent<Text>();*/
            
            this.DiscontentmentText = GameObject.Find("Discontentment").GetComponent<Text>();
            this.TotalProcessingTimeText = GameObject.Find("ProcessTime").GetComponent<Text>();
            this.BestDiscontentmentText = GameObject.Find("BestDicont").GetComponent<Text>();
            this.ProcessedActionsText = GameObject.Find("ProcComb").GetComponent<Text>();
            this.BestActionText = GameObject.Find("BestAction").GetComponent<Text>();
            this.BestActionSequence = GameObject.Find("BestActionSequence").GetComponent<Text>();
            DiaryText = GameObject.Find("DiaryText").GetComponent<Text>();

            this.origin = this.transform.position;
            //initialization of the GOB decision making
            //let's start by creating 4 main goals
            this.DoTasksGoal = new Goal(DO_TASKS_GOAL, 8f);

            this.NoninfectedGoal = new Goal(NONINFECTED_GOAL, 10.0f)
            {
                ChangeRate = 0.1f
            };

            this.BeQuickGoal = new Goal(BE_QUICK_GOAL, 3f)
            {
                ChangeRate = 0.02f
            };

            /*this.GetRichGoal = new Goal(GET_RICH_GOAL, 1f)
            {
                InsistenceValue = 10.0f,
                ChangeRate = 0.2f
            };

            this.GetManaGoal = new Goal(GET_MANA_GOAL, 1.0f);*/

            this.Goals = new List<Goal>();
            this.Goals.Add(this.DoTasksGoal);
            this.Goals.Add(this.BeQuickGoal);
            this.Goals.Add(this.NoninfectedGoal);

            /*this.Goals.Add(this.GetRichGoal);
            this.Goals.Add(this.GetManaGoal);*/

            //initialize the available actions
            //Uncomment commented actions after you implement them

            this.Actions = new List<Action>();

            this.Actions.Add(new DoPreventiveMeasures(this));
            this.Actions.Add(new Test(this));

            //quarantine in a building maybe?
            this.Actions.Add(new Quarantine(this));

            foreach (var healthcareCenter in GameObject.FindGameObjectsWithTag("HealthcareCenter"))
            {
                this.Actions.Add(new Test(this, healthcareCenter));
            } 

            // Initialization of Decision Making Algorithms
            var worldModel = new CurrentStateWorldModel(GameManager, this.Actions, this.Goals);
            this.GOBDecisionMaking = new GOBDecisionMaking(this.Actions, this.Goals);
            this.GOAPDecisionMaking = new DepthLimitedGOAPDecisionMaking(worldModel,this.Actions,this.Goals);
            
            this.Resting = false;

            DiaryText.text += "My Diary \n I awoke. What a wonderful day to kill Monsters! \n";
        }

        void Update()
        {
            if (GameManager.gameEnded) return;

            //Every x amount of times we've got to update things
            if (Time.time > this.nextUpdateTime || GameManager.WorldChanged)
            {
                GameManager.WorldChanged = false;
                this.nextUpdateTime = Time.time + DECISION_MAKING_INTERVAL;

                //first step, perceptions
                //update the agent's goals based on the state of the world
                this.SurviveGoal.InsistenceValue = (GameManager.characterData.MaxHP - GameManager.characterData.HP) + (5 - GameManager.characterData.ShieldHP);

                this.GetManaGoal.InsistenceValue = GameManager.characterData.MaxMana - GameManager.characterData.Mana;

                this.BeQuickGoal.InsistenceValue += GOAL_CHANING_INTERVAL * this.BeQuickGoal.ChangeRate;
                if(this.BeQuickGoal.InsistenceValue > 10.0f)
                {
                    this.BeQuickGoal.InsistenceValue = 10.0f;
                }

                if(GameManager.characterData.Level > this.previousLevel)
                {
                    this.GainLevelGoal.InsistenceValue = 0;
                    this.previousLevel = GameManager.characterData.Level;
                }
                else {
                    this.GainLevelGoal.InsistenceValue += this.GainLevelGoal.ChangeRate; //increase in goal over time
                }

                this.GetRichGoal.InsistenceValue += this.GetRichGoal.ChangeRate; //increase in goal over time
                if (this.GetRichGoal.InsistenceValue > 10)
                {
                    this.GetRichGoal.InsistenceValue = 10.0f;
                }

                if (GameManager.characterData.Money > this.previousGold)
                {
                    this.GetRichGoal.InsistenceValue -= 2; 
                    this.previousGold = GameManager.characterData.Money;
                }

                this.SurviveGoalText.text = "Do Tasks: " + this.SurviveGoal.InsistenceValue;
                this.GainXPGoalText.text = "Noninfected: " + this.GainLevelGoal.InsistenceValue.ToString("F1");
                this.BeQuickGoalText.text = "Be Quick: " + this.BeQuickGoal.InsistenceValue.ToString("F1");
                this.DiscontentmentText.text = "Discontentment: " + this.CalculateDiscontentment().ToString("F1");

                    //To have a new decision lets initialize Decision Making Proccess
                    this.CurrentAction = null;
                    if(GOAPActive)
                        this.GOAPDecisionMaking.InitializeDecisionMakingProcess();
                    else if (GOBActive)
                            this.GOBDecisionMaking.InProgress = true;
            }

            if(this.GOAPActive)
            {
                this.UpdateDLGOAP();
            }
            else if (this.GOBActive)
            {
                this.UpdateGOB();
            }

            if (this.CurrentAction != null)
            {
                if (this.CurrentAction.CanExecute())
                {
                    this.CurrentAction.Execute();
                }                        
            }
        }

        public void AddToDiary(string s)
        {
            DiaryText.text += Time.time + s + "\n";

            //If the diary gets too large we cut it. Plain and simple
            if (DiaryText.text.Length > 600)
                DiaryText.text = DiaryText.text.Substring(500);
        }

        private void UpdateGOB()
        {
            
            bool newDecision = false;
            if (this.GOBDecisionMaking.InProgress)
            {
                //choose an action using the GOB Decision Making process
                var action = this.GOBDecisionMaking.ChooseAction();
                if (action != null && action != this.CurrentAction)
                {
                    this.CurrentAction = action;
                    newDecision = true;
                    if (newDecision)
                    {
                        AddToDiary(Time.time + " I decided to " + action.Name);
                        this.BestActionText.text = "Best Action: " + action.Name + "\n";
                    }

                }

            }

        }

        private void UpdateDLGOAP()
        {
            bool newDecision = false;
            if (this.GOAPDecisionMaking.InProgress)
            {
                //choose an action using the GOB Decision Making process
                var action = this.GOAPDecisionMaking.ChooseAction();
                if (action != null && action != this.CurrentAction)
                {
                    this.CurrentAction = action;
                    newDecision = true;
                }
            }

            this.TotalProcessingTimeText.text = "Process. Time: " + this.GOAPDecisionMaking.TotalProcessingTime.ToString("F");
            this.BestDiscontentmentText.text = "Best Discontentment: " + this.GOAPDecisionMaking.BestDiscontentmentValue.ToString("F");
            this.ProcessedActionsText.text = "Act. comb. processed: " + this.GOAPDecisionMaking.TotalActionCombinationsProcessed;

            if (this.GOAPDecisionMaking.BestAction != null)
            {
                if (newDecision)
                {
                    AddToDiary(Time.time + " I decided to " + GOAPDecisionMaking.BestAction.Name);
                }
                var actionText = "";
                foreach (var action in this.GOAPDecisionMaking.BestActionSequence)
                {
                    if(action != null) actionText += "\n" + action.Name;
                }
                this.BestActionSequence.text = "Best Action Sequence: " + actionText;
                this.BestActionText.text = "Best Action: " + GOAPDecisionMaking.BestAction.Name;
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
    }
}