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
        public Text SymptomsText;
        public Text InfectedText;
        public Text QuarantinedText;
        public Text GoalsText;
        public Text TimeText;
        public Text DiaryText;
        public GameObject GameEnd;

        [Header("Enemy Settings")]
        public bool StochasticWorld;
        public bool SleepingNPCs;
        public bool BehaviourTreeNPCs;

        //fields
        /*public List<GameObject> chests { get; set; }
        public int maxChests = 0;
        public Dictionary<string, List<GameObject>> disposableObjects { get; set; }
        public int maxHpPotions = 0;
        public int maxManaPotions = 0;*/

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

        public void Update()
        {

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + UPDATE_INTERVAL;
                this.characterData.Time += UPDATE_INTERVAL;
            }


            this.SymptomsText.text = "Symptoms: " + this.characterData.symptoms;
            this.InfectedText.text = "Infected: " + this.characterData.infected;
            this.QuarantinedText.text = "Quarantined: " + this.characterData.quarantined;
            this.GoalsText.text = "Goals:";
            this.TimeText.text = "Time: " + this.characterData.time;

            foreach(Goal goal in this.characterData.Goals)
                this.GoalsText.text = this.GoalsText.text + " " + goal.Name;

            /*if(this.characterData.HP <= 0 || this.characterData.Time >= TIME_LIMIT)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "You Died";
            }*/
        }

      
        public void GoHealthcareCenter(GameObject healthcareCenter)
        {
          
            if (healthcareCenter != null && chest.activeSelf && InChestRange(chest))
            {
                this.autonomousCharacter.AddToDiary( " I went to  " + healthcareCenter.name + " to test myself");
                //TODO give out results if positive or negative
                this.WorldChanged = true;
            }
        }

        private bool CheckRange(GameObject obj, float maximumSqrDistance)
        {
            var distance = (obj.transform.position - this.character.transform.position).sqrMagnitude;
            return distance <= maximumSqrDistance;
        }
     
        public bool InChestRange(GameObject chest)
        {

            return this.CheckRange(chest, 16.0f);
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
            /*if(timeElapsed >= 5.0f) {
                this.characterData.HP = Mathf.Min(this.characterData.HP + 2, this.characterData.MaxHP);
                timeElapsed = -1;
                WorldChanged = true;
            }*/
        }
    }
}
