using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointHUD : MonoBehaviour
{
    public CarController carController;
    public TMP_Text checkpointText; // Ziehe die UI-Textkomponente hierhin im Inspector
    private int currentCheckpoint;
    private int totalCheckpoints;

    // Start is called before the first frame update
    void Start()
    {
        // Initialisiere die UI-Textkomponente
        checkpointText.text = "Current Checkpoint: N/A";
    }

    // Update is called once per frame
    void Update()
    {
        // Aktualisiere die UI-Textkomponente mit der Position des aktuellen Checkpoints
        checkpointText.text = "Current Checkpoint: " + currentCheckpoint + " /9";
   
    }
       public void SetCurrentCheckpoint(int checkpoint)
    {
        this.currentCheckpoint = checkpoint+1;
    }
}
