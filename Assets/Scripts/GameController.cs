using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public CarController carController;
    public LapManager lapManager;
    private Coroutine countdownCoroutine;

    private bool isRaceStarted = false;

    private void Start()
    {
        // Starte das Rennen nicht automatisch beim Spielbeginn
        isRaceStarted = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isRaceStarted)
            {
                // Wenn das Rennen bereits gestartet wurde, starte das Rennen erneut
                lapManager.ResetRace();
                carController.ResetCar();
                lapManager.carsActive = false;
                StartCoroutine(lapManager.StartCountdown());

            }
            else
            {
                // Wenn das Rennen noch nicht gestartet wurde, starte es jetzt
                lapManager.StartRace();
                isRaceStarted = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Wenn die Escape-Taste gedrückt wird, lade die aktuelle Szene neu
             StopAllCoroutines();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
           
        }
    }
}