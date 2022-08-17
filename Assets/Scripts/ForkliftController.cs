using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftController : MonoBehaviour
{
    public Transform m_steeringWheel;
    public Transform m_target;
    private Transform m_hand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            m_hand = other.transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            m_hand = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("PlayerHand"))
        //{
        //    Debug.Log("[Wheel] Deleting hand: " + m_hand.name);
        //    m_hand = null;
        //}
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_hand != null)
        {
            Debug.Log("[Wheel] Found a hand");

            m_target.position = m_hand.position;
            m_target.localPosition = new Vector3(m_target.position.x, 0, m_target.position.z);

            Vector3 vHandToWheelDir = m_target.position - m_steeringWheel.position;
            Quaternion qRot = Quaternion.LookRotation(vHandToWheelDir, transform.up);
            m_steeringWheel.rotation = qRot;
        }
    }
}
