using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class dudeController : MonoBehaviour
{

  Animator animator;
  new Rigidbody rigidbody;

  int isWalkingHash;
  int isRunningHash;
  int isRollingHash;
  int isPunchingHash;
  int isRagdollHash;
  int animSpeedHash;
  int rollSpeedHash;

  [HideInInspector] public bool isWalking;
  [HideInInspector] public bool isRunning;
  [HideInInspector] public bool isRolling;
  [HideInInspector] public bool isPunching;
  [HideInInspector] public bool isRagdoll;
  [HideInInspector] public bool isAnimating;
  [SerializeField] public Transform ragdollTransform;

  DudeActions input;

  Vector3 currentMovement;
  Vector3 currentPosition;
  Vector3 ragdollOffset;
  bool movementPressed;
  bool runPressed = false;
  bool rollPressed = false;
  bool punchPressed = false;
  bool ragdollPressed = false;

  [Range(1,10)]
  public float playerSpeedMult = 7.0f;
  [Range(1,10)]
  public float playerRunMult = 1.5f;
  [Range(1,10)]
  public float playerMaxSpeed = 5.0f; // player ground speed max
  [Range(0,1)]
  public float playerRotationSpeed = 0.1f;

  // animation constants
  float walkAnimationBaseSpeed = 1.055f;
  float runAnimationBaseSpeed = 4.075f;
  float rollAnimationBaseSpeed = 3.319f;
  // animation timing constants
  float punchBaseDuration = 1.0f;
  float rollBaseDuration = 1.0f;
  float rollEndTime = 0.0f;
  float punchEndTime = 0.0f;

  // hid detection bools
  [HideInInspector] public bool hitDetected = false;
  [HideInInspector] public GameObject hitTarget = null;

  // ragdoll control
  ragdollControl ragdollControl;

  void Awake()
  {
    input = new DudeActions();
    ragdollControl = GetComponent<ragdollControl>();

    input.CharacterControls.ZAxis.performed += ctx =>
    {
      currentMovement.z = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;
      // Debug.Log(currentMovement);
    };

    input.CharacterControls.XAxis.performed += ctx =>
    {
      currentMovement.x = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;
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
    };

    input.CharacterControls.Punch.performed += ctx =>
    {
      punchPressed = ctx.ReadValueAsButton();
      // Debug.Log(runPressed);
    };

    // ragdoll action
    input.CharacterControls.Ragdoll.performed += ctx =>
    {
      ragdollPressed = ctx.ReadValueAsButton();
      Debug.Log("Ragdoll requested");
    };


    isAnimating = true;

  }

  void Start()
  {
    animator = GetComponent<Animator>(); // animation chart for character
    rigidbody = GetComponent<Rigidbody>(); // kinematic rigidbody

    isWalkingHash = Animator.StringToHash("isWalking");
    isRunningHash = Animator.StringToHash("isRunning");
    isRollingHash = Animator.StringToHash("isRolling");
    isPunchingHash = Animator.StringToHash("isPunching");
    animSpeedHash = Animator.StringToHash("animSpeed");
    rollSpeedHash = Animator.StringToHash("rollSpeed");
    isRagdollHash = Animator.StringToHash("isRagdoll");

    // get ragdoll transform offset for following ragdoll position
    ragdollOffset = transform.position - ragdollTransform.position;
  }

  // Update is called once per frame
  void FixedUpdate()
  {

    HandleInput();
    HandleMovement();
    HandleRotation();

    HandleHitDetection();

  }

  void HandleHitDetection()
  {
    bool isPunching = animator.GetBool(isPunchingHash);

    if (isPunching && hitDetected)
    {
      Debug.Log(string.Format("hit detected! on: {0}", hitTarget.name));
    }
  }

  void HandleMovement()
  {

    if (isAnimating)
    {

    //can convert into squared comparison for speed
    float groundSpeed = Mathf.Sqrt(rigidbody.velocity.x*rigidbody.velocity.x + rigidbody.velocity.z*rigidbody.velocity.z);

    // figure out if we're walking or running
    // isWalking = animator.GetBool(isWalkingHash);
    // isRunning = animator.GetBool(isRunningHash);
    // isRolling = animator.GetBool(isRollingHash);
    // isPunching = animator.GetBool(isPunchingHash);
    // isPunching = animator.GetBool(isPunchingHash);
    // isRagdoll = animator.GetBool(isRagdollHash);

    // add force and clamp velocity
    if (isWalking && !isRunning)
    {
      animator.SetFloat(animSpeedHash, groundSpeed / walkAnimationBaseSpeed);

      if (groundSpeed < (playerMaxSpeed))
      {
        rigidbody.AddForce(playerSpeedMult * currentMovement);
      }
    }

    if (isRunning)
    {
      animator.SetFloat(animSpeedHash, groundSpeed / runAnimationBaseSpeed);

      if (groundSpeed < (playerMaxSpeed*playerRunMult))
      {
        rigidbody.AddForce(playerSpeedMult * playerRunMult * currentMovement);
      }

    }

    if (isRolling)
    {
      animator.SetFloat(rollSpeedHash, groundSpeed / rollAnimationBaseSpeed);
    }

  }

  }

  void HandleRotation()
  {
    currentPosition = transform.position;
    Vector3 currentVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
    currentVelocity.Normalize();

    transform.LookAt(currentPosition + currentVelocity);
  }

  void HandleInput()
  {

    isWalking = animator.GetBool(isWalkingHash);
    isRunning = animator.GetBool(isRunningHash);
    isRolling = animator.GetBool(isRollingHash);
    isPunching = animator.GetBool(isPunchingHash);
    isRagdoll = animator.GetBool(isRagdollHash);

    // walking and running action
    if (movementPressed && !isWalking)
    {
      animator.SetBool(isWalkingHash, true);
    }

    if (!movementPressed && isWalking)
    {
      animator.SetBool(isWalkingHash, false);
    }

    if ((runPressed && isWalking) && !isRunning)
    {
      animator.SetBool(isRunningHash, true);
    }

    if ((!runPressed || !isWalking) && isRunning)
    {
      animator.SetBool(isRunningHash, false);
    }


    // rolling/dash action
    if ((isWalking || isRunning) && rollPressed && !isRolling)
    {
      animator.SetBool(isRollingHash, true);
      rollEndTime = Time.time + rollBaseDuration;
    }

    if (isRolling && Time.time > rollEndTime && !rollPressed)
    {
      animator.SetBool(isRollingHash, false);
    }


    // punch action
    if (punchPressed && !isPunching)
    {
      animator.SetBool(isPunchingHash, true);
      punchEndTime = Time.time + punchBaseDuration;
      // Debug.Log(string.Format("rollEndTime: {0}, currentTime {1}", punchEndTime, Time.time));
    }

    if (isPunching && Time.time > punchEndTime && !punchPressed)
    {
      animator.SetBool(isPunchingHash, false);
    }

    // ragdoll action
    if (!isRagdoll && ragdollPressed)
    {
      animator.SetBool(isRagdollHash, false); // misleading
      ragdollControl.ToggleRagdoll(false); // fall down
      isAnimating = false;
      // Debug.Log("Ragdoll-ing");
    }

    if (!ragdollPressed && !isAnimating)
    {
      ragdollControl.ToggleRagdoll(true);
      // Debug.Log("Stopped Ragdoll-ing");
      animator.SetBool(isRagdollHash, true);
    }

    if (!ragdollPressed && !isAnimating && isRagdoll)
    {
      isAnimating = true;
      animator.SetBool(isRagdollHash, false);
      // Debug.Log("Reset position to match with ragdoll");
      // adjust position to match with ragdoll
      transform.position = ragdollTransform.position;
    }

  }

  void OnEnable ()
  {
    input.CharacterControls.Enable();
  }

  void OnDisable ()
  {
    input.CharacterControls.Disable();
  }

  //  public void Move_UpDown(InputAction.CallbackContext context)
  // {
  //     Debug.Log($"UpDown | Started: {context.started} | Performed: {context.performed} | Canceled: {context.canceled} | MoveVec: {context.ReadValue<Vector2>()}");
  //
  //     _moveVector.y = context.ReadValue<Vector2>().y;
  // }
  //
  // public void Move_LefRight(InputAction.CallbackContext context)
  // {
  //     Debug.Log($"LeftRight | Started: {context.started} | Performed: {context.performed} | Canceled: {context.canceled} | MoveVec: {context.ReadValue<Vector2>()}");
  //
  //     _moveVector.x = context.ReadValue<Vector2>().x;
  // }
}
