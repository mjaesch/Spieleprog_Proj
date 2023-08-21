using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Quelle: https://www.youtube.com/watch?v=c5G2jv7YCxM
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ghost", order = 1)]
public class Ghost : ScriptableObject
{
    public enum RecordOrPlay { Record, Play };
    public RecordOrPlay recordOrPlay;
    public float recordFrequency;

    public List<float> timeStamp;
    public List<Vector3> position;
    public List<Vector3> rotation;

    public void ResetData()
    {
        timeStamp.Clear();
        position.Clear();
        rotation.Clear();
    }
}
