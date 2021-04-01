using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdollControl : MonoBehaviour
{

  [SerializeField] Collider controlCollider;
  // [SerializeField] float respawnTime = 10.0f;
  Rigidbody[] rigidbodies;
  // [SerializeField] Rigidbody topBody;
  bool isRagdoll = false;
  // dudeController controller;
    // Start is called before the first frame update
    void Start()
    {
      rigidbodies = GetComponentsInChildren<Rigidbody>(); // load ragdoll
      // controller = GetComponent<dudeController>();
      ToggleRagdoll(true);
    }

    // private void OnTriggerEnter(Collider target)
    // {
    //   // Debug.Log("Hit detected!");
    //   if (!isRagdoll && target.tag == "Player")
    //   {
    //     ToggleRagdoll(false);
    //   }
    //   // start getBackUp animation
    // }

    public void ToggleRagdoll(bool isAnimating)
    {
      isRagdoll = !isAnimating;

      // controlCollider.enabled = isAnimating;
      foreach (Rigidbody bone in rigidbodies)
      {
        // Debug.Log("Bone id: " + bone.gameObject.GetInstanceID());
        if (bone.gameObject != gameObject)
        {
          bone.isKinematic = isAnimating;
        }
        else if (bone.gameObject == gameObject)
        {
          bone.isKinematic = !isAnimating;
        }
      }

      GetComponent<Animator>().enabled = isAnimating; // should we do it this way?

      // Debug.Log(string.Format("isAnimating set to {0}", isAnimating));


    }

    // private IEnumerator GetBackUp()
    // {
    //   yield return new WaitForSeconds(respawnTime);
    //   ToggleRagdoll(true); // hand back off?
    // }

}
