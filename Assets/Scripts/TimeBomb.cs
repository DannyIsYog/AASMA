using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBomb : MonoBehaviour
{
    public float timetoDeath;
    float timeElapsed;

    private void OnEnable() {
        timeElapsed = 0;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= timetoDeath) {
            gameObject.SetActive(false);
        }
    }
}
