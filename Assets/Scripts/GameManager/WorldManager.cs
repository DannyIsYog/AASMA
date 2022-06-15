using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int time; // starts at 0, represents minutes
    public int sizeOfSlots; // 60 by default to simulate one hour
    public int currentSlot;
    public int worldSpeed = 1; // how fast the world ticks, 1 by default

    public GameObject spawnPointsParent;
    public List<GameObject> spawnPoints = new List<GameObject>();

    public Assets.Scripts.GameManager.GameManager gameManager;


    void Start()
    {
        time = 0;
        InvokeRepeating("tickTheClock", 1f, 1f);

        currentSlot = 0;
    }

    void tickTheClock()
    {
        time += worldSpeed;

        // probably we want this to "warn" all the agents when a new slots starts, for efficiency
        currentSlot = time / sizeOfSlots;
    }

    // to call when a new day starts
    void resetClock()
    {
        time = 0;
    }

    //TODO spawners

    public void spawner()
    {
        int children = spawnPointsParent.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            spawnPoints.Add(spawnPointsParent.transform.GetChild(i).gameObject);
            Debug.Log("Adding");
        }
        foreach (GameObject agent in gameManager.agents)
            agent.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count - 1)].transform.position;
    }
    //TODO state of the world
}
