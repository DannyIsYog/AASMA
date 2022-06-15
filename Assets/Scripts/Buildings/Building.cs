using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Algorithm.DecisionMaking.ForwardModel;

public class Building : MonoBehaviour
{
    public int numberOfPeople = 0; // how many people are inside the building
    public float taskLenght; // how long the task takes, in seconds
    public BuildingTypes typeOfBuilding;

    public float delay = 1.0f;
    public int riskOfInfection; // maybe we want some places to have a higher risk of infection then others?
    public enum BuildingTypes { Supermarket, Hospital, Bank, Restaurant, Park, Work }

    //call this when the agent wants to start a task on this buidling
    public void startTask(GameObject agent)
    {
        StartCoroutine(endTask(agent, taskLenght));
    }

    IEnumerator endTask(GameObject agent, float delay)
    {
        yield return new WaitForSeconds(delay);

        //place here what function to call to warn the agent that the task it did is finished
        //maybe it recieves a bool stating if it got infected while in the buidling
    }

}
