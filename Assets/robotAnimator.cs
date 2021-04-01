using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotAnimator : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isRightWalkingHash;
    int isLeftWalkingHash;
    int XVelocityHash;
    int ZVelocityHash;
    float xVelocity = 0.0f;
    float zVelocity = 0.0f;

    bool rotationAllowed = true;

    // float playerHeading;
    // float playerSpeed;
    Quaternion playerHeading; // player heading in degrees
    Vector3 playerHeadingVelocity = Vector3.zero;
    Vector3 bodyVelocity;
    public float playerTurnSpeed = 1f;
    public float playerAcceleration = 0.5f;


    CharacterController CharController;

    void Awake()
    {
      CharController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
      animator = GetComponent<Animator>();
      XVelocityHash = Animator.StringToHash("XVelocity");
      ZVelocityHash = Animator.StringToHash("ZVelocity");

    }

    // Update is called once per frame
    void Update()
    {

      // button inputs to save on checking
      bool forwardPressed = Input.GetKey(KeyCode.W);
      bool runPressed = Input.GetKey(KeyCode.LeftShift);
      bool rightPressed = Input.GetKey(KeyCode.D);
      bool leftPressed = Input.GetKey(KeyCode.A);
      bool backPressed = Input.GetKey(KeyCode.S);

      playerHeading = gameObject.transform.rotation;

      // forward logic -----------------------

      if (forwardPressed && zVelocity < 1.0f)
      {
        zVelocity += Time.deltaTime*playerAcceleration;
      }

      if (!forwardPressed && zVelocity > 0.0f)
      {
        zVelocity -= Time.deltaTime*playerAcceleration;
      }

      // right logic -----------------------

      if (rightPressed && xVelocity < 1.0f)
      {
        xVelocity += Time.deltaTime*playerAcceleration;
      }

      if (!rightPressed && xVelocity > 0.0f)
      {
        xVelocity -= Time.deltaTime*playerAcceleration;
      }

      // left logic -----------------------

      if (leftPressed && xVelocity > -1.0f)
      {
        xVelocity -= Time.deltaTime*playerAcceleration;
      }

      if (!leftPressed && xVelocity < 0.0f)
      {
        xVelocity += Time.deltaTime*playerAcceleration;
      }

      bodyVelocity = playerHeading * new Vector3(-xVelocity, 0.0f, zVelocity);

      // Debug.Log(bodyVelocity.ToString());
      animator.SetFloat(XVelocityHash, bodyVelocity.x);
      animator.SetFloat(ZVelocityHash, bodyVelocity.z);

      // rotation logic --------------------

      if (leftPressed && rotationAllowed)
      {
        gameObject.transform.rotation = Quaternion.RotateTowards(playerHeading, Quaternion.Euler(0,-90,0), 0.1f);
      }

      if (rightPressed && rotationAllowed)
      {
        // playerHeadingVelocity.y = (90 - playerHeading.y) * Time.deltaTime * playerTurnSpeed;
        gameObject.transform.rotation = Quaternion.RotateTowards(playerHeading, Quaternion.Euler(0,90,0), 0.1f);
      }

      if (forwardPressed && rotationAllowed)
      {
        // playerHeadingVelocity.y = (90 - playerHeading.y) * Time.deltaTime * playerTurnSpeed;
        gameObject.transform.rotation = Quaternion.RotateTowards(playerHeading, Quaternion.Euler(0,0,0), 0.1f);
      }
      // if (!leftPressed && rotationAllowed)
      // {
      //   playerHeadingVelocity.y = 0.0f;
      // }


      // Debug.Log(playerHeading.ToString());

      // if (!rightPressed && rotationAllowed)
      // {
      //   playerHeadingVelocity.y = 0.0f;
      // }
      //
      // if (backPressed && rotationAllowed)
      // {
      //   if (playerHeading.y <= 180)
      //   {
      //     playerHeadingVelocity.y = (180 - playerHeading.y) * Time.deltaTime * playerTurnSpeed;
      //   }
      //   if (playerHeading.y > 180)
      //   {
      //     playerHeadingVelocity.y = (- 180 + playerHeading.y) * Time.deltaTime * playerTurnSpeed;
      //   }
      // }
      //
      //
      // if (!forwardPressed && rotationAllowed)
      // {
      //   playerHeadingVelocity.y = 0.0f;
      // }
      //
      // if (forwardPressed && rotationAllowed)
      // {
      //   if (playerHeading.y <= 180)
      //   {
      //     playerHeadingVelocity.y = (180 - playerHeading.y) * Time.deltaTime * playerTurnSpeed;
      //   }
      //   if (playerHeading.y > 180)
      //   {
      //     playerHeadingVelocity.y = (- 180 + playerHeading.y) * Time.deltaTime * playerTurnSpeed;
      //   }
      // }
      //
      // if (!forwardPressed && rotationAllowed)
      // {
      //   playerHeadingVelocity.y = 0.0f;
      // }
      //
      // Debug.Log(playerHeadingVelocity);
      //
      // gameObject.transform.Rotate(playerHeadingVelocity, Space.World);

    }

    // float SteerTowards(float currentHeading, float desiredHeading)
    // {
    //   // Debug.Log(currentHeading.ToString());
    //   Debug.Log(currentHeading.ToString());
    //   Debug.Log(desiredHeading.ToString());
    //
    //   if (desiredHeading > 180)
    //   {
    //     desiredHeading = 360 - desiredHeading;
    //   }
    //
    //   float steer = desiredHeading - currentHeading;
    //   Debug.Log(steer);
    //   return steer*playerTurnSpeed*Time.deltaTime*sign;
    // }
}
