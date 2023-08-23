using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


/// <summary>
/// Quelle: https://www.youtube.com/watch?v=c5G2jv7YCxM
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ghost", order = 1)]
public class Ghost : ScriptableObject
{
    public enum GhostState { Record, Play, LoadNPlayJSON, SaveToJSON };
    public GhostState ghostState;
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

    public void SaveToJSON()
    {
        string ghostJson = JsonUtility.ToJson(this);
        string path = Application.persistentDataPath + "/GhostData.json";
        File.WriteAllText(path, ghostJson);
    }

    public void LoadFromJSON()
    {
        string ghostJson = File.ReadAllText(Application.persistentDataPath + "/GhostData.json");
        JsonUtility.FromJsonOverwrite(ghostJson, this);
        this.ghostState = GhostState.LoadNPlayJSON;
    }
}
