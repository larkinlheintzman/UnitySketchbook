using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{

  Animator animator;
  new Rigidbody rigidbody;

  int isWalkingHash;
  int isRunningHash;
  int isRollingHash;
  int isPunchingHash;
  int isJumpingHash;
  int isRagdollHash;
  int animSpeedHash;
  int rollSpeedHash;

  [HideInInspector] public bool isWalking;
  [HideInInspector] public bool isRunning;
  [HideInInspector] public bool isRolling;
  [HideInInspector] public bool isPunching;
  [HideInInspector] public bool isJumping;
  [HideInInspector] public bool isRagdoll;
  [HideInInspector] public bool isAnimating;

  DudeActions input;

  Vector3 currentMovement;
  Vector3 currentPosition;

  // button booleans
  bool movementPressed;
  bool runPressed = false;
  bool rollPressed = false;
  bool punchPressed = false;
  bool ragdollPressed = false;
  bool jumpPressed = false;

  // animation end times
  float rollEndTime;
  float rollRecoveryTime = 1.5f;

  // animation allow flags
  bool rollAllowed = false;

  [Range(1,10)]
  public float playerSpeedMult = 7.0f;
  [Range(1,10)]
  public float playerRunMult = 1.5f;
  [Range(1,10)]
  public float playerMaxSpeed = 5.0f; // player ground speed max
  [Range(0,1)]
  public float playerRotationSpeed = 0.1f;

  // hid detection bools
  [HideInInspector] public bool hitDetected = false;
  [HideInInspector] public GameObject hitTarget = null;

  // ragdoll control
  ragdollControl ragdollControl;
  motionController motionController;

  private Dictionary<string, AnimationClip> animationClipDict = new Dictionary<string, AnimationClip>();

  void Awake()
  {

    isAnimating = true;

    rollAllowed = true;

    input = new DudeActions();
    ragdollControl = GetComponent<ragdollControl>(); // turn on and off ragdolling
    motionController = GetComponent<motionController>(); // access motion based info, mostly speed

    input.CharacterControls.ZAxis.performed += ctx =>
    {
      currentMovement.z = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;

      HandleWalkAnimation(movementPressed);
      // Debug.Log(currentMovement);
    };

    input.CharacterControls.XAxis.performed += ctx =>
    {
      currentMovement.x = ctx.ReadValue<float>();
      currentMovement.Normalize();
      movementPressed = currentMovement.x != 0 || currentMovement.z != 0;

      HandleWalkAnimation(movementPressed);
      // Debug.Log(currentMovement);
    };

    input.CharacterControls.Run.performed += ctx =>
    {
      runPressed = ctx.ReadValueAsButton();

      HandleWalkAnimation(movementPressed);
      // Debug.Log(runPressed);
    };

    input.CharacterControls.Roll.performed += ctx =>
    {
      rollPressed = ctx.ReadValueAsButton();
      // Debug.Log(runPressed);
      // can we just set animation bools here?
      HandleRollAnimation(rollPressed);
    };

    input.CharacterControls.Punch.performed += ctx =>
    {
      punchPressed = ctx.ReadValueAsButton();
      HandlePunchAnimation(punchPressed);
    };

    // jump action
    input.CharacterControls.Jump.performed += ctx =>
    {
      jumpPressed = ctx.ReadValueAsButton();
      HandleJumpAnimation(jumpPressed);
    };

    // ragdoll action started
    input.CharacterControls.Ragdoll.started += ctx =>
    {
      ragdollPressed = ctx.ReadValueAsButton();
      // Debug.Log("button down, val : " + ragdollPressed);
      HandleRagdollAnimation(ragdollPressed);
    };
    // ragdoll action started
    input.CharacterControls.Ragdoll.performed += ctx =>
    {
      ragdollPressed = ctx.ReadValueAsButton();
      if (!ragdollPressed)
      {
        // Debug.Log("button up, val : " + ragdollPressed);
        HandleRagdollAnimation(ragdollPressed);
      }
    };

  }

    // Start is called before the first frame update
    void Start()
    {
      animator = GetComponent<Animator>(); // animation chart for character
      rigidbody = GetComponent<Rigidbody>(); // kinematic rigidbody

      UpdateAnimClipTimes();

      isWalkingHash = Animator.StringToHash("isWalking");
      isRunningHash = Animator.StringToHash("isRunning");
      isRollingHash = Animator.StringToHash("isRolling");
      isPunchingHash = Animator.StringToHash("isPunching");
      animSpeedHash = Animator.StringToHash("animSpeed");
      rollSpeedHash = Animator.StringToHash("rollSpeed");
      isRagdollHash = Animator.StringToHash("isRagdoll");
      isJumpingHash = Animator.StringToHash("isJumping");

    }

    public void UpdateAnimClipTimes()
      {
          AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
          foreach(AnimationClip clip in clips)
          {
              switch(clip.name)
              {
                  case "Idle":
                      animationClipDict.Add("Idle", clip);
                      break;
                  case "Walking":
                      animationClipDict.Add("Walking", clip);
                      break;
                  case "Running":
                      animationClipDict.Add("Running", clip);
                      break;
                  case "Uppercut":
                      animationClipDict.Add("Uppercut", clip);
                      break;
                  case "Roll":
                      animationClipDict.Add("Roll", clip);
                      break;
                  case "PunchLeft":
                      animationClipDict.Add("PunchLeft", clip);
                      break;
                  case "Jumping Up":
                      animationClipDict.Add("Jumping Up", clip);
                      break;
                  case "Getting Up":
                      animationClipDict.Add("Getting Up", clip);
                      break;
              }
          }
      }

    // Update is called once per frame
    void FixedUpdate()
    {
      // all we do here is update bools from animator
      isWalking = animator.GetBool(isWalkingHash);
      isRunning = animator.GetBool(isRunningHash);
      isRolling = animator.GetBool(isRollingHash);
      isPunching = animator.GetBool(isPunchingHash);
      isRagdoll = animator.GetBool(isRagdollHash);
      isJumping = animator.GetBool(isJumpingHash);

      // update animations that have timed out
      // rolling animation
      if (isRolling && Time.time > rollEndTime)
      {
        // Debug.Log("ending roll animation at: " + Time.time);
        animator.SetBool(isRollingHash, false);
      }
      if (!isRolling && Time.time > rollEndTime + rollRecoveryTime && !rollAllowed)
      {
        rollAllowed = true; // can go to roll again
        // Debug.Log("allowed to roll again");
      }
    }

    // button specific actions
    void HandleWalkAnimation(bool movementPressed)
    {

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
        // for running case
      }

      if ((!runPressed || !isWalking) && isRunning)
      {
        animator.SetBool(isRunningHash, false);
      }

    }

    // rolling/dash action
    void HandleRollAnimation(bool rollPressed)
    {

      if ((isWalking || isRunning) && rollPressed && !isRolling && rollAllowed)
      {
        animator.SetBool(isRollingHash, true);
        rollEndTime = Time.time + animationClipDict["Roll"].length - 0.01f;
        // Debug.Log("current time: " + Time.time + " end time: " + rollEndTime);

        rollAllowed = false;
      }

    }

    // punch action
    void HandlePunchAnimation(bool punchPressed)
    {

      if (punchPressed && !isPunching)
      {
        animator.SetBool(isPunchingHash, true);
      }

      if (isPunching && !punchPressed)
      {
        animator.SetBool(isPunchingHash, false);
      }

    }

    // jump action
    void HandleJumpAnimation(bool jumpPressed)
    {

      if (jumpPressed && !isJumping)
      {
        animator.SetBool(isJumpingHash, true);
      }

      if (isJumping && !jumpPressed)
      {
        animator.SetBool(isJumpingHash, false);
      }

    }

    // ragdoll action
    void HandleRagdollAnimation(bool ragdollPressed)
    {

      // ragdoll action
      if (!isRagdoll && ragdollPressed)
      {
        animator.SetBool(isRagdollHash, true);
        ragdollControl.ToggleRagdoll(false);
        isAnimating = false;
      }

      if (!ragdollPressed && !isAnimating)
      {
        ragdollControl.ToggleRagdoll(true);

        animator.SetBool(isRagdollHash, true);
        isAnimating = true;
      }

      if (!ragdollPressed && isAnimating && isRagdoll)
      {
        isAnimating = true;
        animator.SetBool(isRagdollHash, false);
      }

    }

    public void UpdateWalkAnimationSpeed(float animationSpeed)
    {
      // Debug.Log("Setting walk animation speed to " + animationSpeed);
      // animator.SetFloat(animSpeedHash, animationSpeed);
    }

    public void UpdateRollAnimationSpeed(float rollSpeed)
    {
      // Debug.Log("Setting roll animation speed to " + rollSpeed);
      // animator.SetFloat(rollSpeedHash, rollSpeed);
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
