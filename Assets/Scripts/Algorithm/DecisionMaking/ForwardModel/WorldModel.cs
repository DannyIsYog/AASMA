using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.Algorithm.DecisionMaking.ForwardModel
{
    public class WorldModel
    {
        public const int TIME_LIMIT = 200;

        private Dictionary<string, object> properties { get; set; }
        private Dictionary<string, float> goalValues { get; set; } 
        public UnityEditor.U2D.Animation.CharacterData characterData { get; private set; }
        private List<Action> actions { get; set; }
        protected IEnumerator<Action> actionEnumerator { get; set; } 
        protected WorldModel parent { get; set; }

        public WorldModel(List<Action> actions)
        {
            this.properties = new Dictionary<string, object>();
            this.goalValues = new Dictionary<string, float>();
            this.actions = new List<Action>(actions);
            this.actions.Shuffle();
            this.actionEnumerator = this.actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
            this.properties = new Dictionary<string, object>();
            this.goalValues = new Dictionary<string, float>();
            this.actions = new List<Action>(parent.actions);
            this.actions.Shuffle();
            this.parent = parent;
            this.actionEnumerator = this.actions.GetEnumerator();
        }

        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
            if (this.properties.ContainsKey(propertyName))
                return this.properties[propertyName];
            else if (this.parent != null)
                return this.parent.GetProperty(propertyName); //TODO possibly
            else
                return null;
            
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            this.properties[propertyName] = value;
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            if (this.goalValues.ContainsKey(goalName))
                return this.goalValues[goalName];
            else if (this.parent != null)
                return this.parent.GetGoalValue(goalName);
            else
                return 0;
            
        }

        public virtual void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
                limitedValue = 10.0f;
            else if (value < 0.0f)
                limitedValue = 0.0f;
            
            this.goalValues[goalName] = limitedValue;
        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;
            /*if((int)GetProperty(Assets.Scripts.GameManager.Properties.HP) <= 0) {
                return float.MaxValue;
            }*/
            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.name);
                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.actionEnumerator.MoveNext())
                action = this.actionEnumerator.Current;
                
            while (action != null && !action.CanExecute(this))
            {
                if (this.actionEnumerator.MoveNext())
                    action = this.actionEnumerator.Current;    
                else
                    action = null;
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return this.actions.Where(a => a.CanExecute(this)).ToArray();
        }
        

        public virtual float GetScore()
        {
            /*if(this.characterData.HP <= 0 || this.characterData.Time >= TIME_LIMIT)
                return 0;
            
            else if(this.characterData.Money >= 25)
                return 1;*/
            return 0;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }

        public List<Action> GetActions(){
            return this.actions;
        }
    }
}
