using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cakeslice;

public class HitDetection : MonoBehaviour
{
  [SerializeField]
  public GameObject player; // fill out in inspector with player character

    void OnTriggerEnter(Collider target)
    {
      Debug.Log("hit detect on hand: " + target.name);
      if (target.tag != "Player")
      {
        player.GetComponent<dudeController>().hitDetected = true;
        player.GetComponent<dudeController>().hitTarget = target.gameObject;

        // light up target with outline component
        if (target.tag == "Enemy")
        {
          target.gameObject.GetComponent<Outline>().eraseRenderer = false;
        }
      }
    }

    void OnTriggerExit(Collider target)
    {
      player.GetComponent<dudeController>().hitDetected = false;
      player.GetComponent<dudeController>().hitTarget = null;

      // turn off light on target
      if (target.tag == "Enemy")
      {
        target.gameObject.GetComponent<Outline>().eraseRenderer = true;
      }
    }

}
