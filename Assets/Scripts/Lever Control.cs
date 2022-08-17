using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverControl : MonoBehaviour
{
    public Transform leverTop;

    [SerializeField] private float fForwardBackwardTilt = 0;
    [SerializeField] private float fSideToSideTilt = 0;
    
    // Update is called once per frame
    private void Update()
    {
        // fForwardBackwardTilt is used as speed to move the object
        fForwardBackwardTilt = leverTop.rotation.eulerAngles.x;

        if (fForwardBackwardTilt < 355 && fForwardBackwardTilt > 290)
        {
            fForwardBackwardTilt = Math.Abs(fForwardBackwardTilt - 360);
            Debug.Log("Lever Moving Backwards" + fForwardBackwardTilt);
        }
        else if (fForwardBackwardTilt > 5 && fForwardBackwardTilt < 74)
        {
            Debug.Log("Lever Moving Forward" + fForwardBackwardTilt);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            transform.LookAt(other.transform.position, transform.up);
        }
    }
}
