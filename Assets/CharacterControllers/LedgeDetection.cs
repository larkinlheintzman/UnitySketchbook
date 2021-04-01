using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField]
    public ColliderHandler ledgeColliderHandler; // assigned in inspector
    [SerializeField]
    public ColliderHandler wallColliderHandler; // assigned in inspector
    [SerializeField]
    // private Dictionary<string, ColliderHandler> colliderHandlers;

    // Update is called once per frame
    public bool DetectLedge()
    {

      bool ledgeDetected = false;
      if (wallColliderHandler.isColliding)
      { 
        if (ledgeColliderHandler.firstContact)
        {
          ledgeDetected = true;
        }
      }
      return ledgeDetected;

    }


}
