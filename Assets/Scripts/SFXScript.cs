using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXScript : MonoBehaviour
{
    public AudioSource Boom;
    public AudioSource Wrong;

    public void PlayBoom()
    {
        Boom.Play();
    }
    public void PlayWrong()
    {
        Wrong.Play();
    }
}
