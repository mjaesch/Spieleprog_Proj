using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
     
     void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Car collided with road!");
        if (collision.gameObject.CompareTag("Road"))
        {
            Debug.Log("Car collided with road!");
            // Perform actions specific to the car colliding with the road
        }else{
            Debug.Log("Car collided with something else!");
            // Perform actions specific to the car colliding with something else
        }
    }
    
}
