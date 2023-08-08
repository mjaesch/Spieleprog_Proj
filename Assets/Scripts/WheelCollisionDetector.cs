using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelCollisionDetector : MonoBehaviour
{
    public ParticleSystem dustEffect;

    private WheelCollider wheelCollider;

    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

     private void FixedUpdate()
    {
        if (wheelCollider.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit))
            {
                if (hit.collider.CompareTag("Desert"))
                {
                    if (!dustEffect.isPlaying)
                    {
                        dustEffect.Play();
                    }
                }
                else
                {
                    if (dustEffect.isPlaying)
                    {
                        dustEffect.Stop();
                    }
                }
            }
        }
        else
        {
            if (dustEffect.isPlaying)
            {
                dustEffect.Stop();
            }
        }
    }
}