using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void PlayLevelGreen(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayLevelDust(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
}
