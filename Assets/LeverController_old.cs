using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController_old : MonoBehaviour
{
    public Transform leverTop;

    [SerializeField] private float forwardBackwardTilt = 0;

    // Update is called once per frame
    private void Update()
    {
        // fForwardBackwardTilt is used as speed to move the object
        forwardBackwardTilt = leverTop.rotation.eulerAngles.x;

        if (forwardBackwardTilt is < 355 and > 290)
        {
            forwardBackwardTilt = Math.Abs(forwardBackwardTilt - 360);
            Debug.Log("Lever Moving Backwards" + forwardBackwardTilt);
        }
        else if (forwardBackwardTilt is > 5 and < 74)
        {
            Debug.Log("Lever Moving Forward" + forwardBackwardTilt);
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
