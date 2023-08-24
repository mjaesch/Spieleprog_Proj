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
        if (hasFocus && !lapManager.openMenu)
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
        //Wichtig Starte das Rennen nicht automatisch beim Spielbeginn
        isRaceStarted = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isRaceStarted)
            {      
                carController.ResetToLastCheckpoint();
                //lapManager.carsActive = false;
                //StartCoroutine(lapManager.StartCountdown());

            }
            else
            {
                // Wenn das Rennen noch nicht gestartet wurde, starte es jetzt
                isRaceStarted = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Wenn die Leertaste gedrückt wird, lade die aktuelle Szene neu
             StopAllCoroutines();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        }
        if(Input.GetKeyDown(KeyCode.Escape) && !lapManager.openMenu){
            lapManager.openMenu = true;
            lapManager.ShowWinScreen();
            lapManager.winText.text = "";
        }else if(Input.GetKeyDown(KeyCode.Escape) && lapManager.openMenu){
            lapManager.HideWinScreen();
            lapManager.openMenu = false;
        }

        if(lapManager.openMenu){
            Cursor.lockState = CursorLockMode.None;
        }else{
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
   
}