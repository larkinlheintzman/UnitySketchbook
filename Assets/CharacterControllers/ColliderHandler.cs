using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHandler : MonoBehaviour
{

    public bool firstContact = false;
    public bool isColliding = false;
    int firstContactCounter = 0;

    void OnTriggerEnter(Collider other)
    {
      firstContact = true;
      firstContactCounter = 5;
      isColliding = true;
      // Debug.Log("first contact for " + gameObject.GetInstanceID());
    }
    void OnTriggerStay(Collider other)
    {
      if (firstContactCounter == 0)
      {
        firstContact = false;
        // Debug.Log("timer out for " + gameObject.GetInstanceID());
      }
      else
      {
        firstContactCounter -= 1;
      }
    }
    void OnTriggerExit(Collider other)
    {
      // Debug.Log("exiting contact " + gameObject.GetInstanceID());
      firstContact = false;
      isColliding = false;
    }
}
