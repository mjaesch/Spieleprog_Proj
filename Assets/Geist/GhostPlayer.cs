using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Quelle: https://www.youtube.com/watch?v=c5G2jv7YCxM
/// </summary>
public class GhostPlayer : MonoBehaviour
{
    public Ghost ghost;
    private float timeValue;
    private int index1;
    private int index2;

    // Start is called before the first frame update
    void Start()
    {
        timeValue = 0;
        if(ghost.ghostState == Ghost.GhostState.LoadNPlayJSON)
        {
            ghost.LoadFromJSON();
        }
    }


    // Update is called once per frame
    void Update()
    {
        timeValue += Time.unscaledDeltaTime;

        if(ghost.ghostState == Ghost.GhostState.Play || ghost.ghostState == Ghost.GhostState.LoadNPlayJSON)
        {
            GetIndex();
            SetTransform();
        }
    }

    private void GetIndex()
    {
        for(int i = 0; i < ghost.timeStamp.Count -2; i++)
        {
            if(ghost.timeStamp[i] == timeValue)
            {
                index1 = i;
                index2 = i;
                return;
            }
            else if(ghost.timeStamp[i] < timeValue && timeValue < ghost.timeStamp[i + 1])
            {
                index1 = i;
                index2 = i + 1;
                return;
            }
        }

        index1 = ghost.timeStamp.Count - 1;
        index2 = ghost.timeStamp.Count - 1;
    }


    private void SetTransform()
    {
        if (index1 == index2)
        {
            this.transform.position = ghost.position[index1];
            this.transform.eulerAngles = ghost.rotation[index1];
        }
        else
        {
            float interpolationFactor = (timeValue - ghost.timeStamp[index1]) / (ghost.timeStamp[index2] - ghost.timeStamp[index1]);

            this.transform.position = Vector3.Lerp(ghost.position[index1], ghost.position[index2], interpolationFactor);
            this.transform.eulerAngles = Vector3.Lerp(ghost.rotation[index1], ghost.rotation[index2], interpolationFactor);
        }
    }
}
