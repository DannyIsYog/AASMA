using Assets.Scripts.IAJ.Unity.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Assets.Scripts.GameManager
{
    public class GameManager : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 2.0f;
        public const int TIME_LIMIT = 200;
        //public fields, seen by Unity in Editor

        public AutonomousCharacter autonomousCharacter;

        private GameObject character;

        [Header("UI Objects")]
        public Text HPText;
        public Text ShieldHPText;
        public Text ManaText;
        public Text TimeText;
        public Text XPText;
        public Text LevelText;
        public Text MoneyText;
        public Text DiaryText;
        public GameObject GameEnd;

        [Header("Enemy Settings")]
        public bool StochasticWorld;
        public bool SleepingNPCs;
        public bool BehaviourTreeNPCs;


        //fields
        public List<GameObject> chests { get; set; }
        public int maxChests = 0;
        public List<GameObject> skeletons { get; set; }
        public int maxSkeletons = 0;
        public List<GameObject> orcs { get; set; }
        public int maxOrcs = 0;
        public List<GameObject> dragons { get; set; }
        public int maxDragons = 0;
        public List<GameObject> enemies { get; set; }
        public Dictionary<string, List<GameObject>> disposableObjects { get; set; }
        public int maxHpPotions = 0;
        public int maxManaPotions = 0;

        public CharacterData characterData { get; private set; }
        public bool WorldChanged { get; set; }

        private float nextUpdateTime = 0.0f;
        private float enemyAttackCooldown = 0.0f;
        public bool gameEnded { get; set; } = false;
        public Vector3 initialPosition { get; set; }

        void Awake()
        {
            UpdateDisposableObjects();
            this.WorldChanged = false;
            this.character = this.autonomousCharacter.gameObject;
            this.characterData = new CharacterData(this.character);
            this.initialPosition = this.character.transform.position;
        }

        public void UpdateDisposableObjects()
        {
            this.enemies = new List<GameObject>();
            this.disposableObjects = new Dictionary<string, List<GameObject>>();

            this.chests = GameObject.FindGameObjectsWithTag("Chest").ToList();
            if(chests.Count > maxChests) maxChests = chests.Count;
            this.skeletons = GameObject.FindGameObjectsWithTag("Skeleton").ToList();
            this.enemies.AddRange(this.skeletons);
            if(skeletons.Count > maxSkeletons) maxSkeletons = skeletons.Count;
            this.orcs = GameObject.FindGameObjectsWithTag("Orc").ToList();
            this.enemies.AddRange(this.orcs);
            if(orcs.Count > maxOrcs) maxOrcs = orcs.Count;
            this.dragons = GameObject.FindGameObjectsWithTag("Dragon").ToList();
            this.enemies.AddRange(this.dragons);
            if(dragons.Count > maxDragons) maxDragons = dragons.Count;

            //adds all enemies to the disposable objects collection
            foreach (var enemy in this.enemies)
            {
                if (disposableObjects.ContainsKey(enemy.name))
                {
                    this.disposableObjects[enemy.name].Add(enemy);
                }
                else this.disposableObjects.Add(enemy.name, new List<GameObject>() { enemy });
            }
            //add all chests to the disposable objects collection
            foreach (var chest in this.chests)
            {
                if (disposableObjects.ContainsKey(chest.name))
                {
                    this.disposableObjects[chest.name].Add(chest);
                }
                else this.disposableObjects.Add(chest.name, new List<GameObject>() { chest });
            }
            //adds all health potions to the disposable objects collection
            int potcount = 0;
            foreach (var potion in GameObject.FindGameObjectsWithTag("HealthPotion"))
            {
                potcount++;
                if (disposableObjects.ContainsKey(potion.name))
                {
                    this.disposableObjects[potion.name].Add(potion);
                }
                else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
            }
            if(potcount > maxHpPotions) maxHpPotions = potcount;
            potcount = 0;
            //adds all mana potions to the disposable objects collection
            foreach (var potion in GameObject.FindGameObjectsWithTag("ManaPotion"))
            {
                if (disposableObjects.ContainsKey(potion.name))
                {
                    this.disposableObjects[potion.name].Add(potion);
                }
                else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
            }
            if(potcount > maxManaPotions) maxManaPotions = potcount;
        }

        public void Update()
        {

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + UPDATE_INTERVAL;
                this.characterData.Time += UPDATE_INTERVAL;
            }


            this.HPText.text = "HP: " + this.characterData.HP;
            this.XPText.text = "XP: " + this.characterData.XP;
            this.ShieldHPText.text = "Shield HP: " + this.characterData.ShieldHP;
            this.LevelText.text = "Level: " + this.characterData.Level;
            this.TimeText.text = "Time: " + this.characterData.Time;
            this.ManaText.text = "Mana: " + this.characterData.Mana;
            this.MoneyText.text = "Money: " + this.characterData.Money;

            if(this.characterData.HP <= 0 || this.characterData.Time >= TIME_LIMIT)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "You Died";
            }
            else if(this.characterData.Money >= 25)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "Victory \n GG EZ";
            }
        }

        /*public void SwordAttack(GameObject enemy)
        {
            int damage = 0;

            NPC enemyData = enemy.GetComponent<NPC>();

            if (enemy != null && enemy.activeSelf && InMeleeRange(enemy))
            {
                this.autonomousCharacter.AddToDiary(" I Sword Attacked " + enemy.name);

                if (this.StochasticWorld)
                {
                    damage = enemyData.dmgRoll.Invoke();
 
                    //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                    int attackRoll = RandomHelper.RollD20() + 7;

                    if (attackRoll >= enemyData.AC)
                    {
                        //there was an hit, enemy is destroyed, gain xp
                        this.enemies.Remove(enemy);
                        this.disposableObjects[enemy.name].Remove(enemy);
                        enemy.SetActive(false);
                        Object.Destroy(enemy);
                    }
                }
                else
                {
                    damage = enemyData.simpleDamage;
                    this.enemies.Remove(enemy);
                    this.disposableObjects[enemy.name].Remove(enemy);
                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                }

                this.characterData.XP += enemyData.XPvalue;

                int remainingDamage = damage - this.characterData.ShieldHP;
                this.characterData.ShieldHP = Mathf.Max(0, this.characterData.ShieldHP - damage);

                if (remainingDamage > 0)
                {
                    this.characterData.HP -= remainingDamage;
                }

                this.WorldChanged = true;
            }
        }

        public void EnemyAttack(GameObject enemy)
        {
            if (Time.time > this.enemyAttackCooldown)
            {

                int damage = 0;

                NPC enemyData = enemy.GetComponent<NPC>();

                if (enemy != null && enemy.activeSelf && InMeleeRange(enemy))
                {

                    this.autonomousCharacter.AddToDiary(" I was Attacked by " + enemy.name);
                    this.enemyAttackCooldown = Time.time + UPDATE_INTERVAL;

                    if (this.StochasticWorld)
                    {
                        damage = enemyData.dmgRoll.Invoke();

                        //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                        int attackRoll = RandomHelper.RollD20() + 7;

                        if (attackRoll >= enemyData.AC)
                        {
                            //there was an hit, enemy is destroyed, gain xp
                            this.enemies.Remove(enemy);
                            this.disposableObjects.Remove(enemy.name);
                            enemy.SetActive(false);
                            Object.Destroy(enemy);
                        }
                    }
                    else
                    {
                        damage = enemyData.simpleDamage;
                        this.enemies.Remove(enemy);
                        this.disposableObjects.Remove(enemy.name);
                        enemy.SetActive(false);
                        Object.Destroy(enemy);
                    }

                    this.characterData.XP += enemyData.XPvalue;

                    int remainingDamage = damage - this.characterData.ShieldHP;
                    this.characterData.ShieldHP = Mathf.Max(0, this.characterData.ShieldHP - damage);

                    if (remainingDamage > 0)
                    {
                        this.characterData.HP -= remainingDamage;
                        this.autonomousCharacter.AddToDiary(" I was wounded with " + remainingDamage + " damage");
                    }

                    this.WorldChanged = true;
                }
            }
        }*/

      
        public void PickUpChest(GameObject chest)
        {
          
            if (chest != null && chest.activeSelf && InChestRange(chest))
            {
                this.autonomousCharacter.AddToDiary( " I opened  " + chest.name );
                this.chests.Remove(chest);
                this.disposableObjects[chest.name].Remove(chest);
                Object.Destroy(chest);
                this.characterData.Money += 5;
                this.WorldChanged = true;
            }
        }

        public void GetHealthPotion(GameObject potion)
        {
            if (potion != null && potion.activeSelf && InPotionRange(potion))
            {
                this.autonomousCharacter.AddToDiary(" I drank " + potion.name);
                this.disposableObjects[potion.name].Remove(potion);
                Object.Destroy(potion);
                this.characterData.HP = this.characterData.MaxHP;
                this.WorldChanged = true;
            }
        }

        public void GetManaPotion(GameObject manaPotion)
        {
            if (manaPotion != null && manaPotion.activeSelf && InPotionRange(manaPotion))
            {
                this.autonomousCharacter.AddToDiary(" I drank " + manaPotion.name);
                this.disposableObjects[manaPotion.name].Remove(manaPotion);
                Object.Destroy(manaPotion);
                this.characterData.Mana = this.characterData.MaxMana;
                this.WorldChanged = true;
            }
        }

        public void LevelUp()
        {
            if (this.characterData.Level >= 4) return;

            if (this.characterData.XP >= this.characterData.Level * 10)
            {
                this.characterData.Level++;
                this.characterData.MaxHP += 10;
                this.characterData.XP = 0;
                this.WorldChanged = true;
                this.autonomousCharacter.AddToDiary(" I leveled up to level " + this.characterData.Level);
            }
        }

        public void ActivateShieldOfFaith() {
            this.characterData.ShieldHP = 5;
            this.characterData.Mana -= 5;
            this.WorldChanged = true;
        }

        /*public void DivineSmite(GameObject enemy) {
            NPC enemyData = enemy.GetComponent<NPC>();

            if (enemy != null && enemy.activeSelf && InMeleeRange(enemy) && enemyData.Type == "Skeleton")
            {
                this.autonomousCharacter.AddToDiary(" I Divine Smited " + enemy.name);

                this.enemies.Remove(enemy);
                this.disposableObjects[enemy.name].Remove(enemy);
                enemy.SetActive(false);
                Object.Destroy(enemy);

                this.characterData.XP += enemyData.XPvalue;
                this.characterData.Mana -= 2;

                this.WorldChanged = true;
            }
        }*/

        private bool CheckRange(GameObject obj, float maximumSqrDistance)
        {
            var distance = (obj.transform.position - this.character.transform.position).sqrMagnitude;
            return distance <= maximumSqrDistance;
        }


        public bool InMeleeRange(GameObject enemy)
        {
            return this.CheckRange(enemy, 16.0f);
        }
     
        public bool InChestRange(GameObject chest)
        {

            return this.CheckRange(chest, 16.0f);
        }

        public bool InPotionRange(GameObject potion)
        {
            return this.CheckRange(potion, 16.0f);
        }

        float timeElapsed = -1;
        float lastTimeStamp = 0;
        public void Rest() {
            if(timeElapsed == -1) {
                timeElapsed = 0;
                lastTimeStamp = Time.time;
            }
            float newTimeStamp = Time.time;
            timeElapsed += newTimeStamp - lastTimeStamp;
            lastTimeStamp = newTimeStamp;
            if(timeElapsed >= 5.0f) {
                this.characterData.HP = Mathf.Min(this.characterData.HP + 2, this.characterData.MaxHP);
                timeElapsed = -1;
                WorldChanged = true;
            }
        }

        public bool Teleport() {
            if(this.characterData.Mana >= 5) {
                this.characterData.Mana -= 5;
                WorldChanged = true;
                return true;
            }
            else
                return false;
        }
    }
}
