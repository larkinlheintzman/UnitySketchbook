using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewPew : MonoBehaviour
{

  DudeActions input;
  public GameObject boolet;
  public float launchForce;

    // Start is called before the first frame update
    void Awake()
    {
      input = new DudeActions();

      input.CharacterControls.Shoot.performed += ctx => ShootBoolet(ctx.ReadValueAsButton());
    }

    void ShootBoolet(bool button)
    {
      GameObject shot = Instantiate(boolet, transform.position + transform.forward, Quaternion.identity) as GameObject;
      // Debug.Log("bullet made");
      shot.GetComponent<Rigidbody>().AddForce(transform.forward * launchForce);
      // Debug.Log("shot fired");
    }


    void OnEnable ()
    {
      input.CharacterControls.Enable();
      // lock cursor to window
      // Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable ()
    {
      input.CharacterControls.Disable();
      // Cursor.lockState = CursorLockMode.None;
    }
}
