using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerController : MonoBehaviour
{

  Animator animator;
  new Rigidbody rigidbody;
  DudeActions input;
  LedgeDetection ledgeDectector;
  [SerializeField]
  Transform playerInputSpace;

  Hashtable RecoveryTable = new Hashtable(); // stores duration for each animation
  Hashtable AnimationLockTable = new Hashtable(); // stores duration player is locked animation

  // bool animationLocked = false;
  int animationLockCount = 0;
  // Hashtable AnimationLockCounts = new Hashtable(); // respective counters
  Hashtable RecoveryCounts = new Hashtable(); // probably just make a class u weirdo
  string[] AnimationSet = {"Walking", "Running", "Idle", "Jumping Up", "PunchLeft", "Falling To Landing", "Falling Idle", "Hanging Idle"}; // TODO convert to hashes
  List<string> interuptableAnimationSet = new List<string>();

  Vector3 currentMovement;
  Vector3 currentPosition;
  Vector3 currentVelocity;
  Vector3 jumpVelocity;
  private Vector3 ragdollOffset;

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

  // animation bools to keep track
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
  [SerializeField, Range(0,1)]
  float groundedMaxDistance = 0.085f;
  // landing params
  [SerializeField, Range(0,1)]

  // [SerializeField]
  // Transform ragdollRightFoot;
  // [SerializeField]
  // Transform ragdollLeftFoot;


  // hid detection bools
  [HideInInspector] public bool hitDetected = false;
  [HideInInspector] public GameObject hitTarget = null;

  // ragdoll control
  ragdollControl ragdollControl;
  motionController motionController;

  private Dictionary<string, AnimationClip> animationClipDict = new Dictionary<string, AnimationClip>();


  void Awake()
  {

    // isAnimating = true;

    // rollAllowed = true;



    input = new DudeActions();
    ragdollControl = GetComponent<ragdollControl>(); // turn on and off ragdolling
    motionController = GetComponent<motionController>(); // access motion based info, mostly speed
    ledgeDectector = GetComponent<LedgeDetection>();

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

  // Start is called before the first frame update
  void Start()
  {
    animator = GetComponent<Animator>(); // animation chart for character
    rigidbody = GetComponent<Rigidbody>(); // rigidbody

    // ragdollOffset = transform.position - ragdollTransform.position;

    // customize interuptable animations
    // interuptableAnimationSet.Add("Walking");
    // interuptableAnimationSet.Add("Idle");
    // interuptableAnimationSet.Add("Running");
    // interuptableAnimationSet.Add("PunchLeft");
    // interuptableAnimationSet.Add("Jumping Up");

    UpdateActionTimes();
    // Debug.Log("checking table for " + key);


    // isWalkingHash = Animator.StringToHash("isWalking");
    // isRunningHash = Animator.StringToHash("isRunning");
    // isRollingHash = Animator.StringToHash("isRolling");
    // isPunchingHash = Animator.StringToHash("isPunching");
    // animSpeedHash = Animator.StringToHash("animSpeed");
    // rollSpeedHash = Animator.StringToHash("rollSpeed");
    // isRagdollHash = Animator.StringToHash("isRagdoll");
    // isJumpingHash = Animator.StringToHash("isJumping");

  }

  public void UpdateActionTimes()
  {
    // AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
    // foreach(AnimationClip clip in clips)
    foreach(string clipName in AnimationSet)
    {
      // pop a late
      switch(clipName)
      {
        case "Idle":
        // animationClipDict.Add("Idle", clip);
        RecoveryTable.Add("Idle", 1); // number of frames for recovery.
        RecoveryCounts.Add("Idle", 0);
        AnimationLockTable.Add("Idle", 0);
        break;
        case "Walking":
        // animationClipDict.Add("Walking", clip);
        RecoveryTable.Add("Walking", 1);
        RecoveryCounts.Add("Walking", 0);
        AnimationLockTable.Add("Walking", 0);
        break;
        case "Running":
        // animationClipDict.Add("Running", clip);
        RecoveryTable.Add("Running", 1);
        RecoveryCounts.Add("Running", 0);
        AnimationLockTable.Add("Running", 0);
        break;
        case "Uppercut":
        // animationClipDict.Add("Uppercut", clip);
        RecoveryTable.Add("Uppercut", 125);
        RecoveryCounts.Add("Uppercut", 0);
        AnimationLockTable.Add("Uppercut", 100);
        break;
        case "Roll":
        // animationClipDict.Add("Roll", clip);
        RecoveryTable.Add("Roll", 150);
        RecoveryCounts.Add("Roll", 0);
        AnimationLockTable.Add("Roll", 100);
        break;
        case "PunchLeft":
        // animationClipDict.Add("PunchLeft", clip);
        RecoveryTable.Add("PunchLeft", 125);
        RecoveryCounts.Add("PunchLeft", 0);
        AnimationLockTable.Add("PunchLeft", 100);
        break;
        case "Jumping Up":
        // animationClipDict.Add("Jumping Up", clip);
        RecoveryTable.Add("Jumping Up", 25);
        RecoveryCounts.Add("Jumping Up", 0);
        AnimationLockTable.Add("Jumping Up", 0);
        break;
        case "Getting Up":
        // animationClipDict.Add("Getting Up", clip);
        RecoveryTable.Add("Getting Up", 200);
        RecoveryCounts.Add("Getting Up", 0);
        AnimationLockTable.Add("Getting Up", 150);
        break;
        case "Falling To Landing":
        // animationClipDict.Add("Falling To Landing", clip);
        RecoveryTable.Add("Falling To Landing", 1);
        RecoveryCounts.Add("Falling To Landing", 0);
        AnimationLockTable.Add("Falling To Landing", 10);
        break;
        case "Falling Idle":
        // animationClipDict.Add("Falling Idle", clip);
        RecoveryTable.Add("Falling Idle", 1);
        RecoveryCounts.Add("Falling Idle", 0);
        AnimationLockTable.Add("Falling Idle", 0);
        break;
        case "Hanging Idle":
        // animationClipDict.Add("Hanging Idle", clip);
        RecoveryTable.Add("Hanging Idle", 1);
        RecoveryCounts.Add("Hanging Idle", 0);
        AnimationLockTable.Add("Hanging Idle", 0);
        break;
      }
    }
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
    UpdateHangPoint();
    // Debug.Log("animation lock count: " + animationLockCount);
    if (animationLockCount > 0) // animation locked to specific
    {
      animationLockCount = animationLockCount - 1; // increment locked animation counter
      foreach(string key in AnimationSet)
      {
        int count = (int)RecoveryCounts[key];
        if (count > 0)
        {
          RecoveryCounts[key] = count - 1;
        }
        else
        {
          if (interuptableAnimationSet.Contains(key))
          {
            animator.ResetTrigger(key);
          }
          // Debug.Log("resetting trigger for " + key);
          // Debug.Log(key + " waiting to unlock");
        }

      }
    }
    else  // no animation lock on
    {
      foreach(string key in AnimationSet)
      {
        int count = (int)RecoveryCounts[key];
        if (count > 0)
        {
          RecoveryCounts[key] = count - 1;
          if (count == 1)
          {
            // if (key != "Idle")
            // {
            // }
            // Debug.Log("unlocking " + key);
            animator.ResetTrigger(key);
          }
        }
      }
    }
  }

  bool CheckRecoveryTable(string key)
  {
    // Debug.Log("checking table for " + key);
    // return (int)RecoveryCounts[key] == (int)RecoveryTable[key];
    // put in interuptable animations here
    return ((int)RecoveryCounts[key] == 0 && animationLockCount == 0) || interuptableAnimationSet.Contains(key);

  }

  void triggerAnimation(string animationKey)
  {
    animator.SetTrigger(animationKey);
    RecoveryCounts[animationKey] = (int)RecoveryTable[animationKey];
    animationLockCount = (int)AnimationLockTable[animationKey];
  }

  void UpdateGrounded()
  {
    // cast rays from radius around player
    isGrounded = Physics.Raycast(transform.position + 0.1f * Vector3.up, -Vector3.up, groundedMaxDistance + 0.1f);

  }


  void UpdateDistanceToGround()
  {
    distanceToGround = maxGroundDistance;
    RaycastHit hit = new RaycastHit();
    if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
      distanceToGround = Mathf.Min(hit.distance, maxGroundDistance);
    }

  }

  void UpdateHangPoint()
  {
    // isHanging = Physics.Raycast(ledgeDetectionTransform.position, MapToInputSpace(Vector3.forward), ledgeGrabDistance);
    isHanging = ledgeDectector.DetectLedge();
  }

  // Update is called once per frame
  void FixedUpdate()
  {

    UpdateRecoveryTimes();
    // Handle animations
    HandleActions();
    HandleJumpMotion();
    HandleRotation();

  }

  void HandleActions()
  {
    // priority sorted (kinda)

    if (jumpPressed && isGrounded && !isAirborne && !isJumping && !isHanging && CheckRecoveryTable("Jumping Up"))
    {
      // Debug.Log("starting jump animation");
      triggerAnimation("Jumping Up");

      // Handle jump motion
      isJumping = true;
      jumpDelayCounter = jumpDelayCount;
      // Debug.Log("triggering jump at: " + Time.time);

      ApplyCurrentInput();

    }

    if (punchPressed && isGrounded && !isAirborne && !isJumping && !isHanging && CheckRecoveryTable("PunchLeft"))
    {
      // Debug.Log("starting punch animation");
      triggerAnimation("PunchLeft");
      // Debug.Log("setting lock for " + "punch" + " to " + animationLockCount);

    }

    // Handle walk animation
    if (movementPressed && !runPressed && isGrounded && !isAirborne && !isJumping && !isHanging && CheckRecoveryTable("Walking"))
    {
      // Debug.Log("starting walk animation");
      triggerAnimation("Walking");

      // Handle walk motion
      ApplyCurrentInput();

      // animator.ResetTrigger("Idle");
    }

    if (movementPressed && runPressed && isGrounded && !isAirborne && !isJumping && !isHanging && CheckRecoveryTable("Running"))
    {
      // Debug.Log("starting run animation");
      triggerAnimation("Running");

      // Handle run motion
      ApplyCurrentInput();
      // animator.ResetTrigger("Idle");
    }

    //Handle idle animation
    if (!movementPressed && !jumpPressed && isGrounded && !isAirborne && !isJumping && !isHanging && CheckRecoveryTable("Idle"))
    {
      // Debug.Log("starting idle animation");
      triggerAnimation("Idle");;
    }

    // Handle falling animation
    if (!isGrounded && !isJumping && !isHanging && CheckRecoveryTable("Falling Idle"))
    {
      // ApplyCurrentInput();
      triggerAnimation("Falling Idle");
      isAirborne = true;
      // Debug.Log("triggering idle air animation");

    }

    // Handle landing animation
    if (isGrounded && isAirborne && rigidbody.velocity.y <= 0.0f && !isHanging && CheckRecoveryTable("Falling To Landing"))
    {
      // ApplyCurrentInput();
      triggerAnimation("Falling To Landing");
      // update animation speed based on vertical speed
      // animator.SetFloat("Landing Speed", 1.0f/distanceToGround);
      isAirborne = false;

      // Handle motion
      ApplyCurrentInput();

    }

    if (isHanging && CheckRecoveryTable("Hanging Idle"))
    {

      animator.SetTrigger("Hanging Idle");
      rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);
      rigidbody.AddForce(Vector3.forward);

      // this would go in the catch animation anyway
      if (rigidbody.useGravity)
      {
        rigidbody.useGravity = false;
      }

    }

    if (!isJumping && isAirborne)
    {
      // Handle air motion
      ApplyCurrentInput();

      // update blend on air pose
      animator.SetFloat("Falling Blend", Mathf.Max(-rigidbody.velocity.y/4.0f, 0.0f));
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

        jumpVelocity = new Vector3(rigidbody.velocity.x,
        ((jumpDelayCount - jumpDelayCounter * 1.0f) / (jumpDelayCount * 1.0f)) * (playerMaxJumpSpeed - playerMinJumpSpeed) + playerMinJumpSpeed, rigidbody.velocity.z);
        // Debug.Log("jomp pressed with: " + jumpVelocity + " counter at: " + jumpDelayCounter);

      }
      else // let go of button
      {

        jumpVelocity = new Vector3(rigidbody.velocity.x,
        ((jumpDelayCount - jumpDelayCounter * 1.0f) / (jumpDelayCount * 1.0f)) * (playerMaxJumpSpeed - playerMinJumpSpeed) + playerMinJumpSpeed, rigidbody.velocity.z);
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

        rigidbody.velocity = jumpVelocity;
        // Debug.Log("jump fired at: " + Time.time);

        isJumping = false; // we have jumped

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
    transform.LookAt(transform.position + MapToInputSpace(currentMovement));
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
