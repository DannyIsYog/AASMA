using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int time; // starts at 0, represents minutes
    public int sizeOfSlots; // 60 by default to simulate one hour
    public int currentSlot;
    public int worldSpeed = 1; // how fast the world ticks, 1 by default
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
    //TODO state of the world
}
