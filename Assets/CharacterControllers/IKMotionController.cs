using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKMotionController : MonoBehaviour
{

  [HideInInspector]
  public new Rigidbody rigidbody;
  DudeActions input;
  LedgeDetection ledgeDectector;
  [SerializeField]
  Transform playerInputSpace;

  [HideInInspector]
  public Vector3 currentMovement;
  [HideInInspector]
  public Vector3 currentPosition;
  [HideInInspector]
  public Vector3 currentVelocity;
  [HideInInspector]
  public Vector3 jumpVelocity;

  bool isGrounded = true;
  private float groundSpeed;
  private float maxGroundDistance = 20.0f;
  private float distanceToGround = 0.0f;

  // button booleans
  bool movementPressed;
  bool runPressed = false;
  bool rollPressed = false;
  bool punchPressed = false;
  bool ragdollPressed = false;
  bool jumpPressed = false;

  // bools to keep track
  bool isJumping = false;
  bool isAirborne = false;
  bool isHanging = false;
  int jumpDelayCount = 10; // maximum possible jump charge
  int jumpDelayBase = 5; // base jump delay
  int jumpDelayCounter = 0; // counter for delay

  [SerializeField, Range(1,20)]
  public float playerSpeedMult = 10.0f;
  [SerializeField, Range(1,10)]
  public float playerAirSpeedMult = 4.0f;
  [SerializeField, Range(1,10)]
  public float playerRunMult = 1.5f;
  [SerializeField, Range(1,10)]
  public float playerMaxSpeed = 3.0f; // player ground speed max
  [SerializeField, Range(1,10)]
  public float playerMaxAirSpeed = 8.0f; // player air speed max
  [SerializeField, Range(0,1)]
  public float playerRotationSpeed = 0.1f;
  [SerializeField, Range(1,20)]
  public float playerMinJumpSpeed = 2f;
  [SerializeField, Range(1,20)]
  public float playerMaxJumpSpeed = 8f;
  [SerializeField, Range(0,5)]
  float groundedMaxDistance = 0.085f;
  // shooty params
  public GameObject boolet;
  public float launchForce;

  // Bit shift the index of the layer (8) to get a bit mask
  [HideInInspector]
  private int layerMask;


  // hid detection bools
  [HideInInspector] public bool hitDetected = false;
  [HideInInspector] public GameObject hitTarget = null;

  // ragdoll control
  ragdollControl ragdollControl;

  private Dictionary<string, AnimationClip> animationClipDict = new Dictionary<string, AnimationClip>();


  void Awake()
  {

    // isAnimating = true;

    // rollAllowed = true;
    layerMask = 1 << 9;

    // This would cast rays only against colliders in layer 8.
    // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
    layerMask = ~layerMask;


    input = new DudeActions();
    ledgeDectector = GetComponent<LedgeDetection>();

    isAirborne = false;
    isHanging = false;
    isJumping = false;
    isGrounded = true;

    // input.CharacterControls.Shoot.performed += ctx => ShootBoolet(ctx.ReadValueAsButton());

    input.CharacterControls.ZAxis.performed += ctx =>
    {
      currentMovement.z = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;

      // HandleWalkAnimation(movementPressed);
      // Debug.Log(currentMovement);
    };

    input.CharacterControls.XAxis.performed += ctx =>
    {
      currentMovement.x = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;

      // HandleWalkAnimation(movementPressed);
      // Debug.Log(currentMovement);
    };

    input.CharacterControls.Run.performed += ctx =>
    {
      runPressed = ctx.ReadValueAsButton();

      // HandleWalkAnimation(movementPressed);
      // Debug.Log(runPressed);
    };

    input.CharacterControls.Roll.performed += ctx =>
    {
      rollPressed = ctx.ReadValueAsButton();
      // Debug.Log(runPressed);
      // can we just set animation bools here?
      // HandleRollAnimation(rollPressed);
    };

    input.CharacterControls.Punch.performed += ctx =>
    {
      punchPressed = ctx.ReadValueAsButton();
      // HandlePunchAnimation(punchPressed);
    };

    // jump action
    input.CharacterControls.Jump.performed += ctx =>
    {
      jumpPressed = ctx.ReadValueAsButton();
      // HandleJumpAnimation(jumpPressed);
    };

    // ragdoll action started
    input.CharacterControls.Ragdoll.started += ctx =>
    {
      ragdollPressed = ctx.ReadValueAsButton();
      // Debug.Log("button down, val : " + ragdollPressed);
      // HandleRagdollAnimation(ragdollPressed);
    };
    // ragdoll action started
    input.CharacterControls.Ragdoll.performed += ctx =>
    {
      ragdollPressed = ctx.ReadValueAsButton();
      if (!ragdollPressed)
      {
        // Debug.Log("button up, val : " + ragdollPressed);
        // HandleRagdollAnimation(ragdollPressed);
      }
    };

  }

  void ShootBoolet(bool button)
  {
    GameObject shot = Instantiate(boolet, transform.position + transform.forward, Quaternion.identity) as GameObject;
    // Debug.Log("bullet made");
    shot.GetComponent<Rigidbody>().AddForce(transform.forward * launchForce);
    // Debug.Log("shot fired");
  }

  // Start is called before the first frame update
  void Start()
  {
    rigidbody = GetComponent<Rigidbody>(); // rigidbody
    // Debug.Log("checking table for " + key);

  }

  public Vector3 MapToInputSpace(Vector3 worldInput)
  {
    Vector3 desiredVelocity;
    if (playerInputSpace) {
      Vector3 forward = playerInputSpace.forward;
      forward.y = 0f;
      forward.Normalize();
      Vector3 right = playerInputSpace.right;
      right.y = 0f;
      right.Normalize();
      desiredVelocity = (forward * worldInput.z + right * worldInput.x);
    }
    else
    {
      desiredVelocity = worldInput;
    }
    // Debug.Log("desired velocity mapped: " + desiredVelocity);
    return desiredVelocity;
  }

  void ApplyCurrentInput()
  {
    // Debug.Log("applying current input: " + currentMovement);
    groundSpeed = Mathf.Sqrt(rigidbody.velocity.x*rigidbody.velocity.x + rigidbody.velocity.z*rigidbody.velocity.z);
    // applies forces from player input
    if (!isAirborne)
    {
      if (runPressed)
      {
        rigidbody.AddForce(playerSpeedMult * playerRunMult * MapToInputSpace(currentMovement));
        // rigidbody.AddForce(rigidbody.position + playerSpeedMult * playerRunMult * MapToInputSpace(currentMovement) * Time.fixedDeltaTime);
        NormalizeVelocity(playerMaxSpeed * playerRunMult);
      }
      else
      {
        rigidbody.AddForce(playerSpeedMult * MapToInputSpace(currentMovement));
        // rigidbody.AddForce(rigidbody.position + playerSpeedMult * MapToInputSpace(currentMovement) * Time.fixedDeltaTime);
        NormalizeVelocity(playerMaxSpeed);
      }
    }
    else {

      rigidbody.AddForce(playerAirSpeedMult * MapToInputSpace(currentMovement));
      // rigidbody.MovePosition(rigidbody.position + playerAirSpeedMult * MapToInputSpace(currentMovement) * Time.fixedDeltaTime);
      NormalizeVelocity(playerMaxAirSpeed);

    }
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

  private void UpdateRecoveryTimes()
  {
    UpdateGrounded();
    UpdateDistanceToGround();
    UpdateHangPoint(); // TODO not ready yet

  }

  // probably useful at some point
  void triggerAnimation(string animationKey)
  {

  }

  void UpdateGrounded()
  {
    // cast rays from radius around player
    // isGrounded = Physics.Raycast(transform.position + 0.1f * Vector3.up, -Vector3.up, groundedMaxDistance + 0.1f);
    if (Physics.Raycast(transform.position + 0.1f * Vector3.up, -Vector3.up, groundedMaxDistance + 0.1f, layerMask) != isGrounded)
    {
      isGrounded = Physics.Raycast(transform.position + 0.1f * Vector3.up, -Vector3.up, groundedMaxDistance + 0.1f);
      Debug.Log("grounded set to: " + isGrounded);
    }

  }


  void UpdateDistanceToGround()
  {
    distanceToGround = maxGroundDistance;
    RaycastHit hit = new RaycastHit();
    if (Physics.Raycast (transform.position, -Vector3.up, out hit, Mathf.Infinity, layerMask)) {
      distanceToGround = Mathf.Min(hit.distance, maxGroundDistance);
    }

  }

  void UpdateHangPoint()
  {
    // isHanging = Physics.Raycast(ledgeDetectionTransform.position, MapToInputSpace(Vector3.forward), ledgeGrabDistance);
    if (isHanging != ledgeDectector.DetectLedge())
    {
      isHanging = ledgeDectector.DetectLedge();
      Debug.Log("hanging set to: " + isHanging);
    }
  }

  // Update is called once per frame
  void FixedUpdate()
  {

    UpdateRecoveryTimes();
    // Handle animations
    // Debug.Log("running actions");
    HandleActions();
    HandleJumpMotion();
    HandleRotation();

  }

  void HandleActions()
  {
    // priority sorted (kinda)

    if (jumpPressed && isGrounded && !isAirborne && !isJumping && !isHanging)
    {
      // Debug.Log("starting jump animation");
      // triggerAnimation("Jumping Up");

      // Handle jump motion
      isJumping = true;
      jumpDelayCounter = jumpDelayCount;
      // Debug.Log("triggering jump at: " + Time.time);

      ApplyCurrentInput();

    }

    if (punchPressed && isGrounded && !isAirborne && !isJumping && !isHanging)
    {
      // Debug.Log("starting punch animation");
      // triggerAnimation("PunchLeft");
      // Debug.Log("setting lock for " + "punch" + " to " + animationLockCount);

    }

    // Handle walk animation
    if (movementPressed && !runPressed && isGrounded && !isAirborne && !isJumping && !isHanging)
    {
      // Debug.Log("starting walk animation");
      // triggerAnimation("Walking");

      // Handle walk motion
      ApplyCurrentInput();

    }

    if (movementPressed && runPressed && isGrounded && !isAirborne && !isJumping && !isHanging)
    {
      // Debug.Log("starting run animation");
      // triggerAnimation("Running");

      // Handle run motion
      ApplyCurrentInput();
    }

    //Handle idle animation
    if (!movementPressed && !jumpPressed && isGrounded && !isAirborne && !isJumping && !isHanging)
    {
      // Debug.Log("starting idle animation");
      // triggerAnimation("Idle");;
    }

    // Handle falling animation
    if (!isGrounded && !isHanging)
    {
      // ApplyCurrentInput();
      // triggerAnimation("Falling Idle");
      // Debug.Log("starting idle air animation");
      isAirborne = true;

    }

    // Handle landing animation
    if (isGrounded && isAirborne && rigidbody.velocity.y <= 0.0f && !isHanging)
    {
      // ApplyCurrentInput();
      // triggerAnimation("Falling To Landing");
      // update animation speed based on vertical speed
      isAirborne = false;
      // Debug.Log("is landing, is airborne: " + isAirborne);
      // Debug.Log("is (still) landing, is jumping: " + isJumping);

      // Handle motion
      ApplyCurrentInput();

    }

    if (isHanging)
    {
      // Debug.Log("is hanging");
      rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);
      rigidbody.AddForce(Vector3.forward);

      // this would go in the catch animation anyway
      if (rigidbody.useGravity)
      {
        rigidbody.useGravity = false;
      }
      if (isGrounded && !rigidbody.useGravity)
      {
        rigidbody.useGravity = true;
      }

    }




    if (!isJumping && isAirborne)
    {
      // Debug.Log("moving in air");
      // Handle air motion
      ApplyCurrentInput();

    }


  }

  void HandleJumpMotion()
  {
    // update counts for delays
    if (isJumping && jumpDelayCounter > jumpDelayBase)
    {
      if (jumpPressed) // still holding button
      {

        jumpDelayCounter -= 1; // count down to lift off
        // float jumpBlendingValue = 1.0f - (jumpDelayCounter*1.0f)/jumpDelayCount;
        // animator.SetFloat("JumpBlend", jumpBlendingValue);

        jumpVelocity = new Vector3(0.0f,
        ((jumpDelayCount - jumpDelayCounter * 1.0f) / (jumpDelayCount * 1.0f)) * (playerMaxJumpSpeed - playerMinJumpSpeed) + playerMinJumpSpeed, 0.0f);
        // Debug.Log("jomp pressed with: " + jumpVelocity + " counter at: " + jumpDelayCounter);

      }
      else // let go of button
      {

        jumpVelocity = new Vector3(0.0f,
        ((jumpDelayCount - jumpDelayCounter * 1.0f) / (jumpDelayCount * 1.0f)) * (playerMaxJumpSpeed - playerMinJumpSpeed) + playerMinJumpSpeed, 0.0f);
        // Debug.Log("jomp not pressed with: " + jumpVelocity + " counter at: " + jumpDelayCounter);

        jumpDelayCounter = jumpDelayBase; // skip to end of charge phase

        // float jumpBlendingValue = 1.0f - (jumpDelayCounter*1.0f)/jumpDelayCount;
        // animator.SetFloat("JumpBlend", jumpBlendingValue);

      }
    }
    else if (isJumping)// in base delay
    {
      jumpDelayCounter -= 1;

      // float jumpBlendingValue = 1.0f - (jumpDelayCounter*1.0f)/jumpDelayCount;
      // animator.SetFloat("JumpBlend", jumpBlendingValue);

      if (jumpDelayCounter == 0 && isGrounded)
      {

        rigidbody.velocity += jumpVelocity;
        // Debug.Log("jump fired at: " + Time.time);

        isJumping = false; // we have jumped

      }
      else if (jumpDelayCounter <= 0)
      {
        jumpDelayCounter = 0;
        isJumping = false; // we could not jump
      }
    }
  }

  void HandleRotation()
  {
    // currentPosition = transform.position;
    // currentVelocity.Normalize();

    currentVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
    transform.LookAt(transform.position + currentVelocity);
    // do something with rotation speed here
    // transform.LookAt(transform.position + MapToInputSpace(currentMovement));
  }


  void OnEnable ()
  {
    input.CharacterControls.Enable();
    // lock cursor to window
    Cursor.lockState = CursorLockMode.Locked;
  }

  void OnDisable ()
  {
    input.CharacterControls.Disable();
    Cursor.lockState = CursorLockMode.None;
  }

}
