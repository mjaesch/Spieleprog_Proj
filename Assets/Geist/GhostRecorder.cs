using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Quelle: https://www.youtube.com/watch?v=c5G2jv7YCxM
/// </summary>
public class GhostRecorder : MonoBehaviour
{
    public Ghost ghost;
    private float timer;
    private float timeValue;

    private void Awake()
    {
        if(ghost.ghostState == Ghost.GhostState.Record)
        {
            ghost.ResetData();
            timeValue = 0;
            timer = 0;
        } else if(ghost.ghostState == Ghost.GhostState.SaveToJSON)
        {
            ghost.SaveToJSON();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        if(ghost.ghostState == Ghost.GhostState.Record && timer >= 1 / ghost.recordFrequency)
        {
            ghost.timeStamp.Add(timeValue);
            ghost.position.Add(this.transform.position);
            ghost.rotation.Add(this.transform.eulerAngles);

            timer = 0;
        }
    }
}
