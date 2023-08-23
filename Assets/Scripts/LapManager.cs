using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class LapManager : MonoBehaviour
{
    public Transform[] checkpoints;
    public TMP_Text lapTimeTextCenter; // UI Text Objekt in der Bildschirmmitte
    public TMP_Text lastLapTimeTextRight; // UI Text Objekt rechts auf dem Bildschirm

    private int currentCheckpointIndex = 0;
    private int lapCount = 0;
    private float lapStartTime = 0f;
    private float lastLapTime = 0f;
    private bool raceStarted = false;
    public bool gameWon = false;
    public CarController carController;
    public CheckpointHUD checkpointHUD;
    public GameObject winCanvas; 
    private Vector3 currentCheckpoint;
    private Vector3 currentCheckpointRotation;
    

    /// <summary>
    /// use Read only!!!
    /// </summary>
    public bool carsActive = false;
    public CheckpointTextAnimation checkpointTextAnimation;

    public TMP_Text countdownText;

    [Tooltip("2 for double speed, 0.5 for half speed")]
    public float CountdownSpeed = 1;


    private void Start()
    {
        countdownText.text = "Get ready!";
        StartCoroutine(StartCountdown());
        lapStartTime = Time.time;
        UpdateLapTimeTextCenter();
        UpdateLastLapTimeTextRight();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            if (other.transform == checkpoints[currentCheckpointIndex])
            {
                currentCheckpoint = transform.position;
                currentCheckpointRotation = transform.rotation.eulerAngles;
                carController.SetLastCheckpoint(currentCheckpoint, currentCheckpointRotation);
                checkpointHUD.SetCurrentCheckpoint(currentCheckpointIndex);

                if (currentCheckpointIndex == 0)
                {
                    checkpointTextAnimation.AnimateCheckpointText(lapTimeTextCenter);
                    // Start the race and lap timing when the first checkpoint is triggered
                    Debug.Log("Erster Punkt erreicht");
                    StartRace();
                   
                    
                }

                if (currentCheckpointIndex == checkpoints.Length - 1)
                {
                    checkpointTextAnimation.AnimateCheckpointText(lapTimeTextCenter);
                   // Finish the lap when the last checkpoint is triggered
                     FinishLap();
                    ShowWinScreen();
                    
                }
                else
                {
                    checkpointTextAnimation.AnimateCheckpointText(lapTimeTextCenter);
                    // Move to the next checkpoint
                    currentCheckpointIndex++;
                }
            }
            else
            {
                // Incorrect checkpoint, reset the lap
                //RestartLap();
            }
        }
    }

    public void StartRace()
    {
        raceStarted = true;
        lapStartTime = Time.time;
    }

    private void FinishLap()
    {
        raceStarted = false;
        gameWon = true;
        lapCount++;
        float lapTime = Time.time - lapStartTime;

       
        if (lastLapTime == 0f || lapTime < lastLapTime)
        {
            // Only update lastLapTime if it's the first lap or faster than the previous lap
            lastLapTime = lapTime;
            UpdateLastLapTimeTextRight();
        }
        currentCheckpointIndex = 0;
    }

    private void StartNewLap()
    {
        lapStartTime = Time.time;
        Debug.Log("Starting Lap " + lapCount + 1);
    }

    private void RestartLap()
    {
        Debug.Log("Restarting Lap");
        currentCheckpointIndex = 0;
    }

    private void Update()
    {
        if (raceStarted)
        {
            UpdateLapTimeTextCenter();
        }
    }

    private void UpdateLapTimeTextCenter()
    {
        float lapTime = Time.time - lapStartTime;
        int minutes = Mathf.FloorToInt(lapTime / 60f);
        int seconds = Mathf.FloorToInt(lapTime % 60f);
        int milliseconds = Mathf.FloorToInt((lapTime * 1000) % 1000);

        lapTimeTextCenter.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    private void UpdateLastLapTimeTextRight()
    {
        int minutes = Mathf.FloorToInt(lastLapTime / 60f);
        int seconds = Mathf.FloorToInt(lastLapTime % 60f);
        int milliseconds = Mathf.FloorToInt((lastLapTime * 1000) % 1000);
        lastLapTimeTextRight.text = lapTimeTextCenter.text;
    }
    public IEnumerator StartCountdown()
{
    int count = 3; // Start bei 3 (oder einem anderen Wert deiner Wahl)

    while (count > 0)
    {
        countdownText.text = count.ToString();
        yield return new WaitForSeconds(1f/CountdownSpeed);
        count--;
    }

    countdownText.text = "Go!";
    yield return new WaitForSeconds(1f/ CountdownSpeed);
    countdownText.text = "";

    // Starte das Rennen hier oder führe andere Aktionen aus, die du vor dem Start ausführen möchtest
    carsActive = true;
}
public void ResetRace()
{
    // Setze alle Runden- und Zeitvariablen zurück
    lapCount = 0;
    lapStartTime = 0f;
    lastLapTime = 0f;
    currentCheckpointIndex = 0;

    // Rufe die Methode StartNewLap() auf, um den Timer erneut zu starten
    StartNewLap();
}
 // Aufruf dieser Methode, wenn das Spiel gewonnen wurde
    public void ShowWinScreen()
    {
        winCanvas.SetActive(true); // Aktiviere den Canvas, um ihn anzuzeigen
    }
}