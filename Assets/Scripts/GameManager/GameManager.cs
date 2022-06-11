using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Agent;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;

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

        [Header("Enemy Settings")]
        public bool StochasticWorld;

        [Header("People specs")]
        public int number;
        public GameObject personPrefab;
        public GameObject[] people;
        public int selfAwarePercentage;
        public int egocentricPercentage;
        public int hypochondriacPercentage;


        //fields
        /*public List<GameObject> chests { get; set; }
        public int maxChests = 0;
        public Dictionary<string, List<GameObject>> disposableObjects { get; set; }
        public int maxHpPotions = 0;
        public int maxManaPotions = 0;*/

        public bool WorldChanged { get; set; }

        private float nextUpdateTime = 0.0f;
        private float enemyAttackCooldown = 0.0f;
        public bool gameEnded { get; set; } = false;
        public Vector3 initialPosition { get; set; }


        void Awake()
        {
            this.WorldChanged = false;
            GeneratePeople(number);
        }

        public void Update()
        {
            PersonData personData = this.people[0].GetComponent<PersonControl>().personData;

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + UPDATE_INTERVAL;
                personData.time += UPDATE_INTERVAL;
            }

            //TODO to change when we have multiple agents
            this.SymptomsText.text = "Symptoms: " + personData.symptoms;
            this.InfectedText.text = "Infected: " + personData.infected;
            this.QuarantinedText.text = "Quarantined: " + personData.quarantined;
            this.UsingMaskText.text = "UsingMask: " + personData.usingMask;
            this.GoalsText.text = "Goals:";
            this.TimeText.text = "Time: " + personData.time;

            foreach(Goal goal in personData.goals.Values)
                this.GoalsText.text = this.GoalsText.text + " " + goal.name;

            /*if(this.peopleData[0].HP <= 0 || this.peopleData[0].Time >= TIME_LIMIT)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "You Died";
            }
        }

      
        public void GoHealthcareCenter(GameObject healthcareCenter)
        {
          
            if (healthcareCenter != null && healthcareCenter.activeSelf && InChestRange(healthcareCenter))
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

        public void GeneratePeople(int number)
        {
            int selfAwareAgents = (number*selfAwarePercentage)/100;
            int egocentricAgents = (number*egocentricPercentage)/100;
            int hypochondriacAgents = (number*hypochondriacPercentage)/100;

            for(int i = 0; i < number; i++)
            {
                GameObject person = Instantiate(personPrefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
                
                Personality p = null;
                //this.initialPosition = this.character.transform.position;

                //self-aware
                if (i < selfAwareAgents){
                    person.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    p = new SelfAwareAgent();
                }
                //egocentric
                else if (i >= selfAwareAgents){
                    person.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    p = new EgocentricAgent();
                }
                //hypochondriac
                else if (i >= selfAwareAgents + egocentricAgents){
                    person.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                    p = new HypochondriacAgent();
                }

                this.people[i] = person;
                person.GetComponent<PersonControl>().personData = new PersonData(person, new Goal[0], p);
            }
        }
    }
}
