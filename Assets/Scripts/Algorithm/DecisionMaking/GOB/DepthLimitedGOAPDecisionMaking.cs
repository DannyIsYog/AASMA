using Assets.Scripts.GameManager;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;

namespace Assets.Scripts.Algorithm.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.goals = goals;
        
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var processedActions = 0;

            var startTime = Time.realtimeSinceStartup;
            
            while(this.CurrentDepth >= 0) {

                /*foreach (Action action in ActionPerLevel)
                {
                    if (action.name == "GoalsFinished" && CurrentDepth == 0)
                        return action;
                    else if (action.name == "Quarantine" && CurrentDepth == 0)
                        return action;
                }*/

                if(CurrentDepth >= MAX_DEPTH) {
                    float value = Models[CurrentDepth].CalculateDiscontentment(goals);
                    
                    if(value < BestDiscontentmentValue) {
                       BestDiscontentmentValue = value;
                       BestAction = ActionPerLevel[0];
                       for (int i = 0; i < BestActionSequence.Length; i++)
                           BestActionSequence[i] = ActionPerLevel[i];
                    }

                    CurrentDepth --;
                }
                else {
                    Action nextAction = Models[CurrentDepth].GetNextAction();
                    if(nextAction != null) {
                        
                        if(nextAction.name == "Test" && CurrentDepth == 0) 
                            return nextAction;

                        Models[CurrentDepth+1] = Models[CurrentDepth].GenerateChildWorldModel();

                        ActionPerLevel[CurrentDepth] = nextAction;
                        nextAction.ApplyActionEffects(Models[CurrentDepth+1]);
                        CurrentDepth += 1;
                        processedActions++;
                        if(processedActions >= ActionCombinationsProcessedPerFrame)
                            return null;
                    }
                    else {
                        if (CurrentDepth > 0) {
                            float value = Models[CurrentDepth-1].CalculateDiscontentment(goals);
                            if(value < BestDiscontentmentValue) {
                                BestDiscontentmentValue = value;
                                BestAction = ActionPerLevel[0];
                                for (int i = 0; i < BestActionSequence.Length; i++)
                                    BestActionSequence[i] = ActionPerLevel[i];
                            }
                        }
                        CurrentDepth--;
                    }
                }
            }

            this.TotalActionCombinationsProcessed += processedActions;
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;
            return this.BestAction;
        }
    }
}
