﻿using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameManager
{
    public class FutureStateWorldModel : WorldModel
    {
        protected GameManager GameManager { get; set; }
        protected int NextPlayer { get; set; }
        protected Action NextEnemyAction { get; set; }
        protected Action[] NextEnemyActions { get; set; }

        public FutureStateWorldModel(GameManager gameManager, List<Action> actions) : base(actions)
        {
            this.GameManager = gameManager;
            this.NextPlayer = 0;
        }

        public FutureStateWorldModel(FutureStateWorldModel parent) : base(parent)
        {
            this.GameManager = parent.GameManager;
            this.NextPlayer = parent.NextPlayer;
            this.NextEnemyAction = parent.NextEnemyAction;
            this.NextEnemyActions = parent.NextEnemyActions;
        }

        public override WorldModel GenerateChildWorldModel()
        {
            return new FutureStateWorldModel(this);
        }

        public override float GetScore()
        {
           /* int money = (int)this.GetProperty(Properties.MONEY);
            int HP = (int)this.GetProperty(Properties.HP);
            float time = (float)this.GetProperty(Properties.TIME);
            if (HP <= 0 || time > 200) return 0.0f;
            else if (money == 25)
            {
                return 1.0f;
            }
            else {
                float moneyWeight = 0.50f;
                float timeWeight = 0.30f;
                float healthWeight = 0.20f;
                return  moneyWeight * money/25 + timeWeight * (1 - time/200) + healthWeight * (1 - HP/30);
            }*/
            return 0;
        }

        public override int GetNextPlayer()
        {
            return this.NextPlayer;
        }

        public override void CalculateNextPlayer()
        {
            /*Vector3 position = (Vector3)this.GetProperty(Properties.POSITION);
            bool enemyEnabled;

            //basically if the character is close enough to an enemy, the next player will be the enemy.
            foreach (var enemy in this.GameManager.enemies)
            {
                enemyEnabled = (bool) this.GetProperty(enemy.name);
                if (enemyEnabled && (enemy.transform.position - position).sqrMagnitude <= 100)
                {
                    this.NextPlayer = 1;
                    this.NextEnemyAction = new SwordAttack(this.GameManager.autonomousCharacter, enemy);
                    this.NextEnemyActions = new Action[] { this.NextEnemyAction };
                    return;
                }
            }
            this.NextPlayer = 0;
            //if not, then the next player will be player 0*/
        }

        public override Action GetNextAction()
        {
            Action action;
            if (this.NextPlayer == 1)
            {
                action = this.NextEnemyAction;
                this.NextEnemyAction = null;
                return action;
            }
            else return base.GetNextAction();
        }

        public override Action[] GetExecutableActions()
        {
            if (this.NextPlayer == 1)
            {
                return this.NextEnemyActions;
            }
            else return base.GetExecutableActions();
        }

    }
}
