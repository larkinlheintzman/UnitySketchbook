using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limbHitDetection : MonoBehaviour
{
  [SerializeField]
  public GameObject entity; // fill out with main object with ragdoll control script
  private ragdollControl toggler; // control the entire characters ragdoll state
  private Rigidbody limbRigidbody; // limbs individual rigid body

  private float forceMultiplier = 1e4f;
  private Vector3 beginTargetPosition; // pose at which limb entered collider
  private Vector3 endTargetPosition; // pose after a number of frames
  private Vector3 targetVelocity; // calculated target velocity


  void Start()
  {
    toggler = entity.GetComponent<ragdollControl>();
    limbRigidbody = GetComponent<Rigidbody>();
  }

  void OnTriggerEnter(Collider target)
  {
    // Debug.Log("hit detect on " + gameObject.name);
    if (target.tag == "Player")
    {
      beginTargetPosition = target.gameObject.transform.position;
      endTargetPosition = beginTargetPosition;
    }
  }

  void OnTriggerStay(Collider target)
  {
    if (target.tag == "Player")
    {
      beginTargetPosition = endTargetPosition;
      endTargetPosition = target.gameObject.transform.position;

      toggler.ToggleRagdoll(false);
      // calc velocity and update
      limbRigidbody.AddForce(forceMultiplier * (endTargetPosition - beginTargetPosition));

      Debug.Log("Detected from hit " + forceMultiplier * (endTargetPosition - beginTargetPosition));
    }
  }

  // void OnTriggerExit(Collider target)
  // {
  //   // player.GetComponent<dudeController>().hitDetected = false;
  //   // player.GetComponent<dudeController>().hitTarget = null;
  //   //
  //   // // turn off light on target
  //   // if (target.tag == "Enemy")
  //   // {
  //   //   target.gameObject.GetComponent<Outline>().eraseRenderer = true;
  //   // }
  // }

}
