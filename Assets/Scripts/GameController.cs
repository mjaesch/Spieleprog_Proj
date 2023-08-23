using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public CarController carController;
    public LapManager lapManager;
    private Coroutine countdownCoroutine;
    

    private bool isRaceStarted = false;
    /// <summary>
    /// Quelle:https://gamedevbeginner.com/how-to-lock-hide-the-cursor-in-unity/#hide_cursor
    /// </summary>
    /// <param name="hasFocus"></param>
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !lapManager.gameWon)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Application is focussed");
        }
        else
        {
            Debug.Log("Application lost focus");
        }
    }
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
                //lapManager.ResetRace();
                carController.ResetToLastCheckpoint();
                //lapManager.carsActive = false;
                //StartCoroutine(lapManager.StartCountdown());

            }
            else
            {
                // Wenn das Rennen noch nicht gestartet wurde, starte es jetzt
                //lapManager.StartRace();
                isRaceStarted = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Wenn die Escape-Taste gedrückt wird, lade die aktuelle Szene neu
             StopAllCoroutines();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
           
        }
        if(lapManager.gameWon){
            Cursor.lockState = CursorLockMode.None;
        }else{
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
   
}