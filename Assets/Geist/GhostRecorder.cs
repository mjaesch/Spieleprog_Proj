using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Quelle: https://www.youtube.com/watch?v=c5G2jv7YCxM
/// </summary>
public class NewBehaviourScript : MonoBehaviour
{
    public Ghost ghost;
    private float timer;
    private float timeValue;

    private void Awake()
    {
        if(ghost.recordOrPlay == Ghost.RecordOrPlay.Record)
        {
            ghost.ResetData();
            timeValue = 0;
            timer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        if(ghost.recordOrPlay == Ghost.RecordOrPlay.Record && timer >= 1 / ghost.recordFrequency)
        {
            ghost.timeStamp.Add(timeValue);
            ghost.position.Add(this.transform.position);
            ghost.rotation.Add(this.transform.eulerAngles);

            timer = 0;
        }
    }
}
