using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour
{
    public float doTasks;
    public float doPreventiveMeasures;
    public float test;
    public float quarantine;
    public float beQuick;
    public float noninfected;

    public Personality(int personality)
    {
        switch(personality)
        {
            // Self-aware agent
            case 0:
                doTasks = 0.5f;
                doPreventiveMeasures = 0.5f;
                test = 0.5f;
                quarantine = 0.5f;
                beQuick = 0.5f;
                noninfected = 0.5f;
                break;
            // Egocentric agent
            case 1:
                doTasks = 1.0f;
                doPreventiveMeasures = 0f;
                test = 0f;
                quarantine = 0f;
                beQuick = 1.0f;
                noninfected = 0f;
                break;
            // Hypochondriac agent
            case 2:
                doTasks = 0.5f;
                doPreventiveMeasures = 1.0f;
                test = 1.0f;
                quarantine = 1.0f;
                beQuick = 0f;
                noninfected = 1.0f;
                break;
        }

    }


}