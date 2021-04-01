using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motionController : MonoBehaviour
{

  new Rigidbody rigidbody;
  animationController animationController;
  DudeActions input;

  Vector3 currentMovement;
  Vector3 currentPosition;
  Vector3 currentVelocity;

  [SerializeField] public Transform ragdollTransform;
  private Vector3 ragdollOffset;


  bool movementPressed;
  bool runPressed;
  bool rollPressed;
  bool punchPressed;
  bool ragdollPressed;
  bool jumpPressed;

  private float groundSpeed;

  // animation constants
  float walkAnimationBaseSpeed = 1.055f;
  float runAnimationBaseSpeed = 4.075f;
  float rollAnimationBaseSpeed = 3.319f;

  float jumpStartTime;
  float jumpDelay = 0.50f;
  float punchStartTime;
  float punchDelay = 0.50f;
  float distToGround;
  bool isGrounded;

  [Range(1,10)]
  public float playerSpeedMult = 7.0f;
  [Range(1,10)]
  public float playerRunMult = 1.5f;
  [Range(1,10)]
  public float playerMaxSpeed = 5.0f; // player ground speed max
  [Range(0,1)]
  public float playerRotationSpeed = 0.1f;
  [Range(1,20)]
  public float playerJumpSpeed = 5.0f;
  [Range(0,1)]
  public float playerPunchSpeed = 0.1f;


  // // hid detection bools
  // [HideInInspector] public bool hitDetected = false;
  // [HideInInspector] public GameObject hitTarget = null;

  // ragdoll control
  // ragdollControl ragdollControl;

  void Awake()
  {
    input = new DudeActions();
    animationController = GetComponent<animationController>();
    // ragdollControl = GetComponent<ragdollControl>();

    input.CharacterControls.ZAxis.performed += ctx =>
    {
      currentMovement.z = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;

      HandleWalkMotion(currentMovement);
      // Debug.Log(currentMovement);
    };
    input.CharacterControls.XAxis.performed += ctx =>
    {
      currentMovement.x = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;

      // Debug.Log("button current movement: " + currentMovement);
      HandleWalkMotion(currentMovement);
      // Debug.Log(currentMovement);
    };

    input.CharacterControls.Run.performed += ctx =>
    {
      runPressed = ctx.ReadValueAsButton();
      // Debug.Log(runPressed);
    };

    input.CharacterControls.Roll.performed += ctx =>
    {
      rollPressed = ctx.ReadValueAsButton();
      // Debug.Log(runPressed);
      // can we just set animation bools here?
      HandleRollMotion(rollPressed);
    };

    input.CharacterControls.Punch.performed += ctx =>
    {
      punchPressed = ctx.ReadValueAsButton();

      HandlePunchMotion(punchPressed);
    };

    input.CharacterControls.Jump.performed += ctx =>
    {
      jumpPressed = ctx.ReadValueAsButton();

      HandleJumpMotion(jumpPressed);
    };

    // ragdoll action
    input.CharacterControls.Ragdoll.started += ctx =>
    {
      ragdollPressed = ctx.ReadValueAsButton();

      HandleRagdollMotion(ragdollPressed);
    };

    // ragdoll action
    input.CharacterControls.Ragdoll.performed += ctx =>
    {
      ragdollPressed = ctx.ReadValueAsButton();

      if (!ragdollPressed)
      {
        HandleRagdollMotion(ragdollPressed);
      }
    };

  }

  // Start is called before the first frame update
  void Start()
  {
    rigidbody = GetComponent<Rigidbody>(); // kinematic rigidbody

    // get ragdoll transform offset for following ragdoll position
    ragdollOffset = transform.position - ragdollTransform.position;
  }

  void NormalizeVelocity(float maxSpeed)
  {

    if (groundSpeed > (maxSpeed))
    {
      // normalize velocity in x and z directions
      currentVelocity = new Vector3(rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);
      currentVelocity.Normalize();
      rigidbody.velocity = new Vector3(currentVelocity.x * maxSpeed, rigidbody.velocity.y, currentVelocity.z * maxSpeed);
      // Debug.Log("Normalized velocity");
    }

  }

  // Update is called once per frame
  void FixedUpdate()
  {

    CheckGrounded();
    HandleMovement();
    HandleRotation();

  }

  void HandleMovement()
  {

    if (!ragdollPressed)
    {

      //can convert into squared comparison for speed
      groundSpeed = Mathf.Sqrt(rigidbody.velocity.x*rigidbody.velocity.x + rigidbody.velocity.z*rigidbody.velocity.z);
      // Debug.Log("current ground speed : " + groundSpeed);

      // add force and clamp velocity
      if (movementPressed && !runPressed)
      {

        // adjust speed based on ground speed of motion controller
        // animator.SetFloat(animSpeedHash, groundSpeed / walkAnimationBaseSpeed);
        animationController.UpdateWalkAnimationSpeed( groundSpeed / walkAnimationBaseSpeed );
        rigidbody.AddForce(playerSpeedMult * currentMovement);
        NormalizeVelocity(playerMaxSpeed);

      }

      if (runPressed)
      {

        // for run case
        animationController.UpdateWalkAnimationSpeed( groundSpeed / runAnimationBaseSpeed );

        rigidbody.AddForce(playerSpeedMult * playerRunMult * currentMovement);
        NormalizeVelocity(playerMaxSpeed * playerRunMult);

      }

      animationController.UpdateRollAnimationSpeed( groundSpeed / rollAnimationBaseSpeed );

      // handle jump
      if (jumpPressed)
      {

        // only apply velocity if beginning
        if (Time.time > jumpStartTime + jumpDelay && isGrounded)
        {
          // Debug.Log("Setting player jump speed");
          rigidbody.velocity = new Vector3(rigidbody.velocity.x, playerJumpSpeed, rigidbody.velocity.z);
        }

      }

      // handle punch
      if (punchPressed)
      {

        // only apply velocity if beginning
        if (Time.time > punchStartTime + punchDelay && isGrounded)
        {
          rigidbody.velocity = rigidbody.velocity + playerPunchSpeed * transform.forward;
        }

      }

    }

  }

  void HandleRotation()
  {
    // currentPosition = transform.position;
    // currentVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
    // currentVelocity.Normalize();

    // do something with rotation speed here
    // transform.LookAt(currentPosition + currentVelocity);
    transform.LookAt(transform.position + currentMovement);
  }

  void HandleWalkMotion(Vector3 walkDirection)
  {

    // :)

  }

  // rolling/dash action
  void HandleRollMotion(bool rollPressed)
  {

    // :)

  }

  // punch action
  void HandlePunchMotion(bool punchPressed)
  {

    // :) sterp fowar
    if (punchPressed)
    {
      punchStartTime = Time.time;
    }

  }

  // jump action
  void HandleJumpMotion(bool jumpPressed)
  {

    if (jumpPressed)
    {

      jumpStartTime = Time.time;

    }

  }

  // ragdoll action
  void HandleRagdollMotion(bool ragdollPressed)
  {
    if (!ragdollPressed)
    {
      // update position to last ragdoll position
      transform.position = ragdollTransform.position;
    }
  }

  void CheckGrounded()
  {

    isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);

  }

  void OnEnable ()
  {
    input.CharacterControls.Enable();
  }

  void OnDisable ()
  {
    input.CharacterControls.Disable();
  }

}
